using System;
using Microsoft.Extensions.Logging;

namespace OrchardCore.Modules
{
    public class HandlerRethrowPolicy
    {
        public Type EventsType { get; set; }

        public Func<Exception, ILogger, object[], bool> RethrowPolicy { get; set; }
    }
}
