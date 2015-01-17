// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

function timedCount() {
    var lbl = document.getElementById('ContentPlaceHolder1_lblUpdatesSinceLoad');
    var hf = document.getElementById('ContentPlaceHolder1_hfPageLoaded');
    if (lbl != null && hf != null) {
        //lbl.innerHTML += "1";

        PageMethods.WmUpdatesSincePageLoad(hf.value, successGettingLogs, timeOut, error);

        setTimeout("timedCount()", 10000);
    }
}

function successGettingLogs(str) {

    var lbl = document.getElementById('ContentPlaceHolder1_lblUpdatesSinceLoad');
    if (lbl != null) {

        lbl.innerHTML = str;

    }

}

function timeOut() {
    var lbl = document.getElementById('ContentPlaceHolder1_lblUpdatesSinceLoad');
    if (lbl != null) {

        lbl.innerHTML = "(time out)";

    }
}

function error() {
    var lbl = document.getElementById('ContentPlaceHolder1_lblUpdatesSinceLoad');
    if (lbl != null) {

        lbl.innerHTML = "(грешка)";

    }
}



/////////////////////////////////////////////////////////////////////

function getScrollY() {
    var scrOfX = 0, scrOfY = 0;
    if (typeof (window.pageYOffset) == 'number') {
        //Netscape compliant
        scrOfY = window.pageYOffset;
        scrOfX = window.pageXOffset;
    } else if (document.body && (document.body.scrollLeft || document.body.scrollTop)) {
        //DOM compliant
        scrOfY = document.body.scrollTop;
        scrOfX = document.body.scrollLeft;
    } else if (document.documentElement && (document.documentElement.scrollLeft || document.documentElement.scrollTop)) {
        //IE6 standards compliant mode
        scrOfY = document.documentElement.scrollTop;
        scrOfX = document.documentElement.scrollLeft;
    }
    //return [scrOfX, scrOfY];
    return scrOfY;
}


function getY(oElement) {

    var iReturnValue = 0;
    while ((oElement != undefined) && (oElement != null)) {
        iReturnValue += oElement.offsetTop;
        oElement = oElement.offsetParent;
    }
    return iReturnValue;
}

function getX(oElement) {
    var iReturnValue = 0;
    while ((oElement != undefined) && (oElement != null)) {
        iReturnValue += oElement.offsetLeft;
        oElement = oElement.offsetParent;
    }
    return iReturnValue;
}

function ShowWhInfo(pnlId, lblId) {

    pnlId = GetWhInfoPanelID(pnlId, true);

    pnl = document.getElementById(pnlId);
    lbl = document.getElementById(lblId);

    if (pnl != null && lbl != null) {

        var yPos = getY(lbl);
        var xPos = getX(lbl);

        pnl.style.position = "absolute";
        if (lbl.offsetHeight) {
            yPos += (lbl.offsetHeight + 1);
        }
        else {
            yPos += 25;
        }


        // "px" needed for Firefox and Chrome
        pnl.style.top = yPos + "px";
        pnl.style.left = xPos + "px";

        pnl.style.visibility = "visible";
    }
}

function HideWhInfo(pnlId) {

    pnlId = GetWhInfoPanelID(pnlId, true);

    pnl = document.getElementById(pnlId);
    if (pnl != null) {
        pnl.style.visibility = "hidden";
    }
}

var tempX = 0;
var tempY = 0;

document.onmousemove = function (event) {
    if (!event) {
        event = window.event;
    }
    tempX = event.clientX;
    tempY = event.clientY;
    if (tempX < 0) { tempX = 0; }
    if (tempY < 0) { tempY = 0; }
}


function insertCommas(nField) {

    if (/^0/.test(nField.value)) {
        nField.value = nField.value.substring(0, 1);
    }
    if (Number(nField.value.replace(/,/g, ""))) {
        var tmp = nField.value.replace(/,/g, "");
        tmp = tmp.toString().split('').reverse().join('').replace(/(\d{3})/g, '$1,').split('').reverse().join('').replace(/^,/, '');
        if (/\./g.test(tmp)) {
            tmp = tmp.split(".");
            tmp[1] = tmp[1].replace(/\,/g, "").replace(/ /, "");
            nField.value = tmp[0] + "." + tmp[1]
        }
        else {
            nField.value = tmp.replace(/ /, "");
        }
    }
    else {
        nField.value = nField.value.replace(/[^\d\,\.]/g, "").replace(/ /, "");
    }
}


function ChangeFocus(UpDownEl, currRow, leftEl, rightEl, keycode) {


    var tb = null;
    var cRow = parseInt(currRow);


    switch (keycode) {
        case 37: // left
            tb = document.getElementById(leftEl);
            break;
        case 38: // up

            for (var i = cRow - 1; i > cRow - 10; i--) {

                tb = document.getElementById(UpDownEl + i.toString());
                if (tb != null) {
                    break;
                }
            }


            break;
        case 39: // right
            tb = document.getElementById(rightEl);
            break;
        case 40: // down

            for (var i = cRow + 1; i < cRow + 10; i++) {

                tb = document.getElementById(UpDownEl + i.toString());
                if (tb != null) {
                    break;
                }
            }


            break;
        default:
            break;
    }

    if (tb != null) {
        tb.focus();
    }

}
