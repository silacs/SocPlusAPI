namespace SocPlus.DTOs;

public class PaginatedResult<T> {
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Skip { get; set; }
    public int Total { get; set; }
    public required IEnumerable<T> Data { get; set; }
}