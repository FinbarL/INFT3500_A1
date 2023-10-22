FROM mcr.microsoft.com/mssql/server:2019-latest

WORKDIR /usr/src/app

COPY . /usr/src/app

ENV SA_PASSWORD=P4ssw0rd!
ENV ACCEPT_EULA=Y
ENV MSSQL_PID=Express

EXPOSE 1433

CMD /bin/bash ./entrypoint.sh