using Microsoft.AspNetCore.Mvc;
using ApiCorreta.Models;
using ApiCorreta.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiCorreta.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutoController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutoController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _context.Produtos.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.Id == id);
        if (produto == null)
        {
            return NotFound($"Produto com ID {id} não encontrado");
        }
        return Ok(produto);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Produto produto)
    {
        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = produto.Id }, produto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Produto produtoAtualizado)
    {
        var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.Id == id);
        if (produto == null)
        {
            return NotFound($"Produto com ID {id} não encontrado");
        }

        produto.Codigo = produtoAtualizado.Codigo;
        produto.Descricao = produtoAtualizado.Descricao;
        produto.Saldo = produtoAtualizado.Saldo;
        _context.Produtos.Update(produto);
        await _context.SaveChangesAsync();

        return Ok(produto);
    }

    // Novo endpoint para reduzir o saldo do produto
    [HttpPut("{id}/reduzir-saldo")]
    public async Task<IActionResult> ReduzirSaldo(int id, [FromBody] ReduzirSaldoDto reduzirSaldoDto)
    {
        var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.Id == id);
        if (produto == null)
        {
            return NotFound($"Produto com ID {id} não encontrado");
        }

        if (produto.Saldo < reduzirSaldoDto.Quantidade)
        {
            return BadRequest($"Saldo insuficiente para o produto {produto.Descricao}. Disponível: {produto.Saldo}, Necessário: {reduzirSaldoDto.Quantidade}");
        }

        produto.Saldo -= reduzirSaldoDto.Quantidade;
        _context.Produtos.Update(produto);
        await _context.SaveChangesAsync();

        return Ok(produto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.Id == id);
        if (produto == null)
        {
            return NotFound($"Produto com ID {id} não encontrado");
        }

        _context.Produtos.Remove(produto);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
