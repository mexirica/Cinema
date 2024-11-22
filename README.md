There is a couple of microservices which implemented **Cinema** modules over **Cinema.Api** and **Notification** microservices with **Relational database (PostgreSQL)** with communicating over **RabbitMQ** and using **Yarp API Gateway**.

![microservices](https://github.com/mexirica/Cinema/blob/master/cinemasystem.drawio.png)


## Whats Including In This Repository

#### Catalog microservice which includes; 
* ASP.NET Core Minimal APIs and latest features of .NET8 and C# 12, Following REST API principles, CRUD
* **Vertical Slice Architecture** implementation with Feature folders and single .cs file includes different classes in one file
* CQRS implementation using MediatR library
* CQRS Validation Pipeline Behaviors with MediatR and FluentValidation
* Use Carter for Minimal API endpoint definition
* Cross-cutting concerns Logging, Global Exception Handling and Health Checks
* Developing **CQRS with using MediatR, FluentValidation and Mapster packages**
* Publish Message Queue with using **MassTransit and RabbitMQ**
* Entity Framework Core ORM — Psql Data Provider, Seeding and Migrations to simplify data access and ensure high performance
* Implementing **CQRS, and Vertical Slicing** with using Best Practices

#### Microservices Notification
* Async Microservices Communication with **RabbitMQ Message-Broker Service**
* Using **MassTransit** for abstraction over RabbitMQ Message-Broker system
* Subscribing messages from Cinema.API microservices	
* Notify the user sending emails.
	
#### Yarp API Gateway Microservice
* Develop API Gateways with **Yarp Reverse Proxy** applying Gateway Routing Pattern
* Yarp Reverse Proxy Configuration; Route, Cluster, Path, Transform, Destinations
* **Authentication**

#### Docker Compose establishment with all microservices on docker;
* Containerization of microservices
* Containerization of databases

## Run The Project
You will need the following tools:

* [.Net Core 8 or later](https://dotnet.microsoft.com/download/dotnet-core/8)
* [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Installing
Follow these steps to get your development environment set up: (Before Run Start the Docker Desktop)
1. Clone the repository
2. At the root directory of solution, run below command:
```csharp
docker-compose up -d
```

3. Wait for docker compose all microservices. That’s it! (some microservices need extra time to work so please wait if not worked in first shut)

### Routes
1.Gateway (localhost:8080)
* **/user/login** Send a POST to login
* **/user/register** Send a POST to sign up 
* **/api/public/** to access public endpoints (Booking to check a seat)
* **/api/protected** to access protected endpoints in booking

* Use the last two as prefixes before the booking path

2.Booking (localhost:8081)
* **/screenings/multiple-bookings** Send a POST to book one or more screening or one or more seats
* **/screenings/{/screeningId}/buy** Send a POST to book a screening without choose a seat
* **/screenings/{screeningId}/choose-seat** Send a POST to book a screening and choose a seat
* **/screenings/{screeningId}/check-seat-available/{seatId}** Send a GET to see if the seat is available for this screening
* **/screenings/{screeningId}/cancel-booking** Send a Delete to cancel the sale
