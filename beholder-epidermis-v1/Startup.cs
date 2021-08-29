namespace beholder_epidermis_v1
{
  using beholder_epidermis_v1.Cache;
  using beholder_epidermis_v1.Models;
  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.OpenApi.Models;

  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.Configure<EpidermisOptions>(Configuration.GetSection("Epidermis"));
      services.AddSingleton<ICacheClient, CacheClient>();

      services.AddControllers().AddDapr();
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "beholder_epidermis_v1", Version = "v1" });
      });

      services.AddHostedService<PulseService>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseDeveloperExceptionPage();
      app.UseSwagger(c =>
      {
        c.RouteTemplate = "/api/epidermis/{documentname}/swagger.json";
      });

      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("v1/swagger.json", "beholder_epidermis_v1 v1");
        c.RoutePrefix = "api/epidermis";
      });

      app.UseRouting();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
