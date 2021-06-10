using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Leads.BackgroundService.Data.Models
{
    public class Queue : Base
    {
        public long QueueId { get; init; }
        public string Token { get; set; }
        public string LeadInformation { get; set; }
        public int QueueStatusId { get; set; }
        public DateTime? QueuedAt { get; set; }
        public DateTime? PickedUpAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public int ProcessAttempts { get; set; }

        [ForeignKey("QueueStatusId")]
        internal QueueStatus QueueStatus { get; set; }
    }
}