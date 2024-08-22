using FinShark.DTOs.Comment;
using FinShark.DTOs.Stock;
using FinShark.Models;
using FinShark.Services.CommentService;
using FinShark.Services.StockService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinShark.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IStockService _stockService;

        public CommentController(ICommentService commentService, IStockService stockService)
        {
            _commentService = commentService;
            _stockService = stockService;
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
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An error occured while processing your request",
                    Detail = e.Message
                });
            }

        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDto>> GetById([FromRoute] int id)
        {
            try
            {
                var comment = await _commentService.GetByIdAsync(id);

                if (comment == null)
                {
                    return NotFound($"no comment with {id} found");
                }
                return Ok(comment);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "An error occured while processing your request",
                    Detail = e.Message
                });
            }
        }
        [HttpPost("{stockId}")]
        public async Task<ActionResult<Comment>> Create([FromRoute] int stockId, [FromBody] CreateCommentRequestDto commentDto)
        {
            try
            {
                var result = await _commentService.CreateAsync(stockId,commentDto);
                if(!await _stockService.StockExists(stockId))
                {
                    return BadRequest("Stock does not exist");
                }
                if (result.Success)
                {
                    return Created($"/api/stock/{result.CreatedCommentId}", result);
                }

                return BadRequest(result);
            }
            catch (Exception e)
            {
                var errorResponse = new ErrorResponse
                {
                    Message = e.Message
                };

                return BadRequest(errorResponse);
            }
        }
    }   
}
