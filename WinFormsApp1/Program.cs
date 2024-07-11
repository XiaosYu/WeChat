using Microsoft.Extensions.DependencyInjection;
using WinFormsApp1.Services;
namespace WinFormsApp1
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var service = new ServiceCollection();

            service.AddScoped<MainForm>();
            service.AddScoped<IDataConverter, DotNetDataConverter>();


            var serviceProvider = service.BuildServiceProvider();

            ApplicationConfiguration.Initialize();
            Application.Run(serviceProvider.GetRequiredService<MainForm>());
        }


    }
}