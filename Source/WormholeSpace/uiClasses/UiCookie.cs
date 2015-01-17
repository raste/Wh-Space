﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Linq;
using System.Web;

namespace WhSpace
{
    public class UiCookie
    {
        public static readonly string CookieName = "options";
        public static readonly string DefaultLanguage = "en";

        public static void SaveSettings(System.Web.SessionState.HttpSessionState Session, HttpResponse Response, HttpRequest Request
            , Language lang, User currUser)
        {

            if (Session == null)
            {
                throw new ArgumentNullException("Session");
            }
            if (Response == null)
            {
                throw new ArgumentNullException("Response");
            }
            if (Request == null)
            {
                throw new ArgumentNullException("Request");
            }


            string strLang = GetLanguageAsString(lang);


            // Cookie/Session format : lang(en/bg) ,id, hash

            System.Text.StringBuilder strCookieSettings = new System.Text.StringBuilder();
            strCookieSettings.Append(strLang);

            if (currUser != null && Session[UiSessionParams.SessionStayLoggedIn] != null)
            {
                BUser bUser = new BUser();

                strCookieSettings.Append(",");
                strCookieSettings.Append(currUser.ID.ToString());
                strCookieSettings.Append(",");
                strCookieSettings.Append(bUser.GetUserHashedLogIn(currUser, true));
            }

            
            HttpContext currentContext = HttpContext.Current;
            HttpCookieCollection cookies = currentContext.Request.Cookies;
            string[] cookieNames = cookies.AllKeys;
            bool cookieExist = false;
            int timesFound = 0;
            for (int i = 0; i < cookieNames.Length; i++)
            {
                if ((CookieName != null) && (CookieName == cookieNames[i]))
                {
                    cookieExist = true;
                    timesFound++;
                }
            }

            HttpCookie clientSettingsCookie = new HttpCookie(CookieName, strCookieSettings.ToString());
            clientSettingsCookie.Expires = DateTime.Today.AddMonths(12);  // TODO: Update cookie expiration mechanism is necessary.

            if (cookieExist)
            {
                if (timesFound > 1)
                {
                    for (int i = 0; i < timesFound; i++)
                    {
                        HttpCookie myCookie = new HttpCookie(CookieName);
                        myCookie.Expires = DateTime.Now.AddDays(-1d);
                        Response.Cookies.Add(myCookie);
                    }

                    Response.Cookies.Add(clientSettingsCookie);
                }
                else
                {
                    Response.Cookies.Set(clientSettingsCookie);
                }
            }
            else
            {
                Response.Cookies.Add(clientSettingsCookie);
            }
                
            Session[UiSessionParams.SessionLangParam] = strLang;

            if (currUser != null)
            {
                Session[UiSessionParams.SessionCurrentUserID] = currUser.ID;
            }
            else
            {
                Session[UiSessionParams.SessionStayLoggedIn] = null;
            }

            Session[UiSessionParams.SessionCheckedForCookie] = null;
        }


        public static Language GetLanguage(Entities objectContext, System.Web.SessionState.HttpSessionState Session)
        {
            Tools.AssertObjectContextExists(objectContext);

            Language lang = Language.EN;
            User currUser = null;

            GetSettings(objectContext, Session, out lang, out currUser);

            return lang;
        }

        public static User GetUser(Entities objectContext, System.Web.SessionState.HttpSessionState Session)
        {
            Tools.AssertObjectContextExists(objectContext);

            Language lang = Language.EN;
            User currUser = null;

            GetSettings(objectContext, Session, out lang, out currUser);

            return currUser;
        }

        private static void GetSettings(Entities objectContext, System.Web.SessionState.HttpSessionState Session, out Language lang, out User user)
        {
            if (Session == null)
            {
                throw new ArgumentNullException("Session");
            }

            Tools.AssertObjectContextExists(objectContext);

            lang = Language.EN;
            user = null;
            string strLang = string.Empty;
            long id = 0;
            string strId = string.Empty;
            string hash = string.Empty;

            object objUtCookie = Session[UiSessionParams.SessionCheckedForCookie];
            if (objUtCookie == null)
            {
                HttpContext currentContext = HttpContext.Current;
                if (currentContext == null)
                {
                    throw new InvalidOperationException("The current HttpContext is not available.");
                }

                string cookieValues = string.Empty;
                HttpCookieCollection cookies = currentContext.Request.Cookies;
                string[] cookieNames = cookies.AllKeys;
                bool cookieFound = false;

                for (int i = 0; (cookieFound == false) && (i < cookieNames.Length); i++)
                {
                    if ((CookieName != null) && (CookieName == cookieNames[i]))
                    {
                        HttpCookie themeCookie = cookies[CookieName];
                        cookieValues = themeCookie.Value ?? string.Empty;
                        cookieFound = true;
                    }
                }

                Session[UiSessionParams.SessionCheckedForCookie] = true.ToString();

                if (!string.IsNullOrEmpty(cookieValues))
                {
                    string[] delimiters = new string[] { "," };
                    string[] textWords = cookieValues.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                    int count = textWords.Count();

                    switch (count) // lang, id, hash
                    {
                        case 1:
                            strLang = textWords[0];
                            break;
                        case 2:
                            strLang = textWords[0];
                            break;
                        case 3:
                            strLang = textWords[0];
                            strId = textWords[1];
                            hash = textWords[2];
                            break;
                        default:
                            break;

                    }
                }
            }
            else
            {
                object objUiLanguange = Session[UiSessionParams.SessionLangParam];

                if (objUiLanguange != null)
                {
                    strLang = objUiLanguange.ToString();
                }
            }

            if(string.IsNullOrEmpty(strLang))
            {
                lang = GetDefaultLanguage();
            }
            else
            {
                if (IsLanguageStringCorrect(strLang) == true)
                {
                    lang = GetStringAsLanguage(strLang);
                }
                else
                {
                    lang = GetDefaultLanguage();
                }
            }

            if (!string.IsNullOrEmpty(strId) && !string.IsNullOrEmpty(hash))
            {
                if (long.TryParse(strId, out id) == true)
                {
                    BUser bUser = new BUser();
                    user = bUser.Get(objectContext, id, hash);
                }
            }

            if (user != null)
            {
                Corporation userCorporation = BCorporation.GetUserCorporation(user, false);
                if (userCorporation != null)
                {
                    Session[UiSessionParams.SessionCurrCorporationID] = userCorporation.ID.ToString();
                }

                Session[UiSessionParams.SessionStayLoggedIn] = "true";
                Session[UiSessionParams.SessionCurrentUserID] = user.ID;
            }

            Session[UiSessionParams.SessionLangParam] = GetLanguageAsString(lang);
        }


        public static bool IsCookiesEnabled(HttpRequest Request, HttpResponse Response)
        {
            string currentUrl = Request.RawUrl;

            if (Request.QueryString["cookieCheck"] == null)
            {
                try
                {
                    HttpCookie c = new HttpCookie("SupportCookies", "true");
                    Response.Cookies.Add(c);

                    if (currentUrl.IndexOf("?") > 0)
                        currentUrl = currentUrl + "&cookieCheck=true";
                    else
                        currentUrl = currentUrl + "?cookieCheck=true";

                    Response.Redirect(currentUrl);
                }
                catch{}
            }

            bool result = true;

            if (!Request.Browser.Cookies || Request.Cookies["SupportCookies"] == null)
            {
                result = false;
            }

            return result;
        }


        public static Language GetDefaultLanguage()
        {
            return GetStringAsLanguage(DefaultLanguage);
        }

        public static string GetLanguageAsString(Language lang)
        {
            string result = string.Empty;

            switch (lang)
            {
                case Language.BG:
                    result = "bg";
                    break;
                case Language.EN:
                    result = "en";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Language = {0}, is not supported!", lang));      
            }

            return result;
        }

        public static Language GetStringAsLanguage(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException("str is null or empty");
            }

            Language result;
            str = str.ToLowerInvariant();

            switch (str)
            {
                case "bg":
                    result = Language.BG;
                    break;
                case "en":
                    result = Language.EN;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Language = {0}, is not supported!", str));
            }

            return result;
        }


        public static bool IsLanguageStringCorrect(string str)
        {
            bool result = false;

            if (!string.IsNullOrEmpty(str))
            {
                Language lang;

                try
                {
                    lang = GetStringAsLanguage(str);

                    result = true;
                }
                catch{ }
            }


            return result;
        }


        public static string GetLanguageFullName(string lang)
        {
            string result = string.Empty;

            lang = lang.ToLowerInvariant();

            switch (lang)
            {
                case "bg":
                    result = HttpContext.GetGlobalResourceObject("Resources", "langBulgarian").ToString();
                    break;
                case "en":
                    result = HttpContext.GetGlobalResourceObject("Resources", "langEnglish").ToString();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Language = {0}, is not supported!", lang));
            }

            return result;
        }

    }
}


public enum Language
{
    EN,
    BG
}