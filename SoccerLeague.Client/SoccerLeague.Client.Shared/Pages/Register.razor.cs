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
    public partial class Register
    {
        private RegisterRequest registerRequest = new();
        private bool isLoading = false;
        private bool acceptTerms = false;
        private bool showTermsError = false;
        private Variant variant = Variant.Outlined;
        private string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@!%*?&])[A-Za-z\d@!%*?&]{8,}$";

        [Inject]
        private AuthenticationService? AuthService { get; set; }

        [Inject] 
        NavigationManager Navigation { get; set; }
        [Inject]
        NotificationService NotificationService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            // Check if already authenticated
            if (AuthService != null && await AuthService.IsAuthenticatedAsync())
            {
                Navigation.NavigateTo("/", replace: true);
            }
        }

        private async Task HandleRegister()
        {
            showTermsError = false;

            if (!acceptTerms)
            {
                showTermsError = true;
                ShowErrorNotification("Please accept the terms and conditions");
                return;
            }

            if (AuthService == null)
            {
                ShowErrorNotification("Authentication service not available");
                return;
            }

            isLoading = true;
            StateHasChanged();

            try
            {
                var result = await AuthService.RegisterAsync(registerRequest);

                if (result.Success)
                {
                    ShowSuccessNotification("Registration successful! Please check your email to verify your account.");

                    // Small delay to show the success message
                    await Task.Delay(1500);

                    // Navigate to login
                    Navigation.NavigateTo("/login", replace: true);
                }
                else
                {
                    ShowErrorNotification(result.Message);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format("An unexpected error occurred: {0}", ex.Message);
                ShowErrorNotification(errorMessage);
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private void ShowSuccessNotification(string message)
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Summary = "Success",
                Detail = message,
                Duration = 6000
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
    }
}
