using System;
using Windows.Devices.Adc;

namespace espSensors
{
    class Photoresistor 
    {
        const int ADC_RESOLUTION = 4095; // ESP32 has 12 bit adc resolution 2^12 = 4096
        const double VIN = 3.3; // 5V 
        const int R = 10000; // resisitor at 10k ohm

        public AdcChannel adcChannel { get; private set; }
        public int measurement { get; private set; }
        public Photoresistor()
        {
            measurement = 0;
            adcChannel = null;
        }
        public Photoresistor(AdcChannel adcChannel)
        {
           this.adcChannel = adcChannel;
           measurement = 0; 
        }

        private void Read()
        {
           measurement = adcChannel.ReadValue();
        }

        public bool HasLightChanged()
        {
            int newMeasurement = this.adcChannel.ReadValue();

            if (measurement == newMeasurement)
            {
                measurement = newMeasurement;
                return true;
            }
            else { return false; }
        }

        public int CalculateLumen()
            {
                Read();
                // Conversion rule
                float Vout = (float)measurement * ((float)VIN / (float)ADC_RESOLUTION);// Conversion analog to voltage
                float RLDR = (R * ((float)VIN - Vout)) / Vout; // Conversion voltage to resistance
                int phys = (int)(500 / (RLDR / 1000)); // Conversion resitance to lumen
                return phys;
            }
    }
    

    class LM35
    {
        const double conversion = 5.048;
        AdcChannel adcChannel;
        int measurement;
        public LM35()
        {
            measurement = 0;
            adcChannel = null;
        }
        public LM35(AdcChannel adcChannel)
        {
            this.adcChannel = adcChannel;
            measurement = 0;
        }

        private void Read()
        {
            measurement = adcChannel.ReadValue();
        }

        public bool HasTempChanged()
        {
            int newMeasurement = adcChannel.ReadValue();

            if (measurement != newMeasurement)
            {
                measurement = newMeasurement;
                return true;
            }
            else { return false; }
        }

        public int CalculateCelsius()
        {
            Read();
            return (int)((double)measurement / conversion) ; // conversion to celsius based on adc resolution
        }
    }
}
