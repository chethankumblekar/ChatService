using System.Net;
using System.Runtime.Serialization;

namespace PlayGround.ChatService
{
    [Serializable]
    internal class HttpResponseException : Exception
    {
        private HttpStatusCode internalServerError;

        public HttpResponseException()
        {
        }

        public HttpResponseException(HttpStatusCode internalServerError)
        {
            this.internalServerError = internalServerError;
        }

        public HttpResponseException(string? message) : base(message)
        {
        }

        public HttpResponseException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected HttpResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}