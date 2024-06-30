using Microsoft.AspNetCore.Mvc;
using IEmailService = SmtpAPI.EmailService.IEmailService;

namespace SmptAPI.Controllers;

[ApiController]
[Route("api/message")]
public class MessageController : ControllerBase
{
    [HttpGet("{fileId}")]
    public async Task<IActionResult> Index([FromRoute] Guid fileId)
    {
        var file = await GetFile(fileId);

        return Ok(file);
    }

    private async Task<Stream> GetFile(Guid fileId)
    {
        HttpClient httpClient = new HttpClient();

        var response = await httpClient.GetAsync($"https://localhost:7208/api/documents/{fileId}");

        return await response.Content.ReadAsStreamAsync();
    }
}
