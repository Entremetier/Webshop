How to start the webshop project!

1. Set up the MSSQL Server Database. Script is in the wwwroot folder under src, sqlScripte. 
   The script contains all DDL and DML commands to get the structure of the DB and the data.
2. Set up a new ConnectionString in the appsettings.json and lapWebshopContext.cs. You will need your device        name as data source, the DB-Name as catalog and your user credentials (ID and Password).
   To hide your ConnectionString you can encrypt it, don't save it on GitHub or other VCS in plain text.
3. If you're going to update the DB you can find the command to scaffold the DB-Context in the wwwroot folder       under Database Dokumentation.txt.
4. If you're going to update something in the code and need to change the DB you can find the update-database       command in the wwwroot folder under Database Dokumentation.txt.
