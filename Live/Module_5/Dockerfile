FROM mcr.microsoft.com/dotnet/runtime:10.0-alpine-extra
WORKDIR testje

COPY ./data.txt .

ENTRYPOINT [ "dotnet" "myapp.dll" ]