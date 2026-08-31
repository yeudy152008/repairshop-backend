using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairshopBackend.Application.DTOs;
using RepairshopBackend.Application.Interfaces;

namespace RepairshopBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PurchasesController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;

    public PurchasesController(IPurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    [HttpGet]
    [Authorize(Policy = "purchases.read")]
    public async Task<IActionResult> GetAll()
    {
        var purchases = await _purchaseService.GetAllAsync();
        return Ok(purchases);
    }

    [HttpPost]
    [Authorize(Policy = "purchases.create")]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseDto dto)
    {
        var (purchase, error) = await _purchaseService.CreateAsync(dto);
        if (error is not null)
        {
            return BadRequest(new { message = error });
        }
        return Ok(purchase);
    }
}