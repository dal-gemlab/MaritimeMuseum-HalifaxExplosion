import * as UnityLoader from './Build/UnityLoader.js';

class WebSocketHandler {
    constructor(addr: string) {
        const ws = new WebSocket(addr);

        ws.onopen = this.onWsOpened;
        ws.onmessage = this.onMessage;
        ws.onclose = this.onConnectionClosed;
    }

    onWsOpened() {
        
    }

    onMessage(event) {
        const msg = event.data;
        const json = JSON.parse(msg);
        const unity = new UnityMessages(json);
    }

    onConnectionClosed() {
        alert("Connection closed");
    }
}

class UnityMessages {
    isBuildingEnlarged: boolean;
    buildingName: string;
    click: boolean;

    constructor(json) {
        //const json = JSON.parse(receivedBuildingName);
        
        this.click = json["click"];
        if (this.click) {
            this.isBuildingEnlarged = json["isBuildingEnlarged"];
            this.buildingName = json["clickedName"];
            if(!this.isBuildingEnlarged)
                this.loadDescriptionFile();
            else
                this.clearPage();
        }

    }

    loadDescriptionFile() {
        const xobj = new XMLHttpRequest();
        xobj.overrideMimeType("application/json");
        xobj.onreadystatechange = () => {
            if (xobj.readyState == 4 && xobj.status == 200) {
                const building = new BuildingDescriptorFile(JSON.parse(xobj.responseText));
                building.updatePage();
            }
        };
        
        xobj.open('GET', `json/${this.buildingName}.json`, true);
        xobj.send();
    }

    clearPage()
    {
        document.getElementById("titleHeading").innerHTML = "";
        document.getElementById("descriptionHeading").innerHTML = "";
        document.getElementById("buildingImage").setAttribute("src","");
    }
}

class BuildingDescriptorFile {

    title: string;
    description: string;
    imagePath: string;

    constructor(data: JSON) {
        this.title = data["title"];
        this.description = data["description"];
        this.imagePath = data["imagePath"];
    }

    updatePage() {
        document.getElementById("titleHeading").innerHTML = this.title;
        document.getElementById("descriptionHeading").innerHTML = this.description;
        document.getElementById("buildingImage").setAttribute("src",this.imagePath);
    }
}

class UnityManager
{
    constructor() {
        UnityLoader.instantiate("gameContainer", "Build/delpoy.json");
    }

}

let wsHandler = new WebSocketHandler("ws://localhost:8888/ws");
let unity = new UnityManager();



