using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace System.Linq.Dynamic.Core.SandCastle
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment environment)
        {
            bool isDev = environment.IsDevelopment();

            app.UseStatusCodePages();

            if (isDev)
                app.UseDeveloperExceptionPage();

            // Serve my app-specific default file, if present.
            DefaultFilesOptions options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(options);

            // Add static files to the request pipeline.
            app.UseStaticFiles();
        }
    }
}