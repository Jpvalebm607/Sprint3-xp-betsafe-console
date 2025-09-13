using System.Globalization;
using Microsoft.EntityFrameworkCore;
using XPBetSafe.ConsoleApp.Data;
using XPBetSafe.ConsoleApp.Services;

// ========= BOOT =========
using var db = new BetsafeDbContext();
// aplica migrações (vamos criar a migration já já)
db.Database.Migrate();

var usuarioSvc = new UsuarioService(db);
var apostaSvc = new ApostaService(db);

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("XP BetSafe (Console) — CRUD + Arquivos + SQLite");
Console.WriteLine("------------------------------------------------");

while (true)
{
    Menu();
    Console.Write("> ");
    var op = Console.ReadLine()?.Trim();

    try
    {
        switch (op)
        {
            case "1": await CadastrarUsuario(); break;
            case "2": await ListarUsuarios(); break;
            case "3": await AtualizarUsuario(); break;
            case "4": await RemoverUsuario(); break;
            case "5": await RegistrarAposta(); break;
            case "6": await ListarApostasPorUsuario(); break;
            case "7": await ExportarJson(); break;
            case "8": await ImportarJson(); break;
            case "9": await ExportarRelatorio(); break;
            case "0": return;
            default: Console.WriteLine("Opção inválida."); break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro: {ex.Message}");
    }

    Console.WriteLine("\nPressione ENTER para continuar...");
    Console.ReadLine();
}

void Menu()
{
    Console.Clear();
    Console.WriteLine("==== MENU ====");
    Console.WriteLine("1) Cadastrar usuário");
    Console.WriteLine("2) Listar usuários");
    Console.WriteLine("3) Atualizar usuário");
    Console.WriteLine("4) Remover usuário");
    Console.WriteLine("5) Registrar aposta");
    Console.WriteLine("6) Listar apostas por usuário");
    Console.WriteLine("7) Exportar registros (JSON)");
    Console.WriteLine("8) Importar registros (JSON)");
    Console.WriteLine("9) Exportar relatório (TXT)");
    Console.WriteLine("0) Sair");
    Console.WriteLine("==============");
}

async Task CadastrarUsuario()
{
    Console.Write("Nome: "); var nome = Console.ReadLine() ?? "";
    Console.Write("Email: "); var email = Console.ReadLine() ?? "";
    Console.Write("Nascimento (dd/mm/aaaa): ");
    var nasc = LerData();
    Console.Write("Nível de Risco (0-100): ");
    var risco = LerInt(0, 100);
    var u = await usuarioSvc.CriarAsync(nome, email, nasc, risco);
    Console.WriteLine($"Usuário criado: {u.Id} - {u.Nome}");
}

async Task ListarUsuarios()
{
    var lista = await usuarioSvc.ListarAsync();
    if (lista.Count == 0) { Console.WriteLine("Nenhum usuário."); return; }
    foreach (var u in lista)
        Console.WriteLine($"{u.Id} | {u.Nome} | {u.Email} | Risco: {u.NivelRisco}");
}

async Task AtualizarUsuario()
{
    Console.Write("ID: "); var id = LerInt();
    Console.Write("Novo nome (vazio = manter): "); var nome = Console.ReadLine();
    Console.Write("Novo email (vazio = manter): "); var email = Console.ReadLine();
    Console.Write("Novo risco (vazio = manter): "); var riscoStr = Console.ReadLine();
    int? risco = null;
    if (!string.IsNullOrWhiteSpace(riscoStr)) risco = LerInt(0, 100, riscoStr);
    var ok = await usuarioSvc.AtualizarAsync(id, nome, email, risco);
    Console.WriteLine(ok ? "Atualizado." : "Não encontrado.");
}

async Task RemoverUsuario()
{
    Console.Write("ID: "); var id = LerInt();
    var ok = await usuarioSvc.RemoverAsync(id);
    Console.WriteLine(ok ? "Removido." : "Não encontrado.");
}

async Task RegistrarAposta()
{
    Console.Write("ID do Usuário: "); var uid = LerInt();
    Console.Write("Data (dd/mm/aaaa): "); var dt = LerData();
    Console.Write("Valor: "); var valor = LerDecimal();
    Console.Write("Plataforma: "); var plat = Console.ReadLine() ?? "";
    Console.Write("Categoria: "); var cat = Console.ReadLine() ?? "";
    var r = await apostaSvc.RegistrarAsync(uid, dt, valor, plat, cat);
    Console.WriteLine($"Aposta registrada: {r.Id} para usuário {uid}");
}

async Task ListarApostasPorUsuario()
{
    Console.Write("ID do Usuário: "); var uid = LerInt();
    var regs = await apostaSvc.ListarPorUsuarioAsync(uid);
    if (regs.Count == 0) { Console.WriteLine("Nenhuma aposta."); return; }
    foreach (var r in regs)
        Console.WriteLine($"{r.Id} | {r.Data:dd/MM/yyyy} | {r.Valor} | {r.Plataforma} | {r.Categoria}");
}

async Task ExportarJson()
{
    var qtd = await apostaSvc.ExportarJsonAsync();
    Console.WriteLine($"Exportado para export/registros.json — {qtd} itens.");
}

async Task ImportarJson()
{
    var qtd = await apostaSvc.ImportarJsonAsync();
    Console.WriteLine($"Importados {qtd} novos registros do JSON.");
}

async Task ExportarRelatorio()
{
    var path = await apostaSvc.ExportarRelatorioTxtAsync();
    Console.WriteLine($"Relatório gerado em: {path}");
}

// ------- helpers -------
DateTime LerData()
{
    while (true)
    {
        var s = Console.ReadLine()?.Trim() ?? "";
        if (DateTime.TryParseExact(s, "dd/MM/yyyy", new CultureInfo("pt-BR"), 0, out var d)) return d;
        Console.Write("Data inválida. Tente (dd/mm/aaaa): ");
    }
}

int LerInt(int? min = null, int? max = null, string? seed = null)
{
    while (true)
    {
        var s = seed ?? Console.ReadLine();
        seed = null;
        if (int.TryParse(s, out var v) &&
            (!min.HasValue || v >= min) &&
            (!max.HasValue || v <= max)) return v;
        Console.Write("Valor inválido. Tente novamente: ");
    }
}

decimal LerDecimal()
{
    while (true)
    {
        var s = Console.ReadLine()?.Trim() ?? "";
        if (decimal.TryParse(s, NumberStyles.Number, new CultureInfo("pt-BR"), out var v) ||
            decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out v))
            return v;
        Console.Write("Número inválido. Tente novamente: ");
    }
}
