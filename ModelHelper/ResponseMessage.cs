namespace CompanyManagement.Helper
{
    public class ResponseMessage
    {
        public int MessageType { get; set; }
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public object ResponseValue { get; set; }
    }
}
