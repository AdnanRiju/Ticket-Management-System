namespace CompanyManagement.Helper
{
    public static class Constrain
    {
        public enum MesssageType { Success = 200, Failed = 400, Unauthorized = 4001, IPBlacklisted = 4002, NotFound = 4003, DataExist = 4004, InvalidRequestFormat = 4005, RequiredParameterMissing = 4006, ExceptionOccured = 4007, UnknownError = 4010 }
    }
}
