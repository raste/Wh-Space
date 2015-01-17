﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;

namespace WhSpace
{
    public class BLog
    {

        public static void Add(Entities objectContext, Corporation currCorporation, User currUser, string msg)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }
            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }

            if (string.IsNullOrEmpty(msg))
            {
                throw new ArgumentException("msg is null or empty");
            }

            Log newLog = new Log();

            newLog.RCorporation = currCorporation;
            newLog.Users = currUser;
            newLog.dateAdded = DateTime.UtcNow;
            newLog.description = msg;

            objectContext.AddToLogSet(newLog);
            Tools.Save(objectContext);
        }

        public int CountUpdatesSincePageLoad(Entities objectContext, Corporation currCorporation, DateTime timeLoaded)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }

            int result = objectContext.LogSet.Count(log => log.dateAdded > timeLoaded && log.RCorporation.ID == currCorporation.ID);

            return result;
        }

        public List<Log> GetLogUpdatesSincePaveLoad(Entities objectContext, Corporation currCorporation, DateTime timeLoaded)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }

            List<Log> logs = objectContext.LogSet.Where(lg => lg.dateAdded > timeLoaded && lg.RCorporation.ID == currCorporation.ID).ToList();

            if (logs != null && logs.Count > 1)
            {
                logs.Reverse();
            }

            return logs;
        }

        public List<Log> GetLastLogs(Entities objectContext, Corporation currCorporation, int count)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (count < 1)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }

            List<Log> logs = objectContext.LogSet.Where(log => log.RCorporation.ID == currCorporation.ID).ToList();

            if (logs != null && logs.Count > 0)
            {
                logs.Reverse();

                int logsCount = logs.Count;

                if (logsCount > count)
                {
                    logs.RemoveRange(count , logsCount - count);
                }
            }

            return logs;

        }

        public List<Log> GetLastLogsFor(Entities objectContext, Corporation currCorporation, User user, int count)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }

            List<Log> logs = objectContext.LogSet.Where(log => log.Users.ID == user.ID && log.RCorporation.ID == currCorporation.ID).ToList();

            if (logs != null && logs.Count > 0)
            {
                logs.Reverse();

                int logsCount = logs.Count;

                if (logsCount > count)
                {
                    logs.RemoveRange(count, logsCount - count);
                }
            }

            return logs;

        }

        public long CountLogs(Entities objectContext, Corporation currCorporation)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }

            return objectContext.LogSet.Where(log => log.RCorporation.ID == currCorporation.ID).Count();
        }




    }
}