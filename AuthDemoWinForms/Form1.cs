using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AuthDemoWinForms
{
    public partial class Form1 : Form
    {
        private static bool loggeIn = false;
        private static readonly string[] scopes = { "user.read" };
        
        public Form1()
        {
            InitializeComponent();
        }

        private string GetCurrentPrincipal()
        {
            var currentUser = WindowsIdentity.GetCurrent();
            return currentUser.Name;

        }

        private async void Button1_Click(object sender, EventArgs e)
        {
            if (!loggeIn)
            {
                //Windows Auth
                WindowsIdentity current = WindowsIdentity.GetCurrent();
                WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
                label1.Text = windowsPrincipal.Identity.Name;

                var authResult = await Login();
                label4.Text = authResult.Account.Username;

                button1.Text = "Log Out";
                loggeIn = true;
            }
            else
            {
                await Logout();
                button1.Text = "Log In";
                loggeIn = false;
            }

        }

        private async Task<AuthenticationResult> Login()
        {
            AuthenticationResult authResult = null;
            var accounts = await Program.PublicClientApp.GetAccountsAsync();
            var firstAccount = accounts.FirstOrDefault();

            try
            {
                authResult = await Program.PublicClientApp.AcquireTokenSilent(scopes, firstAccount)
                    .ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                // A MsalUiRequiredException happened on AcquireTokenSilent.
                // This indicates you need to call AcquireTokenInteractive to acquire a token
                System.Diagnostics.Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

                try
                {
                    authResult = await Program.PublicClientApp.AcquireTokenInteractive(scopes)
                        .WithAccount(accounts.FirstOrDefault())
                        .WithPrompt(Prompt.SelectAccount)
                        .ExecuteAsync();
                }
                catch (MsalException msalex)
                {
                    label1.Text = $"Error Acquiring Token:{System.Environment.NewLine}{msalex}";
                }
            }
            catch (Exception ex)
            {
                label1.Text = $"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}";
            }
            return authResult;
        }

        private async Task Logout()
        {

            var accounts = await Program.PublicClientApp.GetAccountsAsync();
            if (accounts.Any())
            {
                try
                {
                    await Program.PublicClientApp.RemoveAsync(accounts.FirstOrDefault());
                    this.label1.Text = "User has signed-out";
                    this.label4.Text = "User has signed-out";
                }
                catch (MsalException ex)
                {
                    throw new Exception($"Error signing-out user: {ex.Message}");
                }
            }
        }
    }
}
