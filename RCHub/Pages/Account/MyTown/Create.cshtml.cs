using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using RCHub.Models.Entities;
using RCHub.Services;

namespace RCHub.Pages.Account.MyTown
{
    public class CreateModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly TownService _service;
        private readonly DynmapService _dynmapService;

        public CreateModel(IMapper mapper, TownService service, DynmapService dynmapService)
        {
            _mapper = mapper;
            _service = service;
            _dynmapService = dynmapService;
        }


        [BindProperty]
        [MaxLength(255)]
        [Display(Name = "Short Description (max 255 chars)")]
        public string? DescriptionShort { get; set; }
        [BindProperty]
        [Display(Name = "Long Description")]
        public string? DescriptionLong { get; set; }
        [BindProperty]
        [Display(Name = "RulerWiki Link")]
        public string? WikiLink { get; set; }

        public string DisplayName { get; set; }


        public async Task<IActionResult> OnGet()
        {
            var myTown = await _dynmapService.GetPlayerTown(User.Identity.Name) ?? throw new Exception("Your town could not be found!");

            this.DisplayName = myTown.Name;
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var myTown = await _dynmapService.GetPlayerTown(User.Identity.Name) ?? throw new Exception("Your town could not be found!");

            if (!ModelState.IsValid)
            {
                this.DisplayName = myTown.Name;
                return Page();
            }

            var item = new Town();
            _mapper.Map(this, item);    

            item.Name = myTown.Name;
            item.Mayor = User.Identity.Name;

            _service.Add(item);
            await _service.SaveChangesAsync();

            return RedirectToPage("/Account/Profile");
        }
    }
}
