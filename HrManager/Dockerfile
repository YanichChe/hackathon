# Create a stage for building the HrManager application.
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build

COPY ./ /HrManager
COPY ../Nsu.HackathonProblem.Contracts ./Nsu.HackathonProblem.Contracts

WORKDIR /HrManager

# This is the architecture you’re building for, which is passed in by the builder.
# Placing it here allows the previous steps to be cached across architectures.
ARG TARGETARCH

# Restore dependencies
RUN dotnet restore ./HrManager/HrManager.sln

# Build the HrManager application
RUN dotnet publish ./HrManager/HrManager.sln \
    -c Release \
    -o out \
    --no-restore

# Run HrManager application
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine as runtime

WORKDIR /HrManager

# Install curl in the runtime image
RUN apk add --no-cache curl

# Copy everything needed to run the HrManager from the "build" stage.
COPY --from=build /HrManager/out ./

# Switch to a non-privileged user (defined in the base image) that the HrManager will run under.
# Create a new user and change directory ownership
RUN adduser --disabled-password \
  --home /HrManager \
  --gecos '' dotnetuser && chown -R dotnetuser /HrManager
USER dotnetuser

ENTRYPOINT ["./HrManager"]
