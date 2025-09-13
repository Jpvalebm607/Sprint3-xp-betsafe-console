namespace XPBetSafe.ConsoleApp.Domain;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime DataNascimento { get; set; }
    public int NivelRisco { get; set; } // 0-100

    public ICollection<RegistroAposta> Registros { get; set; } = new List<RegistroAposta>();
}
