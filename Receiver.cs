using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace osk
{
    public class Receiver
    {
        private bool messageReceived = false;
        private byte[] message;

        public Receiver()
        {
            registerUDPListener();
        }

        public byte[] GetMessage()
        {
            byte[] poppedMessage = message;
            message = null;
            return poppedMessage;
        }

        private void registerUDPListener()
        {
            IPEndPoint listenSocket = new IPEndPoint(IPAddress.Any, 9100);
            UdpClient udpListener = new UdpClient(listenSocket);

            // We'll be registering a callback function to be called when we get
            // a message. That function will need some context to understand
            // what is going on.
            //
            // We'll make an object, and stitch the appropriate information onto it.
            UdpState udpState = new UdpState();
            udpState.listenSocket = listenSocket;
            udpState.udpListener = udpListener;
            udpState.oskReceiver = this;

            // Start listening, and set up the callback function (with the state object).
            udpListener.BeginReceive(new AsyncCallback(OnUDPCommand), udpState);
        }

        static void OnUDPCommand(IAsyncResult res)
        {
            // This is the callback. If we get a UDP message, we'll come here.
            // The state object we prepared earlier will be available, let's grab it:
            UdpState state = (UdpState)res.AsyncState;
            // ...and pull the bits we need off it:
            IPEndPoint listenSocket = state.listenSocket;
            UdpClient udpClient = state.udpListener;
            Receiver receiver = state.oskReceiver;

            // Now we can get the actual message.
            byte[] received = udpClient.EndReceive(res, ref listenSocket);

            // Store the message on the OSK Receiver object.
            // It's like an inbox. Any time after this, our "customers" can call
            // GetMessage() to see whatever we received.
            receiver.message = received;

            // This completes a cycle.
            // Clean up and re-register the callback to catch the next message.
            udpClient.Close();
            receiver.registerUDPListener();
        }
    }

    public class UdpState
    {
        public IPEndPoint listenSocket;
        public UdpClient udpListener;
        public Receiver oskReceiver;
    }
}
