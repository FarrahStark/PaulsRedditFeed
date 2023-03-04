# Paul's Reddit Feed

ASP.NET Core (.NET 7.0) Reddit API Usage Example.

## Summary

PaulsRedditFeed displays statistics generated from data collected from reddit API.

Uses Redis as a distributed cache, and message queue for loadbalanced deployments of this application. SignalR uses Redis to create sticky
sessions to keep clients connected to the same server instance for their session.

The reddit API is rate limited to 60 requests per second so don't scale past This allows enough data throughput to show the real-time nature of the system, and to demonstrate proper multithreading, logging,
caching, authentication, and websockets usage with sticky sessions.

## Cool Stuff About Paul's Reddit Feed

- Cross platform (tested on: Windows/Linux/macOS)
- Proper use of multithreading using the Task Parallel Library and async/await
  - Keeps background work from blocking requests
  - The RedditMonitor should probably be run in it's own container rather than using threading so the monitoring workload
    doesn't affect client web traffic performance, and so web traffic and monitoring can be scaled independently.
- Distributed consumer provider pattern using redis pub sub
  - Each server gets work from the redis pub sub and handles it so the work is evenly distributed between severs
- Stateless servers for easy scaling
- Response caching
- Sticky websocket sessions to keep clients connected to the same sever using SingalR and Redis
- Docker Compose for quickly standing up a local load balanced environment with 3 servers
  - uses nginx to load balance traffic between 3 instances of PaulsRedditFeed running on linux containers
- UI that uses websockets to update the page as stats updates are reported

## System Architecture

![PaulsRedditFeed Software Architecture Diagram](AppArchitecture.png?raw=true "Title")

### Monitoring Reddit

Since this is a load balanced application, we don't want every instance scanning the same subreddits. We want to spread the monitoring load across
server instances and not duplicate calls. The instances don't talk to eachother so we queue up the monitoring tasks. Then the server instances
process the monitoring tasks in the message queue, so each server can perform a different monitoring task, spreading the work out.

## Getting Started

### Prerequisites

- Powershell Core 7.2+
- .NET 7 SDK
- Docker
- Reddit API Secrets for local environment

### Running the project Locally

#### Install Prerequisites

1. Install/update powershell core to 7.2 or higher https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell?view=powershell-7.3
1. Install Docker https://www.docker.com/
1. Install .NET 7 SDK https://dotnet.microsoft.com/en-us/download/dotnet/7.0
1. Restart your machine
1. Clone this repository
1. Switch Docker to use Linux Containers
1. Run `dev-startup.ps1`
1. Request a copy of the secrets from Paul

#### Running the Project

##### Setup Your user Secrets

1. Create a new `secrets.json` file in one of the following locations:
   - Windows: &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`%APPDATA%\Microsoft\UserSecrets\aae5bb4b-a48c-402c-9522-4e0811d57b4a/secrets.json`
   - Linux/macOS: &nbsp;&nbsp;`~/.microsoft/usersecrets/aae5bb4b-a48c-402c-9522-4e0811d57b4a/secrets.json`
1. Open the created `secrets.json`
1. Paste the secrets from Paul, save and close the file

Once you have the user secrets setup you can choose from one of the following options:

##### (1) Docker Compose (Windows Only)

This will setup a full integration testing envirionment with load balanced servers and live data.
The path's in `docker-compose.yml` are not cross platform at the momement so it will only work out of the box on windows.

1. Make sure the docker engine is running
1. Open a terminal
1. Run `docker compose up -d`
1. Open your web browser to `localhost:4000`
1. Run `docker compose down` to tear down all the containers for PaulsRedditFeed and stop them

##### (2) Visual Studio 2022 (Windows Only)

1. Open `PaulsRedditFeed.sln`
1. Run PaulsRedditFeed project to see it in action

##### (3) .NET CLI (Cross Platform)

1. Open a terminal and navigate to the folder where you cloned the repo
1. Run `dotnet build`
1. Run `dotnet run --project ./PaulsRedditFeed/PaulsRedditFeed.csproj`
1. Open a browser and navigate to `https://localhost:7154` to see it in action

### Running the Tests

This project uses xUnit, and a redis cache dedicated for testing to keep test data separate from the data used to run the app. Integration tests, and unit tests are all xUnit and are run side by side.

1. In a terminal at the folder where you cloned the repo run `dotnet test`

### Notes

- The docker containers created by `dev-startup.ps1` will be restarted automatically on reboot, and must be removed with the Docker GUI or CLI if you want the conainers off your machine. To remove the conainers permanently:
  - Run `docker rm --force PaulsRedditFeedCache`
  - Run `docker rm --force PaulsRedditFeedTestCache`
