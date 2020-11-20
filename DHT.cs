using System;
using Windows.Devices.Gpio;
using nanoFramework.Hardware.Esp32;
using System.Threading.Tasks;

#define MIN_INTERVAL 2000 
#define TIMEOUT UINT_MAX
public class DHT
{
	public:
	DHT(GpioPin pin, byte type, byte count = 6);
	void begin(byte usec = 55);
	float readTemperature(bool S = false, bool force = false);
	float convertCtosF(float);
	float convertFtoC(float);
	float computeHeatIndex(bool isFahrenheit = true);
	float computeHeatIndex(float temperature, float percentHumidity,
						   bool isFahrenheit = true);
	float readHumidity(bool force = false);
	bool read(bool force = false);

	private:
	byte data[5];
	GpioPin _pin, 
	byte _type;
	uint _lastreadtime, _maxcycles;
	bool _lastresult;
	byte pullTime; // Time (in usec) to pull up data line before reading

	uint expectPulse(bool level);

	DHT(GpioPin pin, byte type, byte count) {
		
		(void) count; // Workaround to avoid compiler warning.
		_pin = pin;
		_type = type;

		_maxcycles = 1000/250; // 1 millisecond timeout for
									   // reading pulses from DHT sensor.
									   // Note that count is now ignored as the DHT reading algorithm adjusts itself
									   // based on the speed of the processor.
	}

	void begin(uint8_t usec)
	{
		// set up the pins!
		_pin.SetDriveMode(GpioPinDriveMode.InputPullUp);
		// Using this value makes sure that millis() - lastreadtime will be
		// >= MIN_INTERVAL right away. Note that this assignment wraps around,
		// but so will the subtraction.
		_lastreadtime = GetCurrent - MIN_INTERVAL;
		//DEBUG_PRINT("DHT max clock cycles: ");
		DEBUG_PRINTLN(_maxcycles, DEC);
		pullTime = usec;
	}

	float readTemperature(bool S, bool force)
	{
		float f = NAN;

		if (read(force))
		{
					f = ((word)(data[2] & 0x7F)) << 8 | data[3];
					f *= 0.1;
					if (data[2] & 0x80)
					{
						f *= -1;
					}
					if (S)
					{
						f = convertCtoF(f);
					}
					break;
			}
		}
		return f;
	}

bool DHT::read(bool force)
{
    HighResTimer highres;
    // Check if sensor was read less than two seconds ago and return early
    // to use last reading.
    uint currenttime = highres.GetCurrent;
    if (!force && ((currenttime - _lastreadtime) < MIN_INTERVAL))
    {
        return _lastresult; // return last correct measurement
    }
    _lastreadtime = currenttime;

    // Reset 40 bits of received data to zero.
    data[0] = data[1] = data[2] = data[3] = data[4] = 0;

    // Send start signal.  See DHT datasheet for full signal diagram:
    //   http://www.adafruit.com/datasheets/Digital%20humidity%20and%20temperature%20sensor%20AM2302.pdf

    // Go into high impedence state to let pull-up raise data line level and
    // start the reading process.
    _pin.SetDriveMode(GpioPinDriveMode.InputPullUp);
    Task.Delay(1);

    // First set data line low for a period according to sensor type
    _pin.SetDriveMode(GpioPinDriveMode.Output);
    _pin.Write(GpioPinValue.Low);
    Task.Delay(1100)

    uint cycles[80];
    {
        // End the start signal by setting data line high for 40 microseconds.
        _pin.SetDriveMode(GpioPinDriveMode.InputPullUp);

        // Delay a moment to let sensor pull data line low.
        
        Task.Delay(pullTime);
        // Now start reading the data line to get the value from the DHT sensor.

        // Turn off interrupts temporarily because the next sections
        // are timing critical and we don't want any interruptions.
        InterruptLock lock;
        
        // First expect a low signal for ~80 microseconds followed by a high signal
        // for ~80 microseconds again.
        if (expectPulse(false) == TIMEOUT)
        {
            DEBUG_PRINTLN(F("DHT timeout waiting for start signal low pulse."));
            _lastresult = false;
            return _lastresult;
        }
        if (expectPulse(true) == TIMEOUT)
        {
            DEBUG_PRINTLN(F("DHT timeout waiting for start signal high pulse."));
            _lastresult = false;
            return _lastresult;
        }

        // Now read the 40 bits sent by the sensor.  Each bit is sent as a 50
        // microsecond low pulse followed by a variable length high pulse.  If the
        // high pulse is ~28 microseconds then it's a 0 and if it's ~70 microseconds
        // then it's a 1.  We measure the cycle count of the initial 50us low pulse
        // and use that to compare to the cycle count of the high pulse to determine
        // if the bit is a 0 (high state cycle count < low state cycle count), or a
        // 1 (high state cycle count > low state cycle count). Note that for speed
        // all the pulses are read into a array and then examined in a later step.
        for (int i = 0; i < 80; i += 2)
        {
            cycles[i] = expectPulse(LOW);
            cycles[i + 1] = expectPulse(HIGH);
        }
    } // Timing critical code is now complete.

    // Inspect pulses and determine which ones are 0 (high state cycle count < low
    // state cycle count), or 1 (high state cycle count > low state cycle count).
    for (int i = 0; i < 40; ++i)
    {
        uint lowCycles = cycles[2 * i];
        uint highCycles = cycles[2 * i + 1];
        if ((lowCycles == TIMEOUT) || (highCycles == TIMEOUT))
        {
            Debug.WriteLine("DHT timeout waiting for pulse.");
            _lastresult = false;
            return _lastresult;
        }
        data[i / 8] <<= 1;
        // Now compare the low and high cycle times to see if the bit is a 0 or 1.
        if (highCycles > lowCycles)
        {
            // High cycles are greater than 50us low cycle count, must be a 1.
            data[i / 8] |= 1;
        }
        // Else high cycles are less than (or equal to, a weird case) the 50us low
        // cycle count so this must be a zero.  Nothing needs to be changed in the
        // stored data.
    }

    Debug.WriteLine("Received from DHT:"));
    Debug.Write(data[0], HEX);
    Debug.Write(", ");
    Debug.Write(data[1], HEX);
    Debug.Write(", ");
    DEBUG_PRINT(data[2], HEX);
    DEBUG_PRINT(F(", "));
    DEBUG_PRINT(data[3], HEX);
    DEBUG_PRINT(F(", "));
    DEBUG_PRINT(data[4], HEX);
    DEBUG_PRINT(F(" =? "));
    DEBUG_PRINTLN((data[0] + data[1] + data[2] + data[3]) & 0xFF, HEX);

    // Check we read 40 bits and that the checksum matches.
    if (data[4] == ((data[0] + data[1] + data[2] + data[3]) & 0xFF))
    {
        _lastresult = true;
        return _lastresult;
    }
    else
    {
        DEBUG_PRINTLN(F("DHT checksum failure!"));
        _lastresult = false;
        return _lastresult;
    }
}

// Expect the signal line to be at the specified level for a period of time and
// return a count of loop cycles spent at that level (this cycle count can be
// used to compare the relative time of two pulses).  If more than a millisecond
// ellapses without the level changing then the call fails with a 0 response.
// This is adapted from Arduino's pulseInLong function (which is only available
// in the very latest IDE versions):
//   https://github.com/arduino/Arduino/blob/master/hardware/arduino/avr/cores/arduino/wiring_pulse.c
uint DHT::expectPulse(bool level)
{
#if (F_CPU > 16000000L)
  uint count = 0;
#else
    ushort count = 0; // To work fast enough on slower AVR boards
#endif
    

    return count;
}

}
