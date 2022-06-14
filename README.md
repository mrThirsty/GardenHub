# GardenHub
Small Garden Management app (monitor and water) for my plants. Initially designed for indoor pot plants but will expand to outdoors and beds.

## Intended Platform
This set of applications is intended to be run on raspberry pis. The server is built using Microsoft .Net Framework so could be run anywhere you are able to install the .Net runtime. The clients will be built to talk to the raspberry Pi GPIO setup so it is tightly coupled and may not work on other similar products.

## Application Architecture
GradenHub will have a centralised server and remote clients which will gather the information and report back to the server. 

The server will provide data storage and a management UI to configure actions etc.

## Contributing
You will need the ability to edit and compile C# and .net. an editor such as Visual Studio, Visual Studio Code, or Rider is recommended. 

If you intend to contribute to the application, please fork this repo and work on your own until you are confident your work is complete and working. If you would like to merge your work back into this then please raise a pull request. If the pull request doesn't have a clear description of what functionality was added then it will be rejected.

The technical details are below:
- Database: SQLite
- .net Framework: V6.0
- Task Management: Hangfire.net
- Client/Server communication: MQTT

### Nuget Packages used
 - Server
    - https://github.com/dotnet/MQTTnet
- Client
    - https://www.nuget.org/packages/Iot.Device.Bindings/
    - https://github.com/dotnet/MQTTnet