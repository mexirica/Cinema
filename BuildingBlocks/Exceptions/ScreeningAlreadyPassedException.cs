namespace Cinema.API.Screenings.Exceptions
{
	public class ScreeningAlreadyPassedException : InvalidOperationException
	{
		public ScreeningAlreadyPassedException()
				: base("The screening date has already passed.")
		{
		}

		public ScreeningAlreadyPassedException(string message)
				: base(message)
		{
		}

		public ScreeningAlreadyPassedException(string message, Exception innerException)
				: base(message, innerException)
		{
		}
	}

}
