using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Leads.Domain;

namespace Leads.BackgroundService.Data.Models
{
    [Table("Log", Schema = "http")]
    public class HttpLog : Base
    {
        [Key]
        public long? LogId { get; init; }
        public string IpAddress { get; set; }
        public string Url { get; set; }
        public int StatusCode { get; set; }
        public Enums.HttpMethod MethodId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}