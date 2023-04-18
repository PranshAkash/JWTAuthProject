using JWTAuthProject.AppCode.Enums;
using JWTAuthProject.AppCode.Interface;

namespace JWTAuthProject.Models
{
    public class Response<T> : IResponse<T>
    {
        public ResponseStatus StatusCode { get; set; } = ResponseStatus.Failed;
        public string? ResponseText { get; set; }
        public Exception Exception { get; set; }
        public T Result { get; set; }

        public Response()
        {
            StatusCode = ResponseStatus.Failed;
            ResponseText = ResponseStatus.Failed.ToString();
        }
    }

    public class Response : IResponse
    {
        public ResponseStatus StatusCode { get; set; }
        //public TxnStatus StatusCode { get; set; }
        public string? ResponseText { get; set; }
        public string? Msg { get; set; }

        public Response()
        {
            this.StatusCode = ResponseStatus.Failed;
            this.ResponseText = ResponseStatus.Failed.ToString();
        }
    }
    public class Request<T> : IRequest<T>
    {
        public string AuthToken { get; set; }
        public T Param { get; set; }
    }

}
