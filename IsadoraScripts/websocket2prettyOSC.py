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
hasSentGazeNothing = False;

def on_meassage(ws, message):
    global currentExpanded
    global hasSentGazeNothing
    parsedMessage = json.loads(message)
    if(not parsedMessage["click"] and currentExpanded != parsedMessage["gazedBuilding"] and not parsedMessage["isAnchorUpdate"]):
        if(parsedMessage["gazedBuilding"] != "" ):
            print(parsedMessage["gazedBuilding"])
            sendOSC2Isadora(parsedMessage["gazedBuilding"], False)
            hasSentGazeNothing = False
        else:
            if(not hasSentGazeNothing):
                sendOSC2Isadora("none", False)
                hasSentGazeNothing = True
        
    if(parsedMessage["click"]):
        if(not parsedMessage["isBuildingEnlarged"]):
            sendOSC2Isadora(parsedMessage["gazedBuilding"], True)
            print(parsedMessage["gazedBuilding"])
            currentExpanded = parsedMessage["gazedBuilding"]
        else:
            sendOSC2Isadora("reduce", True)
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
    ws = websocket.WebSocketApp("ws://localhost:8888/ws",
                                on_message=on_meassage,
                                on_error=on_error,
                                on_close=on_close)
    ws.run_forever()
