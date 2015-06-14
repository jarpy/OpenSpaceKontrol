using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace osk
{
    public class OscMessage
    {
        public string addressPattern;
        public string typeTag;
        public float argument;
    }

    public class Receiver
    {
        private bool messageReceived = false;
        private byte[] message;
        private int port;

        public Receiver(int port)
        {
            this.port = port;
            registerUDPListener(this.port);
        }

        public byte[] GetMessageBytes()
        {
            byte[] poppedMessage = message;
            message = null;
            return poppedMessage;
        }

        public OscMessage GetMessage()
        {
            OscMessage result = new OscMessage();
            byte[] received = this.message;
            this.message = null;
            if(received == null)
            {
                return null;
            }
            else
            {
                int startHere = 0;

                // Cut out the OSC Address Pattern
                int endHere = Array.IndexOf(received, (byte)0x00);
                var addressPatternBytes = new byte[endHere];
                Array.Copy(received, startHere, addressPatternBytes, 0, endHere);
                result.addressPattern = System.Text.Encoding.ASCII.GetString(addressPatternBytes);

                // Continue along the recieved bytes, and cut out the OSC Type Tag
                startHere = endHere;
                while (received[startHere] == (byte)0x00 || received[startHere] == (byte)',')
                {
                    startHere++;
                }
                endHere = startHere;
                while (received[endHere] != (byte)0x00)
                {
                    endHere++;
                }
                var typeTag = new byte[endHere - startHere];
                Array.Copy(received, startHere, typeTag, 0, endHere - startHere);
                result.typeTag = System.Text.Encoding.ASCII.GetString(typeTag);

                // Finally the (FIXME: assumed float) OSC Argument
                if(result.typeTag == "f")
                {
                    startHere = endHere + 2;
                    var argument = new byte[4];
                    Array.Copy(received, startHere, argument, 0, 4);
                    // Big-endian -> Little-endian.
                    Array.Reverse(argument);
                    result.argument = System.BitConverter.ToSingle(argument, 0);
                }

                return result;
            }
        }

        private void registerUDPListener(int port)
        {
            IPEndPoint listenSocket = new IPEndPoint(IPAddress.Any, port);
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
            receiver.registerUDPListener(receiver.port);
        }
    }

    public class UdpState
    {
        public IPEndPoint listenSocket;
        public UdpClient udpListener;
        public Receiver oskReceiver;
        public int port;
    }
}
