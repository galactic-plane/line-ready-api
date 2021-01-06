using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace LineReadyApi.Models
{
    public class ApiError
    {
        public ApiError()
        {
        }

        public ApiError(string message)
        {
            Message = message;
        }

        public ApiError(ModelStateDictionary modelState)
        {
            Message = "Invalid parameters.";
            Detail = modelState
                .FirstOrDefault(x => x.Value.Errors.Any()).Value.Errors
                .FirstOrDefault().Exception.StackTrace;
        }

        public string Detail { get; set; }

        public string Message { get; set; }
    }
}