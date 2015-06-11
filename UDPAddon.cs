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
        public static Transmitter transmitter = new Transmitter();
        public static Receiver reciever = new Receiver();
    }
}
