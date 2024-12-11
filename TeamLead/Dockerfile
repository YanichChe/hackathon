# Create a stage for building the TeamLeadlication.
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build

COPY ./ /TeamLead
COPY ../Nsu.HackathonProblem.Contracts ./Nsu.HackathonProblem.Contracts

WORKDIR /TeamLead

# This is the architecture you’re building for, which is passed in by the builder.
# Placing it here allows the previous steps to be cached across architectures.
ARG TARGETARCH

# Restore dependecies
RUN dotnet restore ./TeamLead/TeamLead.sln

# Build the TeamLeadlication
RUN dotnet publish ./TeamLead/TeamLead.sln \
    -c Release \
    -o out \
    --no-restore

# Run TeamLeadlication
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine as runtime

WORKDIR /TeamLead

# Install curl in the runtime image
RUN apk add --no-cache curl

# Copy everything needed to run the TeamLead from the "build" stage.
COPY --from=build /TeamLead/out ./

# Switch to a non-privileged user (defined in the base image) that the TeamLead will run under.
# create a new user and change directory ownership
RUN adduser --disabled-password \
  --home /TeamLead \
  --gecos '' dotnetuser && chown -R dotnetuser /TeamLead
USER dotnetuser

ENTRYPOINT ["./TeamLead"]
