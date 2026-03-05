FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ProductService/ProductService.Startup/ProductService.Startup.csproj", "ProductService/ProductService.Startup/"]
COPY ["Shared/Ecommerce.Shared/Ecommerce.Shared.csproj", "Shared/Ecommerce.Shared/"]
RUN dotnet restore "ProductService/ProductService.Startup/ProductService.Startup.csproj"
COPY . .
WORKDIR "/src/ProductService/ProductService.Startup"
RUN dotnet publish -c Release -o /app/publish ProductService.Startup.csproj

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ProductService.Startup.dll"]
