FROM node:18

# Répertoire de travail
WORKDIR /app

COPY package.json package-lock.json ./

RUN npm ci

COPY . .

# Clear Nx cache before serving
RUN npx nx reset || true

EXPOSE 4200

CMD ["npx", "nx", "run", "main-app:serve", "--host", "0.0.0.0", "--disable-host-check"]