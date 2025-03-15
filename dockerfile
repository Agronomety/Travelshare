FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TravelshareBlazor/TravelshareBlazor.csproj", "TravelshareBlazor/"]
COPY ["TravelshareBackend/TravelshareBackend.csproj", "TravelshareBackend/"]
RUN dotnet restore "TravelshareBlazor/TravelshareBlazor.csproj"
COPY . .
WORKDIR "/src/TravelshareBlazor"
RUN dotnet build "TravelshareBlazor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TravelshareBlazor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TravelshareBlazor.dll"]