# This is a generic Dockerfile for any .NET Worker Service

# Declare build arguments. These will be supplied by docker-compose.yml.
ARG SERVICE_FOLDER
ARG CSPROJ_NAME
ARG DLL_NAME

# Stage 1: Build the application
# Use the pre-built custom SDK base image
FROM studentsystem/dotnet-sdk-base:1.0 AS build
WORKDIR /src

# Copy the source code
COPY . .

# Restore the entire solution from the copied source.
RUN dotnet restore "StudentSystem.sln"

# Publish the specific project for this service.
RUN dotnet publish "${SERVICE_FOLDER}/${CSPROJ_NAME}" -c Release -o /app/out

# Stage 2: Create the final runtime image
# Use the pre-built custom .NET runtime base image
FROM studentsystem/dotnet-runtime-base:1.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "${DLL_NAME}"]
