using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace RCHub.Models.Entities
{
    public class Town : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Mayor { get; set; }
        public string? Thumbnail { get; set; }
        [MaxLength(255)]
        [Display(Name="Short Description (max 255 chars)")]
        public string? DescriptionShort { get; set; }
        [Display(Name="Long Description")]
        public string? DescriptionLong { get; set; }
        [Display(Name="RulerWiki Link")]
        public string? WikiLink { get; set; }
    }
}
