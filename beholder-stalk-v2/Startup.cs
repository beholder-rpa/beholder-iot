namespace beholder_stalk_v2
{
  using beholder_stalk_v2.HardwareInterfaceDevices;
  using beholder_stalk_v2.Services;
  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Http;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;

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
      services.AddLogging(options =>
      {
        options.AddSimpleConsole(c =>
        {
          c.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
        });
      });

      services.AddSingleton<Keyboard>();
      services.AddSingleton<Mouse>();
      services.AddSingleton<Joystick>();

      services.AddTransient<IHumanInterfaceService, KeyboardService>();
      services.AddTransient<IHumanInterfaceService, MouseService>();
      services.AddTransient<IHumanInterfaceService, JoystickService>();

      services.AddGrpc();

      services.AddDaprClient();

      services.AddHostedService<PulseService>();
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
        endpoints.MapGrpcService<StalkGrpcService>();

        endpoints.MapGet("/", async context =>
          {
            await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
          });
      });
    }
  }
}