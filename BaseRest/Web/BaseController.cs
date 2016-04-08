using BaseRest.Boundary;
using BaseRest.General;
using System.Linq;
using System.Web.Http;

namespace BaseRest.Web
{
    [RestfulExceptionFilter]
    public abstract class BaseController<TDmn, TDto, TConverter, TService, TPermissions> : ApiController
        where TDmn : class, IDomain
        where TDto : class, IDto
        where TConverter : IConverter<TDmn, TDto, TPermissions>, new()
        where TService : IService<TDmn, TDto, TConverter, TPermissions>
        where TPermissions : IPermissions
    {
        public TService Service { get; private set; }

        public BaseController(TService service)
        {
            this.Service = service;
        }

        [Route("~/api/{controller}/{id:int}")]
        public virtual IHttpActionResult Get(int id)
        {
            TDto dto = this.Service.Get(id)
                .ApplyOptions(QueryOptions.FromRequestUri(this.Request.RequestUri))
                .FirstOrDefault();

            return dto != null ?
                (IHttpActionResult)this.Ok(dto) : this.NotFound();
        }

        [Route("~/api/{controller}")]
        public virtual IHttpActionResult Get([UrlArray]int[] ids, string include = null)
        {
            TDto[] dtos = this.Service.Get(ids)
                .ApplyOptions(QueryOptions.FromRequestUri(this.Request.RequestUri))
                .ToArray();

            return dtos != null ?
                (IHttpActionResult)this.Ok(dtos) : this.NotFound();
        }

        [Route("~/api/{controller}/{id:int}")]
        public virtual IHttpActionResult Put(int id, TDto dto)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            TDto existing = this.Service.Get(id).FirstOrDefault();
            if (existing == null)
            {
                return this.NotFound();
            }

            bool updated = this.Service.Update(id, dto);
            return updated ?
                this.Get(id) : this.InternalServerError();
        }

        [Route("~/api/{controller}")]
        public virtual IHttpActionResult Post(TDto dto)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(ModelState);
            }

            TDto created = this.Service.Create(dto);
            return this.Created("", created);
        }

        [Route("~/api/{controller}/{id:int}")]
        public virtual IHttpActionResult Delete(int id)
        {
            TDto dto = this.Service.Get(id).FirstOrDefault();
            if (dto == null)
            {
                return this.NotFound();
            }

            bool deleted = this.Service.Delete(dto);
            return deleted ?
                (IHttpActionResult)this.Ok() : this.InternalServerError();
        }
    }
}
