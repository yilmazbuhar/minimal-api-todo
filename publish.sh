#!/bin/bash
cd src
dotnet clean
dotnet restore
dotnet build --configuration Release
dotnet publish --configuration Release -o publish