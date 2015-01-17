﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EveAI.Live;
using EveAI.Map;

namespace WhSpace
{

    public class BSystem
    {

        public Systems Get(Entities objectContext, Corporation currCorporation, long id, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }

            Systems system = objectContext.SystemsSet.FirstOrDefault(sys => sys.ID == id && sys.RCorporation.ID == currCorporation.ID);

            if (throwExcIfNull == true && system == null)
            {
                throw new InvalidOperationException(string.Format("no system with id = {0}", id));
            }

            return system;
        }


        public Systems GetRoot(Entities objectContext, Corporation currCorporation)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }

            Systems system = objectContext.SystemsSet.FirstOrDefault(sys => sys.root == true && sys.RCorporation.ID == currCorporation.ID);

            if (system == null)
            {
                throw new InvalidOperationException("theres not root system in DB");
            }

            return system;
        }

        public List<Systems> GetChildSystems(Systems root)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            List<Systems> childSystems = new List<Systems>();

            root.WhFromSystem.Load();

            List<Wormwhole> wormwholes = root.WhFromSystem.ToList();
            if (wormwholes != null && wormwholes.Count > 0)
            {
                foreach (Wormwhole whole in wormwholes)
                {
                    whole.ToSystemsReference.Load();
                    childSystems.Add(whole.ToSystems);
                }
            }

            return childSystems;
        }


        public bool CheckIfSystemIsAlreadyAdded(Entities objectContext, Corporation currCorporation, string newSysName)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }

            if (string.IsNullOrEmpty(newSysName))
            {
                throw new ArgumentException("newSysName is null or empty");
            }

            bool alreadyAdded = false;

            Systems system = objectContext.SystemsSet.FirstOrDefault(sys => sys.name == newSysName && sys.RCorporation.ID == currCorporation.ID);

            if (system != null)
            {
                alreadyAdded = true;
            }

            return alreadyAdded;
        }


        public void Add(Entities objectContext, Corporation currCorporation, BWormwhole bWormwhole, Systems parentSystem, User currUser, string newSysName
            , int sysClass, int sysEffect , string fromParWhId, string toParWhId, string info, string occupied)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }
            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }

            if (BUser.CanUserOperateWithCorporationItems(currCorporation, currUser) == false)
            {
                throw new InvalidOperationException("User cannot operate with corporation items");
            }

            if (parentSystem == null)
            {
                throw new ArgumentNullException("parentSystem");
            }

            if (bWormwhole == null)
            {
                throw new ArgumentNullException("bWormwhole");
            }

            if (string.IsNullOrEmpty(newSysName))
            {
                throw new ArgumentException("newSysName is null or empty");
            }

            BStaticSystem bStaticSystem = new BStaticSystem();

            StaticCorporationSystem corpSys = bStaticSystem.GetStaticCorpSystem(objectContext, currCorporation, newSysName, false);
            StaticSystemData staticSys = bStaticSystem.GetStaticSystem(objectContext, newSysName, false);

            DateTime now = DateTime.UtcNow;

            if (corpSys == null)
            {              
                if (staticSys != null)
                {
                    bStaticSystem.AddStaticCorporationSystem(objectContext, currCorporation, newSysName, false, now, info, occupied);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(info))
                {
                    info = corpSys.info;
                }
                else if(info != corpSys.info)
                {
                    bStaticSystem.ChangeStaticCorpSysInfo(objectContext, corpSys, info);   
                }

                if (string.IsNullOrEmpty(occupied))
                {
                    occupied = corpSys.occupation;
                }
                else if(occupied != corpSys.occupation)
                {
                    bStaticSystem.ChangeStaticCorpSysOccupation(objectContext, corpSys, occupied);   
                }

                corpSys.dateCurrentMet = now;
            }

            if (sysClass == 0)
            {
                sysClass = GetSystemClassBasedOnSecStatusOrWormholes(objectContext, bWormwhole, newSysName, fromParWhId);
            }


            Systems newSystem = new Systems();

            newSystem.name = newSysName.ToUpperInvariant();
            newSystem.RCorporation = currCorporation;
            newSystem.Users = currUser;
            newSystem.dateAdded = now;

            newSystem.root = false;
            newSystem.occupied = occupied;
            newSystem.info = info;

            if (sysClass == 0)
            {
                newSystem.sysClass = null;
            }
            else
            {
                newSystem.sysClass = sysClass;
            }

            if (sysEffect == 0)
            {
                newSystem.sysEffect = null;
            }
            else
            {
                newSystem.sysEffect = sysEffect;
            }

            objectContext.AddToSystemsSet(newSystem);
            Tools.Save(objectContext);

            bWormwhole.AddWormholesBetweenTwoSystems(objectContext, parentSystem, newSystem, fromParWhId, toParWhId);


            string log = string.Format("{0} {1} {2} {3}.", Tools.GetLogResource("systemAdd", currCorporation), newSysName
                , Tools.GetLogResource("systemAdd2", currCorporation), parentSystem.name);
            BLog.Add(objectContext, currCorporation, currUser, log);
        }

        public int GetSystemClassBasedOnSecStatusOrWormholes(Entities objectContext, BWormwhole bWormwhole, string systemName, string fromParWhId)
        {
            if (string.IsNullOrEmpty(systemName))
            {
                throw new ArgumentException("systemName is null or empty");
            }

            systemName = systemName.ToUpperInvariant();

            EveApi eveApi = new EveApi();
            int sysClass = 0;

            SolarSystem currSystem = eveApi.EveApiCore.SolarSystems.FirstOrDefault(sl => sl.Name.ToUpperInvariant() == systemName);
            if (currSystem != null)
            {
                if (currSystem.Region.Name != "Unknown")
                {
                    sysClass = GetSystemClassBasedOnSecurityStatus(currSystem.Security);
                }
            }

            if (sysClass == 0)
            {
                sysClass = GetSystemClassBasedOnWormholeTarget(objectContext, bWormwhole, fromParWhId);
            }

            return sysClass;
        }

        private int GetSystemClassBasedOnSecurityStatus(float security)
        {
            int sysClass = 0;

            if (security >= 0.5)
            {
                sysClass = 10;
            }
            else if (security >= 0.0)
            {
                sysClass = 9;
            }
            else
            {
                sysClass = 8;
            }

            return sysClass;
        }

        private int GetSystemClassBasedOnWormholeTarget(Entities objectContext, BWormwhole bWormhole , string strWhId)
        {
            int sysClass = 0;

            if (!string.IsNullOrEmpty(strWhId))
            {
                WormwholeIdentification whId = bWormhole.GetWormwholeIdentification(objectContext, strWhId, false);
                if (whId != null)
                {
                    switch (whId.target)
                    {
                        case "Lowsec":
                            sysClass = 9;
                            break;
                        case "Highsec":
                            sysClass = 10;
                            break;
                        case "Nullsec":
                            sysClass = 8;
                            break;
                        case "C1 W-Space":
                            sysClass = 2;
                            break;
                        case "C2 W-Space":
                            sysClass = 3;
                            break;
                        case "C3 W-Space":
                            sysClass = 4;
                            break;
                        case "C4 W-Space":
                            sysClass = 5;
                            break;
                        case "C5 W-Space":
                            sysClass = 6;
                            break;
                        case "C6 W-Space":
                            sysClass = 7;
                            break;
                        default:
                            break;
                            //throw new ArgumentOutOfRangeException(string.Format("Wormhole Identification target : {0} is invalid.", whId.target));
                    }
                }
            }

            return sysClass;
        }



        public void DeleteSystem(Entities objectContext, Corporation currCorporation, Systems currSystem, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currSystem == null)
            {
                throw new ArgumentNullException("currSystem");
            }

            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }

            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }

            if (BUser.CanUserOperateWithCorporationItems(currCorporation, currUser) == false)
            {
                throw new InvalidOperationException("User cannot operate with corporation items");
            }

            if (currSystem.root == true)
            {
                throw new InvalidOperationException("cannot delete root system");
            }

            if (currUser.canEdit == false && currUser.admin == false)
            {
                return;
            }


            BStaticSystem bStaticSystems = new BStaticSystem();
            StaticCorporationSystem corpSys = bStaticSystems.GetStaticCorpSystem(objectContext, currCorporation, currSystem.name, false);
            if(corpSys != null)
            {
                corpSys.datePreviousMet = corpSys.dateCurrentMet;
            }

            /////////////////////
            List<Systems> childSystems = GetChildSystems(currSystem);

            if (childSystems != null && childSystems.Count > 0)
            {
                foreach (Systems system in childSystems)
                {
                    DeleteSystem(objectContext, currCorporation, system, currUser);
                }
            }

            /////////////////////
            BWormwhole bWormshole = new BWormwhole();

            List<Wormwhole> whsFromSystem = new List<Wormwhole>();
            List<Wormwhole> whsToSystem = new List<Wormwhole>();

            GetSystemConnections(currSystem, ref whsToSystem, ref whsFromSystem);

            if (whsFromSystem != null && whsFromSystem.Count > 0)
            {
                foreach (Wormwhole worm in whsFromSystem)
                {
                    bWormshole.DeleteWormwhole(objectContext, currCorporation, currUser, worm, false);
                }
            }

            if (whsToSystem != null && whsToSystem.Count > 0)
            {
                foreach (Wormwhole worm in whsToSystem)
                {
                    bWormshole.DeleteWormwhole(objectContext, currCorporation, currUser, worm, false);
                }
            }

            ///////////////////////

            /////////////////////
            BSignature bSignature = new BSignature();

            List<Signature> signatures = bSignature.GetSystemSignatures(currSystem);
            if (signatures != null && signatures.Count > 0)
            {
                foreach (Signature signature in signatures)
                {
                    bSignature.DeleteSignature(objectContext, currCorporation, currUser, signature, false);
                }
            }
            /////////////////////

            string log = string.Format("{0} {1}.", Tools.GetLogResource("systemDelete", currCorporation), currSystem.name);

            objectContext.DeleteObject(currSystem);
            Tools.Save(objectContext);

            BLog.Add(objectContext, currCorporation, currUser, log);
        }

        public void ChangeSystemName(Entities objectContext, Corporation currCorporation, Systems currSystem, User currUser, string newName)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currSystem == null)
            {
                throw new ArgumentNullException("currSystem");
            }

            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }
            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }

            if (BUser.CanUserOperateWithCorporationItems(currCorporation, currUser) == false)
            {
                throw new InvalidOperationException("User cannot operate with corporation items");
            }

            if (String.IsNullOrEmpty(newName))
            {
                throw new ArgumentException("newname is null or empty");
            }

            string oldName = currSystem.name;
            
            currSystem.name = newName.ToUpperInvariant();
            Tools.Save(objectContext);

            string log = string.Format("{0} {1} {2} {3}.", Tools.GetLogResource("systemNameChange", currCorporation), oldName
                , Tools.GetLogResource("systemNameChange2", currCorporation), newName);
            BLog.Add(objectContext, currCorporation, currUser, log);

            BStaticSystem bStaticSystems = new BStaticSystem();
            StaticCorporationSystem corpSys = bStaticSystems.GetStaticCorpSystem(objectContext, currCorporation, oldName, false);
            if (corpSys != null)
            {
                corpSys.datePreviousMet = corpSys.dateCurrentMet;
            }
            else
            {
                bStaticSystems.AddStaticCorporationSystem(objectContext, currCorporation, newName, true, DateTime.UtcNow, currSystem.info, currSystem.occupied);
            }


            ////////////////
            BWormwhole bWormhole = new BWormwhole();
            List<Wormwhole> whToSys = new List<Wormwhole>();
            bWormhole.GetWhToSystem(currSystem, ref whToSys);
            string strWhToSysId = string.Empty;
            if (whToSys != null && whToSys.Count > 0)
            {
                strWhToSysId = whToSys.First().fromSysWormwholeID;
            }

            int sysClass = 0;
            sysClass = GetSystemClassBasedOnSecStatusOrWormholes(objectContext, bWormhole, currSystem.name, strWhToSysId);
            if (sysClass > 0 && sysClass != currSystem.sysClass)
            {
                ChangeSystemClass(objectContext, currCorporation, currSystem, currUser, sysClass);
            }
            ////////////////////
            

        }

        public void ChangeSystemInfo(Entities objectContext, Corporation currCorporation, Systems currSystem, User currUser, string newInfo)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currSystem == null)
            {
                throw new ArgumentNullException("currSystem");
            }

            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }

            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }

            if (BUser.CanUserOperateWithCorporationItems(currCorporation, currUser) == false)
            {
                throw new InvalidOperationException("User cannot operate with corporation items");
            }

            BStaticSystem bStaticSys = new BStaticSystem();
            bStaticSys.ChangeStaticCorpSysInfo(objectContext, currCorporation, currSystem, newInfo);

            string log = string.Format("{0} {1}.", Tools.GetLogResource("systemInfoUpdate", currCorporation), currSystem.name);

            currSystem.info = newInfo;
            Tools.Save(objectContext);

            BLog.Add(objectContext, currCorporation, currUser, log);
        }

        public void ChangeSystemOccupied(Entities objectContext, Corporation currCorporation, Systems currSystem, User currUser, string newOccupation)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currSystem == null)
            {
                throw new ArgumentNullException("currSystem");
            }

            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }

            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }

            if (BUser.CanUserOperateWithCorporationItems(currCorporation, currUser) == false)
            {
                throw new InvalidOperationException("User cannot operate with corporation items");
            }

            BStaticSystem bStaticSys = new BStaticSystem();
            bStaticSys.ChangeStaticCorpSysOccupation(objectContext, currCorporation, currSystem, newOccupation);

            string log = string.Format("{0} {1}.", Tools.GetLogResource("systemOccupationUpdate", currCorporation), currSystem.name);

            currSystem.occupied = newOccupation;
            Tools.Save(objectContext);

            BLog.Add(objectContext, currCorporation, currUser, log);
        }

        public void ChangeSystemClass(Entities objectContext, Corporation currCorporation, Systems currSystem, User currUser, int newClass)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currSystem == null)
            {
                throw new ArgumentNullException("currSystem");
            }

            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }

            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }

            if (BUser.CanUserOperateWithCorporationItems(currCorporation, currUser) == false)
            {
                throw new InvalidOperationException("User cannot operate with corporation items");
            }

            if (newClass < 1)
            {
                currSystem.sysClass = null;
            }
            else
            {
                currSystem.sysClass = newClass;
            }

            Tools.Save(objectContext);

            string log = string.Format("{0} {1}.", Tools.GetLogResource("systemClassChange", currCorporation), currSystem.name);
            BLog.Add(objectContext, currCorporation, currUser, log);
        }

        public void ChangeSystemEffect(Entities objectContext, Corporation currCorporation, Systems currSystem, User currUser, int newEffect)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currSystem == null)
            {
                throw new ArgumentNullException("currSystem");
            }

            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }

            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }

            if (BUser.CanUserOperateWithCorporationItems(currCorporation, currUser) == false)
            {
                throw new InvalidOperationException("User cannot operate with corporation items");
            }

            if (newEffect < 1)
            {
                currSystem.sysEffect = null;
            }
            else
            {
                currSystem.sysEffect = newEffect;
            }

            Tools.Save(objectContext);

            string log = string.Format("{0} {1}.", Tools.GetLogResource("systemEffectChange", currCorporation), currSystem.name);
            BLog.Add(objectContext, currCorporation, currUser, log);
        }

        public void GetSystemConnections(Systems currSystem, ref List<Wormwhole> toCurrSystem, ref List<Wormwhole> fromCurrSystem)
        {
            if (currSystem == null)
            {
                throw new ArgumentNullException("currSystem");
            }

            currSystem.WhFromSystem.Load();
            currSystem.WhToSystem.Load();

            toCurrSystem = currSystem.WhToSystem.ToList();
            fromCurrSystem = currSystem.WhFromSystem.ToList();
        }

        

        public string GetSystemClass(Systems currSystem)
        {
            if (currSystem == null)
            {
                throw new ArgumentNullException("currSystem");
            }

            string result = string.Empty;

            if (currSystem.sysClass != null)
            {
                switch (currSystem.sysClass)
                {
                    case 1:
                        result = HttpContext.GetGlobalResourceObject("business","sysClassWormhole").ToString(); //"дупка"
                        break;
                    case 2:
                        result = "C1";
                        break;
                    case 3:
                        result = "C2";
                        break;
                    case 4:
                        result = "C3";
                        break;
                    case 5:
                        result = "C4";
                        break;
                    case 6:
                        result = "C5";
                        break;
                    case 7:
                        result = "C6";
                        break;
                    case 8:
                        result = "0.0";
                        break;
                    case 9:
                        result = "low sec";
                        break;
                    case 10:
                        result = "high sec";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("currSystem.sysClass");
                }
            }
            
            return result;
        }


        public string GetSystemEffect(Systems currSystem)
        {
            if (currSystem == null)
            {
                throw new ArgumentNullException("currSystem");
            }

            string result = string.Empty;

            if (currSystem.sysEffect != null)
            {
                switch (currSystem.sysEffect)
                {
                    case 1:
                        result = HttpContext.GetGlobalResourceObject("business","sysEffectNoEffect").ToString(); //"няма ефект";
                        break;
                    case 2:
                        result = "Black Hole";
                        break;
                    case 3:
                        result = "Cataclysmic Variable";
                        break;
                    case 4:
                        result = "Magnetar";
                        break;
                    case 5:
                        result = "Pulsar";
                        break;
                    case 6:
                        result = "Red Giant";
                        break;
                    case 7:
                        result = "Wolf-Rayet";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("currSystem.sysEffect");
                }
            }

            return result;
        }









    }
}