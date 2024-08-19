using FinShark.Data;
using FinShark.DTOs.Stock;
using FinShark.Models;
using FinShark.Services.StockService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinShark.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StockController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Stock>>> GetAll()
        {
            var stocks = await _stockService.GetAll();

            return Ok(stocks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Stock>> GetById([FromRoute] int id)
        {
            try
            {
                var stock = await _stockService.GetStockById(id);

                return Ok(stock);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Stock>> Create([FromBody] CreateStockRequestDto stockDto)
        {
            try
            {
                var result = await _stockService.CreateNewStock(stockDto);

                if(result.Success)
                {
                    return Created($"/api/stock/{result.CreatedStockId}", result);
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

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateStockDto)
        { 
            var result = await _stockService.UpdateStock(id, updateStockDto);

            if(!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById([FromRoute] int id) 
        { 
            var result = await _stockService.DeleteStock(id);
            if (result.Success)
            {
                return NoContent();
            }

            return NotFound(result);
        }
    }
}
