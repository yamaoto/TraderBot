using Microsoft.AspNetCore.Mvc;
using TraderBot.EmailListener.Infrastructure;

namespace TraderBot.EmailListener.Controllers;

[Route("EmailBox")]
public class EmailBoxController : Controller
{
    [HttpPost("Setup")]
    public IActionResult Setup(MailBoxSettings? mailBoxSettings, [FromServices] MailBoxStore store)
    {
        if (mailBoxSettings == null || string.IsNullOrWhiteSpace(mailBoxSettings.MailBoxName) ||
            string.IsNullOrWhiteSpace(mailBoxSettings.Username) ||
            string.IsNullOrWhiteSpace(mailBoxSettings.AppPassword))
        {
            return BadRequest();
        }

        store.Set(mailBoxSettings);
        return Ok();
    }
}