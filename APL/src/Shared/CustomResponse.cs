using APL.Models;
using Microsoft.AspNetCore.Http;

namespace APL.Shared
{

    public class CustomResponse<T>
    {
        public string? status { get; set; }      // "" / "error"
        public string? response { get; set; }    // human-readable message
        public int statusCode { get; set; }      // e.g., 200, 400, 404
        public T? data { get; set; }
    }

    public class ErrorResponse
    {        
        public string? status { get; set; }
        public string? message { get; set; }
        public string? detail { get; set; }      
    }
    public class ResultDto<T>
    {
        public string? status { get; set; }
        public string? message { get; set; }
        public T? result { get; set; }
    }
}
