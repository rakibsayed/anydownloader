using Microsoft.Extensions.DependencyInjection;
using AnyDownloader.Core.Interfaces;
using AnyDownloader.Infrastructure;
using System.Windows;

namespace AnyDownloader.Presentation
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register services and dependencies
            services.AddSingleton<IHttpDownloader, HttpDownloader>();
            services.AddSingleton<IDownloadManager, DownloadManager>();

            // Register the MainWindow with DI
            services.AddSingleton<MainWindow>();
        }

    }
}
