FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /todoapi
EXPOSE 5005

# Copy everything
COPY . ./

# Restore as distinct layers
RUN dotnet restore /todoapi/src
# Build and publish a release
RUN dotnet publish /todoapi/src/Todo.Api -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /todoapi
COPY --from=build-env /todoapi/out .

ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "Todo.Api.dll"]