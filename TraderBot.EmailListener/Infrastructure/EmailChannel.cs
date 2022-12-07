using System.Threading.Channels;
using TraderBot.EmailListener.Commands;

namespace TraderBot.EmailListener.Infrastructure;

public class EmailChannel
{
    private readonly Channel<EmailMessage> _channel;

    public EmailChannel()
    {
        _channel = Channel.CreateUnbounded<EmailMessage>();
    }

    public ChannelReader<EmailMessage> ChannelReader => _channel.Reader;
    public ChannelWriter<EmailMessage> ChannelWriter => _channel.Writer;

}