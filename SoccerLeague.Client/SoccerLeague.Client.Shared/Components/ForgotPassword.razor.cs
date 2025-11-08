using Microsoft.AspNetCore.Components;
using Radzen;
using SoccerLeague.Client.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SoccerLeague.Client.Shared.Components
{
    public partial class ForgotPassword
    {
        private ForgotPasswordRequest request = new();
        private bool isLoading = false;
        private bool emailSent = false;

       [Inject]
        NavigationManager Navigation { get; set; }
      [Inject]
       NotificationService NotificationService { get; set; }
       [Inject]
        HttpClient HttpClient { get; set; }

        private async Task HandleSubmit()
        {
            isLoading = true;
            StateHasChanged();

            try
            {
                // Determine the correct endpoint based on platform
                var endpoint = GetApiEndpoint("/Auth/forgot-password");

                var response = await HttpClient.PostAsJsonAsync(endpoint, request);

                if (response.IsSuccessStatusCode)
                {
                    emailSent = true;
                    ShowSuccessNotification("Password reset instructions sent to your email.");
                }
                else
                {
                    var error = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                    ShowErrorNotification(error?.Message ?? "Failed to send reset email.");
                }
            }
            catch (Exception ex)
            {
                ShowErrorNotification($"An error occurred: {ex.Message}");
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private string GetApiEndpoint(string path)
        {
            // For Web (BFF), use relative path
            // For MAUI, this would be absolute URL - but typically ForgotPassword doesn't need auth
            // so it can go through BFF or direct API
            return path; // Adjust based on your configuration
        }

        private void ShowSuccessNotification(string message)
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Summary = "Success",
                Detail = message,
                Duration = 5000
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

        public class ForgotPasswordRequest
        {
            public string Email { get; set; } = string.Empty;
        }
    }
}
