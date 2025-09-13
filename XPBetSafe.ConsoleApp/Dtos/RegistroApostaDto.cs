namespace XPBetSafe.ConsoleApp.Dtos;

public class RegistroApostaDto
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public DateTime Data { get; set; }
    public decimal Valor { get; set; }
    public string Plataforma { get; set; } = "";
    public string Categoria { get; set; } = "";
}
