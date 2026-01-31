namespace ReassessmentApp.Application.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public int StatusCode { get; set; }

        public ApiResponse(int statusCode, string message, T? data = default, bool success = true)
        {
            StatusCode = statusCode;
            Message = message;
            Data = data;
            Success = success;
        }

        public static ApiResponse<T> SuccessResponse(T data, int statusCode = 200, string message = "Success")
        {
            return new ApiResponse<T>(statusCode, message, data, true);
        }

        public static ApiResponse<T> FailureResponse(int statusCode, string message)
        {
            return new ApiResponse<T>(statusCode, message, default, false);
        }
    }
}
