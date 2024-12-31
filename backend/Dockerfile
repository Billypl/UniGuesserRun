# Stage 1: Build
# Stage 2: Publish 
# Stage 3: Run

# FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# WORKDIR /app
# EXPOSE 5000

# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# WORKDIR /src
# COPY ["YourProjectName.csproj", "./"]
# RUN dotnet restore "./YourProjectName.csproj"
# COPY . .
# RUN dotnet build "./YourProjectName.csproj" -c Release -o /app/build

# FROM build AS publish
# RUN dotnet publish "./YourProjectName.csproj" -c Release -o /app/publish

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "YourProjectName.dll"]





# ----------------------

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
# restore dependencies
COPY ./PartyGame/*.csproj ./
RUN dotnet restore
# build app
COPY ./PartyGame/ ./
EXPOSE 7157
# CMD ["dotnet", "watch", "run", "--urls=http://0.0.0.0:7157"]
CMD ["dotnet","run", "--launch-profile", "https"]


# # -----------------------

# FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS build
# WORKDIR /src
# # restore dependencies
# COPY ["src/PartyGame/PartyGame.csproj", "PartyGame/"]
# RUN dotnet restore "PartyGame/PartyGame.csproj"
# #build
# COPY ["src/PartyGame", "PartyGame/"]
# RUN dotnet build "PartyGame/PartyGame.csproj" -c Release 