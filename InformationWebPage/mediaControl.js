"use strict";
exports.__esModule = true;
//var UnityLoader = require("./Build/UnityLoader.js");
var WebSocketHandler = /** @class */ (function () {
    function WebSocketHandler(addr) {
        var ws = new WebSocket(addr);
        ws.onopen = this.onWsOpened;
        ws.onmessage = this.onMessage;
        ws.onclose = this.onConnectionClosed;
    }
    WebSocketHandler.prototype.onWsOpened = function () {
    };
    WebSocketHandler.prototype.onMessage = function (event) {
        var msg = event.data;
        var json = JSON.parse(msg);
        var unity = new UnityMessages(json);
    };
    WebSocketHandler.prototype.onConnectionClosed = function () {
        alert("Connection closed");
    };
    return WebSocketHandler;
}());
var UnityMessages = /** @class */ (function () {
    function UnityMessages(json) {
        //const json = JSON.parse(receivedBuildingName);
        this.click = json["click"];
        if (this.click) {
            this.isBuildingEnlarged = json["isBuildingEnlarged"];
            this.buildingName = json["clickedName"];
            if (!this.isBuildingEnlarged)
                this.loadDescriptionFile();
        }
    }
    UnityMessages.prototype.loadDescriptionFile = function () {
        var xobj = new XMLHttpRequest();
        xobj.overrideMimeType("application/json");
        xobj.onreadystatechange = function () {
            if (xobj.readyState == 4 && xobj.status == 200) {
                var building = new BuildingDescriptorFile(JSON.parse(xobj.responseText));
                building.updatePage();
            }
        };
        xobj.open('GET', "json/" + this.buildingName + ".json", true);
        xobj.send();
    };
    return UnityMessages;
}());
var BuildingDescriptorFile = /** @class */ (function () {
    function BuildingDescriptorFile(data) {
        this.title = data["title"];
        this.description = data["description"];
        this.imagePath = data["imagePath"];
    }
    BuildingDescriptorFile.prototype.updatePage = function () {
        document.getElementById("titleHeading").innerHTML = this.title;
        document.getElementById("descriptionHeading").innerHTML = this.description;
        document.getElementById("buildingImage").setAttribute("src", this.imagePath);
    };
    return BuildingDescriptorFile;
}());
var UnityManager = /** @class */ (function () {
    function UnityManager() {
        UnityLoader.instantiate("gameContainer", "Build/delpoy.json");
    }
    return UnityManager;
}());
var wsHandler = new WebSocketHandler("ws://localhost:8888/ws");
var unity = new UnityManager();
