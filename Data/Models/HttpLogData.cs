using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Leads.BackgroundService.Data.Models
{
    [Table("LogData", Schema = "http")]
    public class HttpLogData
    {
        [Key]
        public long? LogDataId { get; init; }
        public long? LogId { get; set; }
        public int? LogDataTypeId { get; set; }
        public String Data { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}