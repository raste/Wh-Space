﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhSpace
{
    public class BSignature
    {
        public void Add(Entities objectContext, Corporation currCorporation, Systems currSystem, User currUser, string sigName, int sigType, string sigInfo)
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

            Signature newSignature = new Signature();

            newSignature.Systems = currSystem;
            newSignature.Users = currUser;
            newSignature.name = sigName.ToUpperInvariant(); ;
            newSignature.type = sigType;
            newSignature.info = sigInfo;
            newSignature.dateAdded = DateTime.UtcNow;

            objectContext.SignatureSet.AddObject(newSignature);
            Tools.Save(objectContext);

            string log = string.Format("{0} {1} {2} {3}.", Tools.GetLogResource("signatureAdd", currCorporation), newSignature.name
                , Tools.GetLogResource("signatureAdd2", currCorporation), currSystem.name);
            BLog.Add(objectContext, currCorporation, currUser, log);
        }

        public void UpdateSignature(Entities objectContext, Corporation currCorporation, User currUser, Signature currSignature, string name, int type, string info)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currSignature == null)
            {
                throw new ArgumentNullException("currSignature");
            }
            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }
            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }

            if (currSignature.name != name)
            {
                currSignature.name = name.ToUpperInvariant(); ;
            }

            if (currSignature.type != type)
            {
                currSignature.type = type;
            }

            if (currSignature.info != info)
            {
                currSignature.info = info;
            }

            Tools.Save(objectContext);

            currSignature.SystemsReference.Load();
            string log = string.Format("{0} {1} {2} {3}.", Tools.GetLogResource("signatureEdit", currCorporation), currSignature.name
                , Tools.GetLogResource("signatureEdit2", currCorporation), currSignature.Systems.name);
            BLog.Add(objectContext, currCorporation, currUser, log);
        }

        public void DeleteSignature(Entities objectContext, Corporation currCorporation, User currUser, Signature currSignature, bool makeLog)
        {
            Tools.AssertObjectContextExists(objectContext);

            if (currSignature == null)
            {
                throw new ArgumentNullException("currSignature");
            }
            if (currUser == null)
            {
                throw new ArgumentNullException("currUser");
            }
            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }

            currSignature.SystemsReference.Load();
            string log = string.Format("{0} {1} {2} {3}.", Tools.GetLogResource("signatureDelete", currCorporation), currSignature.name
                , Tools.GetLogResource("signatureDelete2", currCorporation), currSignature.Systems.name);

            objectContext.DeleteObject(currSignature);
            Tools.Save(objectContext);

            if (makeLog == true)
            {
                BLog.Add(objectContext, currCorporation, currUser, log);
            }
        }

        public Signature Get(Entities objectContext, long id, bool throwExcIfNull)
        {
            Tools.AssertObjectContextExists(objectContext);

            Signature signature = objectContext.SignatureSet.FirstOrDefault(sig => sig.ID == id);

            if (throwExcIfNull == true && signature == null)
            {
                throw new InvalidOperationException(string.Format("no signature with id = {0}", id));
            }

            return signature;
        }

        public List<Signature> GetSystemSignatures(Systems currSystem)
        {
            if (currSystem == null)
            {
                throw new ArgumentNullException("currSystem");
            }

            currSystem.Signatures.Load();

            List<Signature> signatures = currSystem.Signatures.ToList();

            if (signatures != null && signatures.Count > 0)
            {
                IEnumerable<Signature> sigs = signatures.OrderBy(sig => sig.name);
                signatures = sigs.ToList();
            }

            return signatures;
        }

        public bool CheckIfThereIsAlreadySignatureForSys(Entities objectContext, Systems currSystem, string name)
        {
            if (currSystem == null)
            {
                throw new ArgumentNullException("currSystem");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name is empty or null");
            }

            bool sigTaken = false;

            Signature signature = objectContext.SignatureSet.FirstOrDefault(sig => sig.name == name && sig.Systems.ID == currSystem.ID);

            if (signature != null)
            {
                sigTaken = true;
            }

            return sigTaken;
        }

        public string GetSignatureType(Signature currSignature)
        {
            if (currSignature == null)
            {
                throw new ArgumentNullException("currSignature");
            }

            string result = string.Empty;

            switch (currSignature.type)
            {
                case 1:
                    result = "magnetometric";
                    break;
                case 2:
                    result = "gravimetric";
                    break;
                case 3:
                    result = "radar";
                    break;
                case 4:
                    result = "ladar";
                    break;
                case 5:
                    result = HttpContext.GetGlobalResourceObject("Business","signatureWormhole").ToString(); //"дупка"
                    break;
                case 6:
                    result = HttpContext.GetGlobalResourceObject("Business","signatureNotScanned").ToString(); //"не е сканирана"
                    break;
                default:
                    throw new ArgumentOutOfRangeException("currSignature.type");
            }

            return result;
        }

    }
}