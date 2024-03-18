using Microsoft.AspNetCore.Mvc;



namespace PizzaApi.Utilities
{
    /// <summary>
    /// Simple helper that handles parsing Service status responses in cases where no output is expected
    /// </summary>
    public class ErrorHandler : ControllerBase
    {
        public ErrorHandler() 
        {
            
        }
        public IActionResult ErrorMessageHandler(ErrorMessage status)
        {
            if (status.IsError) return BadRequest(status.Message);
            return Ok();
        }
    }
}
