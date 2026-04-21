using Microsoft.AspNetCore.Mvc;
using ApiCorreta.Models;
using ApiCorreta.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiCorreta.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotaFiscalController : ControllerBase
{
    private readonly AppDbContext _context;

    public NotaFiscalController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var notas = await _context.NotasFiscais.Include(n => n.Itens).ToListAsync();
        return Ok(notas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var nota = await _context.NotasFiscais.Include(n => n.Itens).FirstOrDefaultAsync(n => n.Id == id);
        if (nota == null) return NotFound();
        return Ok(nota);
    }

    [HttpPost]
    public async Task<IActionResult> Create()
    {
        var ultimaNota = await _context.NotasFiscais.OrderByDescending(n => n.Numero).FirstOrDefaultAsync();
        int proximoNumero = (ultimaNota?.Numero ?? 0) + 1;

        var novaNota = new NotaFiscal
        {
            Numero = proximoNumero,
            Status = "Aberta",
            Itens = new List<ItemNotaFiscal>()
        };
        
        _context.NotasFiscais.Add(novaNota);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = novaNota.Id }, novaNota);
    }

    [HttpPost("{id}/produto")]
    public async Task<IActionResult> AdicionarProduto(int id, [FromBody] ItemNotaFiscal item)
    {
        var nota = await _context.NotasFiscais.Include(n => n.Itens).FirstOrDefaultAsync(n => n.Id == id);
        if (nota == null) return NotFound("Nota não encontrada");
        if (nota.Status != "Aberta") return BadRequest("Nota já está fechada");

        nota.Itens.Add(item);
        await _context.SaveChangesAsync();
        return Ok(nota);
    }

    [HttpPost("{id}/imprimir")]
    public async Task<IActionResult> Imprimir(int id)
    {
        var nota = await _context.NotasFiscais.Include(n => n.Itens).FirstOrDefaultAsync(n => n.Id == id);
        if (nota == null) return NotFound("Nota não encontrada");
        if (nota.Status != "Aberta") return BadRequest("Nota já está fechada");

        foreach (var item in nota.Itens)
        {
            var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.Id == item.ProdutoId);
            if (produto != null)
            {
                if (produto.Saldo < item.Quantidade)
                    return BadRequest($"Saldo insuficiente para {produto.Descricao}");
                
                produto.Saldo -= item.Quantidade;
            }
        }

        nota.Status = "Fechada";
        await _context.SaveChangesAsync();

        await Task.Delay(1000); // Simular processamento
        return Ok(new { mensagem = "Nota fiscal impressa e estoque atualizado!", nota });
    }
}
