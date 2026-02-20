using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soa.AdsService.Services;
using Soa.Contracts.Dtos;

namespace Soa.AdsService.Controllers;

[ApiController]
[Route("ads")]
public class AdsController : ControllerBase
{
    private readonly Soa.AdsService.Services.AdsService _adsService;
    private readonly KafkaProducerService _kafkaProducer;

    public AdsController(Soa.AdsService.Services.AdsService adsService, KafkaProducerService kafkaProducer)
    {
        _adsService = adsService;
        _kafkaProducer = kafkaProducer;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateAdDto dto)
    {
        var ad = await _adsService.CreateAsync(dto);
        await _kafkaProducer.PublishAdNotificationAsync(ad.Id, ad.Title);
        return Ok(ad);
    }

    [Authorize]
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var ads = await _adsService.GetAllAsync();
        return Ok(ads);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOne(string id)
    {
        var ad = await _adsService.GetByIdAsync(id);
        if (ad is null) return NotFound();
        return Ok(ad);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateAdDto dto)
    {
        var ad = await _adsService.UpdateAsync(id, dto);
        if (ad is null) return NotFound();
        return Ok(ad);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var ad = await _adsService.DeleteAsync(id);
        if (ad is null) return NotFound();
        return Ok(ad);
    }
}
