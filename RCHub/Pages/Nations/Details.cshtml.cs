using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RCHub.Models.Dynmap;
using RCHub.Models.Entities;
using RCHub.Services;

namespace RCHub.Pages.Nations
{
    public class DetailsModel : PageModel
    {
        private readonly DynmapService _dynmapService;
        private readonly TownService _townService;

        public DetailsModel(DynmapService dynmapService, TownService townService)
        {
            _dynmapService = dynmapService;
            _townService = townService;
        }


        public string Name { get; set; } = string.Empty;
        public string MapFile { get; set; } = string.Empty;

        public Dictionary<string, DynmapTownInfo> DynmapTownInfos = new();
        public List<Town> TownInfos = new();

        public async Task<IActionResult> OnGetAsync(string id = "Union")
        {
            Name = id.Replace('_', ' ');
            MapFile = id + ".png";

            await _dynmapService.GenerateNationMap(id);

            DynmapTownInfos = await _dynmapService.GetNationTowns(id);
            TownInfos = await _townService.GetByNamesAsync(DynmapTownInfos.Select(i => i.Key).ToList());

            return Page();
        }
    }
}
