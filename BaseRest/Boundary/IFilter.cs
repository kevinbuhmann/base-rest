using System.Linq;
using System.Net;

namespace BaseRest.Boundary
{
    public interface IFilter<TDmn, TPermissions>
        where TDmn : class, IDomain
        where TPermissions : IPermissions
    {
        HttpStatusCode HasPermissions(TPermissions permissions);

        IQueryable<TDmn> Apply(IQueryable<TDmn> query);
    }
}
