namespace TraderBot.OrderController.Infrastructure;

public interface ITelegramService
{
    Task SendMessageAsync(string message, CancellationToken cancellationToken = default);
    Task SendMessageAsync(string message, string chatId, CancellationToken cancellationToken = default);
}