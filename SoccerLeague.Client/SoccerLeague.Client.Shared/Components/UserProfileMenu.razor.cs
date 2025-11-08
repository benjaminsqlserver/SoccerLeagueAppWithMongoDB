using Microsoft.AspNetCore.Components;
using Radzen;
using SoccerLeague.Client.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SoccerLeague.Client.Shared.Components
{
    public partial class UserProfileMenu
    {
        private UserInfo? currentUser;

        [Inject]
        AuthenticationService AuthService { get; set; }
        [Inject] 
        NavigationManager Navigation {  get; set; }
        [Inject]
        DialogService DialogService { get; set; }

        [Inject]
        NotificationService NotificationService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            AuthService.OnAuthenticationStateChanged += HandleAuthStateChanged;
            await LoadUserInfo();
        }

        private async Task LoadUserInfo()
        {
            currentUser = await AuthService.GetCurrentUserAsync();
            StateHasChanged();
        }

        private void HandleAuthStateChanged()
        {
            _ = LoadUserInfo();
        }

        private async Task HandleLogout()
        {
            var confirmed = await DialogService.Confirm(
                "Are you sure you want to sign out?",
                "Confirm Logout",
                new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" }
            );

            if (confirmed == true)
            {
                await AuthService.LogoutAsync();

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = "Logged Out",
                    Detail = "You have been successfully logged out.",
                    Duration = 3000
                });

                Navigation.NavigateTo("/login", replace: true);
            }
        }

        public void Dispose()
        {
            AuthService.OnAuthenticationStateChanged -= HandleAuthStateChanged;
        }
    }
}
