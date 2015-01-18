In order to browse and build the application you will need Visual Studio 2010 or greater.

To run the site you will have to prepare couple of things:  
1. Database (db)  
   Microsoft SQL Server 2005 or higher to host the db is needed.  
   You will need to create db, which will be used by the site. This can be done by running the script ([DB/WhSpace.sql](https://github.com/raste/Wh-Space/blob/master/DB/WhSpace.sql)) or the backup file ([DB/WhSpace.bak](https://github.com/raste/Wh-Space/blob/master/DB/WhSpace.bak)).  
2. Configuration  
   The connection to the database must be configured in [Web.config file](https://github.com/raste/Wh-Space/blob/master/Source/WormholeSpace/Web.config).  

```
<add name="Entities" connectionString="metadata=res://*/WormwholeModel.csdl|res://*/WormwholeModel.ssdl|res://*/WormwholeModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;Data Source=NAME;Initial Catalog=wormhole-space;Integrated Security=True;MultipleActiveResultSets=True&quot;" providerName="System.Data.EntityClient" />
``` 
is the line, which is used by the application to connect with the db. If it is not set up correctly, the site will not be able to connect to the db, and will not start.

The sections, which need to be modified:
* `Data Source=NAME;` - replace `NAME` with the name and address (if it is located on other machine) of the SQL server 
* `Initial Catalog=wormhole-space;` - replace `wormhole-space` with the name of the created db
*  If the database is password protected remove `Integrated Security=True;` and add `;uid=username;pwd=password` after `MultipleActiveResultSets=True` and substitute `username` and `password` with your credentials.

TODO: username and pass



TODO:)

add set startup page -> Index.aspx

   The admin username can only be changed from the database. The password can be changed from site (by going to administration page, selecting corporation and going to profile page).
