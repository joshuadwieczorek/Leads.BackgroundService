using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Leads.BackgroundService.Data.Models
{
    [Table("Adfs")]
    public class Adf : Base
    {
        [Key]
        public Guid? AdfId { get; init; }

        [Column("Adf")]
        public String AdfXml { get; set; }
    }
}