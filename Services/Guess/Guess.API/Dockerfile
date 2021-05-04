#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Services/Guess/Guess.API/Guess.API.csproj", "Services/Guess/Guess.API/"]
COPY ["Building Blocks/EventBus.Messages/EventBus.Messages.csproj", "Building Blocks/EventBus.Messages/"]
COPY ["Services/Guess/Guess.Application/Guess.Application.csproj", "Services/Guess/Guess.Application/"]
COPY ["Services/Guess/Guess.Domain/Guess.Domain.csproj", "Services/Guess/Guess.Domain/"]
COPY ["Services/Guess/Guess.Infrastructure/Guess.Infrastructure.csproj", "Services/Guess/Guess.Infrastructure/"]
RUN dotnet restore "Services/Guess/Guess.API/Guess.API.csproj"
COPY . .
WORKDIR "/src/Services/Guess/Guess.API"
RUN dotnet build "Guess.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Guess.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Guess.API.dll"]
