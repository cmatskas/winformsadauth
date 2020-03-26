using Microsoft.Identity.Client;
using System;
using System.Windows.Forms;

namespace AuthDemoWinForms
{
    static class Program
    {

        public static string ClientId = Environment.GetEnvironmentVariable("ClientId", EnvironmentVariableTarget.User);
        public static string Tenant = Environment.GetEnvironmentVariable("Tenant", EnvironmentVariableTarget.User);
        private static IPublicClientApplication clientApp;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            InitializeAuth();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static IPublicClientApplication PublicClientApp { get { return clientApp; } }

        private static void InitializeAuth()
        {

            clientApp = PublicClientApplicationBuilder.Create(ClientId)
                    .WithRedirectUri("https://login.microsoftonline.com/common/oauth2/nativeclient")
                    .WithAuthority(AzureCloudInstance.AzurePublic, Tenant)
                    .Build();
            TokenCacheHelper.EnableSerialization(clientApp.UserTokenCache);
        }
    }
}