# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=80
EXPOSE 80

FROM continuumio/miniconda3 AS liftover
RUN conda create -y -p /liftover --override-channels -c conda-forge -c bioconda ucsc-liftover=482 \
    && conda clean -afy

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore
ARG USER
WORKDIR /src
COPY ["Ensembl.Data/Ensembl.Data.csproj", "Ensembl.Data/"]
COPY ["Ensembl.Data.Web/Ensembl.Data.Web.csproj", "Ensembl.Data.Web/"]
RUN --mount=type=secret,id=github_token \
    dotnet nuget add source https://nuget.pkg.github.com/dkfz-unite/index.json -n github -u ${USER} -p "$(cat /run/secrets/github_token)" --store-password-in-clear-text \
    && dotnet restore "Ensembl.Data/Ensembl.Data.csproj" \
    && dotnet restore "Ensembl.Data.Web/Ensembl.Data.Web.csproj" \
    && dotnet nuget remove source github

FROM restore AS build
COPY . .
WORKDIR "/src/Ensembl.Data.Web"
RUN --mount=type=secret,id=github_token \
    dotnet nuget add source https://nuget.pkg.github.com/dkfz-unite/index.json -n github -u ${USER} -p "$(cat /run/secrets/github_token)" --store-password-in-clear-text \
    && dotnet build "Ensembl.Data.Web.csproj" -c Release \
    && dotnet nuget remove source github

FROM build AS publish
RUN dotnet publish --no-build "Ensembl.Data.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=liftover /liftover /opt/liftover
COPY --from=publish /app/publish .
ENV PATH="/opt/liftover/bin:${PATH}"
ENTRYPOINT ["dotnet", "Ensembl.Data.Web.dll"]
