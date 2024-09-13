using FinShark.Data;
using FinShark.DTOs.Comment;
using FinShark.DTOs.Stock;
using FinShark.Mappers;
using FinShark.Models;
using Microsoft.EntityFrameworkCore;

namespace FinShark.Services.CommentService
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _dbContext;

        public CommentService(ApplicationDbContext dbContext) {

            _dbContext = dbContext;
        }

        public async Task<CreatedCommentResult> CreateAsync(int stockId, CreateCommentRequestDto commentDto)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            { 
                try {
                    var commentModel = commentDto.ToCommentFromCreateDto(stockId);
                    if(commentModel == null)
                    {
                        return new CreatedCommentResult 
                        {
                            Success = false,
                            Message = "Comment cannot be created",
                            ErrorCode = "CREATE_ERROR"
                        };
                    }
                    await _dbContext.Comments.AddAsync(commentModel);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new CreatedCommentResult
                    {
                        Success = true,
                        Message = "Comment created successfully",
                        CreatedCommentId = commentModel.Id,
                    };
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    return new CreatedCommentResult
                    {
                        Success = false,
                        Message = "Database update failed",
                        ErrorCode = "DB_UPDATE_ERROR"
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new CreatedCommentResult
                    {
                        Success = false,
                        Message = "An unexpected error occurred",
                        ErrorCode = "UNEXPECTED_ERROR"
                    };
                }
            }
        }

        public async Task<IEnumerable<CommentDto>> GetAllAsync()
        {
            var result = await _dbContext.Comments.ToListAsync();
            var comments = result.Select(s=>s.ToCommentDto());
            return comments;
        }

        public async Task<CommentDto> GetByIdAsync(int id)
        {
            var result  = await _dbContext.Comments.FirstOrDefaultAsync(s=>s.Id == id);
            if (result == null)
            {
                throw new KeyNotFoundException($"Comment with ID {id} not found");
            }
            var comment = result.ToCommentDto();
            return comment;
        }

        public async Task<UpdateCommentResult> UpdateAsync(int commentId, UpdateCommentRequestDto commentDto)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var result = await _dbContext.Comments.FirstOrDefaultAsync(s => s.Id == commentId);
                    if (result == null)
                    {
                        return new UpdateCommentResult
                        {
                            Success = false,
                            Message = "Comment cannot be updated",
                            ErrorCode = "CREATE_ERROR"
                        };
                    }
                    result.Title = commentDto.Title;
                    result.Content = commentDto.Content;

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new UpdateCommentResult
                    {
                        Success = true,
                        Message = "Comment updated successfully",

                    };
                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    return new UpdateCommentResult
                    {
                        Success = false,
                        Message = "Database update failed",
                        ErrorCode = "DB_UPDATE_ERROR"
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new UpdateCommentResult
                    {
                        Success = false,
                        Message = "An unexpected error occurred",
                        ErrorCode = "UNEXPECTED_ERROR"
                    };
                }
            }
        }
        public async Task<bool> CommentExists(int id)
        {
            return await _dbContext.Comments.AnyAsync(c => c.Id == id);
        }

        public async Task<UpdateCommentResult> DeleteAsync(int commentId)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var result = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
                    if (result == null) {
                        return new UpdateCommentResult
                        {
                            Success = false,
                            Message = "Comment not found",
                            ErrorCode = "NOT_FOUND"
                        };
                    }

                    _dbContext.Comments.Remove(result);

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new UpdateCommentResult
                    {
                        Success = true,
                        Message = "Stock deleted successfully"
                    };

                }
                catch (DbUpdateException dbEx)
                {
                    await transaction.RollbackAsync();
                    return new UpdateCommentResult
                    {
                        Success = false,
                        Message = "Database update failed",
                        ErrorCode = "DB_UPDATE_ERROR"
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new UpdateCommentResult
                    {
                        Success = false,
                        Message = "An unexpected error occurred",
                        ErrorCode = "UNEXPECTED_ERROR"
                    };
                }
            }
        }
    }
}
