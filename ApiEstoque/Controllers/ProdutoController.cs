using Microsoft.AspNetCore.Mvc;
using ApiCorreta.Models;

namespace ApiCorreta.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutoController : ControllerBase
{
    private static List<Produto> _produtos = new();
    private static int _nextId = 1;

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_produtos);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var produto = _produtos.FirstOrDefault(p => p.Id == id);
        if (produto == null)
        {
            return NotFound($"Produto com ID {id} não encontrado");
        }
        return Ok(produto);
    }

    [HttpPost]
    public IActionResult Create(Produto produto)
    {
        produto.Id = _nextId++;
        _produtos.Add(produto);
        return CreatedAtAction(nameof(GetById), new { id = produto.Id }, produto);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Produto produtoAtualizado)
    {
        var produto = _produtos.FirstOrDefault(p => p.Id == id);
        if (produto == null)
        {
            return NotFound($"Produto com ID {id} não encontrado");
        }

        produto.Codigo = produtoAtualizado.Codigo;
        produto.Descricao = produtoAtualizado.Descricao;
        produto.Saldo = produtoAtualizado.Saldo;

        return Ok(produto);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var produto = _produtos.FirstOrDefault(p => p.Id == id);
        if (produto == null)
        {
            return NotFound($"Produto com ID {id} não encontrado");
        }

        _produtos.Remove(produto);
        return NoContent();
    }

    // Método para permitir que outros controllers acessem a lista de produtos
    public static List<Produto> ObterListaProdutos()
    {
        return _produtos;
    }


    

}