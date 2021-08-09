namespace beholder_stalk_v2
{
    using beholder_stalk_v2.HardwareInterfaceDevices;
    using beholder_stalk_v2.Models;
    using Dapr.Client;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;

    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Keyboard>();
            //services.AddSingleton(new Mouse(Configuration));
            //services.AddSingleton(new Joystick(Configuration));

            services.AddGrpc();

            services.AddDaprClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCloudEvents();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<KeyboardService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });

            var daprClient = app.ApplicationServices.GetRequiredService<DaprClient>();
            var logger = app.ApplicationServices.GetRequiredService<ILogger<Startup>>();
            Task.Run(async () =>
            {
                var beholderStalk = new BeholderServiceInfo
                {
                    ServiceName = "stalk",
                    Version = "v2"
                };

                for (; ; )
                {
                    await Task.Delay(5000);
                    await daprClient.PublishEventAsync(Consts.PubSubName, "beholder/ctaf", beholderStalk);
                    logger.LogInformation("Published service info event.");
                }
            });
        }
    }
}
