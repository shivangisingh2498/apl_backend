namespace APL.Shared
{

    public class CustomResponse<T>
    {
        public string? status { get; set; }      // "success" / "error"
        public string? response { get; set; }    // human-readable message
        public int statusCode { get; set; }      // e.g., 200, 400, 404
        public T? data { get; set; }
    }

}
