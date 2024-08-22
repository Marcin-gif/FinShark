using FinShark.DTOs.Stock;
using FinShark.Models;
using System.Collections.Generic;

namespace FinShark.Services.StockService
{
    public interface IStockService
    {
        Task<IEnumerable<StockDto>> GetAll();
        Task<StockDto> GetStockById(int id);
        Task<CreateStockResult> CreateNewStock(CreateStockRequestDto stock);
        Task<UpdateStockResult> UpdateStock(int id, UpdateStockRequestDto stock);
        Task<DeleteStockResult> DeleteStock(int id);
        Task<bool> StockExists(int id);
    }
}
