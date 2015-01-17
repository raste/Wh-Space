﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhSpace
{
    public class BOperation
    {
        public void Add(Entities objectContext, Corporation currCorporation, int operationType, int operationBased, int opLength, string systemOp
            , int iskCutType, int iskCut, string iskCutInfo, string opInfo, double totalISK, double totalIskWithCut
            , double iskPerOneLength, List<OperationUserRow> opUsers, List<OperationLootRow> opLoot, User currUser)
        {
            CheckAddParams(objectContext, currCorporation, operationType, operationBased, opLength, iskCutType, iskCut
                , opUsers, opLoot, currUser);
             
            // add operation
            Operation newOperation = new Operation();

            newOperation.RCorporation = currCorporation;
            newOperation.RAddedBy = currUser;
            newOperation.dateAdded = DateTime.UtcNow;
            newOperation.dateLastModified = null;
            newOperation.RLastModifiedBy = null;

            if (iskCut > 0)
            {
                newOperation.iskCut = iskCut;
            }
            else
            {
                newOperation.iskCut = null;
            }

            newOperation.iskCutType = iskCutType;
            newOperation.iskCutInfo = iskCutInfo;

            newOperation.iskMadeWithCut = totalIskWithCut;
            newOperation.iskMade = totalISK;
            newOperation.opBasedOnType = operationBased;
            newOperation.opInfo = opInfo;
            newOperation.opLength = opLength;
            newOperation.opType = operationType;
            newOperation.systemName = systemOp;

            objectContext.AddToOperationSet(newOperation);
            Tools.Save(objectContext);

            // add users
            AddUsersToOperation(objectContext, newOperation, opUsers, iskPerOneLength);

            // add loot
            AddLootToOperation(objectContext, newOperation, opLoot);

            // log
            string msg = string.Format("{0} {1}.", Tools.GetLogResource("operationAdd", currCorporation), newOperation.ID);
            BLog.Add(objectContext, currCorporation, currUser, msg);
            

        }

        private void AddUsersToOperation(Entities objectContext, Operation newOperation, List<OperationUserRow> opUsers, double iskPerLength)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (newOperation == null)
            {
                throw new ArgumentNullException("newOperation");
            }
            if (opUsers == null || opUsers.Count < 1)
            {
                throw new ArgumentOutOfRangeException("opUsers.Count < 1");
            }

            foreach (OperationUserRow user in opUsers)
            {
                OperationUser newOpUser = new OperationUser();

                newOpUser.info = user.Info;
                newOpUser.iskMade = user.Time * iskPerLength;
                newOpUser.Operations = newOperation;
                newOpUser.participatingLength = user.Time;
                newOpUser.Users = user.user;

                objectContext.AddToOperationUserSet(newOpUser);
                Tools.Save(objectContext);
            }       
        }

        private void AddLootToOperation(Entities objectContext, Operation newOperation, List<OperationLootRow> opLoot)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (newOperation == null)
            {
                throw new ArgumentNullException("newOperation");
            }
            if (opLoot == null || opLoot.Count < 1)
            {
                throw new ArgumentOutOfRangeException("opLoot.Count < 1");
            }

            foreach (OperationLootRow loot in opLoot)
            {
                OperationLoot newOpLoot = new OperationLoot();

                newOpLoot.Loot1 = loot.loot;
                newOpLoot.Operations = newOperation;
                newOpLoot.pricePerOne = loot.Price;
                newOpLoot.quantity = loot.Quantity;

                objectContext.AddToOperationLootSet(newOpLoot);
                Tools.Save(objectContext);
            }

        }


        private void CheckAddParams(Entities objectContext, Corporation currCorporation, int operationType, int operationBased, int opLength 
            , int iskCutType, int iskCut, List<OperationUserRow> opUsers, List<OperationLootRow> opLoot, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (operationType < 1)
            {
                throw new ArgumentOutOfRangeException("operationType < 1");
            }

            if (operationBased < 1)
            {
                throw new ArgumentOutOfRangeException("operationBased < 1");
            }

            if (opLength < 1)
            {
                throw new ArgumentOutOfRangeException("opLength < 1");
            }

            if (iskCutType < 1)
            {
                throw new ArgumentOutOfRangeException("iskCutType < 1");
            }

            if (opUsers == null || opUsers.Count < 1)
            {
                throw new ArgumentOutOfRangeException("opUsers.Count < 1");
            }

            if (opLoot == null || opLoot.Count < 1)
            {
                throw new ArgumentOutOfRangeException("opLoot.Count < 1");
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

            if (currUser.admin == false && currUser.canEdit == false)
            {
                throw new Exception("currUser cannot edit");
            }

        }

        public Operation Get(Entities objectContext, Corporation currCorporation, long id, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }

            Operation operation = objectContext.OperationSet.FirstOrDefault(op => op.ID == id && op.RCorporation.ID == currCorporation.ID);

            if (throwExcIfNull == true && operation == null)
            {
                throw new InvalidOperationException(string.Format("no operation with id = {0}", id));
            }

            return operation;
        }

        public OperationUser GetOperationUser(Entities objectContext, long id, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(objectContext);

            OperationUser opUser = objectContext.OperationUserSet.FirstOrDefault(opu => opu.ID == id);


            if (throwExcIfNull == true && opUser == null)
            {
                throw new InvalidOperationException(string.Format("no opUser with id = {0}", id));
            }

            return opUser;
        }

        public OperationLoot GetOperationLoot(Entities objectContext, long id, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(objectContext);

            OperationLoot opLoot = objectContext.OperationLootSet.FirstOrDefault(opl => opl.ID == id);


            if (throwExcIfNull == true && opLoot == null)
            {
                throw new InvalidOperationException(string.Format("no opLoot with id = {0}", id));
            }

            return opLoot;
        }

        public List<Operation> GetOperations(Entities objectContext, Corporation currCorporation, int number, int from, int to)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (number < 1)
            {
                throw new ArgumentOutOfRangeException("number < 1");
            }

            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");

            }

            List<Operation> operations = objectContext.OperationSet.Where(op => op.RCorporation.ID == currCorporation.ID).ToList();
            operations.Reverse();

            List<Operation> result = new List<Operation>();

            for (int i = 0; i < operations.Count; i++)
            {
                if (i >= from && i < to)
                {
                    result.Add(operations[i]);
                }
            }
            /*
                if (operations != null && operations.Count > 0)
                {
                    

                    if (operations.Count <= to)
                    {
                        if (from > 0)
                        {
                            operations.RemoveRange(0, from);
                        }
                    }
                    else
                    {
                        if (from > 0)
                        {
                            operations.RemoveRange(0, operations.Count - from);
                            operations.RemoveRange(from, operations.Count - (to - from));
                        }
                        else
                        {
                            operations.RemoveRange(to, operations.Count - to);
                        }


                    }

                }
            */
            return result;
        }

        public List<OperationUser> GetOperationUsers(Operation op)
        {
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }

            op.Operation_Users.Load();
            List<OperationUser> users = op.Operation_Users.ToList();
            return users;
        }

        public List<User> GetUsersParticipatingInOperation(Operation op)
        {
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }

            List<OperationUser> opUsers = GetOperationUsers(op);
            List<User> users = new List<User>();

            foreach (OperationUser user in opUsers)
            {
                user.UsersReference.Load();
                users.Add(user.Users);
            }

            return users;
        }

        public List<OperationLoot> GetOperationLoot(Operation op)
        {
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }

            op.Operation_Loot.Load();
            List<OperationLoot> loot = op.Operation_Loot.ToList();
            return loot;
        }

        public List<Loot> GetDroppedLoodInOperation(Operation op)
        {
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }

            List<OperationLoot> oploots = GetOperationLoot(op);
            List<Loot> loot = new List<Loot>();

            foreach (OperationLoot opLoot in oploots)
            {
                opLoot.Loot1Reference.Load();
                loot.Add(opLoot.Loot1);
            }

            return loot;
        }

        public static string GetOperationType(Operation op)
        {
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }

            string result = string.Empty;

            switch (op.opType)
            {
                case 1:
                    result = HttpContext.GetGlobalResourceObject("Business","operationTypeCombat").ToString(); //"бойна";
                    break;
                case 2:
                    result = HttpContext.GetGlobalResourceObject("Business","operationTypeGasHarvesting").ToString(); //"събиране газ";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Operation type = {0} is not supported", op.opType));
            }

            return result;

        }

        public static string GetOperationBasedOn(Operation op)
        {
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }

            string result = string.Empty;

            switch (op.opBasedOnType)
            {
                case 1:
                    result = HttpContext.GetGlobalResourceObject("Business","operationBasedOnPlexes").ToString(); //"плексове";
                    break;
                case 2:
                    result = HttpContext.GetGlobalResourceObject("Business","operationBasedOnTime").ToString(); //"време";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Operation based on type = {0} is not supported", op.opBasedOnType));
            }

            return result;
        }

        public static string GetOperationLengthType(Operation op, bool edit)
        {
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }

            return GetOperationLengthType(op.opBasedOnType, edit);
        }

        public static string GetOperationLengthType(int opBasedOnType, bool edit)
        {

            string result = string.Empty;

            switch (opBasedOnType)
            {
                case 1:
                    if (edit)
                    {
                        result = HttpContext.GetGlobalResourceObject("Business", "operationPlexesCountLong").ToString(); //plexes "count";
                    }
                    else
                    {
                        result = HttpContext.GetGlobalResourceObject("Business", "operationPlexesLength").ToString(); //plexes "count";
                    }
                    break;
                case 2:
                    if (edit)
                    {
                        result = HttpContext.GetGlobalResourceObject("Business", "operationTimeLengthLong").ToString(); //"length";
                    }
                    else
                    {
                        result = HttpContext.GetGlobalResourceObject("Business", "operationTimeLength").ToString(); //"length";
                    }
                    break;
                default:
                    throw new ArgumentException(string.Format("op.opBasedOnType = {0} is not supported type", opBasedOnType));
            }

            return result;
        }



        public static string GetOperationIskCutType(Operation op)
        {
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }

            string result = string.Empty;

            switch (op.iskCutType)
            {
                case 1:
                    result = "%";
                    break;
                case 2:
                    result = HttpContext.GetGlobalResourceObject("Business","operationIskCutTypeIsk").ToString(); //"isk";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Operation isk cut type = {0} is not supported", op.iskCutType));
            }

            return result;

        }

        public void ChangeOperationIskCutInfo(Entities objectContext, Corporation currCorporation, Operation op, string newInfo, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (op == null)
            {
                throw new ArgumentNullException("op");
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

            if (op.iskCutInfo == newInfo)
            {
                throw new InvalidOperationException("op.iskCutInfo == newInfo");
            }

            op.RLastModifiedBy = currUser;
            op.dateLastModified = DateTime.UtcNow;
            op.iskCutInfo = newInfo;
            Tools.Save(objectContext);

            string msg = string.Format("{0} {1}{2} {3}.", Tools.GetLogResource("operationUpdateTaxInfo", currCorporation), op.ID
                , Tools.GetLogResource("operationGeneralWhichWasAdded", currCorporation), Tools.GetDateTimeInLocalFormat(op.dateAdded));
            BLog.Add(objectContext, currCorporation, currUser, msg);
        }

        public void ChangeOperationIskCut(Entities objectContext, Corporation currCorporation, Operation op, int iskCutType, int iskCut, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (op == null)
            {
                throw new ArgumentNullException("op");
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

            if (iskCutType == op.iskCutType && iskCut == op.iskCut)
            {
                throw new InvalidOperationException("No change in isk cut type and isk cut");
            }

            double totalIskWithCut = op.iskMade;

            switch (iskCutType)
            {
                case 1:
                    if (iskCut > 100)
                    {
                        throw new ArgumentOutOfRangeException("iskCut > 100%");
                    }
                    if (iskCut > 0)
                    {
                        totalIskWithCut -= (totalIskWithCut / 100) * iskCut;
                    }
                    break;
                case 2:
                    if (iskCut > op.iskMade)
                    {
                        throw new ArgumentOutOfRangeException("iskCut > op.iskMade");
                    }
                    if (iskCut > 0)
                    {
                        totalIskWithCut -= iskCut;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("iskCutType is not 1 or 2");
            }

            op.RLastModifiedBy = currUser;
            op.dateLastModified = DateTime.UtcNow;
            op.iskCutType = iskCutType;
            if (iskCut > 0)
            {
                op.iskCut = iskCut;
            }
            else
            {
                op.iskCut = null;
            }
            op.iskMadeWithCut = totalIskWithCut;
            Tools.Save(objectContext);

            RecalculateOpIsk(objectContext, op);

            string msg = string.Format("{0} {1}{2} {3}.", Tools.GetLogResource("operationUpdateTax", currCorporation), op.ID
                , Tools.GetLogResource("operationGeneralWhichWasAdded", currCorporation), Tools.GetDateTimeInLocalFormat(op.dateAdded));
            BLog.Add(objectContext, currCorporation, currUser, msg);
        }

        public void ChangeOperationInfo(Entities objectContext, Corporation currCorporation, Operation op, string newInfo, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (op == null)
            {
                throw new ArgumentNullException("op");
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

            if (op.systemName == newInfo)
            {
                throw new InvalidOperationException("op.systemName == newInfo");
            }

            op.RLastModifiedBy = currUser;
            op.dateLastModified = DateTime.UtcNow;
            op.opInfo = newInfo;
            Tools.Save(objectContext);

            string msg = string.Format("{0} {1}{2} {3}", Tools.GetLogResource("operationInfoUpdated", currCorporation), op.ID
                , Tools.GetLogResource("operationGeneralWhichWasAdded", currCorporation), Tools.GetDateTimeInLocalFormat(op.dateAdded));
            BLog.Add(objectContext, currCorporation, currUser, msg);
        }

        public void ChangeOperationSystem(Entities objectContext, Corporation currCorporation, Operation op, string newSystem, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (op == null)
            {
                throw new ArgumentNullException("op");
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

            if (op.systemName == newSystem)
            {
                throw new InvalidOperationException("op.systemName == newSystem");
            }

            op.RLastModifiedBy = currUser;
            op.dateLastModified = DateTime.UtcNow;
            op.systemName = newSystem;
            Tools.Save(objectContext);

            string msg = string.Format("{0} {1}{2} {3}", Tools.GetLogResource("operationSystemChange", currCorporation), op.ID
                , Tools.GetLogResource("operationGeneralWhichWasAdded", currCorporation), Tools.GetDateTimeInLocalFormat(op.dateAdded));
            BLog.Add(objectContext, currCorporation, currUser, msg);
        }

        public void ChangeOperationLenght(Entities objectContext, Corporation currCorporation, Operation op, int newLength, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (op == null)
            {
                throw new ArgumentNullException("op");
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

            if (CheckIfNewOperationLengthIsValid(op, newLength) == false)
            {
                throw new ArgumentOutOfRangeException("op new length is not valid");
            }

            op.RLastModifiedBy = currUser;
            op.dateLastModified = DateTime.UtcNow;
            op.opLength = newLength;
            Tools.Save(objectContext);

            string msg = string.Format("{0} {1}{2} {3}", Tools.GetLogResource("operationLenghtChange", currCorporation), op.ID
                , Tools.GetLogResource("operationGeneralWhichWasAdded", currCorporation), Tools.GetDateTimeInLocalFormat(op.dateAdded));
            BLog.Add(objectContext, currCorporation, currUser, msg);
        }

        public bool CheckIfNewOperationLengthIsValid(Operation op, int newLength)
        {
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }

            bool result = false;

            if (newLength != op.opLength)
            {

                int maxUserLength = 0;

                List<OperationUser> opUsers = GetOperationUsers(op);
                if (opUsers != null && opUsers.Count > 0)
                {
                    foreach (OperationUser opUser in opUsers)
                    {
                        if (opUser.participatingLength > maxUserLength)
                        {
                            maxUserLength = opUser.participatingLength;
                        }
                    }
                }

                if (maxUserLength <= newLength)
                {
                    result = true;
                }
            }

            return result;
        }

        public void ChangeOperationLoot(Entities objectContext, Corporation currCorporation, OperationLoot opLoot, Loot newLoot, double newPrice, int newQuantity, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (opLoot == null)
            {
                throw new ArgumentNullException("opLoot");
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

            if (!opLoot.OperationsReference.IsLoaded)
            {
                opLoot.OperationsReference.Load();
            }
            if (!opLoot.Loot1Reference.IsLoaded)
            {
                opLoot.Loot1Reference.Load();
            }

            Loot oldLoot = opLoot.Loot1;
            Operation currOp = opLoot.Operations;

            if (newLoot != null && CheckIfLootIsDroppedInOperation(currOp, newLoot) == true)
            {
                throw new InvalidOperationException("newLoot is already dropped during the operation.");
            }

            if (newLoot != null)
            {
                if (opLoot.Loot1.ID == newLoot.ID)
                {
                    throw new InvalidOperationException("New operation loot is equal to edited operation loot");
                }

                opLoot.Loot1 = newLoot;
                Tools.Save(objectContext);
            }

            bool recalculateISK = false;

            if (newPrice != opLoot.pricePerOne)
            {
                if (newPrice < 0)
                {
                    throw new ArgumentOutOfRangeException("newPrice < 0");
                }

                opLoot.pricePerOne = newPrice;
                Tools.Save(objectContext);

                recalculateISK = true;
            }

            if (newQuantity != opLoot.quantity)
            {
                if (newQuantity < 1)
                {
                    throw new ArgumentOutOfRangeException("newQuantity < 1");
                }

                opLoot.quantity = newQuantity;
                Tools.Save(objectContext);

                recalculateISK = true;
            }

            if (recalculateISK == true)
            {
                RecalculateOpIsk(objectContext, currOp);
            }

            currOp.RLastModifiedBy = currUser;
            currOp.dateLastModified = DateTime.UtcNow;
            Tools.Save(objectContext);

            string msg = string.Format("{0} ' {1} ' {2} {3}{4} {5}.", Tools.GetLogResource("operationLootEdit", currCorporation), oldLoot.name
                , Tools.GetLogResource("operationLootEdit2", currCorporation), currOp.ID, Tools.GetLogResource("operationGeneralWhichWasAdded", currCorporation)
                , Tools.GetDateTimeInLocalFormat(currOp.dateAdded));

            BLog.Add(objectContext, currCorporation, currUser, msg);
        }

        public void AddOperationUser(Entities objectContext, Corporation currCorporation, Operation op, User newUser, int newLengh, string newInfo, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (op == null)
            {
                throw new ArgumentNullException("op");
            }
            if (newUser == null)
            {
                throw new ArgumentNullException("newUser");
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

            if (newLengh < 0)
            {
                throw new ArgumentOutOfRangeException("newLengh < 0");
            }
            else if (newLengh > op.opLength)
            {
                throw new ArgumentOutOfRangeException("newLengh > op.opLength");
            }

            if (CheckIfUserIsParticipatingInOperation(op, newUser) == true)
            {
                throw new InvalidOperationException("New user is already participating in operation.");
            }

            OperationUser newOpUser = new OperationUser();
            newOpUser.Operations = op;
            newOpUser.info = newInfo;
            newOpUser.participatingLength = newLengh;
            newOpUser.iskMade = 0;
            newOpUser.Users = newUser;

            Tools.Save(objectContext);

            RecalculateOpIsk(objectContext, op);

            string msg = string.Format("{0} ' {1} ' {2} {3}{4} {5}.", Tools.GetLogResource("operationUserAdd", currCorporation), newUser.name
                , Tools.GetLogResource("operationUserAdd2", currCorporation), op.ID, Tools.GetLogResource("operationGeneralWhichWasAdded", currCorporation)
                , Tools.GetDateTimeInLocalFormat(op.dateAdded));
            BLog.Add(objectContext, currCorporation, currUser, msg);

        }

        public void AddOperationLoot(Entities objectContext, Corporation currCorporation, Operation op, Loot newLoot, double newPrice, int newQuantity, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (op == null)
            {
                throw new ArgumentNullException("op");
            }
            if (newLoot == null)
            {
                throw new ArgumentNullException("newLoot");
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

            if (newPrice < 0)
            {
                throw new ArgumentOutOfRangeException("newPrice < 0");
            }
            else if (newQuantity < 1)
            {
                throw new ArgumentOutOfRangeException("newQuantity < 1");
            }

            if (CheckIfLootIsDroppedInOperation(op, newLoot) == true)
            {
                throw new InvalidOperationException("New loot is already dropped in operation.");
            }

            OperationLoot newOpLoot = new OperationLoot();
            newOpLoot.Loot1 = newLoot;
            newOpLoot.Operations = op;
            newOpLoot.pricePerOne = newPrice;
            newOpLoot.quantity = newQuantity;

            Tools.Save(objectContext);

            RecalculateOpIsk(objectContext, op);

            string msg = string.Format("{0} ' {1} ' {2} {3}{4} {5}.", Tools.GetLogResource("operationLootAdd", currCorporation), newLoot.name
                , Tools.GetLogResource("operationLootAdd2", currCorporation), op.ID, Tools.GetLogResource("operationGeneralWhichWasAdded", currCorporation)
                , Tools.GetDateTimeInLocalFormat(op.dateAdded));
            BLog.Add(objectContext, currCorporation, currUser, msg);

        }

        public void ChangeOperationUser(Entities objectContext, Corporation currCorporation, OperationUser opUser, User newUser, int newLengh, string newInfo, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (opUser == null)
            {
                throw new ArgumentNullException("opUser");
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

            if (!opUser.OperationsReference.IsLoaded)
            {
                opUser.OperationsReference.Load();
            }
            if (!opUser.UsersReference.IsLoaded)
            {
                opUser.UsersReference.Load();
            }

            User oldUser = opUser.Users;
            Operation currOp = opUser.Operations;

            if (newUser != null && CheckIfUserIsParticipatingInOperation(currOp, newUser) == true)
            {
                throw new InvalidOperationException("NewUser is already participating in the operation.");
            }

            if (newUser != null)
            {
                if (opUser.Users.ID == newUser.ID)
                {
                    throw new InvalidOperationException("New operation user is equal to edited operation user");
                }

                opUser.Users = newUser;
                Tools.Save(objectContext);
            }

            if (newInfo != opUser.info)
            {
                opUser.info = newInfo;
                Tools.Save(objectContext);
            }

            if (newLengh != opUser.participatingLength)
            {
                if (newLengh < 0)
                {
                    throw new ArgumentOutOfRangeException("newLengh < 0");
                }
                if (newLengh > currOp.opLength)
                {
                    throw new ArgumentOutOfRangeException("newLengh > currOp.opLength");
                }

                opUser.participatingLength = newLengh;
                Tools.Save(objectContext);

                RecalculateOpIsk(objectContext, currOp);
            }

            currOp.RLastModifiedBy = currUser;
            currOp.dateLastModified = DateTime.UtcNow;
            Tools.Save(objectContext);

            string msg = string.Format("{0} ' {1} ' {2} {3}{4} {5}.", Tools.GetLogResource("operationUserEdit", currCorporation), oldUser.name
                , Tools.GetLogResource("operationUserEdit2", currCorporation), currOp.ID, Tools.GetLogResource("operationGeneralWhichWasAdded", currCorporation)
                , Tools.GetDateTimeInLocalFormat(currOp.dateAdded));

            BLog.Add(objectContext, currCorporation, currUser, msg);


        }

        public bool CheckIfUserIsParticipatingInOperation(Operation op, User user)
        {
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            bool result = false;

            List<OperationUser> opUsers = GetOperationUsers(op);
            foreach (OperationUser opUser in opUsers)
            {
                opUser.UsersReference.Load();
                if (opUser.Users.ID == user.ID)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }


        public bool CheckIfLootIsDroppedInOperation(Operation op, Loot loot)
        {
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }
            if (loot == null)
            {
                throw new ArgumentNullException("loot");
            }

            bool result = false;

            List<OperationLoot> opLoots = GetOperationLoot(op);
            foreach (OperationLoot opLoot in opLoots)
            {
                opLoot.Loot1Reference.Load();
                if (opLoot.Loot1.ID == loot.ID)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public int CountOperations(Entities objectContext, Corporation currCorporation)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation is null");
            }

            return objectContext.OperationSet.Where(op => op.RCorporation.ID == currCorporation.ID).Count();
        }

        public int CountOperationLoot(Operation op)
        {
            if (op == null)
            {
                throw new ArgumentNullException("op is null");
            }

            op.Operation_Loot.Load();
            return op.Operation_Loot.Count;
        }

        public int CountOperationUsers(Operation op)
        {
            if (op == null)
            {
                throw new ArgumentNullException("op is null");
            }

            op.Operation_Users.Load();
            return op.Operation_Users.Count;
        }

        public void DeleteOperation(Entities objectContext, Corporation currCorporation, Operation operation, User currUser)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (operation == null)
            {
                throw new ArgumentNullException("operation is null");
            }
            if (currUser == null)
            {
                throw new ArgumentNullException("currUser is null");
            }

            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }
            if (BUser.CanUserOperateWithCorporationItems(currCorporation, currUser) == false)
            {
                throw new InvalidOperationException("User cannot operate with corporation items");
            }

            string msg = string.Format("{0} {1}{2} {3}.", Tools.GetLogResource("operationDelete", currCorporation), operation.ID
                , Tools.GetLogResource("operationGeneralWhichWasAdded", currCorporation), Tools.GetDateTimeInLocalFormat(operation.dateAdded));

            List<OperationLoot> opLoots = GetOperationLoot(operation);
            List<OperationUser> opUsers = GetOperationUsers(operation);

            foreach (OperationUser opUser in opUsers)
            {
                DeleteOperationUser(objectContext, currCorporation, opUser, currUser, false, false);
            }
            foreach (OperationLoot opLoot in opLoots)
            {
                DeleteOperationLoot(objectContext, currCorporation, opLoot, currUser, false, false);
            }

            objectContext.DeleteObject(operation);
            Tools.Save(objectContext);

            BLog.Add(objectContext, currCorporation, currUser, msg);

        }

        public void DeleteOperationLoot(Entities objectContext, Corporation currCorporation, OperationLoot opLoot, User currUser, bool recalculateOpIsk, bool addLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (opLoot == null)
            {
                throw new ArgumentNullException("opLoot is null");
            }
            if (currUser == null)
            {
                throw new ArgumentNullException("currUser is null");
            }

            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }
            if (BUser.CanUserOperateWithCorporationItems(currCorporation, currUser) == false)
            {
                throw new InvalidOperationException("User cannot operate with corporation items");
            }

            if (!opLoot.OperationsReference.IsLoaded)
            {
                opLoot.OperationsReference.Load();
            }
            if (!opLoot.Loot1Reference.IsLoaded)
            {
                opLoot.Loot1Reference.Load();
            }

            Operation editedOperation = opLoot.Operations;

            string msg = string.Format("{0} ' {1} ' {2} {3}{4} {5}.", Tools.GetLogResource("operationLootDelete", currCorporation), opLoot.Loot1.name
                , Tools.GetLogResource("operationLootDelete2", currCorporation), opLoot.Operations.ID, Tools.GetLogResource("operationGeneralWhichWasAdded", currCorporation)
                , Tools.GetDateTimeInLocalFormat(opLoot.Operations.dateAdded));

            objectContext.DeleteObject(opLoot);
            Tools.Save(objectContext);

            if (recalculateOpIsk == true)
            {
                RecalculateOpIsk(objectContext, editedOperation);
            }

            if (addLog == true)
            {
                BLog.Add(objectContext, currCorporation, currUser, msg);
            }

            editedOperation.RLastModifiedBy = currUser;
            editedOperation.dateLastModified = DateTime.UtcNow;
            Tools.Save(objectContext);

        }

        public void DeleteOperationUser(Entities objectContext, Corporation currCorporation, OperationUser opUser, User currUser, bool recalculateOpIsk, bool addLog)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (opUser == null)
            {
                throw new ArgumentNullException("opUser is null");
            }
            if (currUser == null)
            {
                throw new ArgumentNullException("currUser is null");
            }

            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }
            if (BUser.CanUserOperateWithCorporationItems(currCorporation, currUser) == false)
            {
                throw new InvalidOperationException("User cannot operate with corporation items");
            }

            if (!opUser.OperationsReference.IsLoaded)
            {
                opUser.OperationsReference.Load();
            }
            if (!opUser.UsersReference.IsLoaded)
            {
                opUser.UsersReference.Load();
            }

            Operation editedOperation = opUser.Operations;

            string msg = string.Format("{0} ' {1} ' {2} {3}{4} {5}.", Tools.GetLogResource("operationUserDelete", currCorporation), opUser.Users.name
                , Tools.GetLogResource("operationUserDelete2", currCorporation), editedOperation.ID, Tools.GetLogResource("operationGeneralWhichWasAdded", currCorporation)
                , Tools.GetDateTimeInLocalFormat(opUser.Operations.dateAdded));

            objectContext.DeleteObject(opUser);
            Tools.Save(objectContext);

            if (recalculateOpIsk == true)
            {
                RecalculateOpIsk(objectContext, editedOperation);
            }

            if (addLog == true)
            {
                BLog.Add(objectContext, currCorporation, currUser, msg);
            }

            editedOperation.RLastModifiedBy = currUser;
            editedOperation.dateLastModified = DateTime.UtcNow;
            Tools.Save(objectContext);

        }

        private void RecalculateOpIsk(Entities objectContext, Operation op)
        {
            Tools.AssertObjectContextExists(objectContext);
            if (op == null)
            {
                throw new ArgumentNullException("op is null");
            }

            double totalIsk = 0;
            double totalIskWithCut = 0;
            int totalLength = 0;
            double iskPerLenght = 0;
            double newUserIskMade = 0;

            List<OperationUser> opUsers = GetOperationUsers(op);
            List<OperationLoot> opLoots = GetOperationLoot(op);

            foreach (OperationLoot opLoot in opLoots)
            {
                totalIsk += opLoot.quantity * opLoot.pricePerOne;
            }

            totalIskWithCut = totalIsk;

            if (op.iskCut.HasValue == true)
            {
                switch (op.iskCutType)
                {
                    case 1:
                        totalIskWithCut -= (totalIsk / 100) * op.iskCut.Value;
                        break;
                    case 2:
                        totalIskWithCut -= op.iskCut.Value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("op.iskCutType is not 1 or 2.");
                }
            }

            if (op.iskMade != totalIsk)
            {
                op.iskMade = totalIsk;
                Tools.Save(objectContext);
            }
            if (op.iskMadeWithCut != totalIskWithCut)
            {
                op.iskMadeWithCut = totalIskWithCut;
                Tools.Save(objectContext);
            }
        
            foreach (OperationUser opUser in opUsers)
            {
                totalLength += opUser.participatingLength;
            }

            iskPerLenght = totalIskWithCut / totalLength;

            foreach (OperationUser opUser in opUsers)
            {
                newUserIskMade = opUser.participatingLength * iskPerLenght;
                if (newUserIskMade != opUser.iskMade)
                {
                    opUser.iskMade = newUserIskMade;
                    Tools.Save(objectContext);
                } 
            }
        
        }


        public double GetPriceForLootFromLastOperation(Entities objectContext, Corporation currCorp, long ID)
        {
            Tools.AssertObjectContextExists(objectContext);

            double result = 0;

            List<OperationLoot> lastOpLoot = objectContext.OperationLootSet.Where(ol => ol.Loot1.ID == ID && ol.Operations.RCorporation.ID == currCorp.ID).ToList();
            if (lastOpLoot != null && lastOpLoot.Count > 0)
            {
                result = lastOpLoot.Last().pricePerOne;
            }

            return result;
        }








    }
}