namespace CompanyManagement.ModelHelper
{
    public static class EnumHelper
    {
        public enum PropertyConstant
        {
            Inactive = 0,
            Active,
            Deleted,
        }
        public enum TicketStatus
        {
            Inactive = 0,
            Created,
            Accepted,
            Transferred,
            Declined,
            Escalated,
            Completed,
            Deleted,
        }
    }
    public enum PropertyConstant
    {
        Inactive = 0,
        Active,
        Deleted,
        Blocked
    }

    public enum TicketStatus
    {
        Inactive = 0,
        Created,
        Accepted,
        Transferred,
        Declined,
        Escalated,
        Completed,
        Deleted,
    }
}
