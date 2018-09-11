using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http.Headers;
using System.Diagnostics;

namespace QQute.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
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

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TimerAF : ActionFilterAttribute
    {
        private const string _headerName = "TimerAF";

        private string _property;

        private string _timerName;

        public TimerAF(string TimerName = "")
        {
            _timerName = TimerName;
        }

        //Overide OnActionContext (◕‿◕✿)
        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            _property = $"{_timerName}_{actionContext.ActionDescriptor.ActionName}";
            actionContext.Request.Properties.Add(_property, Stopwatch.StartNew());

            return base.OnActionExecutingAsync(actionContext, cancellationToken);
        }

        //Overide OnActionExecutedContext ⊂(◉‿◉)つ
        public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            var timerOBJ = actionExecutedContext.Request.Properties[_property];

            actionExecutedContext.Response.Headers.Add(_headerName, ((Stopwatch)timerOBJ).ElapsedMilliseconds.ToString() + _timerName + "ms");

            return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }
    }
}