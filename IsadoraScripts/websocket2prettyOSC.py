import json
import websocket
from pythonosc import osc_message_builder
from pythonosc import udp_client

try:
    import thread
except ImportError:
    import _thread as thread
import time


currentExpanded = "none"
lastStarred = "none"
hasSentGazeNothing = False;

def on_meassage(ws, message):
    global currentExpanded
    global hasSentGazeNothing
    global lastStarred
    parsedMessage = json.loads(message)
    if(not parsedMessage["isAnchorUpdate"] and parsedMessage["gazedBuilding"] != ""):
        lastStarred = parsedMessage["gazedBuilding"]
        #print(parsedMessage["gazedBuilding"])

    if(not parsedMessage["click"] and currentExpanded != parsedMessage["gazedBuilding"] and not parsedMessage["isAnchorUpdate"]):
        if(parsedMessage["gazedBuilding"] != "" ):
            #print(parsedMessage["gazedBuilding"])
            sendOSC2Isadora(parsedMessage["gazedBuilding"], 0)
            hasSentGazeNothing = False
        else:
            if(not hasSentGazeNothing):
                sendOSC2Isadora("none", 0)
                hasSentGazeNothing = True
        
    if(parsedMessage["click"]):
        print("click!!!")
        if(not parsedMessage["isBuildingEnlarged"]):
            sendOSC2Isadora(lastStarred, 1)
            print(lastStarred)
            currentExpanded = lastStarred
        else:
            sendOSC2Isadora("reduce", 1)
            print("reduce")
            currentExpanded = "none"


def on_error(ws, error):
    print(error)


def on_close(ws):
    print("WS Closed")


def sendOSC2Isadora(buildingName, click):
    global OSCClient
    msg = osc_message_builder.OscMessageBuilder(address="/isadora")
    oscClient.send_message("/isadora/name", buildingName)
    oscClient.send_message("/isadora/click", click)


if __name__ == "__main__":
    oscClient = udp_client.SimpleUDPClient("localhost", 9054)

    websocket.enableTrace(True)
    ws = websocket.WebSocketApp("ws://192.168.1.6:8888/ws",
                                on_message=on_meassage,
                                on_error=on_error,
                                on_close=on_close)
    ws.run_forever()
