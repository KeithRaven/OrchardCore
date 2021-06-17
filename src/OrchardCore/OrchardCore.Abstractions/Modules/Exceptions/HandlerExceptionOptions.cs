using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace OrchardCore.Modules
{
    public class HandlerExceptionOptions
    {
        private List<HandlerRethrowPolicy> RethrowPolicies { get; } = new List<HandlerRethrowPolicy>();

        public void RethrowFatal()
        {
            Rethrow((ex, logger, args) => ex.IsFatal());
        }

        public void RethrowAll()
        {
            RethrowPolicies.Clear(); // There's no point in keeping multiple policies
            Rethrow((ex,logger,args) => true); // when this one always returns true :)
        }

        public void Rethrow(Func<Exception, ILogger, object[], bool> predicate)
        {
            RethrowPolicies.Add(new HandlerRethrowPolicy { RethrowPolicy = predicate });
        }
        public void Rethrow<TEvents>(Func<Exception, ILogger, object[], bool> predicate)
        {
            RethrowPolicies.Add(new HandlerRethrowPolicy { EventsType = typeof(TEvents), RethrowPolicy = predicate });
        }

        private bool ShouldThrow(Exception exception, ILogger logger, Type eventsType, params object[] args)
        {
            foreach (var policy in RethrowPolicies.Where(rp => rp.EventsType == null || rp.EventsType == eventsType))
            {
                if (policy.RethrowPolicy(exception, logger, args))
                {
                    return true;
                }
            }

            return false;
        }

        public bool ShouldThrow<TEvents>(Exception exception, ILogger logger, params object[] args)
        {
            return ShouldThrow(exception, logger, typeof(TEvents), args);
        }
    }
}
