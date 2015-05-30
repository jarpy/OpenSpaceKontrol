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
            registerUDPListener("FIRST");
        }

        public byte[] GetMessage()
        {
            byte[] poppedMessage = message;
            message = null;
            return poppedMessage;
        }

        private void registerUDPListener(string caller_message)
        {
            System.IO.File.AppendAllText(@"C:\Temp\ksp_osk.log", "  caller : " + caller_message + "\n");

            IPEndPoint remote = new IPEndPoint(IPAddress.Any, 9100);
            UdpClient udp_remote = new UdpClient(remote);

            // initialize the state object passed to the asynch callback
            UdpState udpState = new UdpState();
            udpState.endpoint = remote;
            udpState.udpClient = udp_remote;
            udpState.receiver = this;

            udp_remote.BeginReceive(new AsyncCallback(OnUDPCommand), udpState);
            System.IO.File.AppendAllText(@"C:\Temp\ksp_osk.log", @"Register finished\n");
        }

        static void OnUDPCommand(IAsyncResult res)
        {
            // WriteAllLines creates a file, writes a collection of strings to the file, 
            // and then closes the file.
            UdpState state = (UdpState)res.AsyncState;
            IPEndPoint endPoint = state.endpoint;
            UdpClient udpClient = state.udpClient;
            Receiver receiver = state.receiver;
            byte[] received = udpClient.EndReceive(res, ref endPoint);
            receiver.message = received;
            udpClient.Close();
            receiver.registerUDPListener("NOT FIRST");
        }
    }

    public class UdpState
    {
        public IPEndPoint endpoint;
        public UdpClient udpClient;
        public Receiver receiver;
    }
}
