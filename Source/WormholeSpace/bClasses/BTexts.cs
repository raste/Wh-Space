﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Linq;

namespace WhSpace
{
    public class BTexts
    {
        public Text Get(Entities objectContext, string callName)
        {
            Tools.AssertObjectContextExists(objectContext);

            Text text = objectContext.TextSet.FirstOrDefault(txt => txt.callName == callName);

            if (text == null)
            {
                throw new ArgumentException(string.Format("No text with call name = {0}", callName));
            }

            return text;
        }

        public void ChangeText(Entities objectContext, User currUser, Text text, string name, string description)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }
            if (currUser.globalAdmin == false)
            {
                throw new InvalidOperationException("currUser is not global admin");
            }
            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentException("description is null or empty");
            }

            if (name != text.name)
            {
                text.name = name;
            }
            if (text.description != description)
            {
                text.description = description;
            }

            text.lastModified = DateTime.UtcNow;
            text.RLastModifiedBy = currUser;

            Tools.Save(objectContext);

        }


    }
}