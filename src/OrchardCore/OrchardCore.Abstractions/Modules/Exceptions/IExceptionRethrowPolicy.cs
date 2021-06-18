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
    public interface IExceptionRethrowPolicy<TSource>
    {
        bool ShouldThrow(Exception exception);
    }
}
