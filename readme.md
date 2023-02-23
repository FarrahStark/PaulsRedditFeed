# Paul's Reddit Feed

ASP.NET Core (.NET 6.0) Reddit API Usage Example

## System Architecture

![PaulsRedditFeed Software Architecture Diagram](AppArchitecture.png?raw=true "Title")

## Getting Started

### Prerequisites

Prerequisits are installed automatically

- Powershell Core 7.2+ (used for automating project setup)
- .NET 6 SDK/Runtime
- Docker (used to host a local redis database for caching, sticky websocket sessions and, integration testing)
- Get Reddit API Secrets (request the secrets from paulstark256@gmail.com)

### Running the project Locally

You need to have the ASP.NET webserver built

- Install/update powershell core to 7.2 or higher https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell?view=powershell-7.3
- run `dev-startup.ps1`
- Install Docker https://www.docker.com/
- clone this repository
- open PaulsRedditFeed.sln in Visual Studio 2022
- In Solution Explorer, right-click PaulsRedditFeed project, and select manage user secrets
- A secrets.json file will open
- replace the contents of secrets.json with the the secrets requested from paulstark256@gmail.com
- Save secrets.json and close it
