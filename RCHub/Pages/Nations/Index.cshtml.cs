using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RCHub.Services;

namespace RCHub.Pages.Nations
{
    public class IndexModel : PageModel
    {
        private readonly DynmapService _dynmapService;

        public IndexModel(DynmapService dynmapService)
        {
            _dynmapService = dynmapService;
        }


        public List<string> Nations { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            Nations = await _dynmapService.GetDynmapNations();

            return Page();
        }
    }
}
