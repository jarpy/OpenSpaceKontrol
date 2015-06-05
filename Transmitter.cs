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
        private UdpClient udp = new UdpClient();
        private IPEndPoint recipient = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9000);

        private const string OSCFloat = "f";
        private const string OSCString = "s";

        // initialize telemetry
        public Transmitter()
        {
            udp.Connect(recipient);
        }

        public void transmit(string key, string value)
        {
            SendOSCValue(OSCString, key, System.Text.Encoding.UTF8.GetBytes(value));
        }
        
        public void transmit(string key, double value)
        {
            // KSP uses doubles, but OSC only supports 32-bit floats, so this
            // method handles doubles with a simple cast to float.
            transmit(key, (float)value);
        }

        public void transmit(string key, float value)
        {
            byte[] valueBytes = BitConverter.GetBytes(value);
            // Tricky bit: Kerbal is a native x86 application, so it gives us little-endian
            // floating-point values, but OSC demands big-endian ("network") byte order. So
            // we have to:
            Array.Reverse(valueBytes);
            // before we:
            SendOSCValue(OSCFloat, key, valueBytes);
        }

        private string ConstructAddressPattern(string key)
        {
            string AddressPattern = "/OSK/" + key + "\0";
            // Pad the address pattern to a multiple of 4 bytes (OSC string specification).
            while (AddressPattern.Length % 4 != 0) { AddressPattern += "\0"; }
            return AddressPattern;
        }

        private string ConstructTypeTag(string type)
        {
            return "," + type + "\0\0";
        }

        private void SendOSCValue(string type, string name, byte[] value)
        {
            string addressPattern = ConstructAddressPattern(name);
            string typeTag = ConstructTypeTag(type);

            // Convert what we have to raw bytes.
            byte[] header = System.Text.Encoding.ASCII.GetBytes(addressPattern + typeTag);

            //// Combine the the complete OSC message into a new byte stream.
            // First, we have make an empty byte array that is exactly the right size.
            // This is a C-style thing. If you're used to Ruby/Python, this might
            // seem a bit fiddly.
            int payloadLength = header.Length + value.Length;
            var payload = new byte[payloadLength];

            // Then we copy the two parts into their correct locations in the payload.
            header.CopyTo(payload, 0);
            value.CopyTo(payload, header.Length);

            // We're done. Put it on the wire.
            udp.Send(payload, payloadLength);
        }
    }
}
