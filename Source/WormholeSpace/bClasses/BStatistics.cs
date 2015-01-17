﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;

namespace WhSpace
{
    public class BStatistics
    {
        private static object updateStatistic = new object();

        private Statistic GetLastRecord(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            Statistic currStatistic = objectContext.StatisticSet.OrderByDescending<Statistic, long>(
                new Func<Statistic, long>(IdSelector)).FirstOrDefault<Statistic>();

            return currStatistic;
        }

        private long IdSelector(Statistic statistic)
        {
            if (statistic == null)
            {
                throw new ArgumentNullException("statistic");
            }
            return statistic.ID;
        }

        /// <summary>
        /// Returns statistic from specified Date
        /// </summary>
        public Statistic Get(Entities objectContext, DateTime date)
        {
            Tools.AssertObjectContextExists(objectContext);

            DateTime dateOnly = date.Date;
            DateTime nextDate = dateOnly.AddDays(1);
            Statistic currStatistic = objectContext.StatisticSet.FirstOrDefault<Statistic>
                (stat => (stat.forDate >= dateOnly) && (stat.forDate < nextDate));

            return currStatistic;
        }

        /// <summary>
        /// Creates Ands Add`s statistic for current day
        /// </summary>
        /// <param name="currDate">The date to create a statistics record for.</param>
        private void CreateStatistic(Entities objectContext, DateTime currDate)
        {
            Tools.AssertObjectContextExists(objectContext);

            Statistic lastRecord = GetLastRecord(objectContext);
            if (lastRecord != null)
            {
                if (currDate.Date.Equals(lastRecord.forDate.Date))
                {
                    throw new InvalidOperationException("there is already record for today");
                }
            }

            Statistic newStatistic = new Statistic();
            newStatistic.forDate = currDate;

            newStatistic.usersDeleted = 0;
            newStatistic.visits = 0;
            newStatistic.systemsDeleted = 0;
            newStatistic.systemsAdded = 0;
            newStatistic.signaturesDeleted = 0;
            newStatistic.signaturesAdded = 0;
            newStatistic.registeredUsers = 0;
            newStatistic.registeredCorporations = 0;
            newStatistic.operationsDeleted = 0;
            newStatistic.operationsAdded = 0;
            
            objectContext.AddToStatisticSet(newStatistic);

            Tools.Save(objectContext);


        }

        /// <summary>
        /// Uincreases with 1 , field from current day`s statistic
        /// </summary>
        private void UpdateStatistic(Entities objectContext, string field)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (field == null || field.Length < 1)
            {
                throw new ArgumentException("field is null or empty");
            }

            lock (updateStatistic)
            {
                DateTime dtUtcNow = DateTime.UtcNow;
                Statistic currStatistic = Get(objectContext, dtUtcNow);
                if (currStatistic == null)
                {
                    CreateStatistic(objectContext, dtUtcNow);
                    currStatistic = Get(objectContext, dtUtcNow);
                    if (currStatistic == null)
                    {
                        throw new Exception("statistic for current day didnt create ot parameters are wrong");
                    }
                }

                switch (field)
                {
                    case ("visits"):
                        currStatistic.visits += 1;
                        break;
                    case ("usersDeleted"):
                        currStatistic.usersDeleted += 1;
                        break;
                    case ("systemsDeleted"):
                        currStatistic.systemsDeleted += 1;
                        break;
                    case ("systemsAdded"):
                        currStatistic.systemsAdded += 1;
                        break;
                    case ("signaturesDeleted"):
                        currStatistic.signaturesDeleted += 1;
                        break;
                    case ("signaturesAdded"):
                        currStatistic.signaturesAdded += 1;
                        break;
                    case ("registeredUsers"):
                        currStatistic.registeredUsers += 1;
                        break;
                    case ("registeredCorporations"):
                        currStatistic.registeredCorporations += 1;
                        break;
                    case ("operationsDeleted"):
                        currStatistic.operationsDeleted += 1;
                        break;
                    case ("operationsAdded"):
                        currStatistic.operationsAdded += 1;
                        break;
                    default:
                        throw new InvalidOperationException(string.Format("field = {0} is not supported", field));

                }
                Tools.Save(objectContext);
            }
        }

        public void NewVisit(Entities objectContext)
        {
            UpdateStatistic(objectContext, "visits");
        }

        public void UserRegistered(Entities objectContext)
        {
            UpdateStatistic(objectContext, "registeredUsers");
        }

        public void CorpRegistered(Entities objectContext)
        {
            UpdateStatistic(objectContext, "registeredCorporations");
        }


        public long getTodaysVisits(Entities objectContext)
        {
            Tools.AssertObjectContextExists(objectContext);

            long result = 0;

            DateTime today = DateTime.UtcNow;
            Statistic currStat = Get(objectContext, today);
            if (currStat != null)
            {
                result = currStat.visits;
            }

            return result;
        }

        /// <summary>
        /// Returns Last number od Statistics
        /// </summary>
        public List<Statistic> GetLastNumStatistics(Entities objectContext, long number)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (number < 1)
            {
                throw new ArgumentException("number < 1");
            }

            IEnumerable<Statistic> stats = objectContext.StatisticSet.
                OrderByDescending<Statistic, long>(new Func<Statistic, long>(IdSelector));

            long count = stats.Count<Statistic>();
            long num = number;

            if (count < num)
            {
                num = count;
            }

            List<Statistic> Statistics = new List<Statistic>();

            for (int i = 0; i < num; i++)
            {
                Statistics.Add(stats.ElementAt<Statistic>(i));
            }

            return Statistics;
        }
    }
}