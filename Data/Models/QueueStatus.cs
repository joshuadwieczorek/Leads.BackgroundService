namespace Leads.BackgroundService.Data.Models
{
    public class QueueStatus : Base
    {
        public int? QueueStatusId { get; init; }
        public string QueueStatusName { get; }
    }
}