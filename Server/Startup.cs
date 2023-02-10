using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using ZoomersClient.Server.Hubs;
using ZoomersClient.Server.Services;
using Microsoft.EntityFrameworkCore;
using ZoomersClient.Shared.Data;
using ZoomersClient.Shared.Services;
using AutoMapper;
using ZoomersClient.Shared.Models;

namespace ZoomersClient.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR()
                .AddJsonProtocol(options => {
                    options.PayloadSerializerOptions.PropertyNamingPolicy = null;
                });
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddResponseCompression(opts => 
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream"});
            });

            services.AddAutoMapper(typeof(Startup));
            
            // EF Migrations
            // https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli

            // https://docs.microsoft.com/en-us/ef/core/dbcontext-configuration/#constructor-argument
            services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("ZoomersClient.Server"))
                .EnableSensitiveDataLogging(), ServiceLifetime.Scoped);
                        
            services.AddSingleton<WordPlay>();
            services.AddSingleton<Phrases>();
            services.AddScoped<GameService>();
            services.AddScoped<QuestionService>();
            services.AddTransient<GameConfiguration>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chathub");
                endpoints.MapHub<GameHub>("/gamehub");
                endpoints.MapHub<DrawHub>("/drawhub");
                endpoints.MapHub<WordPlayHub>("/wordplayhub");
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
