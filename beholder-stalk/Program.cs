namespace beholder_stalk
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System.Threading;
    using System.Threading.Tasks;

    class Program
    {
        private static CancellationTokenSource _ctsSource;

        private static IBeholderStalk _beholderStalk;

        static async Task Main()
        {
            // Stand up DI
            var services = new ServiceCollection();
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            var configuration = builder.Build();
            var startup = new Startup(configuration);
            startup.ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();

            // Configure the beholder stalk
            _beholderStalk = serviceProvider.GetRequiredService<IBeholderStalk>();
            await _beholderStalk.Connect();

            using (_ctsSource = new CancellationTokenSource())
            {
                // wait for process application termination.
                await Task.Delay(Timeout.Infinite, _ctsSource.Token).ContinueWith(_ => { }, TaskContinuationOptions.OnlyOnCanceled);
            }
        }
    }
}
