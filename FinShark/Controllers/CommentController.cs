using FinShark.Services.CommentService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinShark.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _commentService.GetAllAsync();

                if (result == null)
                {
                    return NotFound("No comment found");
                }

                return Ok(result);
            }
            catch(Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An error occured while processing your request",
                    Detail = e.Message
                });
            }
            
        }
    }
}
