FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build 
WORKDIR /src

COPY ["DocForce.slnx", "."] 
COPY ["src/DocForce/DocForce.API.csproj", "src/DocForce.API/"] 
COPY ["src/DocForce.Application/DocForce.Application.csproj", "src/DocForce.Application/"] 
COPY ["src/DocForce.Domain/DocForce.Domain.csproj", "src/DocForce.Domain/"]
COPY ["src/DocForce.Infrastructure/DocForce.Infrastructure.csproj", "src/DocForce.Infrastructure/"]

RUN dotnet restore "DocForce.slnx"

COPY . .

RUN dotnet publish "src/DocForce/DocForce.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final 
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "DocForce.API.dll"]