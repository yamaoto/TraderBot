using Microsoft.Extensions.Options;

namespace TraderBot.OrderController.Infrastructure;

public class TelegramService : ITelegramService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TelegramService> _logger;
    private readonly TelegramOptions _options;

    public TelegramService(IOptions<TelegramOptions> options, ILogger<TelegramService> logger,
        HttpClient httpClient)
    {
        _options = options.Value;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        await SendMessageAsync(message, _options.DefaultChatId, cancellationToken);
    }

    public async Task SendMessageAsync(string message, string chatId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Send message to telegram {Message} {ChatId}", message, chatId);

        if (message == null) throw new ArgumentNullException(nameof(message));

        if (chatId == null) throw new ArgumentNullException(nameof(chatId));

        if (_options.Enabled)
        {
            var url =
                $"{_options.Url}/sendMessage?chat_id={chatId}&text={message}";

            try
            {
                using var result =
                    await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                if (!result.IsSuccessStatusCode)
                    _logger.LogError(
                        "Cannot send to Telegram: {StatusCode}, {ReasonPhrase}", (int)result.StatusCode,
                        result.ReasonPhrase);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cannot send to Telegram");
            }
        }
    }
}