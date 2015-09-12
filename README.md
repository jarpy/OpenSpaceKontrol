![OSK on iPad](https://dl.dropboxusercontent.com/u/1326615/Linked%20Images/osk-ipad.png)

Open Space Kontrol provides telemetry and remote control to Kerbal Space Program vessels via [Open Sound Control](http://opensoundcontrol.org/introduction-osc).

OSC is a natural fit for providing low-latency control inputs and telemetry data.

Open Space Kontrol will enable anything that can speak OSC (over UDP) to operate as a KSP controller. In particular, OSK will target [TouchOSC](http://hexler.net/software/touchosc) as a network-attached, bi-directional control surface. However, OSC is _by definition_ an open protocol, so there's nothing stopping you from controlling KSP with your favorite language, microcontroller, synthesizer or midi control surface (a [Launchpad](http://global.novationmusic.com/launch) perhaps? :sunglasses:).

In its current state, only a small sample of telemetry numbers are transmitted, and a few vessel controls are supported, including throttle and stage triggering (which are quite fun to control from a phone/tablet).

OSK is in early development. However, it is already possible to recieve basic telemetry, trigger staging, and control throttle from a network-attached OSC controller, such as TouchOSC. Adding new telemtry elements and controls is relatively easy now. Requests and suggestions are most welcome!
