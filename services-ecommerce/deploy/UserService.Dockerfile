FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["UserService/UserService.Startup/UserService.Startup.csproj", "UserService/UserService.Startup/"]
COPY ["Shared/Ecommerce.Shared/Ecommerce.Shared.csproj", "Shared/Ecommerce.Shared/"]
RUN dotnet restore "UserService/UserService.Startup/UserService.Startup.csproj"
COPY . .
WORKDIR "/src/UserService/UserService.Startup"
RUN dotnet publish -c Release -o /app/publish UserService.Startup.csproj

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "UserService.Startup.dll"]
