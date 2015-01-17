﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

namespace WhSpace
{
    public class OperationUserRow
    {
        public string strID { get; set; }
        public string strTime { get; set; }

        public string Info { get; set; }

        public User user { get; set; }
        public int Time { get; set; }
    }

    public class OperationLootRow
    {
        public string strID { get; set; }
        public string strQuantity { get; set; }
        public string strPrice { get; set; }

        public Loot loot { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
    }
}