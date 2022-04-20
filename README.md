## FlightDemo
To create Flight demo projects for training

# 1. Workspace File Structure Overview :-

./UsefulDocs/DesignDiagram/FlightAppDesign.html -> has design diagram of the demo
./UsefulDocs/Postman/ -> has postman api testing collection exported.

./Library folder -> has the shared DTOs, Service contracts and Utils classes which will be used by client and webservice.
./WebService folder -> contains microservice projects.
./WebService/Flight.Airlines/ folder -> has microservice project related to Aprlines - Add/Update/Delete
./WebService/Flight.Users/ folder -> has microservice project related to Users - Register/Login/Update/Delete
./Tools/APIGateway/ folder -> has microservice project related to API Gateway - routing json file is present in OcelotConfigs/ocelot.json (or) ocelot.{env}.json
----------------------------------------------------------------------------------------------------------------------------------

# 2. SetUp :-
=> <project>/appsettings.json :
			(a) Urls = specifies the hosting url.
			(b) ConnectionStrings/SqlServerConnectionString = specifies DB connection string.
			(c) HeaderValidation (and) AdminValidation = specifies if we want to by pass validation.
			(d) endpointUrls/ValidateAdminUrl = specifies url for admin validation api.
			(e) APIGatewayUrl = api gateway url.
			
=> create databases Airlines and Users. 

=> .\WebService\Flight.Airlines\Migrations = contains migration scripts and db snapshots
=> .\WebService\Flignt.Users\Migrations = contains migration scripts and db snapshots
----------------------------------------------------------------------------------------------------------------------------------

# 3. To run the microservices :-

Prerequisite : dotnet core 3.1 runtime /sdk

Execution :

./WebService/Flight.Airlines/bin/Release/netcoreapp3.1/publish/  -> we can just go to this folder location in powershell/commandPrompt and just execute 
                > dotnet Flight.Airlines.dll
./WebService/Flignt.Users/bin/Release/netcoreapp3.1/publish/  -> we can just go to this folder location in powershell/commandPrompt and just execute 
                > dotnet Flight.Users.dll
Tools/APIGateway/bin/Release/netcoreapp3.1/publish/  -> we can just go to this folder location in powershell/commandPrompt and just execute 
                > dotnet APIGateway.dll

(or)

Simply open the projects in VS2019 and run them there.
-----------------------------------------------------------------------------------------------------------------------------------

# 4. To test the microservices :-

open postman and import the json files from './UsefulDocs/Postman/' folder , U can see all the api test requests present.
Header UserId is important for Airlines module (should be admin userId)
  
