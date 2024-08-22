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
        private readonly ApplicationDbContext _dbConetxt;

        public CommentService(ApplicationDbContext dbContext) {

            _dbConetxt = dbContext;
        }

        public async Task<CreatedCommentResult> CreateAsync(int stockId, CreateCommentRequestDto commentDto)
        {
            using (var transaction = _dbConetxt.Database.BeginTransaction())
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
                    await _dbConetxt.Comments.AddAsync(commentModel);
                    await _dbConetxt.SaveChangesAsync();
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
            var result = await _dbConetxt.Comments.ToListAsync();
            var comments = result.Select(s=>s.ToCommentDto());
            return comments;
        }

        public async Task<CommentDto> GetByIdAsync(int id)
        {
            var result  = await _dbConetxt.Comments.FirstOrDefaultAsync(s=>s.Id == id);
            if (result == null)
            {
                throw new KeyNotFoundException($"Comment with ID {id} not found");
            }
            var comment = result.ToCommentDto();
            return comment;
        }
    }
}
