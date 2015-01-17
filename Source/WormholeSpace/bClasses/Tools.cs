﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Web;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Threading;

namespace WhSpace
{
    public class Tools
    {
        public static void AssertObjectContextExists(Entities objectContext)
        {
            if (objectContext == null)
            {
                throw new ArgumentNullException("objectContext");
            }
        }

        public static void Save(Entities objectContext)
        {
            AssertObjectContextExists(objectContext);
            objectContext.SaveChanges();
        }

        public static Label GetLabel(string text, bool newLineAtEnd)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("text is null or empty");
            }

            Label newLabel = new Label();
            newLabel.Text = text;

            if (newLineAtEnd == true)
            {
                newLabel.Text += "</br>";
            }

            return newLabel;
        }

        public static Label GetLabel(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("text is null or empty");
            }

            Label newLabel = new Label();
            newLabel.Text = text;

            return newLabel;
        }

        public static string BreakLongNumber(long number)
        {
            string strNum = number.ToString("N");

            return strNum;
        }

        public static string BreakDoubleNumber(double number, bool withFractions)
        {
            string strNum = string.Empty;

            if (number == 0)
            {
                strNum = number.ToString();
            }
            else
            {
                if (withFractions == true)
                {
                    strNum = number.ToString("N");
                }
                else
                {
                    strNum = number.ToString("### ### ### ### ### ###");
                }
            }

            return strNum;
        }

        public static string GetSystemNameRedirrectUrl(string sysName)
        {
            if (string.IsNullOrEmpty(sysName))
            {
                throw new ArgumentException("sysName is null or empty");
            }

            string result = string.Empty;

            if (sysName.Length >= 7 && sysName.StartsWith("J"))
            {
                result = string.Format("http://www.wormnav.com/index.php?locus={0}", sysName);
            }
            else
            {
                result = string.Format("http://evemaps.dotlan.net/system/{0}", sysName);
            }

            return result;
        }

        public static String GetFormattedTextFromDB(String text)
        {
            if (text == null || text == string.Empty)
            {
                return text;
            }

            string FormattedStr;

            FormattedStr = text.Replace(Environment.NewLine, "<br/>");
            FormattedStr = FormattedStr.Replace("\n", "<br/>");
            FormattedStr = FormattedStr.Replace("\r", "<br/>");

            FormattedStr = FormattedStr.Replace("<br/> ", "<br/>&nbsp;");
            FormattedStr = FormattedStr.Replace("  ", " &nbsp;");

            return FormattedStr;
        }

        public static string GetDateTimeInLocalFormat(DateTime time)
        {
            CultureInfo currCulture = Thread.CurrentThread.CurrentCulture;
            CultureInfo bgCulture = CultureInfo.CreateSpecificCulture("bg");

            string result = string.Empty;

            if (currCulture.Name == bgCulture.Name)
            {
                result = time.ToLocalTime().ToString("dd MMM yyyy HH:mm:ss");
            }
            else
            {
                result = time.ToLocalTime().ToString();
            }

            return result;
        }


        public static int ParseStringToInt(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException("str is null or empty");
            }

            int result = 0;

            if (int.TryParse(str, out result) == false)
            {
                throw new InvalidCastException(string.Format("Couldn't parse '{0}' to int.", str));
            }

            return result;
        }

        public static HyperLink GetHyperLink(string hyperlinkID, string hyperlinkURL, string hyperlinkText)
        {
            if (string.IsNullOrEmpty(hyperlinkID))
            {
                throw new ArgumentException("hyperlinkID is null");
            }
            if (string.IsNullOrEmpty(hyperlinkURL))
            {
                throw new ArgumentException("hyperlinkURL is null");
            }
            if (string.IsNullOrEmpty(hyperlinkText))
            {
                throw new ArgumentException("hyperlinkText is null");
            }

            HyperLink newHyperlink = new HyperLink();
            newHyperlink.ID = hyperlinkID;
            newHyperlink.Text = hyperlinkText;
            newHyperlink.NavigateUrl = hyperlinkURL;
            return newHyperlink;
        }

        public static HyperLink GetHyperLink(string hyperlinkURL, string hyperlinkText)
        {
            if (string.IsNullOrEmpty(hyperlinkURL))
            {
                throw new ArgumentException("hyperlinkURL is null");
            }
            if (string.IsNullOrEmpty(hyperlinkText))
            {
                throw new ArgumentException("hyperlinkText is null");
            }

            HyperLink newHyperlink = new HyperLink();
            newHyperlink.Text = hyperlinkText;
            newHyperlink.NavigateUrl = hyperlinkURL;
            return newHyperlink;
        }

        public static String TrimString(String text, int LengthWanted, bool cutFromStart, bool withPoints)
        {
            if (text == null || text.Length < 1)
            {
                //throw new BusinessException("text is null or empty");
                return string.Empty;
            }
            if (LengthWanted < 1)
            {
                throw new ArgumentOutOfRangeException("LengthWanted is < 1");
            }

            int length = text.Length;
            if (length > LengthWanted)
            {
                String subStr = "";

                if (cutFromStart)
                {
                    subStr = text.Substring(length - LengthWanted, LengthWanted);
                    if (withPoints)
                    {
                        subStr = string.Format("...{0}", subStr);
                    }
                }
                else
                {
                    subStr = text.Substring(0, length - (length - LengthWanted));
                    if (withPoints)
                    {
                        subStr = string.Format("{0}...", subStr);
                    }
                }
                return subStr;
            }
            else
            {
                return text;
            }
        }
        
        public static void ChangeUiCultureFromSessionAndCookie(Entities objectContext)
        {
            Language lang = UiCookie.GetLanguage(objectContext, HttpContext.Current.Session);

            string strLang = UiCookie.GetLanguageAsString(lang);

            ChangeUICulture(strLang);
        }

        /// <summary>
        /// Changes the UI culture of the current thread depending on the supplied culture identifier.
        /// </summary>
        /// <param name="requestedCultureStr">The requested culture identifier (e.g. "en", "bg", etc.)</param>
        private static void ChangeUICulture(string requestedCultureStr)
        {
            if (string.IsNullOrEmpty(requestedCultureStr) == false)
            {
                try
                {
                    CultureInfo requestedCulture = CultureInfo.CreateSpecificCulture(requestedCultureStr);
                    if (requestedCulture != null)
                    {
                        Thread.CurrentThread.CurrentUICulture = requestedCulture;
                    }

                }
                catch (Exception ex)
                {

                    throw new Exception(ex.ToString());
                }
            }
        }


        public static string GetLogResource(string name, Corporation currCorporation) 
        { 
            if (currCorporation == null)
            {
                throw new ArgumentNullException("currCorporation");
            }
            
            string result = string.Empty;
            CultureInfo corpCulture = CultureInfo.CreateSpecificCulture(currCorporation.logsLanguage);

            if (string.IsNullOrEmpty(name) == false)
            {
                System.Resources.ResourceManager rm =
                    new System.Resources.ResourceManager("WhSpace.BLogs", System.Reflection.Assembly.GetExecutingAssembly());

                result = rm.GetString(name, corpCulture);
            }
            if (string.IsNullOrEmpty(result))
            {
                result = string.Empty;
            }
            return result;
        }


    }
}