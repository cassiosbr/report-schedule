FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /src

COPY ReportSchedule.csproj ./
RUN dotnet restore

COPY . .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

CMD ["dotnet", "run", "--no-launch-profile", "--urls", "http://+:8080"]
