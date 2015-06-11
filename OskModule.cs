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
            base.OnStart(state);
            transmitter = UDPAddon.transmitter;
            receiver = UDPAddon.reciever;
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
                transmitter.transmit("atmospheric_density", vessel.atmDensity);
                transmitter.transmit("crew_count", vessel.GetCrewCount());
                transmitter.transmit("height_from_terrain", vessel.GetHeightFromTerrain());
                transmitter.transmit("total_mass", vessel.GetTotalMass());
                transmitter.transmit("vertical_speed", vessel.verticalSpeed);
                // transmitter.transmit("name", vessel.GetName());

                lastTransmitTime = DateTime.Now.Ticks;
            }

            // Handle commands from the receiver.
            OscMessage command = receiver.GetMessage();
            if (command != null)
            {
                ProcessCommand(command);
            }
        }

        private void ProcessCommand(OscMessage command)
        {
            print(command.addressPattern);
            print(command.typeTag);
            if (command.addressPattern == "/OSK/stage")
            {
                Staging.ActivateNextStage();
            }
            else if (command.addressPattern == "/OSK/murder_crew")
            {
                vessel.MurderCrew();
            }
            else if (command.addressPattern == "/OSK/throttle")
            {
                FlightInputHandler.state.mainThrottle = command.argument;
            }
            else
            {
                System.IO.File.AppendAllText(@"C:\Temp\ksp_osk.log", "Invalid command pattern.\n");
            }
        }
    }
}
