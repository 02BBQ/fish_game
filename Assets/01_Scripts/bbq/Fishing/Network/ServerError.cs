using System;

namespace fishing.Network
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T Data { get; }
        public Error Error { get; }
        
        private Result(bool isSuccess, T data, Error error)
        {
            IsSuccess = isSuccess;
            Data = data;
            Error = error;
        }
        
        public static Result<T> Success(T data) => new Result<T>(true, data, null);
        public static Result<T> Failure(Error error) => new Result<T>(false, default, error);
    }

    public class Error
    {
        public string Message { get; }
        public ErrorType Type { get; }
        public Exception Exception { get; }
        
        public Error(string message, ErrorType type, Exception exception = null)
        {
            Message = message;
            Type = type;
            Exception = exception;
        }
        
        public enum ErrorType
        {
            Network,
            Server,
            Validation,
            Unknown
        }
    }
} 