FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all project files
COPY ["PaymentService/PaymentService.Startup/PaymentService.Startup.csproj", "PaymentService/PaymentService.Startup/"]
COPY ["PaymentService/PaymentService.WebApi/PaymentService.WebApi.csproj", "PaymentService/PaymentService.WebApi/"]
COPY ["Shared/Ecommerce.Shared/Ecommerce.Shared.csproj", "Shared/Ecommerce.Shared/"]

# Restore dependencies
RUN dotnet restore "PaymentService/PaymentService.Startup/PaymentService.Startup.csproj"

# Copy source code
COPY . .

# Build
WORKDIR "/src/PaymentService/PaymentService.Startup"
RUN dotnet build "PaymentService.Startup.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaymentService.Startup.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentService.Startup.dll"]
