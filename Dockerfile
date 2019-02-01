FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app

COPY ./src/KinoDnesApi/*.csproj ./KinoDnesApi/
RUN dotnet restore KinoDnesApi

COPY ./src/ ./
RUN dotnet publish KinoDnesApi -c Release -o ./../out

FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "KinoDnesApi.dll"]

EXPOSE 80
