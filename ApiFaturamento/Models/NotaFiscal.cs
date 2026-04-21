namespace ApiCorreta.Models;

public class NotaFiscal
{
    public int Id { get; set; }
    public int Numero { get; set; }
    public string Status { get; set; } = "Aberta";
    public List<ItemNotaFiscal> Itens { get; set; } = new();
}

public class ItemNotaFiscal
{
    public int Id { get; set; } // <--- ADICIONE ESTA LINHA AQUI
    public int ProdutoId { get; set; }
    public string ProdutoDescricao { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}
