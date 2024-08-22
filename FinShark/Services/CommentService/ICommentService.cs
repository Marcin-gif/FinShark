using FinShark.DTOs.Comment;
using FinShark.Models;

namespace FinShark.Services.CommentService
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentDto>> GetAllAsync();
        Task<CommentDto> GetByIdAsync(int id);
        Task<CreatedCommentResult> CreateAsync(int stockId, CreateCommentRequestDto commentDto);
            
    }
}
