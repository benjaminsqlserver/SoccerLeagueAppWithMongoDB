using Microsoft.AspNetCore.Components;
using SoccerLeague.Client.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SoccerLeague.Client.Shared.Pages
{
    public partial class Home
    {
        [Inject]
        private NavigationManager? Navigation { get; set; }

        [Inject]
        AuthenticationService AuthService { get; set; }

        private UserInfo? currentUser;

        protected override async Task OnInitializedAsync()
        {
            currentUser = await AuthService.GetCurrentUserAsync();
        }
    }
}
