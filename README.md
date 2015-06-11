Open Space Kontrol provides telemetry and remote control to Kerbal Space Program vessels via [Open Sound Control](http://opensoundcontrol.org/introduction-osc).

OSC is a natural fit for providing low-latency control inputs and telemetry data.

Open Space Kontrol will enable anything that can speak OSC (over UDP) to operate as a KSP controller. In particular, OSK will target [TouchOSC](http://hexler.net/software/touchosc) as a network-attached, bi-directional control surface. However, OSC is _by definition_ an open protocol, so there's nothing stopping you from controlling KSP with your favorite language, microcontroller, synthesizer or midi control surface (a [Launchpad](http://global.novationmusic.com/launch) perhaps? :sunglasses:).

In its current state, only a small sample of telemetry numbers are transmitted, and very few vessel controls are supported, including "ActivateNextStage" (which is quite fun to trigger from a phone/tablet).

OSK is in early development. However, it is already possible to recieve some telemetry and to trigger staging from a network-attached OSC controller, such as TouchOSC.

Pull requests are most welcome!
