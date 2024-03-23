using System.ComponentModel.DataAnnotations.Schema;

namespace RCHub.Models.Entities
{
    public class AppUser : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Username { get; set; }
        public ulong DiscordId { get; set; }

    }
}
