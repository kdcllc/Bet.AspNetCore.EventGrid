#------------------------------------------------------------------------------------------------------------------------
# BUILD:        docker build -t kdcllc/bet-eventgrid-web-sample:v1 -f "./src/Bet.AspNetCore.EventGrid.WebApp/Dockerfile" .
#
# RUN:          docker run --rm -d -p 5200:80 -e EnabledSSL=false kdcllc/bet-eventgrid-web-sample:v1
#
# USAGE:        This is AspNetCore Web Application to process custom webhooks.
#
#               docker push kdcllc/bet-eventgrid-web-sample:v1
#------------------------------------------------------------------------------------------------------------------------

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["src/Bet.AspNetCore.EventGrid.Abstractions/Bet.AspNetCore.EventGrid.Abstractions.csproj", "src/Bet.AspNetCore.EventGrid.Abstractions/"]
COPY ["src/Bet.AspNetCore.EventGrid.WebApp/Bet.AspNetCore.EventGrid.WebApp.csproj", "src/Bet.AspNetCore.EventGrid.WebApp/"]
COPY ["src/Bet.AspNetCore.EventGrid/Bet.AspNetCore.EventGrid.csproj", "src/Bet.AspNetCore.EventGrid/"]
COPY ["src/Bet.AspNetCore.EventGrid.Viewer/Bet.AspNetCore.EventGrid.Viewer.csproj", "src/Bet.AspNetCore.EventGrid.Viewer/"]
RUN dotnet restore "src/Bet.AspNetCore.EventGrid.WebApp/Bet.AspNetCore.EventGrid.WebApp.csproj"
COPY . .
WORKDIR "/src/src/Bet.AspNetCore.EventGrid.WebApp"
RUN dotnet build "Bet.AspNetCore.EventGrid.WebApp.csproj" -c Release -f netcoreapp3.0 -o /app/build

FROM build AS publish
RUN dotnet publish "Bet.AspNetCore.EventGrid.WebApp.csproj" -c Release -f netcoreapp3.0 -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bet.AspNetCore.EventGrid.WebApp.dll"]
