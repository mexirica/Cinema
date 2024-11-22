
namespace Cinema.API.Exceptions
{
    public class ScreeningAlreadyPassedException : ConflictException
    {
        public ScreeningAlreadyPassedException()
            : base("The screening date has already passed.")
        {
        }

        public ScreeningAlreadyPassedException(string message)
            : base(message)
        {
        }
    }

}
