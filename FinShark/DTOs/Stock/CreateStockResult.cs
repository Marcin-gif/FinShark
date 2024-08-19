namespace FinShark.DTOs.Stock
{
    public class CreateStockResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public int? CreatedStockId { get; set; }
    }
}
