#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Tyler.ReportWriting.ApiRouting/Tyler.ReportWriting.ApiRouting.csproj", "Tyler.ReportWriting.ApiRouting/"]
RUN dotnet restore "Tyler.ReportWriting.ApiRouting/Tyler.ReportWriting.ApiRouting.csproj"
COPY . .
WORKDIR "/src/Tyler.ReportWriting.ApiRouting"
RUN dotnet build "Tyler.ReportWriting.ApiRouting.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tyler.ReportWriting.ApiRouting.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tyler.ReportWriting.ApiRouting.dll"]