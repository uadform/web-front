namespace web_front.DTOs.Responses
{
    public class ErrorResponse
    {
        public required int Status { get; set; }
        public string? Message { get; set; }
    }
}
