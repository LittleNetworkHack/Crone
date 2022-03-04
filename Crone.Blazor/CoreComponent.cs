namespace Crone.Blazor
{
	public class CoreComponent : ComponentBase, IAsyncDisposable
	{
		private Lazy<Task<IJSObjectReference>> lazyModule;

		[Inject]
		ILogger<CoreComponent> Logger { get; set; }

		[Inject]
		public IJSRuntime JSRuntime { get; set; }

		public CoreComponent()
		{
			lazyModule = new(InitializeModule);
		}

		protected virtual async Task<IJSObjectReference> InitializeModule()
		{
			/// When component is in same assembly as running web application
			/// Asm:	MyApp
			/// Type:	MyApp.Components.SubFolder.MyComponent
			/// Path:	wwwroot/Components/SubFolder/MyComponent.razor.js
			/// -------------------------------------------------------------------------
			/// When component is in referenced library
			/// Asm:	MyLib.Components
			/// Type:	MyLib.Components.SubFolder.MyComponent
			/// Path:	wwwroot/_content/MyLib.Components/SubFolder/MyComponent.razor.js
			var type = GetType();
			var pack = type.Assembly.GetName().Name;
			var root = Assembly.GetEntryAssembly() == type.Assembly ? "." : $"./_content/{pack}";
			var path = type.FullName.Remove(0, pack.Length).Replace('.', '/');
			var ext = ".razor.js";
			var file = root + path + ext;

			try
			{
				var result = await JSRuntime.InvokeAsync<IJSObjectReference>("import", file);
				await LogInfo_ModuleLoad(file);
				return result;
			}
			catch (Exception ex)
			{
				await LogError_ModuleLoad(ex, file);
				return null;
			}
		}

		public async ValueTask InvokeModuleVoidAsync(string method, params object[] args)
		{
			try
			{
				var module = await lazyModule.Value;
				await module.InvokeVoidAsync(method, args);
			}
			catch (Exception ex)
			{
				await LogError_MethodExec(ex, method);
			}
		}

		public async ValueTask<T> InvokeModuleAsync<T>(string method, params object[] args)
		{
			try
			{
				var module = await lazyModule.Value;
				return await module.InvokeAsync<T>(method, args);
			}
			catch (Exception ex)
			{
				await LogError_MethodExec(ex, method);
				return default;
			}
		}

		public virtual async ValueTask DisposeAsync()
		{
			if (lazyModule?.IsValueCreated is not true)
				return;

			var module = await lazyModule.Value;
			await module.DisposeAsync();
		}


		async Task LogInfo_ModuleLoad(string file)
		{
			var msg = $"Loaded module: {file}";
			Logger?.LogDebug(msg);
			await JSRuntime.LogDebugAsync(msg);
		}
		async Task LogError_ModuleLoad(Exception ex, string file)
		{
			var msg = $"Exception while loading module: {file}";
			Logger?.LogError(ex, msg);
			await JSRuntime.LogErrorAsync(msg);
		}
		async Task LogError_MethodExec(Exception ex, string method)
		{
			var msg = $"Exception while executing method: {method}";
			Logger.LogError(ex, msg);
			await JSRuntime.LogErrorAsync(msg);
		}
	}
}
