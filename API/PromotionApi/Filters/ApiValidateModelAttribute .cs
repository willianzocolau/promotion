using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PromotionApi
{
    public class ApiValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
                context.Result = new BadRequestObjectResult(Utils.Error("Invalid model")/*context.ModelState*/);
            base.OnActionExecuting(context);
        }
    }
}
