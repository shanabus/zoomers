using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZoomersClient.Shared.Services;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Plk.Blazor.DragDrop;
using Blazored.LocalStorage;

namespace ZoomersClient.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            
            builder.Services.AddBlazorDragDrop();
            builder.Services.AddSpeechSynthesis();
            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddSingleton<GameService>();

            await builder.Build().RunAsync();
        }
    }
}
