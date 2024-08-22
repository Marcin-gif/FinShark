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

        public async Task<CreateStockResult> CreateNewStock(CreateStockRequestDto stock)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var stockModel = stock.ToStockFromCreateDto();
                    if (stockModel == null)
                    {
                        return new CreateStockResult 
                        { 
                            Success = false,
                            Message = "Stock cannot be created",
                            ErrorCode = "CREATE_ERROR"
                        };
                    }
                    await _dbContext.Stocks.AddAsync(stockModel);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new CreateStockResult
                    { 
                        Success = true,
                        Message = "Stock created successfully",
                        CreatedStockId = stockModel.Id
                    };
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    return new CreateStockResult
                    {
                        Success = false,
                        Message = "Database update failed",
                        ErrorCode = "DB_UPDATE_ERROR"
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new CreateStockResult
                    {
                        Success = false,
                        Message = "An unexpected error occurred",
                        ErrorCode = "UNEXPECTED_ERROR"
                    };
                }
            }
        }

        public async Task<DeleteStockResult> DeleteStock(int id)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var deleteStock = await _dbContext.Stocks.FirstOrDefaultAsync(x => x.Id == id);
                    if (deleteStock == null)
                    {
                        return new DeleteStockResult
                        {
                            Success = false,
                            Message = "Stock not found",
                            ErrorCode = "NOT_FOUND"
                        };
                    }

                    _dbContext.Stocks.Remove(deleteStock);

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new DeleteStockResult
                    {
                        Success = true,
                        Message = "Stock deleted successfully"
                    };
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    return new DeleteStockResult
                    {
                        Success = false,
                        Message = "Database update failed",
                        ErrorCode = "DB_UPDATE_ERROR"
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new DeleteStockResult
                    {
                        Success = false,
                        Message = "An unexpected error occurred",
                        ErrorCode = "UNEXPECTED_ERROR"
                    };
                }

            }

        }

        public async Task<IEnumerable<StockDto>> GetAll()
        {
            var stocks = await _dbContext.Stocks.Include(c=>c.Comments).ToListAsync();
            var stockDto = stocks.Select(s => s.ToStockDto());
            return stockDto;
        }

        public async Task<StockDto> GetStockById(int id)
        {
            var stock = await _dbContext.Stocks.Include(c => c.Comments).FirstOrDefaultAsync(c=>c.Id==id);

            if (stock == null)
            {
                throw new KeyNotFoundException($"Stock with ID {id} not found");
            }
            return stock.ToStockDto();
        }

        public async Task<bool> StockExists(int id)
        {
            return await _dbContext.Stocks.AnyAsync(c => c.Id==id);
        }

        public async Task<UpdateStockResult> UpdateStock(int id, UpdateStockRequestDto stock)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var existingStock = await _dbContext.Stocks.FirstOrDefaultAsync(c => c.Id == id);
                    if (existingStock == null)
                    {
                        return new UpdateStockResult
                        {
                            Success = false,
                            Message = "Stock not found",
                            ErrorCode = "NOT_FOUND"
                        };
                    }

                    existingStock.Symbol = stock.Symbol;
                    existingStock.CompanyName = stock.CompanyName;
                    existingStock.Purchase = stock.Purchase;
                    existingStock.LastDiv = stock.LastDiv;
                    existingStock.Industry = stock.Industry;
                    existingStock.MarketCap = stock.MarketCap;

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new UpdateStockResult 
                    { 
                        Success = true,
                        Message = "Stock updated successfully"
                    };
                }
                catch (DbUpdateException ex)
                {
                    await transaction.RollbackAsync();
                    return new UpdateStockResult 
                    { 
                       Success = false, 
                       Message = "Database update failed", 
                       ErrorCode = "DB_UPDATE_ERROR" 
                    };
                }
                catch(Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new UpdateStockResult
                    {
                        Success = false,
                        Message = "An unexpected error occured",
                        ErrorCode = "UNEXPECTED_ERROR"
                    };
                }
            }
        }
    }
}
