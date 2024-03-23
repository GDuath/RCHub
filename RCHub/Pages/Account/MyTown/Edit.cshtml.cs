using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using RCHub.Models.Entities;
using RCHub.Services;

namespace RCHub.Pages.Account.MyTown
{
    public class EditModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly TownService _service;
        private readonly DynmapService _dynmapService;

        public EditModel(IMapper mapper, TownService service, DynmapService dynmapService)
        {
            _mapper = mapper;
            _service = service;
            _dynmapService = dynmapService;
        }

        [BindProperty]
        public string Id { get; set; }

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


        public async Task<IActionResult> OnGet(string id)
        {
            if (id == null) return new NotFoundResult();
            
            var item = await _service.FindAsync(id);
            if (item == null) return NotFound();

            _mapper.Map(item, this);

            this.DisplayName = item.Name;
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var item = await _service.FindAsync(this.Id);

            if (!ModelState.IsValid)
            {
                this.DisplayName = item.Name;
                return Page();
            }

            _mapper.Map(this, item);    

            _service.Update(item);
            await _service.SaveChangesAsync();

            return RedirectToPage("/Account/Profile");
        }
    }
}
