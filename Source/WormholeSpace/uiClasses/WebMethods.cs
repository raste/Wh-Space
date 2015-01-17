﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Web;
using System.Text;

namespace WhSpace
{
    public class WebMethods
    {

        public static string WmUpdatesSincePageLoad(string dateLoaded)
        {
            DateTime pageLoaded;

            if (DateTime.TryParse(dateLoaded, out pageLoaded) == false)
            {
                return string.Empty;
            }

            pageLoaded = pageLoaded.ToUniversalTime();

            Entities objContext = new Entities();
            Tools.ChangeUiCultureFromSessionAndCookie(objContext);

            BLog blog = new BLog();
            BCorporation bCorporation = new BCorporation();
            StringBuilder result = new StringBuilder();

            object objCorporation = HttpContext.Current.Session[UiSessionParams.SessionCurrCorporationID];
            if (objCorporation == null)
            {
                return string.Empty;
            }

            long id = 0;
            if (long.TryParse(objCorporation.ToString(), out id) == false)
            {
                return string.Empty;
            }

            Corporation currCorporation = bCorporation.Get(objContext, id, false);
            if (currCorporation == null)
            {
                return string.Empty;
            }

            int count = blog.CountUpdatesSincePageLoad(objContext, currCorporation, pageLoaded);

            int lastLogs = 10;
            if (count < 10)
            {
                lastLogs = count;
            }

            if (count > 0)
            {
                List<Log> updates = blog.GetLastLogs(objContext, currCorporation, lastLogs);

                string strNewLine = "<br/>";

                result.Append("<div class=\"updatesPnl\">");

                result.Append("<div class=\"errors\" style=\"text-align:center;margin-bottom:5px;\">");
                result.Append(string.Format("{0} {1} {2}", HttpContext.GetGlobalResourceObject("WebMethods", "dynUpdatesMeantime")
                    , updates.Count, HttpContext.GetGlobalResourceObject("WebMethods", "dynUpdatesMeantime2"))); //Направени са {0} промени междувременно!

                int i = updates.Count;

                if (count > 10)
                {
                    result.Append(strNewLine);
                    result.Append(string.Format("{0} {1} {2}", HttpContext.GetGlobalResourceObject("WebMethods", "dynUpdatesLastShown"), 10
                        , HttpContext.GetGlobalResourceObject("WebMethods", "dynUpdatesLastShown2")));  // Показани са последните 10.
                }

                result.Append("</div>");

                

                foreach (Log log in updates)
                {
                    if (!log.UsersReference.IsLoaded)
                    {
                        log.UsersReference.Load();
                    }

                    result.Append("<span class=\"users\">");
                    result.Append(log.Users.name);
                    result.Append("</span>");
                    result.Append(string.Format(" : {0}", log.description));
                    result.Append(strNewLine);


                }

                result.Append("</div>");

            }

            return result.ToString();
        }


    }
}