FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /todoapp

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore /todoapp/src
# Build and publish a release
RUN dotnet publish /todoapp/src/Todo.Api -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "Todo.App.dll"]