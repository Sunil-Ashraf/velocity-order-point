using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Domain.Common
{
    public class APIResponse
    {
        public static APIResponse Create()
        {
            return new APIResponse
            {
            };
        }
        public static APIResponse Create(bool success)
        {
            return new APIResponse
            {
                Success = success
            };
        }
        public static APIResponse Create(bool success, string message)
        {

            return new APIResponse
            {
                Success = success,
                Message = message,
            };
        }
        public static Response<T> Create<T>(T data)
        {
            return new Response<T>
            {
                Data = data
            };
        }
        public static Response<T> Create<T>(bool success, string message, T data)
        {
            return new Response<T>
            {
                Data = data,
                Success = success,
                Message = message,
            };
        }
        public static Response<T> Create<T>(bool success, T data)
        {
            return new Response<T>
            {
                Data = data,
                Success = success,
            };
        }

        public bool Success { get; set; } = true;
        public string Message { get; set; }
        public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.UtcNow;
    }
    public class Response<T> : APIResponse
    {
        public T Data { get; set; }
    }
}
