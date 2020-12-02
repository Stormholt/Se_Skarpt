using System;
using Windows.Devices.Adc;

namespace espSensors
{
    class Photoresistor 
    {
        const int ADC_RESOLUTION = 4095; // ESP32 has 12 bit adc resolution 2^12 = 4096
        const int VIN = 5; // 5V 
        const int R = 10000; // resisitor at 10k ohm

        public AdcChannel adcChannel { get; private set; }
        public int measurement { get; private set; }
        public Photoresistor()
        {
            this.measurement = 0;
            this.adcChannel = null;
        }
        public Photoresistor(AdcChannel adcChannel)
        {
            this.adcChannel = adcChannel;
            this.measurement = 0; 
        }

        private void Read()
        {
            this.measurement = this.adcChannel.ReadValue();
        }

        public bool HasLightChanged()
        {
            int newMeasurement = this.adcChannel.ReadValue();

            if (this.measurement == newMeasurement)
            {
                this.measurement = newMeasurement;
                return true;
            }
            else { return false; }
        }

        public int CalculateLumen()
            {
                 this.Read();
                // Conversion rule
                float Vout = (float)measurement * (VIN / (float)ADC_RESOLUTION);// Conversion analog to voltage
                float RLDR = (R * (VIN - Vout)) / Vout; // Conversion voltage to resistance
                int phys = (int)(500 / (RLDR / 1000)); // Conversion resitance to lumen
                return phys;
            }
    }
    

    class LM35
    {
        const double conversion = 8.048;
        AdcChannel adcChannel;
        int measurement;
        public LM35()
        {
            this.measurement = 0;
            this.adcChannel = null;
        }
        public LM35(AdcChannel adcChannel)
        {
            this.adcChannel = adcChannel;
            this.measurement = 0;
        }

        private void Read()
        {
            this.measurement = this.adcChannel.ReadValue();
        }

        public bool HasTempChanged()
        {
            int newMeasurement = this.adcChannel.ReadValue();

            if (this.measurement != newMeasurement)
            {
                this.measurement = newMeasurement;
                return true;
            }
            else { return false; }
        }

        public int CalculateCelsius()
        {
            this.Read();
            return (int)((double)measurement / conversion) ; // conversion to celsius based on adc resolution
        }
    }
}
