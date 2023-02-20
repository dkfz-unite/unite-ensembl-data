FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS restore
WORKDIR /src
COPY ["Ensembl.Data/Ensembl.Data.csproj", "Ensembl.Data/"]
COPY ["Ensembl.Data.Web/Ensembl.Data.Web.csproj", "Ensembl.Data.Web/"]
RUN dotnet restore "Ensembl.Data/Ensembl.Data.csproj"
RUN dotnet restore "Ensembl.Data.Web/Ensembl.Data.Web.csproj"

FROM restore as build
COPY . .
WORKDIR "/src/Ensembl.Data.Web"
RUN dotnet build --no-restore "Ensembl.Data.Web.csproj" -c Release

FROM build AS publish
RUN dotnet publish --no-build "Ensembl.Data.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ensembl.Data.Web.dll"]