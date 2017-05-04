FROM stefanscherer/node-windows:6.7.0-nano

ADD . /app 
WORKDIR /app

COPY package.json .
COPY main.js .

RUN npm install

EXPOSE 8081

CMD [ "node", "main.js" ] 