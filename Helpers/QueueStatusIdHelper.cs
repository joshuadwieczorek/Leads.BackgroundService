namespace Leads.BackgroundService.Helpers
{
    public static class QueueStatusIdHelper
    {
        public static int Calculate(
              int queueStatusId
            , bool errorOccurred = false)
        {
            if (queueStatusId == 2 && errorOccurred)
                return 4;

            else if (queueStatusId == 2)
                return 3;

            else if (queueStatusId == 6 && errorOccurred)
                return 8;

            else if (queueStatusId == 6)
                return 7;

            else
                return 4;
        }
    }
}