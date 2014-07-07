using System.Web.Http.Filters;

namespace RESTable.Infrastructure.RESTable
{
    public interface IOrderedFilter : IFilter
    {
        int Order { get; set; }
    }
}