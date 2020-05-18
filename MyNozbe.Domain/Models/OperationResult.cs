namespace MyNozbe.Domain.Models
{
    public class OperationResult<T>
    {
        public T ResultObject { get; set; }

        public string ErrorMessage { get; set; }

        public OperationResultStatus StatusCode { get; set; }

        public static OperationResult<T> Ok()
        {
            return new OperationResult<T>()
            {
                StatusCode = OperationResultStatus.Ok
            };
        }

        public static OperationResult<T> Ok(T resultObject)
        {
            return new OperationResult<T>()
            {
                ResultObject = resultObject,
                StatusCode = OperationResultStatus.Ok
            };
        }

        public static OperationResult<T> NotFound()
        {
            return new OperationResult<T>()
            {
                StatusCode = OperationResultStatus.NotFound
            };
        }

        public static OperationResult<T> ValidationFailed(string errorMessage)
        {
            return new OperationResult<T>()
            {
                ErrorMessage = errorMessage,
                StatusCode = OperationResultStatus.ValidationFailed
            };
        }
    }

    public enum OperationResultStatus
    {
        Ok,
        NotFound,
        ValidationFailed
    }
}