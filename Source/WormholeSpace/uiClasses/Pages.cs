﻿// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace WhSpace
{
    public class Pages
    {

        /// <summary>
        /// Checks page parameter if its correct , if its not redirrects to error page
        /// </summary>
        public static void CheckPageParameters(HttpResponse Response, long itemsNumber, string strItemsPerPage,
            string strCurrPage, string errorRedirectUrl, out long page, out long itemsOnPage)
        {
            if (Response == null)
            {
                throw new ArgumentNullException("HttpResponse");
            }
            if (string.IsNullOrEmpty(errorRedirectUrl))
            {
                throw new ArgumentException("errorRedirectUrl is null or empty");  
            }

            page = 1;
            itemsOnPage = 0;

            if (!long.TryParse(strItemsPerPage, out itemsOnPage))
            {
                Response.Redirect(errorRedirectUrl);
            }

            if (itemsNumber > 0)
            {
                if (!string.IsNullOrEmpty(strCurrPage) && long.TryParse(strCurrPage, out page))
                {
                    if (itemsOnPage > 0)
                    {
                        if (page > 1)
                        {
                            long expression = itemsOnPage * (page - 1);
                            if (itemsNumber > expression)
                            {
                                // valid page
                            }
                            else
                            {
                                Response.Redirect(errorRedirectUrl);
                            }
                        }
                        else if (page < 1)
                        {
                            Response.Redirect(errorRedirectUrl);
                        }
                        else
                        {
                            page = 1;
                        }
                    }
                    else
                    {
                        Response.Redirect(errorRedirectUrl);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(strCurrPage))
                    {
                        Response.Redirect(errorRedirectUrl);
                    }
                }
            }
            else
            {
                
            }


        }

        /// <summary>
        /// True if page parameters are ok, false if not
        /// </summary>
        public static bool CheckPageParameters(long itemsNumber, string strItemsPerPage,
            string strCurrPage, out long page, out long itemsOnPage)
        {

            bool correctParameters = false;

            page = 1;
            itemsOnPage = 0;

            if (!long.TryParse(strItemsPerPage, out itemsOnPage))
            {
                return false;
            }

            if (itemsNumber > 0)
            {
                if (!string.IsNullOrEmpty(strCurrPage) && long.TryParse(strCurrPage, out page))
                {
                    if (itemsOnPage > 0)
                    {
                        if (page > 1)
                        {
                            long expression = itemsOnPage * (page - 1);
                            if (itemsNumber > expression)
                            {
                                correctParameters = true;
                            }
                        }
                        else if (page < 1)
                        {


                        }
                        else
                        {
                            correctParameters = true;
                            page = 1;
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(strCurrPage))
                    {
                        correctParameters = true;
                    }
                }
            }
            else
            {
                correctParameters = true;
            }

            return correctParameters;
        }

        public static bool CheckPageParameters(long itemsNumber, long page, long itemsOnPage)
        {

            bool correctParameters = false;

            if (itemsNumber > 0)
            {

                if (itemsOnPage > 0)
                {
                    if (page > 1)
                    {
                        long expression = itemsOnPage * (page - 1);
                        if (itemsNumber > expression)
                        {
                            correctParameters = true;
                        }
                    }
                    else if (page < 1)
                    {


                    }
                    else
                    {
                        correctParameters = true;
                    }
                }

            }
            else
            {
                correctParameters = true;
            }

            return correctParameters;
        }

        public static PlaceHolder GetPagesPlaceHolder(long itemsNumber, long itemsPerPage, long currPage, string urlToAssignTo)
        {
            PlaceHolder newPh = new PlaceHolder();

            if (itemsNumber > 0 && itemsPerPage > 0 && currPage >= 0 &&
                !string.IsNullOrEmpty(urlToAssignTo) && itemsNumber > itemsPerPage)
            {

                string strPageParam = "";
                if (urlToAssignTo.Contains('?'))
                {
                    strPageParam = "&page=";
                }
                else
                {
                    strPageParam = "?page=";
                }

                long numOfPages = itemsNumber / itemsPerPage;
                if ((itemsNumber % itemsPerPage) > 0)
                {
                    numOfPages++;
                }

                Label pageLbl = new Label();
                newPh.Controls.Add(pageLbl);
                pageLbl.Text = HttpContext.GetGlobalResourceObject("Business","page").ToString() + " ";


                int numOfLinks = 5;

                if (numOfPages <= numOfLinks)
                {
                    for (int i = 1; i <= numOfPages; i++)
                    {
                        if (i != currPage)
                        {
                            HyperLink pgLink = Tools.GetHyperLink(string.Format("{0}{1}{2}", urlToAssignTo, strPageParam, i)
                                , string.Format("&nbsp;{0}&nbsp;", i.ToString()));
                            newPh.Controls.Add(pgLink);

                        }
                        else
                        {
                            Label lbCurrPg = new Label();
                            newPh.Controls.Add(lbCurrPg);
                            lbCurrPg.Text = string.Format("&nbsp;{0}&nbsp;", currPage.ToString());
                        }
                    }
                }
                else
                {
                    HyperLink firstPage = Tools.GetHyperLink(string.Format("{0}{1}1", urlToAssignTo, strPageParam), "&nbsp;<<&nbsp;");
                    newPh.Controls.Add(firstPage);

                    int pb = 0;    // pages behind
                    int pa = 0;    // pages after

                    int pagesBehindAndAfter = numOfLinks / 2;


                    for (int pagesBehind = pagesBehindAndAfter; pagesBehind > 0; pagesBehind--)
                    {
                        if (currPage - pagesBehind >= 1)
                        {
                            pb++;
                        }
                    }

                    for (int pagesAfter = 1; pagesAfter <= pagesBehindAndAfter; pagesAfter++)
                    {
                        if (currPage + pagesAfter <= numOfPages)
                        {
                            pa++;
                        }
                    }

                    if (pb < pagesBehindAndAfter)
                    {
                        pa += pagesBehindAndAfter - pb;
                    }
                    if (pa < pagesBehindAndAfter)
                    {
                        pb += pagesBehindAndAfter - pa;
                    }

                    for (long cp = (currPage - pb); cp < currPage; cp++)
                    {

                        HyperLink pg2Link = Tools.GetHyperLink(string.Format("{0}{1}{2}", urlToAssignTo, strPageParam, cp)
                            , string.Format("&nbsp;{0}&nbsp;", cp.ToString()));
                        newPh.Controls.Add(pg2Link);
                    }


                    Label lblCurrPage = new Label();
                    lblCurrPage.Text = string.Format("&nbsp;{0}&nbsp;", currPage.ToString());
                    newPh.Controls.Add(lblCurrPage);


                    for (long cp = (currPage + 1); cp <= (pa + currPage); cp++)
                    {
                        HyperLink hlPage = Tools.GetHyperLink(string.Format("{0}{1}{2}", urlToAssignTo, strPageParam, cp)
                            , string.Format("&nbsp;{0}&nbsp;", cp.ToString()));

                        newPh.Controls.Add(hlPage);
                    }

                    HyperLink lastPage = Tools.GetHyperLink(string.Format("{0}{1}{2}", urlToAssignTo, strPageParam, numOfPages), "&nbsp;>>&nbsp;");
                    newPh.Controls.Add(lastPage);
                }

            }

            return newPh;
        }

        public static PlaceHolder GetPagesPlaceHolderSmallStyle(long itemsNumber, long itemsPerPage, long currPage, string urlToAssignTo)
        {
            PlaceHolder newPh = new PlaceHolder();

            if (itemsNumber > 0 && itemsPerPage > 0 && currPage >= 0 &&
                !string.IsNullOrEmpty(urlToAssignTo) && itemsNumber > itemsPerPage)
            {

                string styleClass = "pagesLinksSmall";

                string strPageParam = "";
                if (urlToAssignTo.Contains('?'))
                {
                    strPageParam = "&page=";
                }
                else
                {
                    strPageParam = "?page=";
                }

                long numOfPages = itemsNumber / itemsPerPage;
                if ((itemsNumber % itemsPerPage) > 0)
                {
                    numOfPages++;
                }

                Label pageLbl = new Label();
                newPh.Controls.Add(pageLbl);
                pageLbl.Text = HttpContext.GetGlobalResourceObject("Business", "page").ToString();

                pageLbl.CssClass = styleClass;

                int numOfLinks = 5;

                if (numOfPages <= numOfLinks)
                {
                    for (int i = 1; i <= numOfPages; i++)
                    {
                        if (i != currPage)
                        {
                            HyperLink pgLink = Tools.GetHyperLink(string.Format("{0}{1}{2}", urlToAssignTo, strPageParam, i)
                                , string.Format("&nbsp;{0}&nbsp;", i.ToString()));
                            newPh.Controls.Add(pgLink);

                            pgLink.CssClass = styleClass;
                        }
                        else
                        {
                            Label lbCurrPg = new Label();
                            newPh.Controls.Add(lbCurrPg);
                            lbCurrPg.Text = string.Format("&nbsp;{0}&nbsp;", currPage.ToString());

                            lbCurrPg.CssClass = styleClass;
                        }
                    }
                }
                else
                {
                    HyperLink firstPage = Tools.GetHyperLink(string.Format("{0}{1}1", urlToAssignTo, strPageParam), "&nbsp;<<&nbsp;");
                    newPh.Controls.Add(firstPage);

                    firstPage.CssClass = styleClass;

                    int pb = 0;    // pages behind
                    int pa = 0;    // pages after

                    int pagesBehindAndAfter = numOfLinks / 2;


                    for (int pagesBehind = pagesBehindAndAfter; pagesBehind > 0; pagesBehind--)
                    {
                        if (currPage - pagesBehind >= 1)
                        {
                            pb++;
                        }
                    }

                    for (int pagesAfter = 1; pagesAfter <= pagesBehindAndAfter; pagesAfter++)
                    {
                        if (currPage + pagesAfter <= numOfPages)
                        {
                            pa++;
                        }
                    }

                    if (pb < pagesBehindAndAfter)
                    {
                        pa += pagesBehindAndAfter - pb;
                    }
                    if (pa < pagesBehindAndAfter)
                    {
                        pb += pagesBehindAndAfter - pa;
                    }

                    for (long cp = (currPage - pb); cp < currPage; cp++)
                    {

                        HyperLink pg2Link = Tools.GetHyperLink(string.Format("{0}{1}{2}", urlToAssignTo, strPageParam, cp)
                            , string.Format("&nbsp;{0}&nbsp;", cp.ToString()));
                        newPh.Controls.Add(pg2Link);

                        pg2Link.CssClass = styleClass;
                    }


                    Label lblCurrPage = new Label();
                    lblCurrPage.Text = string.Format("&nbsp;{0}&nbsp;", currPage.ToString());
                    newPh.Controls.Add(lblCurrPage);


                    for (long cp = (currPage + 1); cp <= (pa + currPage); cp++)
                    {
                        HyperLink hlPage = Tools.GetHyperLink(string.Format("{0}{1}{2}", urlToAssignTo, strPageParam, cp)
                            , string.Format("&nbsp;{0}&nbsp;", cp.ToString()));

                        newPh.Controls.Add(hlPage);
                        hlPage.CssClass = styleClass;
                    }

                    HyperLink lastPage = Tools.GetHyperLink(string.Format("{0}{1}{2}", urlToAssignTo, strPageParam, numOfPages), "&nbsp;>>&nbsp;");
                    newPh.Controls.Add(lastPage);

                    lastPage.CssClass = styleClass;
                }

            }

            return newPh;
        }

        /// <summary>
        /// returns numbers from which item number to which item number must be shown at current page
        /// </summary>
        public static void GetFromItemNumberToItemNumber(long currPage, long itemsOnPage, out long from, out long to)
        {
            from = itemsOnPage * (currPage - 1);
            to = itemsOnPage * currPage;
        }

    }
}