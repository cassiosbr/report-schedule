FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /src

# tzdata garante suporte ao IANA timezone dentro do container (America/Sao_Paulo)
RUN apt-get update \
    && apt-get install -y --no-install-recommends tzdata \
    && rm -rf /var/lib/apt/lists/*

# Copia os csproj primeiro para aproveitar cache do restore
COPY ReportSchedule/ReportSchedule.csproj ReportSchedule/
COPY ReportSchedule.Worker/ReportSchedule.Worker.csproj ReportSchedule.Worker/
RUN dotnet restore ReportSchedule.Worker/ReportSchedule.Worker.csproj

# Copia o código
COPY ReportSchedule/ ReportSchedule/
COPY ReportSchedule.Worker/ ReportSchedule.Worker/

WORKDIR /src/ReportSchedule.Worker

ENV TZ=America/Sao_Paulo

CMD ["dotnet", "run", "--no-launch-profile", "--project", "ReportSchedule.Worker.csproj"]
