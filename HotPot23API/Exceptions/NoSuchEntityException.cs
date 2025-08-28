namespace HotPot23API.Exceptions
{
    public class NoSuchEntityException : Exception
    {
        private string _message;
        public NoSuchEntityException()
        {
            _message = "Entity with the given Id not present";
        }
        public override string Message => _message;
    }
}
