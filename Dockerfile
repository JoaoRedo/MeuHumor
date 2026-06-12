FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY src/MeuHumor.Api/MeuHumor.Api.csproj src/MeuHumor.Api/
RUN dotnet restore src/MeuHumor.Api/MeuHumor.Api.csproj

COPY src/MeuHumor.Api/ src/MeuHumor.Api/
RUN dotnet publish src/MeuHumor.Api/MeuHumor.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .
EXPOSE 8080

ENTRYPOINT ["dotnet", "MeuHumor.Api.dll"]
