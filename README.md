# JobSharp

JobSharp is a job scraper written in C# that scrapes job listings and processes them. This project includes multiple components, such as a console application and a Discord bot.

This is meant to be a personal project for learning purposes. It is not meant to be used for commercial purposes, use at your own risk.

## Features

- Scrape job listings from LinkedIn
- Process and handle job listings
- Integration with Discord for job notifications

## Installation

### Prerequisites
- .NET 6.0 or later
- Discord account (for Discord bot)

### Clone the repository
```bash
git clone https://github.com/01Pollux/JobSharpEx.git
cd JobSharpEx
```

### Configuration
#### Console Application
- Navigate to `JobSharp.App` directory.
- Edit `appsettings.json` file to your preferences.
- Add your secrets using the user-secrets tool.
- Secrets include:
- `LinkedIn:Email` - Your LinkedIn email
- `LinkedIn:Password` - Your LinkedIn password
```bash
dotnet user-secrets set LinkedIn:Email "your-email"
dotnet user-secrets set LinkedIn:Password "your-password"
```

#### Discord Bot
- Navigate to `JobSharp.DiscordBot` directory.
- Edit `appsettings.json` file to your preferences.
- Add your Discord bot token and other secrets using the user-secrets tool.
- Secrets include
- `LinkedIn:Email` - Your LinkedIn email
- `LinkedIn:Password` - Your LinkedIn password
- `Discord:AccessToken` - Your Discord bot token
- `Discord:ChannelId` - Your Discord channel ID
```bash
dotnet user-secrets set LinkedIn:Email "your-email"
dotnet user-secrets set LinkedIn:Password "your-password"
dotnet user-secrets set Discord:AccessToken "your-discord-bot-token"
dotnet user-secrets set Discord:ChannelId your-discord-channel-id
```

### Usage
#### Console Application
- To run the console application:
```bash
cd JobSharp.App
dotnet run
```

#### Discord Bot
- To run the Discord bot:
```bash
cd JobSharp.DiscordBot
dotnet run
```

## Screenshots

![image](https://github.com/user-attachments/assets/612fe097-4bbe-4a03-a59f-33a1ace82cd2)

![image](https://github.com/user-attachments/assets/0c247c51-ed46-4a8f-868b-291391e7d4c8)

## Contributing

Contributions are welcome! Please fork the repository and create a pull request.

## License

This project is licensed under the MIT License.

## Maintainers

- [01Pollux](https://github.com/01Pollux)

## Acknowledgements

- [LinkedIn Jobs](https://www.linkedin.com/jobs/)
- [Discord](https://discord.com/)
- [.NET](https://dotnet.microsoft.com/)
- [Selenium](https://www.selenium.dev/)
- [Discord.Net](https://github.com/discord-net/Discord.Net)

```
