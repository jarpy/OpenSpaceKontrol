using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace osk
{
    class Transmitter
    {
        private IPEndPoint listen_socket = new IPEndPoint(IPAddress.Parse("10.31.225.105"), 9000);
        private UdpClient udp = new UdpClient();

        // initialize telemetry
        public Transmitter()
        {
            udp.Connect(listen_socket);
        }

        public void transmit(string key, double value)
        {
            // KSP uses doubles, but OSC only supports 32-bit floats.
            float floatValue = (float)value;

            // Contruct the OSC message. First the Address Pattern:
            string addressPattern = "/OSK/" + key + "\0";
            // Pad the address pattern to a multiple of 4 bytes (OSC string specification).
            while (addressPattern.Length % 4 != 0) { addressPattern += "\0"; }

            // Now the OSC Type Tag for a single float value.
            string floatTypeTag = ",f\0\0"; // again, padded to 4 byte boundry.

            // Convert what we have to byte streams.
            byte[] oscHeaderBytes = System.Text.Encoding.ASCII.GetBytes(addressPattern + floatTypeTag);
            byte[] oscArgumentBytes = BitConverter.GetBytes(floatValue);
            // Tricky bit: Kerbal is a native x86 application, so it gives us little-endian
            // floating-point values, but OSC demands big-endian ("network") byte order. So...
            Array.Reverse(oscArgumentBytes);

            // Put it all together in a new byte stream, ready to go on the wire.
            var payload = new byte[oscHeaderBytes.Length + oscArgumentBytes.Length];
            oscHeaderBytes.CopyTo(payload, 0);
            oscArgumentBytes.CopyTo(payload, oscHeaderBytes.Length);

            // Fire!
            udp.Send(payload, payload.Length);
        }
    }
}
