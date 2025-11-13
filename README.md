# ProjetoBaterias

## PASSOS PARA EXECUTAR

1️⃣ ABRIR O PROJETO
   - Extraia o arquivo ZIP
   - Abra a pasta TrocaBateriaWebApp no VS 2022

2️⃣ CONFIGURAR CONNECTION STRING
   - Abra: appsettings.json
   - Ajuste a connection string para seu ambiente:

3️⃣ RESTAURAR PACOTES
   Abra o terminal na pasta TrocaBateriaWebApp e execute:
   
   cd TrocaBateriaWebApp
   dotnet restore

4️⃣ CRIAR O BANCO DE DADOS
   No mesmo terminal, execute:
   
   dotnet ef database update
   
   ⚠️ Se o comando não funcionar, instale a ferramenta:
   dotnet tool install --global dotnet-ef

5️⃣ EXECUTAR O PROJETO
   
 Via Visual Studio 2022: Abra o .sln e pressione F5
