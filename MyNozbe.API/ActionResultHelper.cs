using Microsoft.AspNetCore.Mvc;
using MyNozbe.Domain.Models;

namespace MyNozbe.API
{
    public class ActionResultHelper<T> : ControllerBase
    {
        public ActionResult<T> GetActionResult(OperationResult<T> operationResult, bool returnResultObject = true)
        {
            if (operationResult.StatusCode == OperationResultStatus.NotFound)
            {
                {
                    return NotFound();
                }
            }

            if (operationResult.StatusCode == OperationResultStatus.ValidationFailed)
            {
                {
                    return BadRequest(operationResult.ErrorMessage);
                }
            }

            if (operationResult.StatusCode == OperationResultStatus.Ok)
            {
                if (returnResultObject)
                {
                    return Ok(operationResult.ResultObject);
                }

                return NoContent();
            }

            return BadRequest();
        }
    }
}