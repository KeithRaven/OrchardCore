using System;
using System.Linq;
using System.Threading.Tasks;

namespace OrchardCore.Modules
{
    public interface IEventDispatcher<TEvents, out TCategoryName>
    {
        Task InvokeAsync<T1>(Func<TEvents, T1, Task> dispatch, T1 arg1);
    }
}
