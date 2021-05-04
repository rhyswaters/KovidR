#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Background Processes/CaseNumbersWorker/CaseNumbersWorker.csproj", "Background Processes/CaseNumbersWorker/"]
COPY ["Building Blocks/EventBus.Messages/EventBus.Messages.csproj", "Building Blocks/EventBus.Messages/"]
RUN dotnet restore "Background Processes/CaseNumbersWorker/CaseNumbersWorker.csproj"
COPY . .
WORKDIR "/src/Background Processes/CaseNumbersWorker"
RUN dotnet build "CaseNumbersWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CaseNumbersWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CaseNumbersWorker.dll"]
