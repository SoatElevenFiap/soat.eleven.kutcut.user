FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/Soat.Eleven.Kutcut.User.Api/Soat.Eleven.Kutcut.Users.Api.csproj src/Soat.Eleven.Kutcut.User.Api/
COPY src/Soat.Eleven.Kutcut.User.Application/Soat.Eleven.Kutcut.Users.Application.csproj src/Soat.Eleven.Kutcut.User.Application/
COPY src/Soat.Eleven.Kutcut.User.Domain/Soat.Eleven.Kutcut.Users.Domain.csproj src/Soat.Eleven.Kutcut.User.Domain/
COPY src/Soat.Eleven.Kutcut.User.Infra/Soat.Eleven.Kutcut.Users.Infra.csproj src/Soat.Eleven.Kutcut.User.Infra/

RUN dotnet restore src/Soat.Eleven.Kutcut.User.Api/Soat.Eleven.Kutcut.Users.Api.csproj

COPY src/ ./src/

WORKDIR /src/src/Soat.Eleven.Kutcut.User.Api
RUN dotnet publish Soat.Eleven.Kutcut.Users.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Soat.Eleven.Kutcut.Users.Api.dll"]