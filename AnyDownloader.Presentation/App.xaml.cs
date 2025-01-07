using Microsoft.Extensions.DependencyInjection;
using AnyDownloader.Core.Interfaces;
using AnyDownloader.Infrastructure;
using AnyDownloader.Infrastructure.Utilities;
using AnyDownloader.Core.Services;
using AnyDownloader.Presentation.Views; 
using System;
using System.Windows;


namespace AnyDownloader.Presentation
{
    /// <summary>
    /// Represents the entry point for the WPF application and handles dependency injection setup.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Provides the application's service provider for dependency injection.
        /// </summary>
        public IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// Configures services and builds the service provider for dependency injection.
        /// </summary>
        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            // Register a global exception handler for unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            DispatcherUnhandledException += OnDispatcherUnhandledException;
        }

        /// <summary>
        /// Configures application services and registers dependencies for dependency injection.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        private void ConfigureServices(IServiceCollection services)
        {
            // Register ILogger
            services.AddSingleton<ILogger, LoggingProvider>();

            // Register FileSystem
            services.AddSingleton<IFileSystem, FileSystem>();

            // Register DownloadManager
            services.AddSingleton<IDownloadManager, DownloadManager>();

            // Register HttpClient with IHttpDownloader
            services.AddHttpClient<IHttpDownloader, HttpDownloader>();

            // Register Views
            services.AddSingleton<MainWindow>();
            services.AddTransient<DownloaderPage>();
        }


        /// <summary>
        /// Handles the application startup event.
        /// Initializes and displays the main window.
        /// </summary>
        /// <param name="e">Startup event arguments.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Launch MainWindow
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        /// <summary>
        /// Handles unhandled exceptions in the current AppDomain.
        /// Logs critical errors for analysis.
        /// </summary>
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var logger = ServiceProvider.GetRequiredService<ILogger>();
            logger.LogError("Critical application error occurred.", e.ExceptionObject as Exception);
        }


        /// <summary>
        /// Handles unhandled exceptions in the WPF Dispatcher.
        /// Logs UI-related errors for analysis.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var logger = ServiceProvider.GetRequiredService<ILogger>();
            logger.LogError("Unhandled UI error occurred.", e.Exception);

            // Prevent application crash
            e.Handled = true;
        }
    }
}
