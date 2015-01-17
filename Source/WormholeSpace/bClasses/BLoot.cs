﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;

namespace WhSpace
{
    public class BLoot
    {
        public List<Loot> GetLootWithType(Entities objectContext, int type)
        {
            Tools.AssertObjectContextExists(objectContext);

            List<Loot> loot = objectContext.LootSet.Where(lt => lt.type == type).ToList();

            if (loot == null || loot.Count < 1)
            {
                throw new ArgumentOutOfRangeException(string.Format("There is no loot with type = {0}", type));
            }

            return loot;
        }

        public Loot Get(Entities objectContext, long id, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(objectContext);

            Loot loot = objectContext.LootSet.FirstOrDefault(lot => lot.ID == id);


            if (throwExcIfNull == true && loot == null)
            {
                throw new InvalidOperationException(string.Format("no loot with id = {0}", id));
            }

            return loot;
        }

    }
}