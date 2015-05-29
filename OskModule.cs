using UnityEngine;
using System.Net;
using System.Net.Sockets;

namespace OSK
{
    public class OskModule : PartModule
    {
        private UdpClient client = new UdpClient();
        private IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9599);

        public override void OnStart(StartState state)
        {
            client.Connect(ep);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            transmit("altitude", vessel.altitude.ToString());
        }

        private void transmit(string key, string value)
        {
            send_string(key + "=" + value);
        }

        private void send_string(string s)
        {
            s = s + "\n";
            byte[] payload = System.Text.Encoding.ASCII.GetBytes(s);
            client.Send(payload, payload.Length);

        }
    }
}
