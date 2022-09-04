FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /src
COPY src/*/*.csproj ./
RUN for file in $(ls *.*proj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done
RUN dotnet restore "src/RoomScheduling.Host/RoomScheduling.Host.csproj"
COPY . .
WORKDIR "/src/src/RoomScheduling.Host"
RUN dotnet build "RoomScheduling.Host.csproj" -c Release -o /app/build

FROM build-env AS publish
RUN dotnet publish "RoomScheduling.Host.csproj" -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS="http://*:80"
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DotNet.Docker.dll"]d