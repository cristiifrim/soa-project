# Ad Tracker

Ad Tracker is a classified ads platform. The frontend uses a microfrontend setup with Angular and Nx (Module Federation). The backend is .NET: an API gateway with reverse proxy, JWT auth, SignalR hubs for chat and notifications, and microservices using MongoDB, Kafka, and RabbitMQ.

## Frontend

- **Microfrontends (Angular + Nx)**
  - Main app: **http://localhost:4200**
  - Login: **http://localhost:4201**
  - Dashboard: **http://localhost:4202**

Remotes are declared in `frontend-mf/apps/main-app/public/module-federation.manifest.json`. The main app loads these remotes at runtime.

- **SignalR** (loaded from CDN) is used for real-time chat and notifications; the gateway exposes hubs at `/hubs/chat` and `/hubs/notifications`.

## Backend (.NET)

- **Soa.Gateway** (port 3000) – Reverse proxy to microservices, SignalR hubs (chat, notifications), CORS.
- **Soa.UserService** – Users and JWT authentication.
- **Soa.AdsService** – Ads; publishes events to Kafka.
- **Soa.ChatService** – Chat messages; uses RabbitMQ.

All .NET services live under `backend-dotnet/`. The gateway is the single entry point for the frontend (API and SignalR).

## Infrastructure

- **MongoDB** – Data store for users, ads, and chat (e.g. port 27017).
- **Kafka** (+ Zookeeper) – Event streaming (e.g. new ad notifications).
- **RabbitMQ** – Message broker for chat (ports 5672, 15672 for management UI).

Connection details and hosts (e.g. `kafka:9093`, `rabbitmq:5672`) are in `docker-compose.yml` and each service’s configuration.

## Docker

All services are defined in `docker-compose.yml`: gateway, user-service, ads-service, chat-service, main-app, login, dashboard, MongoDB, RabbitMQ, Kafka, Zookeeper.

**Start everything:**

```bash
docker-compose up -d
```

Then open the main app at **http://localhost:4200**.

**Rebuild after code changes (e.g. frontend):**

```bash
docker-compose build --no-cache main-app login dashboard
docker-compose up -d main-app login dashboard
```
