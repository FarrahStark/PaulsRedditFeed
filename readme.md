# Paul's Reddit Feed

ASP.NET Core (.NET 6.0) Twitter Search API Usage Example

## Getting Started

### Prerequisites

Prerequisits are installed automatically

- Powershell Core 7.2+ (used for automating project setup)
- Visual Studio 2022
- Docker (used to host a local redis database for caching, sticky websocket sessions and, integration testing)
- Get Reddit API Secrets (request the secrets from paulstark256@gmail.com)

### Getting the Project Running Locally

- Install/update powershell core to 7.2 or higher
- run `dev-startup.ps1`
- Install Docker for windows
- clone this repository
- open PaulsRedditFeed.sln in Visual Studio 2022
- In Solution Explorer, right-click PaulsRedditFeed project, and select manage user secrets
- A secrets.json file will open
- replace the contents of secrets.json with the the secrets requested from paulstark256@gmail.com
- Save secrets.json and close it
