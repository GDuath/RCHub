using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RCHub.Models.Dynmap;
using RCHub.Models.Entities;
using RCHub.Services;

namespace RCHub.Pages.Account
{
    public class ProfileModel : PageModel
    {
        private readonly DynmapService _dynmapService;
        private readonly TownService _townService;

        public ProfileModel(DynmapService dynmapService, TownService townService)
        {
            _dynmapService = dynmapService;
            _townService = townService;
        }

        public DynmapTownInfo MyDynmapTown { get; set; }
        public Town MyTown { get; set; }

        public async Task<IActionResult> OnGet()
        {
            MyDynmapTown = await _dynmapService.GetPlayerTown(User.Identity.Name);

            if (MyDynmapTown != null)
            {
                MyTown = await _townService.FindByNameAsync(MyDynmapTown.Name);
            }

            return Page();
        }
    }
}
