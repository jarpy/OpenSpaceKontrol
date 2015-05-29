using UnityEngine;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace OSK
{
    public class OskModule : PartModule
    {
        private UdpClient client = new UdpClient();
        private IPEndPoint server = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9599);
        private Int64 lastTransmitTime;
        private const int ticksPerMillisecond = 10000;

        public override void OnStart(StartState state)
        {
            lastTransmitTime = 0;
            client.Connect(server);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            // Cap the transmission rate at 10 updates per second.
            Int64 now = DateTime.Now.Ticks;
            if (now > (lastTransmitTime + (ticksPerMillisecond * 100)))
            {
                transmit("altitude", vessel.altitude);
                transmit("vertical_speed", vessel.verticalSpeed);
                lastTransmitTime = DateTime.Now.Ticks;
            }
        }

        private void transmit(string key, double value)
        {
            // KSP uses doubles, but OSC only supports 32-bit floats.
            float floatValue = (float)value;

            // Contruct the OSC message. First the Address Pattern:
            string addressPattern = "/osk/" + key + "\0";
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
            client.Send(payload, payload.Length);
        }
    }
}
