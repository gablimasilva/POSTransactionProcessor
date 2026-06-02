namespace Processor.Domain.Exceptions {

    public class ExternalAuthorizationException : Exception {
        public ExternalAuthorizationException(string message) : base(message) { }
    }

}
