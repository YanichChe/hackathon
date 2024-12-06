# Create a stage for building the Juniorlication.
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build

COPY ./ /Junior
COPY ../Nsu.HackathonProblem.Contracts ./Nsu.HackathonProblem.Contracts

WORKDIR /Junior

# This is the architecture you’re building for, which is passed in by the builder.
# Placing it here allows the previous steps to be cached across architectures.
ARG TARGETARCH

# Restore dependecies
RUN dotnet restore ./Junior/Junior.sln

# Build the Juniorlication
RUN dotnet publish ./Junior/Junior.sln \
    -c Release \
    -o out \
    --no-restore

# Run Juniorlication
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine as runtime

WORKDIR /Junior

# Copy everything needed to run the Junior from the "build" stage.
COPY --from=build /Junior/out ./

# Switch to a non-privileged user (defined in the base image) that the Junior will run under.
# create a new user and change directory ownership
RUN adduser --disabled-password \
  --home /Junior \
  --gecos '' dotnetuser && chown -R dotnetuser /Junior
USER dotnetuser

ENTRYPOINT ["./Junior"]
