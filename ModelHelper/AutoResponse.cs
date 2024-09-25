

using static CompanyManagement.Helper.Constrain;

namespace CompanyManagement.Helper
{
    public static class AutoResponse
    {
        //Success = 200, Unauthorized= 4001, NotFound = 4002, IPBlacklisted = 4003, ExceptionOccured = 4004, InvalidRequestFormat = 4005, RequiredParameterMissing = 4022
        public static ResponseMessage SuccessMessage(string message)
        {
            return new ResponseMessage { MessageType = (int)MesssageType.Success, StatusCode = (int)MesssageType.Success, StatusMessage = message };
        }
        public static ResponseMessage SuccessMessage(string message, object? ReturnObj)
        {
            return new ResponseMessage { MessageType = (int)MesssageType.Success, StatusCode = (int)MesssageType.Success, StatusMessage = message, ResponseValue = ReturnObj };
        }
        public static ResponseMessage SuccessMessage(object? ReturnObj)
        {
            return new ResponseMessage { MessageType = (int)MesssageType.Success, StatusCode = (int)MesssageType.Success, StatusMessage = "success", ResponseValue = ReturnObj };
        }
        public static ResponseMessage UpdateMessage(string message, object ReturnObj)
        {
            return new ResponseMessage { MessageType = (int)MesssageType.Success, StatusCode = (int)MesssageType.Success, StatusMessage = message, ResponseValue = ReturnObj };
        }
        public static ResponseMessage ExistMessage(string message)
        {
            return new ResponseMessage { MessageType = (int)MesssageType.DataExist, StatusCode = (int)MesssageType.DataExist, StatusMessage = message };
        }

        public static ResponseMessage NotFoundMessage(string message)
        {
            return new ResponseMessage { MessageType = (int)MesssageType.NotFound, StatusCode = (int)MesssageType.NotFound, StatusMessage = message };
        }
        public static ResponseMessage IPBlacklistedMessage()
        {
            return new ResponseMessage { MessageType = (int)MesssageType.IPBlacklisted, StatusCode = (int)MesssageType.IPBlacklisted, StatusMessage = "Unauthorized IP Address" };
        }
        public static ResponseMessage UnauthorizedMessage(string message)
        {
            return new ResponseMessage { MessageType = (int)MesssageType.Unauthorized, StatusCode = (int)MesssageType.Unauthorized, StatusMessage = message };
        }
        public static ResponseMessage ErrorMessage(string message)
        {
            return new ResponseMessage { MessageType = (int)MesssageType.Failed, StatusCode = (int)MesssageType.Failed, StatusMessage = message };
        }
        public static ResponseMessage UnknownErrorMessage(string message)
        {
            return new ResponseMessage { MessageType = (int)MesssageType.UnknownError, StatusCode = (int)MesssageType.UnknownError, StatusMessage = message };
        }
        public static ResponseMessage ExceptionOccuredMessage(string message)
        {
            return new ResponseMessage { MessageType = (int)MesssageType.ExceptionOccured, StatusCode = (int)MesssageType.ExceptionOccured, StatusMessage = message };
        }
        public static ResponseMessage InvalidRequestFormatMessage(string message)
        {
            return new ResponseMessage { MessageType = (int)MesssageType.InvalidRequestFormat, StatusCode = (int)MesssageType.InvalidRequestFormat, StatusMessage = message };
        }
        public static ResponseMessage RequiredParameterMissingMessage(string message)
        {
            return new ResponseMessage { MessageType = (int)MesssageType.RequiredParameterMissing, StatusCode = (int)MesssageType.RequiredParameterMissing, StatusMessage = message };
        }
    }
}
