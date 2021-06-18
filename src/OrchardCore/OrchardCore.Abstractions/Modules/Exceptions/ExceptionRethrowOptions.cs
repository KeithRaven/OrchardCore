using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace OrchardCore.Modules
{
    public class ExceptionRethrowOptions
    {
        public class RethrowPolicy
        {
            public Type EventsType { get; set; }
            public Func<HttpContext, Exception, bool> RethrowPolicies { get; set; }
        }

        private List<RethrowPolicy> _rethrowPolicies = new List<RethrowPolicy>();
        public IReadOnlyCollection<RethrowPolicy> RethrowPolicies => _rethrowPolicies;

        public void RethrowFatal()
        {
            Rethrow((context, ex) => ex.IsFatal());
        }

        public void RethrowAll()
        {
            _rethrowPolicies.Clear(); // There's no point in keeping multiple policies
            Rethrow((context,ex) => true); // when this one always returns true :)
        }

        public void Rethrow(Func<HttpContext, Exception, bool> predicate)
        {
            _rethrowPolicies.Add(new RethrowPolicy { RethrowPolicies = predicate });
        }
        public void Rethrow<TEvents>(Func<HttpContext, Exception, bool> predicate)
        {
            _rethrowPolicies.Add(new RethrowPolicy { EventsType = typeof(TEvents), RethrowPolicies = predicate });
        }

        
    }
}
