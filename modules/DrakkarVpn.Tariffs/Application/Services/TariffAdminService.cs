using DrakkarVpn.Core.Api.Modules.Tariffs.Application.Abstracts;
using DrakkarVpn.Core.Api.Modules.Tariffs.Application.DTOs;
using DrakkarVpn.Core.Api.Modules.Tariffs.Domain;
using DrakkarVpn.Core.Api.Modules.Tariffs.Domain.ValueObjects;
using FluentValidation;

namespace DrakkarVpn.Core.Api.Modules.Tariffs.Application.Services;

public sealed class TariffAdminService : ITariffAdminService
{
    private readonly ITariffRepository _repo;
    private readonly IValidator<TariffCreateDto> _createValidator;
    private readonly IValidator<TariffUpdateDto> _updateValidator;

    public TariffAdminService(
        ITariffRepository repo,
        IValidator<TariffCreateDto> createValidator,
        IValidator<TariffUpdateDto> updateValidator)
    {
        _repo = repo;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Guid> CreateAsync(TariffCreateDto dto, CancellationToken ct)
    {
        await _createValidator.ValidateAndThrowAsync(dto, ct);

        var tariff = Tariff.CreateNew(dto.Name, dto.Duration, dto.Price, dto.DefaultMaxDevices, dto.Kind);
        await _repo.AddAsync(tariff, ct);

        return tariff.Id.Value;
    }

    public async Task EnableAsync(Guid tariffId, CancellationToken ct)
    {
        if (tariffId == Guid.Empty)
            throw new ArgumentException("TariffId is empty", nameof(tariffId));

        var tariff = await _repo.GetByIdAsync(new TariffId(tariffId), ct);
        if (tariff is null)
            throw new InvalidOperationException($"Tariff {tariffId} not found");

        tariff.Enable();
    }

    public async Task DisableAsync(Guid tariffId, CancellationToken ct)
    {
        if (tariffId == Guid.Empty)
            throw new ArgumentException("TariffId is empty", nameof(tariffId));

        var tariff = await _repo.GetByIdAsync(new TariffId(tariffId), ct);
        if (tariff is null)
            throw new InvalidOperationException($"Tariff {tariffId} not found");

        tariff.Disable();
    }

    public async Task UpdateAsync(Guid tariffId, TariffUpdateDto dto, CancellationToken ct)
    {
        if (tariffId == Guid.Empty)
            throw new ArgumentException("TariffId is empty", nameof(tariffId));

        await _updateValidator.ValidateAndThrowAsync(dto, ct);

        var tariff = await _repo.GetByIdAsync(new TariffId(tariffId), ct);
        if (tariff is null)
            throw new InvalidOperationException($"Tariff {tariffId} not found");

        tariff.Update(dto.Name, dto.Price, dto.Duration, dto.DefaultMaxDevices);
    }
}