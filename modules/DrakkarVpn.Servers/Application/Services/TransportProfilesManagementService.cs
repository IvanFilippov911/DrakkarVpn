using DrakkarVpn.Core.Api.Modules.Servers.Application.Abstractions;
using DrakkarVpn.Servers.Application.Abstractions.Services;
using DrakkarVpn.Servers.Application.DTOs.TransportProfiles;
using DrakkarVpn.Servers.Application.Errors;
using DrakkarVpn.Servers.Domain.Aggregates;
using DrakkarVpn.Servers.Domain.Enums;
using DrakkarVpn.Shared;
using DrakkarVpn.Shared.Errors.DomainErrors;

namespace DrakkarVpn.Servers.Application.Services;

public sealed class TransportProfilesManagementService : ITransportProfilesManagementService
{
    private readonly ITransportProfileReadRepository _readRepository;
    private readonly ITransportProfileWriteRepository _writeRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public TransportProfilesManagementService(
        ITransportProfileReadRepository readRepository,
        ITransportProfileWriteRepository writeRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<PagedResponseDto<TransportProfileDto>> GetPagedAsync(
        GetTransportProfilesFilterDto filter,
        CancellationToken ct)
    {
        var page = filter.Page <= 0 ? 1 : filter.Page;
        var pageSize = Math.Clamp(filter.PageSize <= 0 ? 25 : filter.PageSize, 1, 200);
        var normalizedFilter = filter with { Page = page, PageSize = pageSize };

        var (items, total) = await _readRepository.GetPagedAsync(normalizedFilter, ct);
        if (total == 0)
            return PagedResponseDto<TransportProfileDto>.Empty(page, pageSize);

        return PagedResponseDto<TransportProfileDto>.From(items, page, pageSize, total);
    }

    public async Task<Guid> CreateAsync(CreateTransportProfileDto dto, CancellationToken ct)
    {
        dto = dto ?? throw new ArgumentNullException(nameof(dto));

        var normalizedName = dto.Name.Trim();
        if (await _writeRepository.ExistsByNameAsync(normalizedName, excludeId: null, ct))
        {
            throw new DomainException(
                DomainArea.Servers,
                TransportProfileErrorCodes.NameAlreadyExists,
                $"Transport profile with name '{normalizedName}' already exists.");
        }

        var utcNow = _dateTimeProvider.UtcNow.UtcDateTime;
        var id = Guid.NewGuid();
        var entity = BuildNewProfile(id, dto, utcNow);

        await _writeRepository.AddAsync(entity, ct);
        return id;
    }

    public async Task<bool> UpdateAsync(
        Guid profileId,
        UpdateTransportProfileDto dto,
        CancellationToken ct)
    {
        dto = dto ?? throw new ArgumentNullException(nameof(dto));

        var entity = await _writeRepository.GetAsync(profileId, ct);
        if (entity is null)
            return false;

        var normalizedName = dto.Name.Trim();
        if (await _writeRepository.ExistsByNameAsync(normalizedName, profileId, ct))
        {
            throw new DomainException(
                DomainArea.Servers,
                TransportProfileErrorCodes.NameAlreadyExists,
                $"Transport profile with name '{normalizedName}' already exists.");
        }

        var utcNow = _dateTimeProvider.UtcNow.UtcDateTime;
        entity.Update(
            dto.Name,
            dto.GlobalPriority,
            dto.RealitySni,
            dto.RealityShortId,
            dto.RealityFingerprint,
            dto.RealityDest,
            dto.GrpcServiceName,
            dto.GrpcAuthority,
            utcNow);
        return true;
    }

    public async Task<bool> EnableAsync(Guid profileId, CancellationToken ct)
    {
        var entity = await _writeRepository.GetAsync(profileId, ct);
        if (entity is null)
            return false;

        entity.Enable(_dateTimeProvider.UtcNow.UtcDateTime);
        return true;
    }

    public async Task<bool> DisableAsync(Guid profileId, CancellationToken ct)
    {
        var entity = await _writeRepository.GetAsync(profileId, ct);
        if (entity is null)
            return false;

        entity.Disable(_dateTimeProvider.UtcNow.UtcDateTime);
        return true;
    }

    public async Task<DeleteTransportProfileResult> DeleteAsync(Guid profileId, CancellationToken ct)
    {
        var entity = await _writeRepository.GetAsync(profileId, ct);
        if (entity is null)
            return DeleteTransportProfileResult.NotFound;

        if (await _writeRepository.IsUsedInAnyActivationAsync(profileId, ct))
            return DeleteTransportProfileResult.InUse;

        await _writeRepository.DeleteAsync(entity, ct);
        return DeleteTransportProfileResult.Deleted;
    }

    private static TransportProfile BuildNewProfile(Guid id, CreateTransportProfileDto dto, DateTime utcNow)
    {
        if (dto.SecurityType != SecurityType.Reality)
        {
            throw new DomainException(
                DomainArea.Servers,
                TransportProfileErrorCodes.UnsupportedSecurityType,
                $"Security type '{dto.SecurityType}' is not supported for transport profile creation.");
        }

        return dto.TransportType switch
        {
            TransportType.Tcp => TransportProfile.CreateRealityTcp(
                id,
                dto.Name,
                dto.GlobalPriority,
                dto.RealitySni,
                dto.RealityShortId,
                dto.RealityFingerprint,
                dto.RealityDest,
                utcNow),

            TransportType.Grpc => TransportProfile.CreateRealityGrpc(
                id,
                dto.Name,
                dto.GlobalPriority,
                dto.RealitySni,
                dto.RealityShortId,
                dto.RealityFingerprint,
                dto.RealityDest,
                EnsureGrpcServiceName(dto.GrpcServiceName),
                dto.GrpcAuthority,
                utcNow),

            _ => throw new ArgumentOutOfRangeException(nameof(dto.TransportType), dto.TransportType, "Unsupported transport type.")
        };
    }

    private static string EnsureGrpcServiceName(string? grpcServiceName)
    {
        if (string.IsNullOrWhiteSpace(grpcServiceName))
        {
            throw new DomainException(
                DomainArea.Servers,
                TransportProfileErrorCodes.GrpcServiceNameRequired,
                "GrpcServiceName is required when transport type is Grpc.");
        }

        return grpcServiceName;
    }
}
