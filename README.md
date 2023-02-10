# zoomers
A set of zoom-friendly, family-friendly, games


# Requirements
Currently requires the [ASP.Net Core 5.0 runtime](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-5.0.17-windows-x64-installer?cid=getdotnetcore)

I also recommend the Blazor Wasm Debugging Extension


# Running the game
From the Server directory, run the application:

```
dotnet run
```

## Database
Out-of-the-box the game uses a database (using Entity Framework Core 5.0 on SQL). Add the Connection in a User Secret like this:
```
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(LocalDb)\MSSQLLocalDB;Database=YOUR_DATABASE;Integrated Security=SSPI;"
```

Run the EF updates to build out the database objects:
```
dotnet ef database update
```

## Host URL
You will want to configure the Game's Host URL by updating the appsettings(.Development).json or setting a user secret for `Game:HostUrl`.  You can use a local address but some configuration will be required.  It may be as simple as using your computer name depending on your personal settings.

Alternatively, you can use a DDNS provider like https://my.noip.com/ and configure a firewall to rule to route the traffic to your host machine.  

Of course, hosting this in a cloud service may be easier but incur some charges.

## Add Questions
You can add and update questions by visiting https://localhost:50001/admin/questions

*You will want to add at least a dozen or so questions to have a fun time with this game.  The more the merrier!*

## Playing the Game
From the Home page, enter the parameters or click the 'random' button to have it choose for your.  Click 'Create Game' when you are ready to start.


# Options
## Local file
Originally, the game used a local set of questions.  To view or modify questions in that file, check out ```Server\Data\Games\word-play.csv```.

You will also need to modify a few other places for this to work correctly.

## Changing Phrases
The game can be further customized by modifying responses in the `answers-finished-phrases.csv` and `player-joined-phrases.csv` files.