using Furion;
using Microsoft.Extensions.DependencyInjection;
using WinFormsApp1;

namespace QQSendMessage
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            var services = Inject.Create();
            services.Build();

            ApplicationConfiguration.Initialize();
            Application.Run(new QQSendMessageFrm());
        }
    }
}