
# build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY . .

RUN dotnet restore KanbanBoard.WebApi/KanbanBoard.WebApi.csproj
RUN dotnet publish KanbanBoard.WebApi/KanbanBoard.WebApi.csproj -c Release -o published /p:UseAppHost=false

# runtime stage

FROM mcr.microsoft.com/dotnet/aspnet:8.0 
WORKDIR /app

COPY --from=build /app/published .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://0.0.0.0:8080

ENTRYPOINT ["dotnet", "KanbanBoard.WebApi.dll"]
