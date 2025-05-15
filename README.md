# Personalized Recommendation Engine - Microservices Architecture

A scalable recommendation system built with microservices, gRPC, Kafka, and MongoDB that provides personalized product recommendations based on user interactions.

## Table of Contents
- [Features](#features)
- [System Architecture](#system-architecture)
- [Services](#services)
- [Technologies](#technologies)
- [API Documentation](#Technologies)

## Features
- **Real-time recommendations** based on user preferences
- **Event-driven architecture** with Kafka
- **High-performance** gRPC communication
- **JWT authentication** with role-based access
- **Hybrid recommendation** ()

## Services
  1. API Gateway
  Entry point for all client requests
  
  REST & GraphQL endpoints
  
  JWT validation
  
  Routes requests to microservices
  
  2. User Service
  Manages user profiles and preferences
  
  gRPC endpoints:
  
  GetUserPreferences
  
  CreateUser
  
  3. Product Service
  Product catalog management
  
  gRPC endpoints:
  
  GetProductsByCategory
  
  CreateProduct
  
  4. Recommendation Service
  Kafka consumer processing user interactions
  
  Generates and stores recommendations

## Technologies
  Component	Technology Stack
  API Gateway	ASP.NET Core, Hot Chocolate (GraphQL)
  Services	gRPC, Protocol Buffers
  Event Streaming	Apache Kafka
  Databases	MongoDB
  Authentication	JWT, ASP.NET Core Identity
  Containerization	Docker, Docker Compose
  Monitoring	OpenTelemetry, Prometheus

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
