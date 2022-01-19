FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Wordleans.Sample/Wordleans.Sample.csproj", "Wordleans.Sample/"]
RUN dotnet restore "Wordleans.Sample/Wordleans.Sample.csproj"
COPY . .
WORKDIR "/src/Wordleans.Sample"
RUN dotnet build "Wordleans.Sample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Wordleans.Sample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wordleans.Sample.dll"]
