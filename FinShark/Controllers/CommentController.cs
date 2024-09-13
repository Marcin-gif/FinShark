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
                if(!ModelState.IsValid)
                    return BadRequest(ModelState);

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
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CommentDto>> GetById([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

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
        [HttpPost("{stockId:int}")]
        public async Task<ActionResult<Comment>> Create([FromRoute] int stockId, [FromBody] CreateCommentRequestDto commentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!await _stockService.StockExists(stockId))
                {
                    return BadRequest("Stock does not exist");
                }

                var result = await _commentService.CreateAsync(stockId, commentDto);
                
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

        [HttpPut("{commentId:int}")]
        public async Task<ActionResult<Comment>> Update([FromRoute] int commentId, [FromBody] UpdateCommentRequestDto commentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!await _commentService.CommentExists(commentId))
                {
                    return BadRequest("Comment does not exist");
                }
                
                var result = await _commentService.UpdateAsync(commentId, commentDto);

                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch(Exception e)
            {
                var errorResponse = new ErrorResponse
                {
                    Message= e.Message
                };
                return BadRequest(errorResponse);
            }
        }

        [HttpDelete("{commentId:int}")]
        public async Task<IActionResult> Delete([FromRoute] int commentId)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (! await _commentService.CommentExists(commentId))
                {
                    return BadRequest("Comment does not exist");
                }

                var result = await _commentService.DeleteAsync(commentId);

                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch(Exception e)
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
