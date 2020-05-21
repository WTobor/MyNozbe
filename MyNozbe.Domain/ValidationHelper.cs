using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using MyNozbe.Domain.Models;

namespace MyNozbe.Domain
{
    public static class ValidationHelper
    {
        public static OperationResult<T> GetValidationFailedOperationResult<T>(ValidationResult result)
        {
            var validationErrors = GetValidationErrorMessage(result.Errors);
            return OperationResult<T>.ValidationFailed(validationErrors);
        }

        public static string GetValidationErrorMessage(IEnumerable<ValidationFailure> errors)
        {
            return string.Join(";", errors.Select(x => x.ErrorMessage));
        }
    }
}