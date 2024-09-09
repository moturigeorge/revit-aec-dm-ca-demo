# Revit AEC DM CA DEMO

![platforms](https://img.shields.io/badge/platform-windows-lightgray.svg)
[![Visual Studio Community 2022](https://img.shields.io/badge/Visual%20Studio-2022-green.svg)](https://visualstudio.microsoft.com/vs/community/)
![.NET](https://img.shields.io/badge/.NET%20Framework-4.8-blue.svg)
[![Revit 2024](https://img.shields.io/badge/Revit-2024-lightgrey.svg)](http://autodesk.com/revit)

[![.ACC](https://img.shields.io/badge/ACC-green.svg)](https://aps.autodesk.com/en/docs/acc/v1/overview/introduction/)
[![.Parameters Service](https://img.shields.io/badge/Parameters%20-v1-green.svg)](https://aps.autodesk.com/en/docs/parameters/v1/overview/introduction/)


![Advanced](https://img.shields.io/badge/Level-Advanced-red.svg)
[![MIT](https://img.shields.io/badge/License-MIT-blue.svg)](http://opensource.org/licenses/MIT)


# Description

This sample demonstrates the following features:
- List parameters from mongo db based on Revit model family,
- Update the asset value of the selected family from a Revit panel. 


# Thumbnail

![thumbnail](Resources/screenshot.png)


# Demonstration
- Start Revit, Open a Revit project, select the family(door or window). A panel will be added next to the properties panel with the assets of the family. 


# Setup

## Prerequisites

- [APS credentials](https://forge.autodesk.com/en/docs/oauth/v2/tutorials/create-app)
- [Visual Studio Community 2022](https://visualstudio.microsoft.com/vs/community/) or [.NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- Terminal (for example, [Windows Command Prompt](https://en.wikipedia.org/wiki/Cmd.exe) 
or [macOS Terminal](https://support.apple.com/guide/terminal/welcome/mac)) if you don't have Visual Studio.

## Running locally
If you have Visual Studio Community 2022 installed
- Clone this repository
- Build and run the project

If you do not have it installed
- Clone this repository
- Install dependencies: `dotnet restore`
- Setup environment variables in the appsettings.json File:
  - `ApiKey` - your api key to access mongo db
  - `DataSource` - your mongo db data source
  - `Database` - your mongoDB database
  - `Collection` - your mongoDB collection
- Run the project: `dotnet run`


Follow the `Demonstration` section above to play with addin.

## Documentation

For more information, see the documentation:

- [ACC Assets API](https://aps.autodesk.com/en/docs/acc/v1/overview/field-guide/assets/)
- [Revit API: Shared Parameters](https://aps.autodesk.com/en/docs/parameters/v1/overview/introduction/)

# License

This sample is licensed under the terms of the [MIT License](http://opensource.org/licenses/MIT). Please see the [LICENSE](LICENSE) file for full details.

# Written by
[Moturi Magati George](https://www.linkedin.com/in/moturigeorge/), [Autodesk Partner Development](http://aps.autodesk.com)
