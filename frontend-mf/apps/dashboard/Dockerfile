FROM node:18

WORKDIR /app

COPY package.json package-lock.json ./

RUN npm ci

COPY . .

EXPOSE 4202

CMD ["npx", "nx", "run", "dashboard:serve", "--host", "0.0.0.0", "--disable-host-check"]