# Personalized Recommendation Engine - Microservices Architecture

![Architecture Diagram](docs/architecture.png)

A scalable recommendation system built with microservices, gRPC, Kafka, and MongoDB that provides personalized product recommendations based on user interactions.

## Table of Contents
- [Features](#features)
- [System Architecture](#system-architecture)
- [Services](#services)
- [Technologies](#technologies)
- [Installation](#installation)
- [Configuration](#configuration)
- [API Documentation](#api-documentation)
- [Testing](#testing)
- [Deployment](#deployment)
- [Contributing](#contributing)

## Features
- **Real-time recommendations** based on user preferences
- **Event-driven architecture** with Kafka
- **High-performance** gRPC communication
- **JWT authentication** with role-based access
- **Hybrid recommendation** (content-based + collaborative filtering)
- **Containerized** with Docker

## System Architecture
```mermaid
graph TD
  A[Client] --> B[API Gateway]
  B --> C[User Service]
  B --> D[Product Service]
  B --> E[Kafka]
  E --> F[Recommendation Service]
  C --> G[MongoDB Users]
  D --> H[MongoDB Products]
  F --> I[MongoDB Recommendations]
