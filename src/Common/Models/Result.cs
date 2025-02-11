namespace Common.Models;

public record Result<T>
{
    public T? Data { get; init; }
    public bool IsSuccess { get; init; }
    public int? ErrorCode { get; set; }
    public string? Message { get; init; }
}

public record PaginationDto<T>
{
    public IList<T> Data { get; set; }
    public int Size { get; set; }
    public int Page { get; set; }
}