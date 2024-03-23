using System.ComponentModel.DataAnnotations.Schema;

namespace RCHub.Models.Entities
{
    public interface IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        string? Id { get; set; }
    }
}
