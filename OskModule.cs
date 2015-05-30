using UnityEngine;
using System;
using System.Linq;

namespace osk
{
    public class OskModule : PartModule
    {
        private Transmitter transmitter;
        private Int64 lastTransmitTime;
        private const int ticksPerMillisecond = 10000;

        private Receiver receiver;
        public bool message_received;

        public override void OnStart(StartState state)
        {
            System.IO.File.WriteAllText(@"C:\Temp\ksp_osk.log", @"On Start Called\n");
            transmitter = new Transmitter();
            receiver = new Receiver();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            // Send telemetry to the transmitter.
            // Cap the transmission rate at 10 updates per second.
            Int64 now = DateTime.Now.Ticks;
            if (now > (lastTransmitTime + (ticksPerMillisecond * 100)))
            {
                transmitter.transmit("altitude", vessel.altitude);
                transmitter.transmit("vertical_speed", vessel.verticalSpeed);
                transmitter.transmit("crew_count", (double)vessel.GetCrewCount());
                lastTransmitTime = DateTime.Now.Ticks;

            }

            // Handle commands from the receiver.
            byte[] messageBytes = receiver.GetMessage();
            if (messageBytes != null)
            {
                ProcessCommand(messageBytes);
            }
        }

        private void ProcessCommand(byte[] oscMessage)
        {
            string command = System.Text.Encoding.ASCII.GetString(oscMessage);
            if (command.StartsWith("/OSK/stage\0"))
            {
                Staging.ActivateNextStage();
            }
            else if (command.StartsWith("/OSK/murder_crew\0"))
            {
                vessel.MurderCrew();
            }
            else
            {
                System.IO.File.AppendAllText(@"C:\Temp\ksp_osk.log", "Invalid command pattern.\n");
            }
        }
    }
}
