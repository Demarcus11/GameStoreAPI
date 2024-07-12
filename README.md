# Tutorial Project Description

We created a REST API for a game store.

It has these CRUD options:

    GAMES ENDPOINTS
    - GET: /games | Gets all games
    - GET: /games/{id} | Gets a game by id
    - POST: /games | Creates a new game
    - PATCH: /games/{id} | Updates a game
    - DELETE: /games/{id} | Deletes a game

    IMAGES ENDPOINT
    - POST: /images | Uploads a file to storage

We have 2 versions: v1 and v2. v2 has only 2 endpoints, GET: /games and GET /games/{id}. v1 has all endpoints listed above. This is just to demonstrate the importance of having different versions of the API, so you don't break clients applications when updates are made to the API.

Example of API GET Request:

    GET: /games

    ```json
    {
        "id": 1,
        "name": "Street Fighter II",
        "genre": "Fighting",
        "price": 9.99,
        "releaseDate": "1991-02-01T00:00:00",
        "imageUri": "https://gamestore01.blob.core.windows.net/images/StreetFighterII.jpg"
    }
    ```

We also have filtering by limit and genre:

    - GET: /games/limit=10
    - GET: /games/genre=Racing

We also have authentication on the POST and PATCH routes using OAuth 2.0.

We also have an interface that uses the API to demonstrate a real life use case of an REST API.

Great video to watch for building a REST API using ASP.NET Core: <https://youtu.be/bKCzoR01lpE?si=UcvhGDRga7gYtWjz>

## Entities folder

The Entities folder holds all the OOP classes such as our Game.cs class.

## Route Groups

At first we added all our routes inside Program.cs as seperate routes, but there's a better more organized way of doing this.

We can group endpoints that all use a common prefix. Instead of writing /games for all our endpoints for /games, we use a route group.

    ```C#
    var group = server.MapGroup("/games");
    ```

Now we can just write / or /{id} instead of /games or /games/{id}.

## Server-Side Validation

Server-side validation is validating things such as POST requests server side. For example, a user might send an empty string for the name even though the name is required. This will still go through and the server will send back a 201 message, but we don't want that. If the name is an empty string we want to send back a 204 for bad request error. This is where server-side validation comes in.

We use "attributes" to achieve this. In our Game.cs class, we can validate any property using attirbutes. Here we add validation to the Name property.

    ```C#
    [Required] // <--- Attribute
    [StringLength(50)]
    public required string Name { get; set; }
    ```

The `[Required]` attribute makes sure the Name property isn't an empty string.

The `[StringLength()]` attribute makes sure the Name property isn't more than 50 characters.

## NuGet

NuGet is npm for .NET. You can import and packages. The package we use for this project is `MinimalApis.Extensions` by damianedwards. You can find the CLI by going to NuGet.org.

We can see our packages by going into the .csproj file:

    ```C#
    <Project Sdk="Microsoft.NET.Sdk.Web">
    <PropertyGroup>
        <TargetFramework>net8.0</TargetFramework>
        <Nullable>enable</Nullable>
        <ImplicitUsings>enable</ImplicitUsings>
        <RootNamespace>C___ASP.NET_Core_</RootNamespace>
    </PropertyGroup>

    <ItemGroup> // <--- packages go here
        <PackageReference Include="MinimalApis.Extensions" Version="0.11.0" />
    </ItemGroup>

    </Project>
    ```

We can validate all our endpoints using this validation package by going to the route group and adding the `.WithParameterValidation() function.

    ```C#
    var group = server.MapGroup("/games")
                .WithParameterValidation();
    ```

For example, if I make a POST request with an an empty name, price over 100, and invalid URI, using this package i'll get this validation message:

    ```JSON
    {
        "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        "title": "One or more validation errors occurred.",
        "status": 400,
        "errors": {
            "Name": [
                "The Name field is required."
            ],
            "Price": [
                "The field Price must be between 1 and 100."
            ],
            "ImageUri": [
                "The ImageUri field is not a valid fully-qualified http, https, or ftp URL."
            ]
        }
    }
    ```

## Repository pattern

A repository is an abstraction that encapsulates the logic for accessing and manipulating data.

Let's say you're application is using a SQL database. In 5 years, an SQL database doesn't fit current requirements and you want to switch to a NoSQL database. You would have to rewrite application logic in order to talk to the different database and rewriting this logic can be error prone and time consuming.

A repository sits inbetween the application logic and the database. The application logic talks to the repository and the repository talks to the database. This way if the database needs to change we only change the repository code and not the application logic.

Repository pattern:

    - Decouples the application logic from the data layer
    - Minimizes duplicate data access logic

InMemGamesPropsitory.cs class:

    - holds logic for talking to database/array, it's a non-static class, so it can be instaniated.
    - we use it inside our MapGamesRouter function inside the GamesRoutes.cs class.
    - we have decoupled the management of the data from the routes themselves.

## Dependency Injection

First, we look at what a dependency is. Imagine we have 2 classes, MyService and MyLogger. MyService uses MyLogger's 'LogThis("foo")' method whenever it does an operation. We say MyLogger is a dependency of MyService because it MyService depends on MyLogger.

Now, for MyService to start using MyLogger it has to create a new instance of MyLogger in its constructor:

    ```C#
    public MyService()
    {
        var logger = new MyLogger();
        logger.LogThis("Im Ready");
    }
    ```

The issue with this is that what if the author of MyLogger changes how the logger is used by making you need to have a MyFileWritter object passed into the MyLogger constructor in order to be used:

    ```C#
    public MyService()
    {
        var writter = new MyFileWritter("output log");
        var logger = new MyLogger(writter);
        logger.LogThis("Im Ready");
    }
    ```

Issues:

    - MyService is tightly coupled to the Logger dependency. Any changes to MyLogger require changes to MyService.

    - MyService needs to know how to construct and configure the MyLogger dependency.

    - It's hard to test MyService since the MyLogger dependency cannot be mocked or stubbed.

To fix this we use dependency injection. Instead of MyLogger being instaniated inside the constructor of MyService, we 'inject' it into the MyService constrcutor:

    ```C#
    public MyService(MyLogger logger)
    {
        logger.LogThis("Im Ready");
    }
    ```

Now, MyService doesn't need to know how to construct or configure the logger, it just receives it and starts using it. We'll who constructs it?

.NET provides the IServiceProvider container and we can register MyLogger with it. When a HTTP Request is made and it needs an instance of MyService, the service container will notice its dependencies and will resolve, construct, and inject those dependencies into a new instance of MyService via its constructor.

Benefits:

    - MyService won't be affected by changes to its dependencies.

    - MyServices doesn't need to know how to construct or configure its dependencies.

    - Dependencies can also be injected as parameters to minimal API endpoints.

    - Opens the door to using Dependency Inversion. Which is the belief that code should depend on abstractions as opposed to concrete implementations. Instead of MyService depending on MyLogger directly, we can make a ILogger abstraction and have MyService depend on that and MyLogger implement ILogger so we can use any Logger we want. If we wanted to use a different Logger called CloudLogger then we don't have to change MyService. This is useful because dependencies can be swapped out for a different implementation without modifying MyService. Code is also cleaner, easier to modify, and reuse.

One more thing to understand called Service Lifetimes. So our app will use dependency injection by registering MyLogger to IServiceProvider abstraction. When a HTTP Request arrives, IServiceProvider will resolve, construct, and inject a MyLogger instance into MyService.

Question, if another request arrives, should IServiceProvider create another instance of MyLogger or use the previous one? or another service in our application uses MyLogger, should it use the same MyLogger instance for all services? the answer is in Service Lifetimes which you configure when you register a dependency with IServiceProvider.

    - The Transient Service Lifetime: Let's say MyLogger is a very lightweight and stateless service, so its ok to create a new instance everytime a class needs it. In that case we register MyLogger with the AddTransient<MyLogger>() method. Everytime a HTTP request arrives IServiceProvider will resolve, construct, and inject a new instance of MyLogger.

    - The Scoped Service Lifetime: If MyLogger is a class that keeps track of state that needs to be shared across multiple classes that participate in an HTTP Request, we use the AddScoped<MyLogger>(). When a HTTP Request arrives IServiceProvider will reuse the same instance of MyLogger for all services that participate in that HTTP Request. However, if a new HTTP Request is made, a new instance is created and the cycle repeats. So instances are "scoped" to the HTTP Request that made it.

    Example, if you do a POST request to add a new game then do a GET request to get all games, the new game wouldn't be visible because scoped lifetime creates a new instance of the class for each new HTTP request. the new game is scoped to the previous HTTP Request.

    - The Singleton Service Lifetime: If MyLogger is not lightweight and keeps track of state that needs to be shared between all services that use MyLogger then we use AddSingleton<MyLogger>(). When a HTTP Request is made IServiceProvider creates a new instance and uses it for any service that depends on MyLogger. If a new HTTP Request is made then it will use the same MyLogger instance for the lifetime of the app. (We use this for this project). When using an array we need the state to stay the same through all HTTP requests, so we use this one.

    For this project, our service is the repository that talks to the database. Instead of constructing the repository inside the GamesRoutes class, we want to decouple the InMemGamesRepository class from GamesRoutes. We do this by creating an interface just like in the MyLogger example called IGamesRepository and GamesRoutes implements that interface and we pass in that interface into each route. IServiceProvider will find out which class implements the interface passed in and construct an object.

## Data Transfer Object

A Data Transfer Object (DTO) is an object that carries data between processes or applications. In the context of a REST API, a DTO can be considered a contract between the client and server.

    Clients request game data from the REST API and the REST API requests the data from the repository.
    Clients ---GET /games/1--> REST API ----Retrieves game with ID 1---> Repository

    The Repository finds and returns the data as an "Entity": id: 1, Name: "Fifa 25", Price: 69.99 to the REST API and the REST API sends the Entity as JSON to the Client: { "id": 1, "name": "Fifa 25", "price": 69.99 }.

    Clients <---Entity as JSON--- REST API <----Entity---- Repository

The issue with this is that what if requirements change and you have to change "price" to "retailPrice" and add a new field "SecretCode" that should only be used for backend purposes and shouldn't be sent to the clients. This will break the client because it's expecting "price" and not "retailPrice" so their frontend code will no longer work, it also exposes a secret not meant for it. REST APIs should never return the entities that came from the repository to clients, but instead should translate entities into DTO's that conform the the agreement between client and server which for this case means we always return "id", "name" and "price" to clients and nothing else. The DTO acts as a contract that defines the expectations and requirements for how data will be exchanged between client and server.

## .NET Configuration to Access SQL Server

- Standup a SQL Server instance using Docker
- Use the .NET configuration system
- Use the .NET Secret Manager to store app secrets

Intro to Docker
Docker is a platform that packages and runs applications in isolated environments called containers.

Our Web API needs a database to store and query games. We use SQLServer for this.

SQLServer and all its dependencies are packaged into a "Docker image." This image is available on Docker Hub, where we can download and run it. To do this, we use the Docker Engine, included with Docker Desktop. Docker images can run anywhere the Docker Engine is available.

To download an image, use the `docker pull` command. To run it, use the `docker run` command.

Once the image runs, it becomes a "Docker container," which, in this case, is a SQLServer. The database itself is stored outside the container, so it isn't deleted when the container is destroyed.

Docker commands for our Docker image

- Starting SQL Server:

  ```powershell
  $sa_password = "[SA PASSWORD HERE]"
  docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=$sa_password" -e "MSSQL_PID=Evaluation" -p 1433:1433 -v sqlvolume:/var/opt/mssql --name sqlpreview --hostname sqlpreview -d --rm --name mssql mcr.microsoft.com/mssql/server:2022-preview-ubuntu-22.04
  ```

.NET Configuration System

- To define a connect string, in appsettings.json:

  ```json
  {
    "Logging": {
      "LogLevel": {
        "Default": "Information",
        "Microsoft.AspNetCore": "Warning"
      }
    },
    "AllowedHosts": "*",
    // Add this line, <name of connection string>: <connection string>
    "ConnectionStrings": {
      "GameStoreContext": "Server=localhost; Database=GameStore; User Id=sa; Password=PASSWORD-GOES-HERE;TrustServerCertficate=True"
    }
  }
  ```

We can read the connection string by using the `GetConnectionString()` method on the Configuration object:

    ```C#
    var connString = builder.Configuration.GetConnectionString("<name of connection string>");
    ```

## Storing secrets for local development

Storing the password for the connection string is a big security risk.

We can use Secret Manager to store our connection string.

Use command to initialize secret manager:

    ```powershell
    dotnet user-secrets init
    ```

Inside your .csproj file you'll see a <UserSecretsId> tag:

    ```csproj
    <Project Sdk="Microsoft.NET.Sdk.Web">

    <PropertyGroup>
        <TargetFramework>net8.0</TargetFramework>
        <Nullable>enable</Nullable>
        <ImplicitUsings>enable</ImplicitUsings>
        <RootNamespace>C___ASP.NET_Core_</RootNamespace>
        <UserSecretsId>a3984134-4510-4023-b121-d1611548d177</UserSecretsId> <------ Here
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="MinimalApis.Extensions" Version="0.11.0" />
    </ItemGroup>

    </Project>
    ```

command to set the connection string to secret manager

Video: 3:06:57

    ```powershell
    $sa_password = "[SA PASSWORD HERE]"
    dotnet user-secrets set "ConnectionStrings:GameStoreContext" "Server=localhost; Database=GameStore; User Id=sa; Password=$sa_password;TrustServerCertificate=True"
    ```

After its set then we no longer need the "ConnectionsStrings" section in appsettings.json because now its coming from secret manager.

## Using Entity Framework Core

- Understand Entity Framework Core
- Implement an Entity Framework DbContext
- Use database migrations
- Implement an Entity Framework repository
- Use the asynchronous programming model

INTRODUCTION TO ENTITY FRAMEWORK CORE

The Need for Object-Relational Mapping (ORM)

- Whenever the request to receive a game from the API, we want to create that game in the database. Whenever the client requests a list of games, we want to query the list from the database.

- There's a problem. The database doesn't speak the same language as our ASP.NET Core Web API. The Web API is coded in C#, but the SQL Server only understands the SQL language. This means our API has to translate the request into a SQL query, then send that query to the SQL Server, then the SQL Server has to read the resulting SQL Server rows, then translate the SQL Server rows to the API response.

- This presents a few problems, as a C# developer you need to learn a new language (SQL) to create the queries and you need to learn it well to get good performance out fo it. You also need to write a lot of additional data-access code that translates C# into SQL and vice versa which is error prone. You would also need to manually keep C# models in sync with the schema of the DB tables.

What is Object-Relational Mapping (ORM)?

- Imagine your app is created using object-oriented programming. Therefore, it includes objects to represent, "Songs", "Artists", and "Playlists". If you're working with a relational database you'll have tables that correspond to those objects. Instead of having to write custom code to map those objects to tables each time you need to send or recieve data to or from the database, you can setup a map between them so your program can keep working with objects while another component (Object-Relational Mapper) takes care of transforming objects to tables and vice versa. In summary, it's a technique for converting data between a relational database and an object-oriented program.

What is Entity Frameowrk Core?

- It's a lightweight, flexible, open source and cross-platform object-relational mapper for .NET. It sits between the API and the SQL Server database. It translates your C# data access code into SQL statements that SQL Server understands. It also translates any resulting data from the database into objects that the API can easily manage.

- Benefits include: No need to learn a new language, Minimal data-access code (LINQ - Language Integrated Query), Tooling to keep C# models in sync with DB tables, It keeps track of changes made to your C# objects at runtime, so it knows what changes to send to the DB, and it supports multiple DB providers.

How to add Entity Framework Code support into the API

- Watch video: 3:13:19

Asynchronous Programming Model

- Watch video: 4:00:43

Video Creds: <https://youtu.be/bKCzoR01lpE?si=jEv3Eawx4OjLXPhE>, ASP.NET Core Tutorial for Beginners | .NET 7 by Julio Casal
#   G a m e S t o r e A P I  
 