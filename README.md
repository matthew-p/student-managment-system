# student-managment-system

A test CRUD project with a WebAPI backend and Winforms client. 

This needs to have MSSql express running on the same machine as the WebApi StudentManagement App (the app looks to local host for the Sql server), which needs Dotnet Core 2.0.

https://docs.microsoft.com/en-us/dotnet/core/linux-prerequisites?tabs=netcore2x

https://docs.microsoft.com/en-us/sql/linux/sql-server-linux-setup

Run the initial .sql files in StudentManagement / SqlScripts to create-school.sql then create-students.sql to create the database and table respectively. 

This requires /opt/mssql/data/ to exist with permissions to access it. 

Next, execute all of the stored procedure .sql scripts in the SqlScripts / Procedures subdirectory, as the repository expects them. 
```bash
sqlcmd -S localhost -U SA -P 'yourPassword' -i SqlScripts/Procedures/DeleteStudent.sql.
```

The connection strings are set in the appsettings.json files for the WebApi StudentManagment project, and the Test project. They can be different, and are intended to be filled with whatever secret is necessay by the full build process. The client expects the API to be available at 192.168.56.10.

The web api is built with Dotnet Core 2.0 on Ubuntu, listens on port 5000. 

The Client is a Winforms desktop app built in Visual Studio 2017. 

install mssql tools (bash)
```bash
curl https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -

curl https://packages.microsoft.com/config/ubuntu/16.04/prod.list | sudo tee /etc/apt/sources.list.d/msprod.list

sudo apt-get update 
sudo apt-get install mssql-tools unixodbc-dev

echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bashrc
source ~/.bashrc
```
