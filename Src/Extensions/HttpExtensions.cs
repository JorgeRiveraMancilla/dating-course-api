using System.Text.Json;
using dating_course_api.Src.Helpers.Pagination;

namespace dating_course_api.Src.Extensions
{
    public static class HttpExtensions
    {
        private static readonly JsonSerializerOptions _jsonOptions =
            new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public static void AddPaginationHeader<T>(this HttpResponse response, PagedList<T> data)
        {
            var paginationHeader = new PaginationHeader(
                data.CurrentPage,
                data.PageSize,
                data.TotalCount,
                data.TotalPages
            );

            response.Headers.Append(
                "Pagination",
                JsonSerializer.Serialize(paginationHeader, _jsonOptions)
            );
            response.Headers.Append("Access-Control-Expose-Headers", "Pagination");
        }
    }
}
