using FinShark.Data;
using FinShark.DTOs.Comment;
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

        public async Task<IEnumerable<CommentDto>> GetAllAsync()
        {
            var result = await _dbConetxt.Comments.ToListAsync();
            var comments = result.Select(s=>s.ToCommentDto());
            return comments;
        }
    }
}
