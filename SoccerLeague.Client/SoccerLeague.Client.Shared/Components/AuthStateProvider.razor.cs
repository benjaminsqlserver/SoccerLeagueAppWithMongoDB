using Microsoft.AspNetCore.Components;
using SoccerLeague.Client.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SoccerLeague.Client.Shared.Components
{
    public partial class AuthStateProvider
    {
        [Inject]
        AuthenticationService AuthService { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public bool RequiresAuth { get; set; } = false;

        [Inject]
        private NavigationManager? Navigation { get; set; }

        private bool isLoading = true;
        private bool isAuthenticated = false;
        private bool requiresAuth => RequiresAuth;

        protected override async Task OnInitializedAsync()
        {
            AuthService.OnAuthenticationStateChanged += HandleAuthStateChanged;
            await CheckAuthenticationState();
        }

        private async Task CheckAuthenticationState()
        {
            isLoading = true;
            StateHasChanged();

            try
            {
                isAuthenticated = await AuthService.IsAuthenticatedAsync();
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private void HandleAuthStateChanged()
        {
            _ = CheckAuthenticationState();
        }

        private void NavigateToLogin()
        {
            if (Navigation != null)
            {
                var returnUrl = Navigation.Uri;
                Navigation.NavigateTo($"/login?returnUrl={Uri.EscapeDataString(returnUrl)}");
            }
        }

        public void Dispose()
        {
            AuthService.OnAuthenticationStateChanged -= HandleAuthStateChanged;
        }
    }
}
