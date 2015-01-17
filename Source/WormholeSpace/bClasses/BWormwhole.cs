﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;

namespace WhSpace
{
    public class BWormwhole
    {

        public void AddWormholesBetweenTwoSystems(Entities objectContext, Systems parentSystem, Systems childSystem, string fromParWhId, string toParWhId)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (parentSystem == null)
            {
                throw new ArgumentNullException("parentSystem");
            }

            if (childSystem == null)
            {
                throw new ArgumentNullException("childSystem");
            }

            Wormwhole newWormwhole = new Wormwhole();

            newWormwhole.FromSystems = parentSystem;
            newWormwhole.ToSystems = childSystem;
            newWormwhole.fromSysWormwholeID = fromParWhId.ToUpperInvariant();

            if (!string.IsNullOrEmpty(toParWhId))
            {
                newWormwhole.toSysWormwholeID = toParWhId.ToUpperInvariant();
            }

            objectContext.AddToWormwholeSet(newWormwhole);
            Tools.Save(objectContext);
        }

        public void UpdateWormwhole(Entities objectContext, Corporation currCorporation, User currUser, Wormwhole currWormwhole, Systems forSystem, string newWhId)
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

            if (currWormwhole == null)
            {
                throw new ArgumentNullException("currWormwhole");
            }

            if (forSystem == null)
            {
                throw new ArgumentNullException("forSystem");
            }

            currWormwhole.FromSystemsReference.Load();
            currWormwhole.ToSystemsReference.Load();

            if (currWormwhole.FromSystems.ID != forSystem.ID && currWormwhole.ToSystems.ID != forSystem.ID)
            {
                throw new InvalidOperationException("selected wormhole is not for the selected system");
            }

            string log = string.Empty;

            if (currWormwhole.FromSystems.ID == forSystem.ID)
            {
                currWormwhole.fromSysWormwholeID = newWhId.ToUpperInvariant();

                log = string.Format("{0} {1} {2} {3}.", Tools.GetLogResource("wormEdit", currCorporation), currWormwhole.FromSystems.name
                    , Tools.GetLogResource("wormEdit2", currCorporation), currWormwhole.ToSystems.name);

                
                BSystem bSystem = new BSystem();
                int sysClass = bSystem.GetSystemClassBasedOnSecStatusOrWormholes(objectContext, this, currWormwhole.ToSystems.name, currWormwhole.fromSysWormwholeID);
                if (sysClass > 0 && sysClass != currWormwhole.ToSystems.sysClass)
                {
                    bSystem.ChangeSystemClass(objectContext, currCorporation, currWormwhole.ToSystems, currUser, sysClass);
                }
                
            }
            else
            {
                currWormwhole.toSysWormwholeID = newWhId.ToUpperInvariant();

                log = string.Format("{0} {1} {2} {3}.", Tools.GetLogResource("wormEdit", currCorporation), currWormwhole.ToSystems.name
                    , Tools.GetLogResource("wormEdit2", currCorporation), currWormwhole.FromSystems.name);

               
                BSystem bSystem = new BSystem();
                int sysClass = bSystem.GetSystemClassBasedOnSecStatusOrWormholes(objectContext, this, currWormwhole.FromSystems.name, currWormwhole.toSysWormwholeID);
                if (sysClass > 0 && sysClass != currWormwhole.FromSystems.sysClass)
                {
                    bSystem.ChangeSystemClass(objectContext, currCorporation, currWormwhole.FromSystems, currUser, sysClass);
                }
                
            }

            Tools.Save(objectContext);

            BLog.Add(objectContext, currCorporation, currUser, log);
        }

        public void DeleteWormwhole(Entities objectContext, Corporation currCorporation, User currUser, Wormwhole currWormwhole, bool makeLog)
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

            if (currWormwhole == null)
            {
                throw new ArgumentNullException("currWormwhole");
            }

            currWormwhole.ToSystemsReference.Load();
            currWormwhole.FromSystemsReference.Load();

            string log = string.Format("{0} {1} {2} {3} {4}", Tools.GetLogResource("wormDelete", currCorporation), currWormwhole.FromSystems.name
                , Tools.GetLogResource("wormDelete2", currCorporation), currWormwhole.ToSystems.name
                , Tools.GetLogResource("wormDelete3", currCorporation));

            objectContext.DeleteObject(currWormwhole);
            Tools.Save(objectContext);

            if (makeLog == true)
            {
                BLog.Add(objectContext, currCorporation, currUser, log);
            }
        }

        public Wormwhole GetWormwhole(Entities objectContext, long whId, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(objectContext);

            Wormwhole worm = objectContext.WormwholeSet.FirstOrDefault(wh => wh.ID == whId);

            if (throwExcIfNull == true && worm == null)
            {
                throw new InvalidOperationException(string.Format("there is no wormwhole with id = {0}", whId));
            }

            return worm;
        }

        public WormwholeIdentification GetWormwholeIdentification(Entities objectContext, string strWhId, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (string.IsNullOrEmpty(strWhId))
            {
                throw new ArgumentException("strWhId is null or empty");
            }

            WormwholeIdentification whId = objectContext.WormwholeIdentification.FirstOrDefault(wh => wh.ID == strWhId);

            if (throwExcIfNull == true && whId == null)
            {
                throw new ArgumentException(string.Format("No WormwholeIdentification with id = {0}", strWhId));
            }

            return whId;
        }

        public void GetWhToSystem(Systems currSystem, ref List<Wormwhole> toCurrSystem)
        {
            if (currSystem == null)
            {
                throw new ArgumentNullException("currSystem");
            }

            currSystem.WhToSystem.Load();

            toCurrSystem = currSystem.WhToSystem.ToList();
        }





    }
}