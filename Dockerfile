FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["QuranSchool.Api.csproj", "./"]
RUN dotnet restore "QuranSchool.Api.csproj"

COPY . .
RUN dotnet publish "QuranSchool.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# أدوات PostgreSQL مطلوبة للنسخ الاحتياطي والاستعادة (pg_dump / pg_restore).
RUN apt-get update \
    && apt-get install -y --no-install-recommends postgresql-client \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "QuranSchool.Api.dll"]
