FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build-env
WORKDIR /app

COPY ./src/KinoDnesApi/*.csproj ./KinoDnesApi/
RUN dotnet restore KinoDnesApi

COPY ./src/ ./
RUN dotnet publish KinoDnesApi -c Release -o ./../out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0
WORKDIR /app
COPY --from=build-env /out .

USER 5000
ENTRYPOINT ["dotnet", "KinoDnesApi.dll"]

EXPOSE 80
