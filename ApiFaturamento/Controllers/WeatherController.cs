using Microsoft.AspNetCore.Mvc;

namespace ApiCorreta.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    // Lista em memória para armazenar as previsões
    private static List<WeatherForecast> _forecasts = new();

    // Construtor para inicializar com dados exemplo
    public WeatherController()
    {
        if (_forecasts.Count == 0)
        {
            var summaries = new[] 
            { 
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", 
                "Warm", "Balmy", "Hot", "Sweltering", "Scorching" 
            };

            for (int i = 1; i <= 5; i++)
            {
                _forecasts.Add(new WeatherForecast(
                    DateOnly.FromDateTime(DateTime.Now.AddDays(i)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ));
            }
        }
    }

    // GET: api/weather - Listar todas as previsões
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_forecasts);
    }

    // GET: api/weather/{id} - Buscar uma previsão pelo ID
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        if (id < 0 || id >= _forecasts.Count)
        {
            return NotFound($"Previsão com ID {id} não encontrada");
        }
        return Ok(_forecasts[id]);
    }

    // POST: api/weather - Criar uma nova previsão
    [HttpPost]
    public IActionResult Create(WeatherForecast newForecast)
    {
        _forecasts.Add(newForecast);
        return CreatedAtAction(nameof(GetById), new { id = _forecasts.Count - 1 }, newForecast);
    }

    // PUT: api/weather/{id} - Atualizar uma previsão existente
    [HttpPut("{id}")]
    public IActionResult Update(int id, WeatherForecast updatedForecast)
    {
        if (id < 0 || id >= _forecasts.Count)
        {
            return NotFound($"Previsão com ID {id} não encontrada");
        }

        _forecasts[id] = updatedForecast;
        return Ok(_forecasts[id]);
    }

    // DELETE: api/weather/{id} - Remover uma previsão
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (id < 0 || id >= _forecasts.Count)
        {
            return NotFound($"Previsão com ID {id} não encontrada");
        }

        _forecasts.RemoveAt(id);
        return NoContent();
    }
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}