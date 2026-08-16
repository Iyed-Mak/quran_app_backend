FROM mcr.microsoft.com/dotnet/sdk:8.0.424 AS build
WORKDIR /src

COPY ["QuranSchool.Api.csproj", "./"]
RUN dotnet restore "QuranSchool.Api.csproj"

COPY . .
RUN dotnet publish "QuranSchool.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0.30 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# أدوات PostgreSQL مطلوبة للنسخ الاحتياطي والاستعادة (pg_dump / pg_restore).
# نثبّت عميل الإصدار 18 من مستودع PGDG لأن إصدار الخادم على Render هو 18،
# ويجب ألا يقل إصدار العميل عن إصدار الخادم وإلا يفشل pg_dump برسالة
# "server version mismatch".
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl ca-certificates gnupg \
    && curl -fsSL https://www.postgresql.org/media/keys/ACCC4CF8.asc | gpg --dearmor -o /usr/share/keyrings/pgdg.gpg \
    && echo "deb [signed-by=/usr/share/keyrings/pgdg.gpg] https://apt.postgresql.org/pub/repos/apt bookworm-pgdg main" > /etc/apt/sources.list.d/pgdg.list \
    && apt-get update \
    && apt-get install -y --no-install-recommends postgresql-client-18 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "QuranSchool.Api.dll"]
