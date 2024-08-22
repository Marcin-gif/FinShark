namespace FinShark.DTOs.Comment
{
    public class CreatedCommentResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public int? CreatedCommentId { get; set; }
    }
}
