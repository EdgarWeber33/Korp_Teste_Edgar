using Microsoft.AspNetCore.Mvc;
using ApiCorreta.Models;
using ApiCorreta.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace ApiCorreta.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotaFiscalController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;

    public NotaFiscalController(AppDbContext context, HttpClient httpClient )
    {
        _context = context;
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("http://localhost:5182" ); // Porta da ApiEstoque
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
        if (nota == null) return NotFound("Nota fiscal não encontrada");
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
        if (nota.Status != "Aberta") return BadRequest("Não é possível adicionar produtos a uma nota fechada");

        nota.Itens.Add(item);
        await _context.SaveChangesAsync();
        return Ok(nota);
    }

    [HttpPost("{id}/imprimir")]
    public async Task<IActionResult> Imprimir(int id)
    {
        var nota = await _context.NotasFiscais.Include(n => n.Itens).FirstOrDefaultAsync(n => n.Id == id);
        if (nota == null) return NotFound("Nota fiscal não encontrada");
        if (nota.Status != "Aberta") return BadRequest("Esta nota fiscal já está fechada. Não é possível imprimir novamente.");

        // Lista para armazenar itens que tiveram o estoque reduzido com sucesso
        var itensComEstoqueReduzido = new List<ItemNotaFiscal>();

        foreach (var item in nota.Itens)
        {
            var reduzirSaldoDto = new { Quantidade = item.Quantidade };
            var content = new StringContent(JsonSerializer.Serialize(reduzirSaldoDto), Encoding.UTF8, "application/json");
            
            // Chama a ApiEstoque para reduzir o saldo
            var response = await _httpClient.PutAsync($"api/produto/{item.ProdutoId}/reduzir-saldo", content );

            if (!response.IsSuccessStatusCode)
            {
                // Se a baixa de estoque falhar para qualquer item, reverter os itens já processados
                foreach (var itemReverter in itensComEstoqueReduzido)
                {
                    var reverterSaldoDto = new { Quantidade = -itemReverter.Quantidade }; // Adiciona de volta ao estoque
                    var reverterContent = new StringContent(JsonSerializer.Serialize(reverterSaldoDto), Encoding.UTF8, "application/json");
                    await _httpClient.PutAsync($"api/produto/{itemReverter.ProdutoId}/reduzir-saldo", reverterContent );
                }
                return BadRequest($"Falha ao reduzir estoque para o produto ID {item.ProdutoId}. Erro: {await response.Content.ReadAsStringAsync()}");
            }
            itensComEstoqueReduzido.Add(item);
        }

        // Fechar a nota
        nota.Status = "Fechada";
        await _context.SaveChangesAsync();

        // Simular processamento de impressão
        await Task.Delay(1000); // Espera 1 segundo para simular impressão

        return Ok(new { mensagem = "Nota fiscal impressa e estoque atualizado!", nota });
    }
}
