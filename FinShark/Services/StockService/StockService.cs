using FinShark.Data;
using FinShark.DTOs.Stock;
using FinShark.Mappers;
using FinShark.Models;
using Microsoft.EntityFrameworkCore;

namespace FinShark.Services.StockService
{
    public class StockService : IStockService
    {
        private readonly ApplicationDbContext _dbContext;

        public StockService(ApplicationDbContext dbConetxt)
        {
            _dbContext = dbConetxt;
        }

        public async Task<int> CreateNewStock(CreateStockRequestDto stock)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var stockModel = stock.ToStockFromCreateDto();
                    if (stockModel == null)
                    {
                        throw new ArgumentException("Invalid stock data");
                    }
                    await _dbContext.Stocks.AddAsync(stockModel);
                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();

                    return stockModel.Id;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new InvalidOperationException("Failed to create new stock");
                }
            }
        }

        public async Task<bool> DeleteStock(int id)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var deleteStock = await _dbContext.Stocks.FirstOrDefaultAsync(x => x.Id == id);
                    if (deleteStock == null)
                    {
                        return false;
                    }

                    _dbContext.Stocks.Remove(deleteStock);

                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }

            }

        }

        public async Task<IEnumerable<StockDto>> GetAll()
        {
            var stocks = await _dbContext.Stocks.ToListAsync();
            var stockDto = stocks.Select(s => s.ToStockDto());
            return stockDto;
        }

        public async Task<StockDto> GetStockById(int id)
        {
            var stock = await _dbContext.Stocks.FindAsync(id);

            if (stock == null)
            {
                throw new KeyNotFoundException($"Stock with ID {id} not found");
            }
            return stock.ToStockDto();
        }

        public async Task<bool> UpdateStock(int id, UpdateStockRequestDto stock)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var existingStock = await _dbContext.Stocks.FirstOrDefaultAsync(c => c.Id == id);
                    if (existingStock == null)
                    {
                        return false;
                    }

                    existingStock.Symbol = stock.Symbol;
                    existingStock.CompanyName = stock.CompanyName;
                    existingStock.Purchase = stock.Purchase;
                    existingStock.LastDiv = stock.LastDiv;
                    existingStock.Industry = stock.Industry;
                    existingStock.MarketCap = stock.MarketCap;

                    await _dbContext.SaveChangesAsync();
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }
    }
}
