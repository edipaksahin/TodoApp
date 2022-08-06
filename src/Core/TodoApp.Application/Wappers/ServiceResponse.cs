using System;
using System.Collections.Generic;
using System.Text;

namespace TodoApp.Application.Wappers
{
    public class ServiceResponse<T> : BaseResponse
    {
        public T Data { get; set; }

        public ServiceResponse(T data, string message = "Succsess", bool success = true)
        {
            Data = data;
            Message = message;
            Success = success;
        }
    }
}
