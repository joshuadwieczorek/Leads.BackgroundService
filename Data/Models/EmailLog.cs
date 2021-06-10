using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Leads.BackgroundService.Data.Models
{
    [Table("EmailLog")]
    public class EmailLog : Base
    {
        [Key]
        public long EmailLogId { get; init; }
        public string From { get; set; }
        public string Recipients { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public Guid? AdfId { get; set; }
        public DateTime SentAt { get; set; }        

        [ForeignKey("AdfId")]
        public Adf Adf { get; set; }
    }
}