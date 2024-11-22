using BuildingBlocks.MessageBus;

namespace Cinema.API.Helpers;

/// <summary>
///     Factory class for creating different types of messages related to ticket purchases and cancellations.
/// </summary>
public static class MessageFactory
{
    /// <summary>
    ///     Creates a message indicating that a ticket has been purchased.
    /// </summary>
    /// <param name="customer">The customer who purchased the ticket.</param>
    /// <param name="screenings">The screenings for which the ticket was purchased.</param>
    /// <returns>A message indicating that the ticket has been purchased.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the customer or screenings are null.</exception>
    /// <exception cref="ArgumentException">Thrown when the screenings collection contains null elements.</exception>
    public static Message CreateTicketPurchasedMessage(Customer customer, IEnumerable<Screening> screenings)
    {
        ArgumentNullException.ThrowIfNull(customer);

        if (screenings is null || !screenings.Any()) throw new ArgumentNullException(nameof(screenings));
        if (screenings.Any(s => s == null))
            throw new ArgumentException("Screenings collection contains null elements", nameof(screenings));

        if (screenings.Count() == 1)
            return CreateGenericTicketPurchasedMessage(customer, screenings.First());

        var movieTitles = string.Join(", ", screenings.Select(s => s.Movie.Title));
        var content =
            $"Hi {customer.Name},\nYou have successfully purchased a ticket for the screenings {movieTitles}.";
        return new Message(customer.Email, "Ticket purchased", content);
    }


    /// <summary>
    ///     Creates a message indicating that a ticket has been cancelled.
    /// </summary>
    /// <param name="customer">The customer who cancelled the ticket.</param>
    /// <param name="screening">The screening for which the ticket was cancelled.</param>
    /// <returns>A message indicating that the ticket has been cancelled.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the customer or screening is null.</exception>
    public static Message CreateTicketCancelledMessage(Customer customer, Screening screening)
    {
        ArgumentNullException.ThrowIfNull(customer, nameof(customer));
        ArgumentNullException.ThrowIfNull(screening, nameof(screening));

        var content =
            $"Hi {customer.Name},\nYou have successfully cancelled the ticket for the screening {screening.Movie.Title} on {screening.Date}.";
        return new Message(customer.Email, "Ticket cancelled", content);
    }


    /// <summary>
    ///     Creates a message indicating that a single ticket has been purchased.
    /// </summary>
    /// <param name="customer">The customer who purchased the ticket.</param>
    /// <param name="screening">The screening for which the ticket was purchased.</param>
    /// <param name="seat">The seat for which the ticket was purchased.</param>
    /// <returns>A message indicating that the ticket has been purchased.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the customer, screening, or seat is null.</exception>
    public static Message CreateSingleTicketPurchasedMessage(Customer customer, Screening screening, Seat seat)
    {
        ArgumentNullException.ThrowIfNull(customer, nameof(customer));
        ArgumentNullException.ThrowIfNull(screening, nameof(screening));
        ArgumentNullException.ThrowIfNull(seat, nameof(seat));

        var content =
            $"Hi {customer.Name},\nYou have successfully purchased a ticket for the screening {screening.Movie.Title}, seat {seat.Number}{seat.Row} on {screening.Date}.";
        return new Message(customer.Email, "Ticket purchased", content);
    }

    public static Message CreateGenericTicketPurchasedMessage(Customer customer, Screening screening)
    {
        ArgumentNullException.ThrowIfNull(customer, nameof(customer));
        ArgumentNullException.ThrowIfNull(screening, nameof(screening));

        var content =
            $"Hi {customer.Name},\nYou have successfully purchased a ticket for the screening {screening.Movie.Title} on {screening.Date}.";
        return new Message(customer.Email, "Ticket purchased", content);
    }
}