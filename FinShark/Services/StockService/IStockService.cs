using FinShark.DTOs.Stock;
using FinShark.Models;
using System.Collections.Generic;

namespace FinShark.Services.StockService
{
    public interface IStockService
    {
        Task<IEnumerable<StockDto>> GetAll();
        Task<StockDto> GetStockById(int id);
        Task<int> CreateNewStock(CreateStockRequestDto stock);
        Task<bool> UpdateStock(int id, UpdateStockRequestDto stock);
        Task<bool> DeleteStock(int id);
    }
}
