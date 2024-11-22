namespace BuildingBlocks.MessageBus;

public class Message
{
    public Message()
    {
    }

    public Message(string to, string subject, string body)
    {
        To = to;
        Subject = subject;
        Body = body;
    }

    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}