using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crone.Blazor
{
	public static class JSRuntimeExtensions
	{
        public static async ValueTask LogDebugAsync(this IJSRuntime runtime, string message)
        {
            if (runtime is null)
                return;

            await runtime.InvokeVoidAsync("console.debug", message);
        }
        public static async ValueTask LogInfoAsync(this IJSRuntime runtime, string message)
        {
            if (runtime is null)
                return;

            await runtime.InvokeVoidAsync("console.log", message);
        }
        public static async ValueTask LogWarnAsync(this IJSRuntime runtime, string message)
        {
            if (runtime is null)
                return;

            await runtime.InvokeVoidAsync("console.warn", message);
        }
        public static async ValueTask LogErrorAsync(this IJSRuntime runtime, string message)
        {
            if (runtime is null)
                return;

            await runtime.InvokeVoidAsync("console.error", message);
        }

        public static async ValueTask AlertAsync(this IJSRuntime runtime, string message)
        {
            if (runtime is null)
                return;

            await runtime.InvokeVoidAsync("alert", message);
        }
        public static async ValueTask<T> PromptAsync<T>(this IJSRuntime runtime, string message, string placeholder = "Value")
		{
            if (runtime is null)
                return default;

            return await runtime.InvokeAsync<T>("prompt", message, placeholder);
		}
    }
}
