FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Presentation/UserTable/UserTable.csproj", "Presentation/UserTable/"]
COPY ["Infrastructure/UserTable.Persistence/UserTable.Persistence.csproj", "Infrastructure/UserTable.Persistence/"]
COPY ["Core/UserTable.Domain/UserTable.Domain.csproj", "Core/UserTable.Domain/"]
COPY ["Core/UserTable.Application/UserTable.Application.csproj", "Core/UserTable.Application/"]
RUN dotnet restore "Presentation/UserTable/UserTable.csproj"

COPY . .
WORKDIR "/src/Presentation/UserTable"
RUN dotnet build "UserTable.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserTable.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserTable.dll"]