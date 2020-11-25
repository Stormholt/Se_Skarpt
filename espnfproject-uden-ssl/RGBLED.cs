using System;
using Windows.Devices.Gpio;
using System.Diagnostics;

using SeSkarptEnumClasses;

namespace RGBled
{
    class RGBLED
    {
        public GpioPin R { get; private set; }
        public GpioPin G { get; private set; }
        public GpioPin B { get; private set; }

        public GpioPinDriveMode mode { get; private set; }

        public Ledcolor ledcolor { get; private set; }

        public RGBLED(GpioPin red, GpioPin green, GpioPin blue)
        {
            R = red;
            G = green;
            B = blue;
            ledcolor = Ledcolor.Off;
            mode = GpioPinDriveMode.Output;
            R.SetDriveMode(mode);
            G.SetDriveMode(mode);
            B.SetDriveMode(mode);
            controlLED(ledcolor);

        }

        public void controlLED(Ledcolor color)
        {
            if (Ledcolor.Red.Equals(color))
            {
                this.ledcolor = Ledcolor.Red;
                this.R.Write(GpioPinValue.High);
                this.G.Write(GpioPinValue.Low);
                this.B.Write(GpioPinValue.Low);
            }
            else if (Ledcolor.Green.Equals(color))
            {
                this.ledcolor = Ledcolor.Green;
                this.R.Write(GpioPinValue.Low);
                this.G.Write(GpioPinValue.High);
                this.B.Write(GpioPinValue.Low);
            }
            else if (Ledcolor.Blue.Equals(color))
            {
                this.ledcolor = Ledcolor.Blue;
                this.R.Write(GpioPinValue.Low);
                this.G.Write(GpioPinValue.Low);
                this.B.Write(GpioPinValue.High);
            }
            else if (Ledcolor.Magenta.Equals(color))
            {
                this.ledcolor = Ledcolor.Magenta;
                this.R.Write(GpioPinValue.High);
                this.G.Write(GpioPinValue.Low);
                this.B.Write(GpioPinValue.High);

                Debug.WriteLine(ledcolor.Name);
            }
            else if (Ledcolor.Cyan.Equals(color))
            {
                this.ledcolor = Ledcolor.Cyan;
                this.R.Write(GpioPinValue.Low);
                this.G.Write(GpioPinValue.High);
                this.B.Write(GpioPinValue.High);
            }
            else if (Ledcolor.Yellow.Equals(color))
            {
                this.ledcolor = Ledcolor.Yellow;
                this.R.Write(GpioPinValue.High);
                this.G.Write(GpioPinValue.High);
                this.B.Write(GpioPinValue.Low);
            }
            else if (Ledcolor.White.Equals(color))
            {
                this.ledcolor = Ledcolor.White;
                this.R.Write(GpioPinValue.High);
                this.G.Write(GpioPinValue.High);
                this.B.Write(GpioPinValue.High);
            }
            else // No other option
            {
                this.ledcolor = Ledcolor.Off;
                this.R.Write(GpioPinValue.Low);
                this.G.Write(GpioPinValue.Low);
                this.B.Write(GpioPinValue.Low);
            }

        }
    }
}
