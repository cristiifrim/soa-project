FROM node:18

WORKDIR /app

COPY package.json package-lock.json ./

RUN npm ci

COPY . .

EXPOSE 4201

CMD ["npx", "nx", "run", "login:serve", "--host", "0.0.0.0", "--disable-host-check"]