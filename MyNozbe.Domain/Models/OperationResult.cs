namespace MyNozbe.Domain.Models
{
    public class OperationResult<T>
    {
        public OperationResult(T resultObject)
        {
            ResultObject = resultObject;
            StatusCode = OperationResultStatus.Ok;
        }

        public OperationResult(OperationResultStatus statusCode)
        {
            StatusCode = statusCode;
        }

        public OperationResult(string errorMessage, OperationResultStatus statusCode)
        {
            ErrorMessage = errorMessage;
            StatusCode = statusCode;
        }

        public T ResultObject { get; set; }

        public string ErrorMessage { get; set; }

        public OperationResultStatus StatusCode { get; set; }
    }

    public enum OperationResultStatus
    {
        Ok,
        NotFound,
        ValidationFailed
    }
}