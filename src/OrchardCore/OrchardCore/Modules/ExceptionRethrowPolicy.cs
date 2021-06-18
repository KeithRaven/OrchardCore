using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OrchardCore.Modules
{
    public class ExceptionRethrowPolicy<TSource> : IExceptionRethrowPolicy<TSource>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ExceptionRethrowOptions _handlerExceptionOptions;

        public ExceptionRethrowPolicy(
            IHttpContextAccessor httpContextAccessor,
            IOptions<ExceptionRethrowOptions> handlerExceptionOptions)
        {
            _httpContextAccessor = httpContextAccessor;
            _handlerExceptionOptions = handlerExceptionOptions.Value;
        }

        public bool ShouldThrow(Exception exception)
        {
            foreach (var policy in _handlerExceptionOptions.RethrowPolicies.Where(rp => rp.EventsType == null || rp.EventsType == typeof(TSource)))
            {
                if (policy.RethrowPolicies(_httpContextAccessor.HttpContext, exception))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
