### Preparations

In order to browse and build the application you will need Visual Studio 2010 or greater.

To run the site you will have to prepare couple of things:  

1. Database (db)  
  Microsoft SQL Server 2005 or higher to host the db is needed.  
  You will need to create db, which will be used by the site. This can be done by running the script ([DB/WhSpace.sql](https://github.com/raste/Wh-Space/blob/master/DB/WhSpace.sql)) or restoring via the backup file ([DB/WhSpace.bak](https://github.com/raste/Wh-Space/blob/master/DB/WhSpace.bak)).  
2. Connection string configuration  
  The connection to the database must be configured in [Web.config file](https://github.com/raste/Wh-Space/blob/master/Source/WormholeSpace/Web.config).  

  ```
<add name="Entities" connectionString="metadata=res://*/WormwholeModel.csdl|res://*/WormwholeModel.ssdl|res://*/WormwholeModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;Data Source=NAME;Initial Catalog=wormhole-space;Integrated Security=True;MultipleActiveResultSets=True&quot;" providerName="System.Data.EntityClient" />
  ``` 
  is the line, which is used by the application to connect with the db. If it is not set up correctly, the site will not be able to connect to the db, and will not start.

  The sections, which need to be modified:
    * `Data Source=NAME;` - replace `NAME` with the name and address (if it is located on other machine) of the SQL server 
    * `Initial Catalog=wormhole-space;` - replace `wormhole-space` with the name of the created db
    *  If the database is password protected remove `Integrated Security=True;` and add `;uid=username;pwd=password` after `MultipleActiveResultSets=True` and substitute `username` and `password` with your credentials.
      The connection string will look like this whit credentials  
  ```
<add name="Entities" connectionString="metadata=res://*/WormwholeModel.csdl|res://*/WormwholeModel.ssdl|res://*/WormwholeModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;Data Source=NAME;Initial Catalog=wormhole-space;MultipleActiveResultSets=True;uid=username;pwd=password&quot;" providerName="System.Data.EntityClient" />
 ```  
   
   **IMPORTANT**: The connection string line must be on ONE row.
3. Logs configuration (again in web.config file)
  ```
  <param name="File" value="D:\Projects\EveWormholes\trunk\WormholeSpace\logs\log.txt" />
  ```
  This project uses open source library "log4net" (http://logging.apache.org/) for logging of exceptions (errors) to log files. Basically if site crashes at some operation, the error will be written to a log file.
  
  You may not be interested in this functionality, so there are 2 options:
    * To not use logging: 
      replace `<level value="DEBUG" />` with `<level value="OFF" />` in `configuration > log4net > root` node or replace `D:\Logs\log.txt` in `<param name="File" value="D:\Logs\log.txt" />` with invalid path
    * To use logging:
      Create directory in which the logs files will be.  
      
      *NOTES when application is uploaded to server:*  
         * The logs directory must be sub-directory of the application dir.  
         * You need to explictly give rights to the Worker process (Example name : Plesk IIS WP User (ASPNET_WP), name is different based on provider), in order application to read/write log files. The necessary permissions are : READ, WRITE, MODIFY, READ AND EXECUTE. 
      
      Replace `D:\Logs\log.txt` in `<param name="File" value="D:\Logs\log.txt" />` with the physical path to the directory, in which the log files will be. If everything done correct, log file will be created on first start up.
      
      *NOTE:* Be sure that the log files cannot be downloaded by clients by typing the address of a log file in a browser.
4. Set [Index.aspx](https://github.com/raste/Wh-Space/blob/master/Source/WormholeSpace/Index.aspx) as start page in Visual Studio. 

### Run

Now you are ready to run the application.  
Try to browse the site. If it starts ..congratilations! Click on the demo link to see if there is connection from site to database. If the page opens, then everything is OK. You are ready to use it.  

Initial users:  
  * The main administrator with username `admin` and password `admin`. To change the password go to administration page, select corporation and go to profile page
  * Guest user with username `guest` and password `guest`

**If there are problems**:  
  * If you specified valid directory for "log4net", and set everything correct, there should be a log file in the specified directory. Open it and see if there are ERROR entries.  
  * If you haven't configured "log4net" or it isn't writing logs, you can change the following line in Web.config: `<customErrors mode="On" />` with `<customErrors mode="Off" />`. This will enable the application to show the errors in the browser (but this way everyone will be able to see them if they encounter one). After this start the project and see what error it will show.  
  * Read this file again and see if you didn't miss anything. Most likely the connection string is not configured properly or there is something missing in the database.
