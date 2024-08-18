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
                var newStockId = await _stockService.CreateNewStock(stockDto);

                return Created($"/api/stock/{newStockId}", null);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateStockDto)
        { 
            var result = await _stockService.UpdateStock(id, updateStockDto);

            if(!result)
            {
                return NotFound();
            }

            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById([FromRoute] int id) 
        { 
            var result = await _stockService.DeleteStock(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
