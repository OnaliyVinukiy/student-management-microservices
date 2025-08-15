#!/bin/bash

# This script standardizes the Dockerfiles for all microservices in the project.
# It uses two different templates: one for ASP.NET Core services (APIs/WebApp)
# and one for .NET Worker services.
#
# USAGE: Run this script from the 'src' directory.
#
# ./update_dockerfiles.sh

echo "🚀 Starting Dockerfile standardization process..."

# -----------------------------------------------------------------------------
# Template for ASP.NET Core services (APIs and the WebApp)
# Uses the 'dotnet-aspnet-base' runtime image.
# The single quotes around 'EOF' are important to prevent shell variable expansion.
# -----------------------------------------------------------------------------
read -r -d '' API_DOCKERFILE_CONTENT <<'EOF'
# This is a generic Dockerfile for any ASP.NET Core service

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
# Use the pre-built custom ASP.NET runtime base image
FROM studentsystem/dotnet-aspnet-base:1.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "${DLL_NAME}"]
EOF

# -----------------------------------------------------------------------------
# Template for .NET Worker services
# Uses the 'dotnet-runtime-base' image, which is smaller.
# -----------------------------------------------------------------------------
read -r -d '' WORKER_DOCKERFILE_CONTENT <<'EOF'
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
EOF


# -----------------------------------------------------------------------------
# List of services for each template
# -----------------------------------------------------------------------------
API_SERVICES=(
    "CourseManagementAPI"
    "EnrollmentManagementAPI"
    "StudentManagementAPI"
    "WebApp"
)

WORKER_SERVICES=(
    "AuditlogService"
    "EnrollmentEventHandler"
    "NotificationService"
    "TimeService"
    "TuitionService"
)

# -----------------------------------------------------------------------------
# Loop through and update the files
# -----------------------------------------------------------------------------

echo ""
echo "Updating Dockerfiles for API and WebApp services..."
for service in "${API_SERVICES[@]}"; do
    if [ -d "$service" ]; then
        echo "$API_DOCKERFILE_CONTENT" > "$service/Dockerfile"
        echo "  - ✓ Updated $service/Dockerfile"
    else
        echo "  - ✗ Warning: Directory not found for service '$service'. Skipping."
    fi
done

echo ""
echo "Updating Dockerfiles for Worker services..."
for service in "${WORKER_SERVICES[@]}"; do
    if [ -d "$service" ]; then
        echo "$WORKER_DOCKERFILE_CONTENT" > "$service/Dockerfile"
        echo "  - ✓ Updated $service/Dockerfile"
    else
        echo "  - ✗ Warning: Directory not found for service '$service'. Skipping."
    fi
done

echo ""
echo "✅ All Dockerfiles have been updated successfully."