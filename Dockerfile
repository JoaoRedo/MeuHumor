FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY src/MeuHumor.Api/MeuHumor.Api.csproj src/MeuHumor.Api/
RUN dotnet restore src/MeuHumor.Api/MeuHumor.Api.csproj

COPY src/MeuHumor.Api/ src/MeuHumor.Api/
RUN dotnet publish src/MeuHumor.Api/MeuHumor.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends tzdata \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_ENVIRONMENT=Production
ENV TZ=America/Sao_Paulo

COPY --from=build /app/publish .
EXPOSE 8080

ENTRYPOINT ["dotnet", "MeuHumor.Api.dll"]
