# Paul's Reddit Feed

Highly scalable ASP.NET Core (.NET 7.0) web application that displays data asynchronously retrieved from the Reddit API.

## Summary

PaulsRedditFeed allows users to select N-number of subreddits to watch for active users, and the current hottest post of that subreddit

Uses Redis as a distributed cache, and message queue for loadbalanced deployments of this application. SignalR uses Redis to create sticky
sessions to keep clients connected to the same server instance for their session.

The reddit API is rate limited to 60 requests per second so for testing and demonstration purposes I created a simulated reddit API with no
rate limits. This allows enough data throughput to show the real-time nature of the system, and to demonstrate proper multithreading, logging,
caching, authentication, and websockets usage with sticky sessions.

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

1. Install/update PowerShell core to 7.2 or higher https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell?view=powershell-7.3
1. Install Docker https://www.docker.com/
1. Install .NET 7 SDK https://dotnet.microsoft.com/en-us/download/dotnet/7.0
1. Restart your machine
1. Clone this repository
1. Make sure Docker is running
1. Switch Docker to use Linux Containers
1. Run `dev-startup.ps1` from a PowerShell terminal
1. Request a copy of the secrets from Paul

#### Running the Project

##### Visual Studio 2022

1. Open `PaulsRedditFeed.sln`
1. In Solution Explorer, right-click PaulsRedditFeed project, and select manage user secrets
1. A secrets.json file will open
1. Replace the contents of secrets.json with the the secrets from Paul
1. Save secrets.json and close it
1. Run PaulsRedditFeed project to see it in action

##### .NET CLI

1. Create a new `secrets.json` file in one of the following locations:
   - Windows: &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`%APPDATA%\Microsoft\UserSecrets\aae5bb4b-a48c-402c-9522-4e0811d57b4a/secrets.json`
   - Linux/macOS: &nbsp;&nbsp;`~/.microsoft/usersecrets/aae5bb4b-a48c-402c-9522-4e0811d57b4a/secrets.json`
1. Open the created `secrets.json`
1. Paste the secrets from Paul, save and close the file
1. Open a terminal and navigate to the folder where you cloned the repo
1. If you haven't before run `dotnet dev-certs https --trust` to enable local https for the app
1. Run `dotnet build PaulsRedditFeed.sln`
1. Run `dotnet run --project ./PaulsRedditFeed/PaulsRedditFeed.csproj`
1. Open a browser and navigate to `https://localhost:7154` to see it in action

### Running the Tests

This project uses xUnit, and a redis cache dedicated for testing to keep test data separate from the data used to run the app. Integration tests, and unit tests are all xUnit and are run side by side.

1. In a terminal at the folder where you cloned the repo run `dotnet test`

### Notes

- The docker containers created by `dev-startup.ps1` will be restarted automatically on reboot, and must be removed with the Docker GUI or CLI if you want the conainers off your machine. To remove the conainers permanently:
  - Run `docker rm --force PaulsRedditFeedCache`
  - Run `docker rm --force PaulsRedditFeedTestCache`
