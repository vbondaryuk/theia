FROM microsoft/dotnet-framework:4.6.2

ADD . /app
WORKDIR /app
COPY Theia.Api/bin/Release .

EXPOSE 6535

ENTRYPOINT ["Theia.Api.exe"]