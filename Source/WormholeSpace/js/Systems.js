// Wormhole space / wh-space (https://github.com/raste/Wh-Space)(http://wh-space.wiadvice.com)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

function ConnectSystems(obj1, obj2, line, bg) {
    if (obj1.line && obj1.from && obj1.to) {
        line = obj1;
        obj1 = line.from;
        obj2 = line.to;
    }
    var bb1 = obj1.getBBox(),
        bb2 = obj2.getBBox(),
        p = [{ x: bb1.x + bb1.width / 2, y: bb1.y - 1 },
        { x: bb1.x + bb1.width / 2, y: bb1.y + bb1.height + 1 },
        { x: bb1.x - 1, y: bb1.y + bb1.height / 2 },
        { x: bb1.x + bb1.width + 1, y: bb1.y + bb1.height / 2 },
        { x: bb2.x + bb2.width / 2, y: bb2.y - 1 },
        { x: bb2.x + bb2.width / 2, y: bb2.y + bb2.height + 1 },
        { x: bb2.x - 1, y: bb2.y + bb2.height / 2 },
        { x: bb2.x + bb2.width + 1, y: bb2.y + bb2.height / 2 }],
        d = {}, dis = [];
    for (var i = 0; i < 4; i++) {
        for (var j = 4; j < 8; j++) {
            var dx = Math.abs(p[i].x - p[j].x),
                dy = Math.abs(p[i].y - p[j].y);
            if ((i == j - 4) || (((i != 3 && j != 6) || p[i].x < p[j].x) && ((i != 2 && j != 7) || p[i].x > p[j].x) && ((i != 0 && j != 5) || p[i].y > p[j].y) && ((i != 1 && j != 4) || p[i].y < p[j].y))) {
                dis.push(dx + dy);
                d[dis[dis.length - 1]] = [i, j];
            }
        }
    }
    if (dis.length == 0) {
        var res = [0, 4];
    } else {
        res = d[Math.min.apply(Math, dis)];
    }
    var x1 = p[res[0]].x,
        y1 = p[res[0]].y,
        x4 = p[res[1]].x,
        y4 = p[res[1]].y;
    dx = Math.max(Math.abs(x1 - x4) / 2, 10);
    dy = Math.max(Math.abs(y1 - y4) / 2, 10);
    var x2 = [x1, x1, x1 - dx, x1 + dx][res[0]].toFixed(3),
        y2 = [y1 - dy, y1 + dy, y1, y1][res[0]].toFixed(3),
        x3 = [0, 0, 0, 0, x4, x4, x4 - dx, x4 + dx][res[1]].toFixed(3),
        y3 = [0, 0, 0, 0, y1 + dy, y1 - dy, y4, y4][res[1]].toFixed(3);

    var path = ["M", x1.toFixed(3), y1.toFixed(3), "C", x2, y2, x3, y3, x4.toFixed(3), y4.toFixed(3)].join(",");


    if (line && line.line) {
        line.bg && line.bg.attr({ path: path });
        line.line.attr({ path: path });
    } else {
        var color = typeof line == "string" ? line : "#000";

        var lineObj = paper.path(path).attr({ stroke: color, fill: "none" });
        lineObj.toBack();
    }


};


var paper = null;
var objSystems = new Array();
var indentX = 180;
var indentY = 64;

function StartDrawing() {
    if ((typeof (stellarSystems) != "undefined") && (stellarSystems != null)) {
        var stellarSystemsLength = stellarSystems.length;

        if (stellarSystemsLength > 0) {

            InicializeRaphael();

            var i = 0;
            for (i = 0; i < stellarSystemsLength; i++) {
                var stellarSystem = stellarSystems[i];
                DrawSystem(stellarSystem)
            }
        }

    }

}

function InicializeRaphael() {

    var holderDiv = "systemsHolder";

    holder = document.getElementById(holderDiv);

    if (!holder) {
        alert("couldnt get holder div");
    }

    var stellarSystemsLength = stellarSystems.length;

    var maxLevelX = 0;
    var maxLevelY = 0;

    var i = 0;
    for (i = 0; i < stellarSystemsLength; i++) {
        var stellarSystem = stellarSystems[i];

        if (stellarSystems[i].LevelX > maxLevelX) {
            maxLevelX = stellarSystems[i].LevelX;
        }
        if (stellarSystems[i].LevelY > maxLevelY) {
            maxLevelY = stellarSystems[i].LevelY;
        }
    }

    var holderHeigh = 90 + maxLevelY * indentY;
    var holderWidth = 120 + maxLevelX * (indentX + 20);

    if (holderWidth > 1020) {

        var pageDiv = document.getElementById("pageDiv");
        if (pageDiv != null) {
            pageDiv.style.width = holderWidth + "px";
        } else {
            alert("couldnt get pageDiv");
        }

    }

    /////
    paper = Raphael(holderDiv, holderWidth, holderHeigh);
    ////

    holder.style.height = holderHeigh + "px";
    holder.style.width = holderWidth + "px";

}

function GetSystemX(system) {
    if (system) {

        var startX = 70;

        var sysX = startX + indentX * system.LevelX;;
        return sysX;

    } else {
        alert("system is null or undefined");
    }
}

function GetSystemY(system) {
    if (system) {

        var startY = 40;

        var sysY = startY + indentY * system.LevelY;;
        return sysY;

    } else {
        alert("system is null or undefined");
    }
}

function DrawSystem(system) {

    if (system == null) {
        return;
    }

    var sysX = GetSystemX(system);
    var sysY = GetSystemY(system);

    var sysName = system.Name;
    if (system.SysClass != null && system.SysClass.length > 0) {
        sysName += "\n(" + system.SysClass + ")";
    }

    var sysText;

    if (system.LevelX != null && system.LevelX > 0) {

        // child system

        var childSys = paper.ellipse(sysX, sysY, 45, 28);

        childSys.sysID = system.ID;
        childSys.click(OnSysClick);

        ///////

        sysText = paper.text(sysX, sysY, sysName);

        sysText.sysID = system.ID;
        sysText.click(OnSysClick);
        ///////

        ColorSystem(system, childSys, sysText);

        objSystems.push(childSys);

        var parentIndex = GetSystemIndex(system.ParentID);
        var parentSys = stellarSystems[parentIndex];
        var parentSysEllipse = objSystems[parentIndex];

        if (parentSysEllipse) {

            var lineColor = GetConnectionColor(system);
            var whColor = GetWormholeColor(system);

            ConnectSystems(parentSysEllipse, childSys, lineColor, "#fff");
            DrawWormholes(parentSys, system, whColor);

        } else {
            alert("error");
        }

    } else {

        // root

        var rootSys = paper.ellipse(sysX, sysY, 40, 30);
        rootSys.sysID = system.ID;
        rootSys.click(OnSysClick);

        sysText = paper.text(sysX, sysY, sysName);
        sysText.sysID = system.ID;
        sysText.click(OnSysClick);

        ColorSystem(system, rootSys, sysText);

        objSystems.push(rootSys);
    }

}

function GetConnectionColor(system) {
    if (!system) {
        return "#000";
    }

    if (system.LevelX < 1) {
        return "#000";
    }

    var sameLvlSysAdded = 0;
    var sysIndex = GetSystemIndex(system.ID);

    var i = 0;
    for (i = 0; i < sysIndex; i++) {
        var stellarSystem = stellarSystems[i];

        if (stellarSystem.LevelX == system.LevelX) {
            sameLvlSysAdded++;
        }
    }

    if ((sameLvlSysAdded % 2) == 1) {
        return "#858585"; //#000
    } else {
        return "#825E48"; //#6C4900
    }

}

function GetWormholeColor(system) {
    if (!system) {
        return "#000";
    }

    if (system.LevelX < 1) {
        return "#000";
    }

    var sameLvlSysAdded = 0;
    var sysIndex = GetSystemIndex(system.ID);

    var i = 0;
    for (i = 0; i < sysIndex; i++) {
        var stellarSystem = stellarSystems[i];

        if (stellarSystem.LevelX == system.LevelX) {
            sameLvlSysAdded++;
        }
    }

    if ((sameLvlSysAdded % 2) == 1) {
        return "#353535"; //#000
    } else {
        return "#6C4900"; //#6C4900
    }

}

function ColorSystem(system, ellipseSystem, textSysName) {

    if (!system) {
        alert("system is null or undefined");
        return;
    }

    var selected = false;
    var sysColor = "#f00";
    var sysStroke = "#fff";
    var sysStrokeWidth = 2;
    var textFontSize = 12;
    var textColor = "#000";

    if (system.ID == GetSelectedSysID()) {

        // selected
        sysStrokeWidth = 5;
        selected = true;
    }

    if (system.LevelX < 1) {

        // root
        sysColor = "#A600A6";
        sysStroke = "#6A006A";
        textColor = "#fff";
        textFontSize = 14;

    } else {

        // not selected
        switch (system.SysClass) {

            case "0.0":
                sysColor = "#CC0000";
                sysStroke = "#840000";
                textColor = "#fff";
                break;
            case "low sec":
                sysColor = "#93841E";
                sysStroke = "#7D5500";
                textColor = "#fff";
                break;
            case "high sec":
                sysColor = "#009F00";
                sysStroke = "#006600";
                textColor = "#fff";
                break;
            default:
                sysColor = "#F2F4FF";
                sysStroke = "#0657B9";
                textColor = "#0974EA";
                break;
        }
    }

    var sysName = textSysName.attrs.text;
    var classIndex = sysName.indexOf("(");
    if (classIndex > 0) {
        sysName = sysName.substring(0, classIndex);
    }
    if (sysName.length > 16) {
        textFontSize = 8;
    }
    else if (sysName.length > 11) {
        textFontSize = 9;
    }

    ellipseSystem.attr({ fill: sysColor, stroke: sysStroke, "stroke-width": sysStrokeWidth, cursor: "pointer" });
    textSysName.attr({ fill: textColor, "font-size": textFontSize, cursor: "pointer" });

    if (selected == false) {

        var divId = GetSystemInfoPanelID(system.ID);

        ellipseSystem.sysInfoPnlID = divId;
        textSysName.sysInfoPnlID = divId;

        ellipseSystem.mouseover(OnSysOver);
        ellipseSystem.mouseout(OnSysOut);

        textSysName.ellipseIndex = objSystems.length;

        textSysName.mouseover(OnSysOver);
        textSysName.mouseout(OnSysOut);
    }
}

function DrawWormholes(systemFrom, systemTo, textColor) {

    var sysY1 = GetSystemY(systemFrom);
    var sysY2 = GetSystemY(systemTo);

    var sysX1 = GetSystemX(systemFrom);
    var sysX2 = GetSystemX(systemTo);

    var changePos = ChangeSysWormholePosition(systemTo, systemFrom);

    var textCenterX = (sysX1 + sysX2) / 2;
    var textCenterY = (sysY1 + sysY2) / 2;

    var whFromSysX = textCenterX;
    var whFromSysY = textCenterY;

    var whToSysX = textCenterX;
    var whToSysY = textCenterY;

    if (sysY1 != sysY2) {

        whFromSysX = textCenterX + 23;
        whToSysX = textCenterX - 23;

    } else {

        whFromSysY = textCenterY - 8;
        whToSysY = textCenterY + 8;
    }

    // draws labels near systemTo ellipse if previous same Level X system's levelY = systemTo.levelY - 1
    if (changePos == true) {

        textCenterX = sysX2 - 73;
        textCenterY = sysY2 - 30;

        whFromSysX = textCenterX + 23;
        whToSysX = textCenterX - 23;

        whFromSysY = textCenterY;
        whToSysY = textCenterY;
    }


    var whFromSys = null;
    var whToSys = null;

    if (systemTo.WhFromParent) {

        whFromSys = paper.text(whFromSysX, whFromSysY, systemTo.WhFromParent + " >");
        whFromSys.attr({ fill: textColor, cursor: "pointer", "font-size": 11 });  //stroke: "#fff"

        //whFromSys.whID = systemTo.WhFromParent;
        whFromSys.whInfoPnl = GetWhInfoPanelID(systemTo.WhFromParent, true);

        whFromSys.mouseover(OnWhOver);
        whFromSys.mouseout(OnWhOut);
    }
    if (systemTo.WhToParent) {
        whToSys = paper.text(whToSysX, whToSysY, "< " + systemTo.WhToParent);
        whToSys.attr({ fill: textColor, cursor: "pointer", "font-size": 11 });

        //whToSys.whID = systemTo.WhToParent;
        whToSys.whInfoPnl = GetWhInfoPanelID(systemTo.WhToParent, true);

        whToSys.mouseover(OnWhOver);
        whToSys.mouseout(OnWhOut);
    }
}

function GetWhInfoPanelID(whName, buildId) {

    if (whName == null) {
        return "";
    }

    var partID = whName;

    if (buildId == true) {
        partID = "wh" + whName + "info";
    }

    var divID = "";
    var noInfoId = "whNoInfo";

    var divs = document.getElementsByTagName("div");
    for (var i = 0; i < divs.length; i++) {

        if (divs[i].id.indexOf(noInfoId) >= 0) {
            noInfoId = divID = divs[i].id;
        }

        if (divs[i].id.indexOf(partID) >= 0) {
            divID = divs[i].id;
            break;
        }
    }

    if (divID.length < 1) {
        divID = noInfoId;
    }

    return divID;
}

function GetSystemInfoPanelID(systemID) {

    if (systemID == null) {
        return "";
    }

    var partID = "sys" + systemID + "info";

    var divID = "";

    var divs = document.getElementsByTagName("div");
    for (var i = 0; i < divs.length; i++) {

        if (divs[i].id.indexOf(partID) >= 0) {
            divID = divs[i].id;
            break;
        }
    }

    return divID;
}

function ChangeSysWormholePosition(system, parent) {

    var change = false;
    var parentY = parent.LevelY;
    var currSysY = system.LevelY;

    if (currSysY > parentY + 1) {
        change = true;
    }

    return change;
}

function GetSystemIndex(systemID) {

    var stellarSystemsLength = stellarSystems.length;

    var i = 0;
    var index = -1;
    for (i = 0; i < stellarSystemsLength; i++) {
        var stellarSystem = stellarSystems[i];

        if (stellarSystem.ID == systemID) {
            index = i;
            return index;
        }
    }

    if (index < 1) {
        alert("could not find system with ID = " + systemID);
    }

}

function GetSelectedSysID() {

    var ph = document.getElementById('ContentPlaceHolder1_hfSelectedSys');
    if (ph != null) {

        return ph.value;

    } else {
        alert("couldnt get hidden field");
    }

}

function OnSysClick() {

    var ph = document.getElementById('ContentPlaceHolder1_hfSelectedSys');
    if (ph != null) {

        ph.value = this.sysID;
        __doPostBack();

    } else {
        alert("couldnt get hidden field");
    }

};

function OnWhOver() {

    var div = document.getElementById(this.whInfoPnl);

    if (div != null) {

        var mouseX = tempX;
        var mouseY = tempY + getScrollY();

        div.style.position = "absolute";

        // "px" needed for Firefox and Chrome
        div.style.top = mouseY + "px";
        div.style.left = mouseX + 10 + "px";

        div.style.visibility = "visible";
    }
}

function OnWhOut() {

    var div = document.getElementById(this.whInfoPnl);

    if (div != null) {
        div.style.visibility = "hidden";
    }
}

function OnSysOver() {

    var div = document.getElementById(this.sysInfoPnlID);
    if (div != null) {

        var mouseX = tempX;
        var mouseY = tempY + getScrollY();;

        div.style.position = "absolute";

        // "px" needed for Firefox and Chrome
        div.style.top = mouseY + "px";
        div.style.left = mouseX + 10 + "px";

        div.style.visibility = "visible";
    }

    if (this.ellipseIndex != null) {
        // text

        ellipse = objSystems[this.ellipseIndex];
        if (ellipse) {
            ellipse.attr({ "stroke-width": 4 });
        }
    } else {
        // ellipse
        this.attr({ "stroke-width": 4 });
    }

}

function OnSysOut() {

    var div = document.getElementById(this.sysInfoPnlID);

    if (div != null) {
        div.style.visibility = "hidden";
    }

    if (this.ellipseIndex != null) {
        // text
        ellipse = objSystems[this.ellipseIndex];
        if (ellipse) {
            ellipse.attr({ "stroke-width": 2 });
        }
    } else {
        // ellipse
        this.attr({ "stroke-width": 2 });
    }
}

