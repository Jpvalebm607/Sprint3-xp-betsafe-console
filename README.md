# 🎰 XP BetSafe - Sprint 3 (ConsoleApp)

Aplicação em **C# (.NET 8)** com **Entity Framework Core + SQLite**, desenvolvida para apoiar a prevenção e tratamento de **apostas compulsivas** no Brasil.  
Este projeto faz parte da **Sprint 3** e contempla os requisitos de manipulação de banco de dados, persistência em arquivos e estruturação de código limpo.

---

## 📌 Objetivo

O **XP BetSafe** é uma plataforma digital que visa:
- Monitorar comportamento de apostas;
- Identificar padrões de risco usando dados do usuário;
- Gerar relatórios e autoavaliações;
- Facilitar o encaminhamento para suporte psicológico;
- Armazenar e manipular dados de forma segura e estruturada.

---

## 🛠 Tecnologias utilizadas

- **C# (.NET 8.0 LTS)**
- **Entity Framework Core 9**
- **SQLite** (banco de dados leve e local)
- **JSON / TXT** (manipulação de arquivos)
- **Visual Studio 2022 / VS Code**
- **Git + GitHub** (versionamento)

---

## 📂 Estrutura do Projeto

XPBetSafe/
│── XPBetSafe.sln
│── .gitignore
│
└── XPBetSafe.ConsoleApp/
├── Program.cs
├── Data/
│ └── BetsafeDbContext.cs
├── Domain/
│ ├── Usuario.cs
│ └── RegistroAposta.cs
├── Dtos/
│ └── RegistroApostaDto.cs
├── Services/
│ ├── UsuarioService.cs
│ └── ApostaService.cs
├── Migrations/
│ ├── InitialCreate.cs
│ ├── InitialCreate.Designer.cs
│ └── BetsafeDbContextModelSnapshot.cs
└── XPBetSafe.ConsoleApp.csproj


---

## 📊 Funcionalidades principais

- Cadastro e listagem de usuários

- Registro e consulta de apostas

- Persistência de dados com EF Core (CRUD completo)

- Manipulação de arquivos TXT/JSON

- Estrutura de classes separada em Domain, DTOs, Services e Data


 ---

## 👥 Integrantes
- RM98650 - João Pedro Cruz

- RM551169 - Tiago Paulino

- RM98668 - Victor Eid

