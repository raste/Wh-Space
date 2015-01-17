﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;
using EveAI.Live;

namespace WhSpace
{
    public class BStaticSystem
    {
        protected object AddingCorporationSystem = new object();

        public void UpdateApiSystemsData(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (Configuration.ShouldApiUpdate(objectContext) == false)
            {
                return;
            }

            EveApi api = new EveApi();

            List<StaticSystemData> systemsData = objectContext.StaticSystemDataSet.ToList();

            List<EveAI.Live.Map.MapKill> kills = api.GetMapKillEntries();
            List<EveAI.Live.Map.MapJump> jumps = api.GetMapJumpEntries();

            bool dataInKills = true;
            bool dataInJumps = true;

            if (kills == null || kills.Count < 1)
            {
                dataInKills = false;
            }
            if (jumps == null || jumps.Count < 1)
            {
                dataInJumps = false;
            }

          
            if (dataInJumps == true || dataInKills == true)
            {
                EveAI.Live.Map.MapKill mapKill = null;
                EveAI.Live.Map.MapJump mapJump = null;

                foreach (StaticSystemData ssd in systemsData)
                {
                    if (dataInKills == true)
                    {
                        mapKill = kills.FirstOrDefault(k => k.SolarSystem.Name == ssd.System);
                        if (mapKill != null)
                        {
                            ssd.npcKills = mapKill.FactionKills;
                            ssd.podKills = mapKill.PodKills;
                            ssd.shipKills = mapKill.ShipKills;
                        }
                        else
                        {
                            ssd.npcKills = 0;
                            ssd.podKills = 0;
                            ssd.shipKills = 0;
                        }
                    }

                    if (dataInJumps)
                    {
                        mapJump = jumps.FirstOrDefault(k => k.SolarSystem.Name == ssd.System);
                        if (mapJump != null)
                        {
                            ssd.jumps = mapJump.Jumps;
                        }
                        else
                        {
                            ssd.jumps = 0;
                        }
                    }
                }

                Tools.Save(objectContext);
            }

            Configuration.UpdateDateLastApiCall(objectContext);

        }


        public StaticCorporationSystem GetStaticCorpSystem(Entities objectContext, Corporation corp, string name, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (corp == null)
            {
                throw new ArgumentNullException("corp");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name is null or empty");
            }

            StaticCorporationSystem corpSys = objectContext.StaticCorporationSystemSet.FirstOrDefault(ss => ss.RCorporation.ID == corp.ID && ss.RStaticSystem.System == name);

            if(throwExcIfNull == true && corpSys == null)
            {
                throw new InvalidOperationException(string.Format("There is no static corporation system with name : {0} for corporation ID : {1}"
                    , name, corp.ID));
            }

            return corpSys;
        }

        public void AddStaticCorporationSystem(Entities objectContext, Corporation corp, string sysName, bool saveContext, DateTime utcNow, string info, string occupied)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (corp == null)
            {
                throw new ArgumentNullException("corp");
            }
            if (string.IsNullOrEmpty(sysName))
            {
                throw new ArgumentException("sysName is null or empty");
            }

            lock(AddingCorporationSystem)
            {
                StaticCorporationSystem newCorpSys = GetStaticCorpSystem(objectContext, corp, sysName, false);
                if(newCorpSys != null)
                {
                    throw new InvalidOperationException(string.Format("There is already StaticCorporationSystem name : {0} for corporation ID : {1}", sysName, corp.ID));
                }

                newCorpSys = new StaticCorporationSystem();

                newCorpSys.RCorporation = corp;
                newCorpSys.RStaticSystem = GetStaticSystem(objectContext, sysName, true);
                newCorpSys.dateCurrentMet = utcNow;
                newCorpSys.dateFirstMet = utcNow;
                newCorpSys.datePreviousMet = utcNow;
                newCorpSys.info = info;
                newCorpSys.occupation = occupied;

                objectContext.AddToStaticCorporationSystemSet(newCorpSys);

                if (saveContext == true)
                {
                    Tools.Save(objectContext);
                }
            }
        }

        public StaticSystemData GetStaticSystem(Entities objectContext, string sysName, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (string.IsNullOrEmpty(sysName))
            {
                throw new ArgumentException("sysName is null or empty");
            }

            StaticSystemData system = objectContext.StaticSystemDataSet.FirstOrDefault(ss => ss.System == sysName);

            if(throwExcIfNull == true && system == null)
            {
                 throw new InvalidOperationException(string.Format("There is no StaticSystem with name : {0}", sysName));
            }

            return system;
        }


        public void ChangeStaticCorpSysInfo(Entities objectContext, Corporation currCorp, Systems currSystem, string newInfo)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCorp == null)
            {
                throw new ArgumentNullException("currCorp");
            }
            if (currSystem == null)
            {
                throw new ArgumentNullException("currSystem");
            }

            StaticCorporationSystem staticCorpSys = GetStaticCorpSystem(objectContext, currCorp, currSystem.name, false);
            if (staticCorpSys != null)
            {
                staticCorpSys.info = newInfo;

                Tools.Save(objectContext);
            }

        }

        public void ChangeStaticCorpSysInfo(Entities objectContext, StaticCorporationSystem currSystem, string newInfo)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currSystem == null)
            {
                throw new ArgumentNullException("currSystem");
            }

            currSystem.info = newInfo;

            Tools.Save(objectContext);
            

        }

        public void ChangeStaticCorpSysOccupation(Entities objectContext, Corporation currCorp, Systems currSystem, string newOccupation)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currCorp == null)
            {
                throw new ArgumentNullException("currCorp");
            }
            if (currSystem == null)
            {
                throw new ArgumentNullException("currSystem");
            }

            StaticCorporationSystem staticCorpSys = GetStaticCorpSystem(objectContext, currCorp, currSystem.name, false);
            if (staticCorpSys != null)
            {
                staticCorpSys.occupation = newOccupation;

                Tools.Save(objectContext);
            }

        }

        public void ChangeStaticCorpSysOccupation(Entities objectContext, StaticCorporationSystem currSystem, string newOccupation)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (currSystem == null)
            {
                throw new ArgumentNullException("currSystem");
            }

            currSystem.occupation = newOccupation;

            Tools.Save(objectContext);
        }
    }
}