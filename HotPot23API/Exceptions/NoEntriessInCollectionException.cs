namespace HotPot23API.Exceptions
{
    public class NoEntriessInCollectionException : Exception
    {
        private string _message;
        public NoEntriessInCollectionException()
        {
            _message = "Collection is empty";
        }
        public override string Message => _message;
    }
}
