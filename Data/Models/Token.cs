using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Leads.BackgroundService.Data.Models
{
    [Table("Tokens")]
    public class Token : Base
    {
        [Key]
        public Guid? TokenId { get; set; }
        public int? RooftopId { get; set; }
        public int? LeadProviderId { get; set; }
        public string Configuration { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsRolling { get; set; }

        [ForeignKey("LeadProviderId")]
        public LeadProvider LeadProvider { get; set; }

        public bool IsExpired
            => ExpiresAt.HasValue && DateTime.Now >= ExpiresAt.Value;
    }
}