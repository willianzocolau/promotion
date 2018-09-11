# Exemplo de comandos do Entity Framework Core
Primeiramente, você deve estar na pasta "root" do projeto (onde se encontra o arquivo .csproj).
Depois será necessário mudar a "enviroment variable" `ASPNETCORE_ENVIRONMENT` para `Production`.
Exemplo no PowerShell: `$env:ASPNETCORE_ENVIRONMENT = 'Production'`

# Comandos
- Criar migration: `dotnet ef migrations add <Nome> -v`
- Criar script: `dotnet ef migrations scripts -o Scripts/<Nome>.sql -v`