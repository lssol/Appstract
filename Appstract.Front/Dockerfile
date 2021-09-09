FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Appstract.Front/Appstract.Front.csproj", "Appstract.Front/"]
RUN dotnet restore "Appstract.Front/Appstract.Front.csproj"
COPY . .
WORKDIR "/src/Appstract.Front"
RUN dotnet build "Appstract.Front.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Appstract.Front.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Appstract.Front.dll"]
