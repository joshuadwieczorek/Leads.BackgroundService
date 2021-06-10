using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Leads.BackgroundService.Data.Models
{
    [Table("LeadProviders")]
    public class LeadProvider : Base
    {
        [Key]
        public int? LeadProviderId { get; init; }
        public string Name { get; set; }
    }
}