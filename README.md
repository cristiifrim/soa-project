# Ad Tracker

Ad Tracker is a classified ads platform. The frontend uses a microfrontend setup with Angular and Nx (Module Federation). The backend is NestJS with JWT auth, Kafka for notifications, and RabbitMQ for chat.

## Frontend

- **Microfrontends (Angular + Nx)**
  - Main app: **http://localhost:4200**
  - Login app: **http://localhost:4201**

Remote apps are declared in `module.federation.manifest.json` inside the main-app. Example:

```json
{
  "login": "http://localhost:4201/"
}
```

The main app loads and uses these remotes; the same pattern applies for other remotes (e.g. dashboard).

## Backend

- **REST API with JWT authentication**

Protected routes use the JWT guard, for example:

```typescript
@UseGuards(AuthGuard('jwt'))
@Get('all')
findAll() {
  return this.adsService.findAll();
}
```

## Microservices

- **Kafka** – streaming events (e.g. when a new ad is posted) for notifications.
- **RabbitMQ** – message broker for ad chat.

Brokers are configured for Docker (e.g. `kafka:9093`, `rabbitmq:5672`). See the backend and `docker-compose.yml` for connection options.

## Docker

All services (backend, main-app, login, dashboard, MongoDB, Kafka, RabbitMQ, etc.) are defined in `docker-compose.yml`.

**Start everything:**

```shell
docker-compose up -d
```

Then open the main app at http://localhost:4200.
