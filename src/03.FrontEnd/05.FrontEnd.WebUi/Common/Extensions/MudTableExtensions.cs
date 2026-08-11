using Pertamina.Common.Dto.Enums;
using Pertamina.Common.Dto.Requests;
using Pertamina.Common.Dto.Responses;

namespace EBVL.FrontEnd.WebUi.Common.Extensions;

public static class MudTableExtensions
{
    public static T ToPaginatedListRequest<T>(this TableState state, string? searchKeyword) where T : PaginatedListRequest, new()
    {
        return new()
        {
            Page = state.Page + 1,
            PageSize = state.PageSize,
            SearchText = searchKeyword,
            SortField = state.SortLabel,
            SortOrder = state.SortDirection switch
            {
                SortDirection.None => null,
                SortDirection.Ascending => SortOrder.Ascending,
                SortDirection.Descending => SortOrder.Descending,
                _ => null
            }
        };
    }

    public static TableData<T> ToTableData<T>(this PaginatedListResponse<T> result)
    {
        return new TableData<T>() { TotalItems = result.TotalCount, Items = result.Items };
    }
}
