# GameLobbyBootstrap

Hello! This repo exists as a very basic ASP.NET project which demos a frontend, a backend, interaction between the two, Swagger, and details important pieces of an ASP.NET project for beginners. The example "intent" for this project is to host a multiplayer Game Lobby.

### Note: Since the `GameState` in the repository's code is in-memory, it will NOT work for a multi-instance environment, where users may be bounced between instances. The first thing to do is to refactor this to use a distributed database, which is meant to be an exercise for the reader. "TODO"s are listed in appropriate places to direct such refactoring.

Below is a list of files to guide the understanding of a project on this tech stack. It is not much different from a Microsoft Template for ReactJs with ASP.NET APIs, but I have included some code that can handle game lobby state to put things into context. The main files of note:

* `Program.cs` -- defines middleware, swagger generation, backend routes, Dependency Injections, etc.
* `GameController.cs` -- a controller to get you started with example API
* (`WeatherForecastController.cs`) -- included in the MSFT template, along with Front-End code that interacts with this; just another example of an API
* `GameStateProvider.cs` -- the "DAL" for the game state and supporting interactions. Note the caches with expiration rules
* `ClientApp/src/setupProxy.js` -- defines which route prefixes to pass down to the backend
* Not a file persay, but the UI has a link to the `/swagger` endpoint which shows the generated swagger UI. This should be helpful in sharing or testing API schemas.

Notes:
* There are some other notes and todos in the code comments which should help further guide understanding of the codebase
* Ideally the ClientApp here would be TypeScript, which is an enhancement I would like to make to this repo
* "Nuget" packages are the .NET equivalent of npm packages; there are many and they are fantastic!
* This _should_ run cross-platform... In Visual Studio on Windows, the 'play'/'start' button starts up the react app as well as the asp.net application
* To deploy this easily with VisualStudio 2022 (Community edition is free), right-click on the project from the "Solution Explorer" window and "Publish to Azure...". It will require:
  * An Azure account
  * An App Service Plan -- a shared hosting container for multiple apps
  * An App Service, which will deploy onto the App Service Plan; this resource will incur costs. For development, a "free" tier might be sufficient, but runs on shared resources in the cloud. The basic ("B1"?) SKU is about $10 a month

Note: there's nothing wrong with hosting this in AWS, but it won't be as simple as right-click->deploy
