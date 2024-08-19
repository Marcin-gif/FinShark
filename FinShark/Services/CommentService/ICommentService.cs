using FinShark.DTOs.Comment;
using FinShark.Models;

namespace FinShark.Services.CommentService
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentDto>> GetAllAsync();
    }
}
