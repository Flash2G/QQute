using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http.Headers;

namespace QQute.Filters
{
    public class CacheAF : ActionFilterAttribute
    {
        private bool _allowCache;

        public CacheAF(bool DoYouWantCache = false)
        {
            _allowCache = DoYouWantCache;
        }

        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }

        public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            if (_allowCache)
            {
                actionExecutedContext.Response.Headers.CacheControl = new CacheControlHeaderValue
                {
                    Private = true,
                    Public = false,
                    NoStore = false,
                    MaxAge = TimeSpan.FromSeconds(10)
                };

                //Pragma
                actionExecutedContext.Request.Headers.Date = DateTime.UtcNow;
                actionExecutedContext.Request.Content.Headers.Expires = DateTime.UtcNow.AddSeconds(10);
            }
            return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }
    }
}