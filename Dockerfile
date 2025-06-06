FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=80
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore
WORKDIR /src
COPY ["Ensembl.Data/Ensembl.Data.csproj", "Ensembl.Data/"]
COPY ["Ensembl.Data.Web/Ensembl.Data.Web.csproj", "Ensembl.Data.Web/"]
RUN dotnet restore "Ensembl.Data/Ensembl.Data.csproj"
RUN dotnet restore "Ensembl.Data.Web/Ensembl.Data.Web.csproj"

FROM restore AS build
COPY . .
WORKDIR "/src/Ensembl.Data.Web"
# --no-restore
RUN dotnet build "Ensembl.Data.Web.csproj" -c Release

FROM build AS publish
RUN dotnet publish --no-build "Ensembl.Data.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ensembl.Data.Web.dll"]