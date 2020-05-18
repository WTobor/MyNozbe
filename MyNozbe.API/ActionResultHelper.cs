using Microsoft.AspNetCore.Mvc;
using MyNozbe.Domain.Models;

namespace MyNozbe.API
{
    public static class ActionResultHelper<T>
    {
        public static ActionResult<T> GetActionResult(OperationResult<T> operationResult,
            bool returnResultObject = true)
        {
            switch (operationResult.StatusCode)
            {
                case OperationResultStatus.NotFound:
                {
                    return new NotFoundResult();
                }

                case OperationResultStatus.ValidationFailed:
                {
                    return new BadRequestObjectResult(operationResult.ErrorMessage);
                }

                case OperationResultStatus.Ok:
                    if (returnResultObject)
                    {
                        return new OkObjectResult(operationResult.ResultObject);
                    }

                    return new NoContentResult();
                default:
                    return new BadRequestResult();
            }
        }
    }
}