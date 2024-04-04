FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FiDa.csproj", "./"]
RUN dotnet restore "FiDa.csproj"
COPY . .
WORKDIR /src
RUN dotnet build "FiDa.csproj" -c Release -o /app/publish

FROM build AS publish
RUN dotnet publish "FiDa.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "FiDa.dll" ]