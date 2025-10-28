namespace DrakkarVpn.Shared;

public sealed record PagedResponseDto<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int Total,
    int TotalPages
)
{
    public static PagedResponseDto<T> Empty(int page, int pageSize) =>
        new(Array.Empty<T>(), page, pageSize, 0, 0);

    public static PagedResponseDto<T> From(
        IReadOnlyList<T> items, int page, int pageSize, int total) =>
        new(items, page, pageSize, total, total > 0 ? (int)Math.Ceiling((double)total / pageSize) : 0);
}