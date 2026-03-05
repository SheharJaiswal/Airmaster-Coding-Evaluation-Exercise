FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OrderService/OrderService.Startup/OrderService.Startup.csproj", "OrderService/OrderService.Startup/"]
COPY ["Shared/Ecommerce.Shared/Ecommerce.Shared.csproj", "Shared/Ecommerce.Shared/"]
RUN dotnet restore "OrderService/OrderService.Startup/OrderService.Startup.csproj"
COPY . .
WORKDIR "/src/OrderService/OrderService.Startup"
RUN dotnet publish -c Release -o /app/publish OrderService.Startup.csproj

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrderService.Startup.dll"]
