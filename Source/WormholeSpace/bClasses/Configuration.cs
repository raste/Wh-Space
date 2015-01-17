﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Linq;

namespace WhSpace
{
    public class Configuration
    {
        public static readonly int MaxPasswordLength = 100;
        public static readonly int MaxUsernameLength = 50;
        public static readonly int MaxSystemNameLength = 20;
        public static readonly int MaxCorporationNameLength = 100;
       
        public static readonly int MaxWormholeNameLength = 0;
        public static readonly int MaxSignatureNameLength = 0;

        public static readonly int MaxFeedBackNameLength = 200;

        public static readonly string TextFeedBackAdmin = "FeedBackPageAdminAboutText";
        public static readonly string TextAboutAdmin = "AboutPageAdminAboutText";

        private static int MinutesBetweenApiCalls = 20;

        public static bool ShouldApiUpdate(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            pConfiguration config = objectContext.pConfigurationSet.First();

            bool result = false;

            if (config.DateLastApiCall.HasValue == true)
            {
                TimeSpan span = DateTime.UtcNow - config.DateLastApiCall.Value;

                if (span.TotalMinutes >= MinutesBetweenApiCalls)
                {
                    result = true;
                }
            }
            else
            {
                result = true;
            }

            return result;
        }

        public static void UpdateDateLastApiCall(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            pConfiguration config = objectContext.pConfigurationSet.First();

            config.DateLastApiCall = DateTime.UtcNow;

            Tools.Save(objectContext);
        }

    }
}