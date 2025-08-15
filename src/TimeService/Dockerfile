# Use ARG to easily specify which project this Dockerfile is for.
# These will be passed in from your docker-compose.yml file.
ARG SERVICE_FOLDER
ARG CSPROJ_NAME
ARG DLL_NAME

# --- BUILD STAGE (uses the larger SDK image) ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copy all source code into the container.
#    (Make sure your .dockerignore file is in the 'src' directory to exclude bin/obj folders)
COPY . .

# 2. Change to the service's directory and run publish.
#    This single command will handle restoring packages, building, and publishing.
#    This is more reliable than a separate restore/publish with --no-restore.
WORKDIR "/src/${SERVICE_FOLDER}"
RUN dotnet publish "${CSPROJ_NAME}" -c Release -o /app/out


# --- FINAL STAGE (uses a smaller runtime or aspnet image) ---
# For TimeService (a background worker), the 'runtime' image is more efficient.
# For APIs, use mcr.microsoft.com/dotnet/aspnet:8.0
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "${DLL_NAME}"]