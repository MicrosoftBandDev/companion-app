# Band Center (companion-app)
A replacement of the Microsoft Health app for Microsoft Band 1/2

## Building the project
Band Center uses .NET MAUI, which is still in the preview stages. Please follow the [installation guide in the .NET MAUI documentation](https://docs.microsoft.com/en-us/dotnet/maui/get-started/installation),
and remember that you need Visual Studio 2022 *Preview*.

Once you have the development environment set up, you're ready to clone the repo.
```bash
git clone https://github.com/MicrosoftBandDev/companion-app.git
cd ./companion-app
```

In the future, you may also need to run the following:
```bash
git submodule init
git submodule update
```

Once the repo has been downloaded, you can get started by opening [the BandCenter solution](https://github.com/MicrosoftBandDev/companion-app/blob/main/BandCenter/BandCenter.sln)
in VS 2022 Preview.
