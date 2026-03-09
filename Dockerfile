# Stage 1: Build (Onde a compilação acontece)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 1. Copia os projetos para restaurar dependências (otimiza o cache)
COPY ["src/Soat.Eleven.Kutcut.User.Api/Soat.Eleven.Kutcut.Users.Api.csproj", "src/Soat.Eleven.Kutcut.User.Api/"]
COPY ["src/Soat.Eleven.Kutcut.User.Application/Soat.Eleven.Kutcut.Users.Application.csproj", "src/Soat.Eleven.Kutcut.User.Application/"]
COPY ["src/Soat.Eleven.Kutcut.User.Domain/Soat.Eleven.Kutcut.Users.Domain.csproj", "src/Soat.Eleven.Kutcut.User.Domain/"]
COPY ["src/Soat.Eleven.Kutcut.User.Infra/Soat.Eleven.Kutcut.Users.Infra.csproj", "src/Soat.Eleven.Kutcut.User.Infra/"]

# 2. Restaura as dependências
RUN dotnet restore "src/Soat.Eleven.Kutcut.User.Api/Soat.Eleven.Kutcut.Users.Api.csproj"

# 3. Copia o restante do código fonte
COPY . .

# 4. Build - Resolve o erro de metadados e do .resx antecipadamente
RUN dotnet build "src/Soat.Eleven.Kutcut.User.Api/Soat.Eleven.Kutcut.Users.Api.csproj" \
    -c Release \
    /p:EnableDefaultEmbeddedResourceItems=false

# --- Stage 2: Migrator (Imagem para o Job do Kubernetes) ---
FROM build AS migrator
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Define o diretório onde o comando EF deve ser executado (dentro da API)
WORKDIR "/src/src/Soat.Eleven.Kutcut.User.Api"

# Comando que será completado pelos 'args' do Helm
ENTRYPOINT ["dotnet", "ef", "database", "update"]

# --- Stage 3: Publish (Gera os binários finais da API) ---
FROM build AS publish
WORKDIR "/src/src/Soat.Eleven.Kutcut.User.Api"
RUN dotnet publish "Soat.Eleven.Kutcut.Users.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false \
    /p:EnableDefaultEmbeddedResourceItems=false

# --- Stage 4: Final (Imagem de Produção Leve) ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
# Copia da pasta /app/publish criada no estágio anterior
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Soat.Eleven.Kutcut.Users.Api.dll"]