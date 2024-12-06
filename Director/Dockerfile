# Create a stage for building the Director application.
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build

COPY ./ /Director
COPY ../Nsu.HackathonProblem.Contracts ./Nsu.HackathonProblem.Contracts

WORKDIR /Director

# This is the architecture you’re building for, which is passed in by the builder.
# Placing it here allows the previous steps to be cached across architectures.
ARG TARGETARCH

# Restore dependencies
RUN dotnet restore ./Director/Director.sln

# Build the Director application
RUN dotnet publish ./Director/Director.sln \
    -c Release \
    -o out \
    --no-restore

# Run Director application
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine as runtime

WORKDIR /Director

# Install curl in the runtime image
RUN apk add --no-cache curl

# Copy everything needed to run the Director from the "build" stage.
COPY --from=build /Director/out ./

# Switch to a non-privileged user (defined in the base image) that the Director will run under.
# Create a new user and change directory ownership
RUN adduser --disabled-password \
  --home /Director \
  --gecos '' dotnetuser && chown -R dotnetuser /Director
USER dotnetuser

ENTRYPOINT ["./Director"]
