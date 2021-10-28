# Ambient Lighting
This is a small console application built on .NET Framework 4 that interfaces with an Arduino board and RGB lighting strip.  

The goal was to create a small application that would take the average color of the monitor and light the RGB strip.

Ultimately, this is a .NET version of the code found below.  I did this so I could setup a GUI to turn it on/off.  The appliation also does its best
to detect screen lock to turn on/off, if you want.

For the most part, buying accessories and getting a motherboard with RGB inputs deprecated this application.

# Arduino code and board setup
I followed the [instructions found from this post](https://forum.arduino.cc/t/arduino-based-ambilight-for-you-computer/51963) to setup the board.  
