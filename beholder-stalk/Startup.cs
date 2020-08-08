namespace beholder_stalk
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IConfigurationRoot configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.AddConsole(o =>
                      {
                          o.TimestampFormat = o.TimestampFormat = "[HH:mm:ss] ";
                      });
                builder.AddDebug();
            });

            services.AddSingleton(Configuration);
            services.AddSingleton(new Keyboard(Configuration));
            services.AddSingleton(new Mouse(Configuration));
            services.AddSingleton(new Joystick(Configuration));
            
            var senseHatFramebufferDevPath = LedMatrix.GetFrameBufferDevicePath();
            if (!string.IsNullOrWhiteSpace(senseHatFramebufferDevPath))
            {
                services.AddSingleton(new LedMatrix(senseHatFramebufferDevPath));
            }

            services.AddSingleton<IBeholderStalk, BeholderStalk>();
        }
    }
}
