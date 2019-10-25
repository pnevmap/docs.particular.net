# Build the sender from a .NET Core SDK image
FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY . .
WORKDIR /src/Sender
RUN dotnet publish -c Release -o /app

# Create the application image using the output from the build
FROM microsoft/dotnet:2.1-runtime AS base
FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Sender.dll"]