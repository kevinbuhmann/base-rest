using System.Web.Http.ModelBinding;

namespace BaseRest.Web
{
    public class UrlArrayAttribute : ModelBinderAttribute
    {
        public UrlArrayAttribute()
            : base(typeof(CommaDelimitedArrayModelBinder))
        {
        }
    }
}
