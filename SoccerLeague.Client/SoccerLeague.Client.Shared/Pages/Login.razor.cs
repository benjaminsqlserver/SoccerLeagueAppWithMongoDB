using Microsoft.AspNetCore.Components;
using Radzen;
using SoccerLeague.Client.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SoccerLeague.Client.Shared.Pages
{
    public partial class Login
    {
        private LoginRequest loginRequest = new();
        private bool isLoading = false;
        private bool showSocialLogin = false; // Set to true to enable Google login
        private Variant variant = Variant.Outlined;

        [Inject] NavigationManager Navigation { get; set; }
        [Inject] NotificationService NotificationService { get; set; }

        [Inject]
        private AuthenticationService? AuthService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            // Check if already authenticated
            if (AuthService != null && await AuthService.IsAuthenticatedAsync())
            {
                Navigation.NavigateTo("/", replace: true);
            }
        }

        private async Task HandleLogin()
        {
            if (AuthService == null)
            {
                ShowErrorNotification("Authentication service not available");
                return;
            }

            isLoading = true;
            StateHasChanged();

            try
            {
                var result = await AuthService.LoginAsync(loginRequest);

                if (result.Success)
                {
                    ShowSuccessNotification("Login successful!");

                    // Navigate to home or return URL
                    var returnUrl = Navigation.Uri.Contains("returnUrl")
                        ? GetReturnUrl()
                        : "/";

                    Navigation.NavigateTo(returnUrl, replace: true);
                }
                else
                {
                    ShowErrorNotification(result.Message);
                }
            }
            catch (Exception ex)
            {
                ShowErrorNotification($"An unexpected error occurred: {ex.Message}");
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private async Task HandleGoogleLogin()
        {
            // TODO: Implement Google OAuth login
            ShowInfoNotification("Google login not yet implemented");
            await Task.CompletedTask;
        }

        private string GetReturnUrl()
        {
            var uri = new Uri(Navigation.Uri);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            return query["returnUrl"] ?? "/";
        }

        private void ShowSuccessNotification(string message)
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Summary = "Success",
                Detail = message,
                Duration = 4000
            });
        }

        private void ShowErrorNotification(string message)
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Error",
                Detail = message,
                Duration = 6000
            });
        }

        private void ShowInfoNotification(string message)
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Info,
                Summary = "Info",
                Detail = message,
                Duration = 4000
            });
        }
    }
}
