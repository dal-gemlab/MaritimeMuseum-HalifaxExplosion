#!/usr/bin/env python
# -*- coding: utf-8 -*-

import logging
import tornado.web
import tornado.websocket
import tornado.ioloop
import tornado.options
import os.path
import json

from tornado.options import define, options

define("port", default=8888, help="run on the given port", type=int)



class Application(tornado.web.Application):
    def __init__(self):
        handlers = [(r"/ws", MainHandler)]

        
        settings = dict(
            debug=True,
            #static_path=os.path.join(os.path.dirname(__file__), "static")
        )
        tornado.web.Application.__init__(self, handlers, **settings)


class MainHandler(tornado.websocket.WebSocketHandler):
    connectedClients = []

    def check_origin(self, origin):
        return True

    def open(self):
        MainHandler.connectedClients.append(self)
        logging.info("A client connected.")

    def on_close(self):
        MainHandler.connectedClients.remove(self)
        logging.info("A client disconnected")

    def on_message(self, message):
        for client in MainHandler.connectedClients:
            client.write_message(message)
        #logging.info("message: {}".format(message))

def main():
    tornado.options.parse_command_line()
    app = Application()
    app.listen(options.port)
    tornado.ioloop.IOLoop.instance().start()


if __name__ == "__main__":
    main()