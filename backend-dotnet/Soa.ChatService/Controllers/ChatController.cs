using Microsoft.AspNetCore.Mvc;
using Soa.ChatService.Services;
using Soa.Contracts.Dtos;

namespace Soa.ChatService.Controllers;

[ApiController]
[Route("chat")]
public class ChatController : ControllerBase
{
    private readonly Soa.ChatService.Services.ChatService _chatService;
    private readonly RabbitMqPublisher _rabbitMqPublisher;

    public ChatController(Soa.ChatService.Services.ChatService chatService, RabbitMqPublisher rabbitMqPublisher)
    {
        _chatService = chatService;
        _rabbitMqPublisher = rabbitMqPublisher;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateChatDto dto)
    {
        var message = await _chatService.CreateMessageAsync(dto);
        await _rabbitMqPublisher.PublishChatMessageAsync(dto.AdId, dto.UserId, dto.Message);
        return Ok(message);
    }

    [HttpGet("{adId}")]
    public async Task<IActionResult> GetByAdId(string adId)
    {
        var messages = await _chatService.GetMessagesByAdIdAsync(adId);
        return Ok(messages);
    }
}
