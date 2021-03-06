﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Rug.Osc;

namespace StudyControlApp.Model
{
    internal class OscController
    {
        public Semaphore ReceiverSemaphore { get; }
        public bool OscConnected { get; private set; }

        public delegate void DataReceived(OscPacket data);
        public event DataReceived OnDataReceived;

        private readonly OscReceiver receiver;
        private const int ReceiverPort = 9090;
        private const string ReceiverPath = "/logger";

        private readonly OscSender sender;
        public readonly int SenderPort = 9091;
        private const string SenderPath = "/control";

        private readonly Thread receiverThread;
        private OscPacket data;

        public string HoloLensAddr { get; }

        public OscController(string holoLensAddr)
        {
            HoloLensAddr = holoLensAddr;
            sender = new OscSender(IPAddress.Parse(HoloLensAddr),SenderPort);
            sender.Connect();
            receiver = new OscReceiver(ReceiverPort);
            receiverThread = new Thread(ListenLoop);
            ReceiverSemaphore = new Semaphore(1,1);

        }

        public void StartReceiving()
        {
            try
            {
                receiver.Connect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            receiverThread.Start();
            //Task.Factory.StartNew(ListenLoop);
        }

        public void StopReceiving()
        {
            receiver.Close();
            receiverThread.Join();
        }

        public void SendCommand(string command)
        {
            Task.Factory.StartNew(() => sender.Send(new OscMessage(SenderPath, command)));
        }

        private void ListenLoop()
        {

            try
            {
                while (receiver.State != OscSocketState.Closed)
                {
                    if (receiver.State == OscSocketState.Connected)
                    {
                        //ReceiverSemaphore.WaitOne();
                        data = receiver.Receive();
                        if (((OscMessage) data).Address == ReceiverPath)
                            OnDataReceived?.Invoke(data);
                        //ReceiverSemaphore.Release();
                    }
                }
            }
            catch (Exception ex)
            {
                if (receiver.State == OscSocketState.Connected)
                {
                    Console.WriteLine("Exception in listen loop");
                    Console.WriteLine(ex.Message);

                    throw;
                }
                if (receiver.State == OscSocketState.Closed)
                {
                    //TODO: Please fix me....
                    ;//VERY nasty hack to quick fix closing the sockect on application shutdown

                }
                else
                {
                    throw;
                }
                
            }
        }

    }
}
