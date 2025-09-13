namespace XPBetSafe.ConsoleApp.Domain;

public class RegistroAposta
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public DateTime Data { get; set; }
    public decimal Valor { get; set; }
    public string Plataforma { get; set; } = "";
    public string Categoria { get; set; } = "";
}
