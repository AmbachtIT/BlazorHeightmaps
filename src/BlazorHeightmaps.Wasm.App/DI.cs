namespace BlazorHeightmaps.Wasm.App
{
	public static class DI
	{

		public static IServiceCollection AddBlazorHeightmapsWasmApp(this IServiceCollection services, string baseAddress)
		{
			services.AddHttpClient("Default", client => client.BaseAddress = new Uri(baseAddress));
			return services;
		}

	}
}
