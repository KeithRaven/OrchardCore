using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OrchardCore.Modules
{
    public class EventDispatcher<TEvents, TSource> : IEventDispatcher<TEvents, TSource>
    {
        private readonly IEnumerable<TEvents> _events;
        private readonly ILogger _logger;
        private readonly IExceptionRethrowPolicy<TEvents> _exceptionPolicy;

        public EventDispatcher(
            IEnumerable<TEvents> events,
            ILogger<TSource> logger,
            IExceptionRethrowPolicy<TEvents> eventsExceptionPolicy)
        {
            _events = events;
            _logger = logger;
            _exceptionPolicy = eventsExceptionPolicy;
        }

        public async Task InvokeAsync<T1>(Func<TEvents, T1, Task> dispatch, T1 arg1)
        {
            foreach (var sink in _events)
            {
                try
                {
                    await dispatch(sink, arg1);
                }
                catch (Exception ex)
                {
                    if (_exceptionPolicy.ShouldThrow(ex))
                    {
                        throw;
                    }
                    else
                    {
                        LogException(ex, sink.GetType().FullName, arg1);
                    }
                }
            }
        }

        public async Task InvokeAsync<T1,T2>(Func<TEvents, T1, T2, Task> dispatch, T1 arg1, T2 arg2)
        {
            foreach (var sink in _events)
            {
                try
                {
                    await dispatch(sink, arg1, arg2);
                }
                catch (Exception ex)
                {
                    if (_exceptionPolicy.ShouldThrow(ex))
                    {
                        throw;
                    }
                    else
                    {
                        LogException(ex, sink.GetType().FullName, arg1, arg2);
                    }
                }
            }
        }

        private void LogException(Exception ex, string method, params object[] args)
        {
            _logger.LogError(ex, "{Type} thrown from {Method} by {Exception}",
                typeof(TEvents).Name,
                method,
                ex.GetType().Name);
        }
    }
}
