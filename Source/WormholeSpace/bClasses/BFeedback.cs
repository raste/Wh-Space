﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;

namespace WhSpace
{
    public class BFeedback
    {


        public void Add(Entities objectContext, User currUser, string name, string description)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }

            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentException("description is null or empty");
            }

            if (currUser.globalAdmin == true)
            {
                throw new InvalidOperationException("this method is only for non global admins");
            }

            if (HaveUserAddedFeedback(currUser) == true)
            {
                throw new InvalidOperationException("this user already have feedback");
            }

            FeedBack newFeed = new FeedBack();

            newFeed.dateAdded = DateTime.UtcNow;
            newFeed.description = description;
            newFeed.name = name;
            newFeed.RUser = currUser;
            newFeed.lastModified = newFeed.dateAdded;

            objectContext.AddToFeedBackSet(newFeed);
            Tools.Save(objectContext);

        }

        public bool HaveUserAddedFeedback(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (user.globalAdmin == true)
            {
                throw new InvalidOperationException("this method is only for non global admins");
            }

            bool result = false;

            user.FeedBack.Load();

            if (user.FeedBack.Count > 0)
            {
                result = true;
            }

            return result;
        }

        public FeedBack GetUserFeedback(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            FeedBack feed = null;

            user.FeedBack.Load();

            if (user.FeedBack.Count > 0)
            {
                feed = user.FeedBack.First();
            }

            return feed;
        }

        public void DeleteUserFeedback(Entities objectContext, User user)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.FeedBack.Load();

            List<FeedBack> feedback = user.FeedBack.ToList();

            if (feedback != null && feedback.Count > 0)
            {
                foreach (FeedBack feed in feedback)
                {
                    objectContext.DeleteObject(feed);
                    Tools.Save(objectContext);
                }
            }
        }

        public List<FeedBack> Get(Entities objectContext, int number, int from, int to)
        {
            Tools.AssertObjectContextExists(objectContext);

            List<FeedBack> result = new List<FeedBack>();
            List<FeedBack> all = objectContext.FeedBackSet.ToList();
            all.Reverse();

            for (int i = 0; i < all.Count; i++)
            {
                if (i >= from && i < to)
                {
                    result.Add(all[i]);
                }
            }

            return result;
        }

        public FeedBack Get(Entities objectContext, long id, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(objectContext);

            FeedBack feed = objectContext.FeedBackSet.FirstOrDefault(f => f.ID == id);

            if (feed == null && throwExcIfNull == true)
            {
                throw new ArgumentException(string.Format("no feedback with id = {0}", id));
            }

            return feed;
        }

        public void EditFeedBack(Entities objectContext, User currUser, FeedBack text, string name, string description)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }
            if (currUser.globalAdmin == true)
            {
                throw new InvalidOperationException("global admins cant edit feedback");
            }
            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentException("description is null or empty");
            }

            text.RUserReference.Load();
            if (text.RUser.ID != currUser.ID)
            {
                throw new InvalidOperationException(string.Format("User ID: {0} cannot edit User ID: {1} feedback", currUser.ID, text.RUser.ID));
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
            Tools.Save(objectContext);
        }

        public void DeleteFeedBack(Entities objectContext, User currUser, FeedBack text)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }

            if (currUser.globalAdmin == false)
            {
                text.RUserReference.Load();
                if (text.RUser.ID != currUser.ID)
                {
                    throw new InvalidOperationException(string.Format("User ID: {0} cannot delete User ID: {1} feedback", currUser.ID, text.RUser.ID));
                }
            }

            objectContext.DeleteObject(text);
            Tools.Save(objectContext);
        }

        public long CountFeedback(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            return objectContext.FeedBackSet.Count();
        }


    }
}