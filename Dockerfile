#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["OpenAPIASPNET.csproj", "."]
RUN dotnet restore "./OpenAPIASPNET.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "OpenAPIASPNET.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenAPIASPNET.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
ENV GRAYLOG_ADDRESS=""
ENV AMQP_HOSTNAME="192.168.0.2"
ENV AMQP_PORT="5672"
ENV AMQP_USERNAME="guest"
ENV AMQP_PASSWORD=""
ENV POSTGRESQL_CONNECTION="Server=192.168.0.2;Port=5432;Database=postgres;User ID=postgres;Password=testtesttest;"
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenAPIASPNET.dll"]