using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MagicVilla_VillaAPI.Filters
{
    public class CustomExceptionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if(context.Exception is FileNotFoundException fileNotFoundException)
            {
                context.Result = new ObjectResult("File not found but handled in filter.")
                {
                    StatusCode = 503
                };
                // this will notify that exception has being handled. no need to run ErrorHandlingController
                context.ExceptionHandled = true; 
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}
