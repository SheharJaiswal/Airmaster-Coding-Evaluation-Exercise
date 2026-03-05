FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CartService/CartService.Startup/CartService.Startup.csproj", "CartService/CartService.Startup/"]
COPY ["Shared/Ecommerce.Shared/Ecommerce.Shared.csproj", "Shared/Ecommerce.Shared/"]
RUN dotnet restore "CartService/CartService.Startup/CartService.Startup.csproj"
COPY . .
WORKDIR "/src/CartService/CartService.Startup"
RUN dotnet publish -c Release -o /app/publish CartService.Startup.csproj

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CartService.Startup.dll"]
