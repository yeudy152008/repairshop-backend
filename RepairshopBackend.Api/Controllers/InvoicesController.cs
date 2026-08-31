using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairshopBackend.Application.DTOs;
using RepairshopBackend.Application.Interfaces;

namespace RepairshopBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoicesController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet]
    [Authorize(Policy = "invoices.read")]
    public async Task<IActionResult> GetAll()
    {
        var invoices = await _invoiceService.GetAllAsync();
        return Ok(invoices);
    }

    [HttpPost("direct")]
    [Authorize(Policy = "invoices.create")]
    public async Task<IActionResult> CreateDirect([FromBody] CreateDirectInvoiceDto dto)
    {
        var (invoice, error) = await _invoiceService.CreateDirectAsync(dto);
        if (error is not null)
        {
            return BadRequest(new { message = error });
        }
        return Ok(invoice);
    }

    [HttpPost("from-work-order")]
    [Authorize(Policy = "invoices.create")]
    public async Task<IActionResult> CreateFromWorkOrder([FromBody] CreateInvoiceFromOrderDto dto)
    {
        var (invoice, error) = await _invoiceService.CreateFromWorkOrderAsync(dto);
        if (error is not null)
        {
            return BadRequest(new { message = error });
        }
        return Ok(invoice);
    }
}