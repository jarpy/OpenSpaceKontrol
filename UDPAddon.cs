using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace osk
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class UDPAddon : MonoBehaviour
    {
        public static Transmitter transmitter;
        public static Receiver reciever;

        public void Awake()
        {
            print("OSK is Awake().");
            string[] configLines = System.IO.File.ReadAllLines(@"GameData\OSK\osk.ini");
            Dictionary<string, string> config = new Dictionary<string, string>();
            foreach (string line in configLines)
            {
                print("OSK config: " + line);
                string[] keyValuePair = line.Trim('\n').Split('=');
                config.Add(keyValuePair[0], keyValuePair[1]);
            }
            transmitter = new Transmitter(config["remote_ip"], int.Parse(config["transmit_port"]));
            reciever = new Receiver(int.Parse(config["receive_port"]));
        }
    }
}
