﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using log4net;
using log4net.Config;

namespace WhSpace
{
    public class Global : System.Web.HttpApplication
    {
        private static ILog log = LogManager.GetLogger(typeof(Global));

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            XmlConfigurator.Configure();
            if (log.IsInfoEnabled)
            {
                log.Info("Application started!");
            }
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown
            XmlConfigurator.Configure();
            if (log.IsInfoEnabled)
            {
                log.Info("Application end!");
            }

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

            Exception ex = Server.GetLastError();
            if (log.IsErrorEnabled)
            {
                log.Error(ex);
            }

            if (Request.UrlReferrer.ToString().Contains("Index.aspx") == false)
            {
                Response.Redirect("Index.aspx");
            }
        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Session started. Client IP address: \"{0}\".", Request.UserHostAddress);
            }
        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }

    }
}
