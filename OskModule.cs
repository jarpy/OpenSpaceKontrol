using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;

namespace OSK
{
    public class OskModule : PartModule
    {
        private UdpClient client = new UdpClient();
        private IPEndPoint server = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9599);
        private Int64 last_transmit_time;
        private const int ticks_per_millisecond = 10000;

        public override void OnStart(StartState state)
        {
            last_transmit_time = 0;
            client.Connect(server);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            // Cap the transmission rate at 10 updates per second.
            Int64 now = DateTime.Now.Ticks;
            if (now > (last_transmit_time + (ticks_per_millisecond * 100)))
            {
                transmit("altitude", vessel.altitude.ToString());
                transmit("vertical_speed", vessel.verticalSpeed.ToString());
                last_transmit_time = DateTime.Now.Ticks;
            }
        }

        private void transmit(string key, string value)
        {
            string message = key + "=" + value + "\n";
            byte[] network_payload = System.Text.Encoding.ASCII.GetBytes(message);
            client.Send(network_payload, network_payload.Length);
        }
    }
}
