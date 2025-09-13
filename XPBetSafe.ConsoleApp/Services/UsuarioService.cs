using Microsoft.EntityFrameworkCore;
using XPBetSafe.ConsoleApp.Data;
using XPBetSafe.ConsoleApp.Domain;

namespace XPBetSafe.ConsoleApp.Services;

public class UsuarioService
{
    private readonly BetsafeDbContext _db;
    public UsuarioService(BetsafeDbContext db) => _db = db;

    public async Task<Usuario> CriarAsync(string nome, string email, DateTime nascimento, int risco)
    {
        var u = new Usuario
        {
            Nome = nome.Trim(),
            Email = email.Trim().ToLower(),
            DataNascimento = nascimento,
            NivelRisco = Math.Clamp(risco, 0, 100)
        };
        _db.Usuarios.Add(u);
        await _db.SaveChangesAsync();
        return u;
    }

    public Task<List<Usuario>> ListarAsync() =>
        _db.Usuarios.AsNoTracking().OrderBy(u => u.Nome).ToListAsync();

    public async Task<bool> AtualizarAsync(int id, string? nome = null, string? email = null, int? risco = null)
    {
        var u = await _db.Usuarios.FindAsync(id);
        if (u is null) return false;

        if (!string.IsNullOrWhiteSpace(nome)) u.Nome = nome.Trim();
        if (!string.IsNullOrWhiteSpace(email)) u.Email = email.Trim().ToLower();
        if (risco.HasValue) u.NivelRisco = Math.Clamp(risco.Value, 0, 100);

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoverAsync(int id)
    {
        var u = await _db.Usuarios.FindAsync(id);
        if (u is null) return false;
        _db.Usuarios.Remove(u);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<Usuario?> ObterAsync(int id) => _db.Usuarios.FindAsync(id).AsTask();
}
