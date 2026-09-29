namespace server.Models.Common
{
    // ============================================================================
    // PAGED RESULT - Kết quả phân trang chuẩn cho các endpoint search/list
    // ============================================================================
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
