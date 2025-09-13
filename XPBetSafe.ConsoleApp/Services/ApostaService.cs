using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using XPBetSafe.ConsoleApp.Data;
using XPBetSafe.ConsoleApp.Domain;
using XPBetSafe.ConsoleApp.Dtos;

namespace XPBetSafe.ConsoleApp.Services;

public class ApostaService
{
    private readonly BetsafeDbContext _db;
    private static readonly JsonSerializerOptions _json = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    public ApostaService(BetsafeDbContext db) => _db = db;

    public async Task<RegistroAposta> RegistrarAsync(int usuarioId, DateTime data, decimal valor, string plataforma, string categoria)
    {
        var existe = await _db.Usuarios.AnyAsync(u => u.Id == usuarioId);
        if (!existe) throw new InvalidOperationException("Usuário não encontrado.");

        var r = new RegistroAposta
        {
            UsuarioId = usuarioId,
            Data = data,
            Valor = valor,
            Plataforma = plataforma.Trim(),
            Categoria = categoria.Trim()
        };
        _db.Registros.Add(r);
        await _db.SaveChangesAsync();
        return r;
    }

    public Task<List<RegistroAposta>> ListarPorUsuarioAsync(int usuarioId) =>
        _db.Registros.AsNoTracking()
           .Where(r => r.UsuarioId == usuarioId)
           .OrderByDescending(r => r.Data)
           .ToListAsync();

    public async Task<int> ExportarJsonAsync(string caminho = "export/registros.json")
    {
        Directory.CreateDirectory(Path.GetDirectoryName(caminho)!);
        var dados = await _db.Registros.AsNoTracking().ToListAsync();

        var dtos = dados.Select(r => new RegistroApostaDto
        {
            Id = r.Id,
            UsuarioId = r.UsuarioId,
            Data = r.Data,
            Valor = r.Valor,
            Plataforma = r.Plataforma,
            Categoria = r.Categoria
        }).ToList();

        var json = JsonSerializer.Serialize(dtos, _json);
        await File.WriteAllTextAsync(caminho, json, Encoding.UTF8);
        return dtos.Count;
    }

    public async Task<int> ImportarJsonAsync(string caminho = "export/registros.json")
    {
        if (!File.Exists(caminho)) return 0;
        var json = await File.ReadAllTextAsync(caminho, Encoding.UTF8);
        var dtos = JsonSerializer.Deserialize<List<RegistroApostaDto>>(json, _json) ?? new();

        int inseridos = 0;
        foreach (var d in dtos.OrderBy(x => x.Id))
        {
            if (!await _db.Usuarios.AnyAsync(u => u.Id == d.UsuarioId)) continue;

            var existe = await _db.Registros.AnyAsync(r =>
                r.UsuarioId == d.UsuarioId &&
                r.Data == d.Data &&
                r.Valor == d.Valor &&
                r.Plataforma == d.Plataforma &&
                r.Categoria == d.Categoria);

            if (!existe)
            {
                _db.Registros.Add(new RegistroAposta
                {
                    UsuarioId = d.UsuarioId,
                    Data = d.Data,
                    Valor = d.Valor,
                    Plataforma = d.Plataforma,
                    Categoria = d.Categoria
                });
                inseridos++;
            }
        }
        await _db.SaveChangesAsync();
        return inseridos;
    }

    public async Task<string> ExportarRelatorioTxtAsync(string caminho = "export/relatorio.txt")
    {
        Directory.CreateDirectory(Path.GetDirectoryName(caminho)!);

        var linhas = new List<string>();
        linhas.Add($"Relatório XP BetSafe — {DateTime.Now:dd/MM/yyyy HH:mm}");
        linhas.Add(new string('-', 50));

        var query = await _db.Registros.AsNoTracking()
            .GroupBy(r => r.UsuarioId)
            .Select(g => new
            {
                UsuarioId = g.Key,
                Quantidade = g.Count(),
                Total = g.Sum(x => x.Valor)
            }).ToListAsync();

        foreach (var g in query)
        {
            var user = await _db.Usuarios.FindAsync(g.UsuarioId);
            var nome = user?.Nome ?? $"Usuário {g.UsuarioId}";
            linhas.Add($"{nome} — Registros: {g.Quantidade} — Total Apostado: {g.Total.ToString("C", new CultureInfo("pt-BR"))}");
        }

        await File.WriteAllLinesAsync(caminho, linhas, Encoding.UTF8);
        return caminho;
    }
}
