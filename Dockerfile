FROM microsoft/dotnet-framework:4.6.2

ADD . /app
WORKDIR /app
COPY Theia.Api/bin/Debug .

EXPOSE 65308

ENTRYPOINT ["cmd.exe", "/k", "Theia.Api.exe"]