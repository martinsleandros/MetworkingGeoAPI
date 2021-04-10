FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /src

COPY ["/MetworkingGeo.Presentation/MetworkingGeo.Presentation.csproj", "/MetworkingGeo.Presentation/"]
COPY ["/MetworkingGeo.Application/MetworkingGeo.Application.csproj", "/MetworkingGeo.Application/"]
COPY ["/MetworkingGeo.Domain/MetworkingGeo.Domain.csproj", "/MetworkingGeo.Domain/"]
COPY ["/MetworkingGeo.Infra/MetworkingGeo.Infra.csproj", "/MetworkingGeo.Infra/"]
COPY ["/MetworkingGeo.WorkerService/MetworkingGeo.WorkerService.csproj", "/MetworkingGeo.WorkerService/"]

RUN dotnet restore /MetworkingGeo.Presentation/MetworkingGeo.Presentation.csproj
COPY . .

WORKDIR "/src/MetworkingGeo.Presentation"
RUN dotnet build "MetworkingGeo.Presentation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MetworkingGeo.Presentation.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5001
ENV ASPNETCORE_URLS=http://*:5001

ENTRYPOINT ["dotnet", "MetworkingGeo.Presentation.dll","--environment=Production"]

