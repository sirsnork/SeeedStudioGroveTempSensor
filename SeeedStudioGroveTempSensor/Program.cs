using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using Toolbox.NETMF;
using seeedStudio.Grove.SerialLCD;

namespace seeedStudio.Grove.TempSensor
{
    public class Program
    {
        private static AnalogInput temp = new AnalogInput(AnalogChannels.ANALOG_PIN_A0);
        private static double value = 0;
        private static double resistance = 0;
        private static double temperature = 0;
        private static int B = 3975;
        private static string roundedtemp = "";
        private static double adcVoltage = 3.3;
        private static int adcResolution = 4095;

        public static void Main()
        {

            // initialise the LCD display
            LCD lcd = new LCD("COM2");

            // Timer turns Backlight off after 30 seconds of inactivity
            Timer backlightTimer = new Timer(BacklightTimerOff, lcd, 0, 30000);

            Thread.Sleep(2000);

            // Turn on Backlight for LCD, flickers due to power when only running on USB
            lcd.backlight();

            while (true)
            {
                value = temp.ReadRaw();
                resistance = (double)((adcResolution * (1 + (adcVoltage/5))) - value) * 10000 / value; //get the resistance of the sensor;
                temperature = (1 / (Log(resistance / 10000, 10) / B + 1 / 298.15) - 273.15);//convert to temperature via datasheet ;
                Debug.Print("Current temperature is " + temperature );
                roundedtemp = Tools.Round((float)temperature, 2);
                lcd.Clear();
                lcd.SetCursor(0, 0);
                lcd.print(System.Text.Encoding.UTF8.GetBytes(" Current Temp:"));
                lcd.SetCursor(0, 1);
                lcd.print(System.Text.Encoding.UTF8.GetBytes("      " + roundedtemp));
                Thread.Sleep(1000);
            }
        }

        static void BacklightTimerOff(object state)
        {
            LCD lcd = (LCD)state;
            lcd.noBacklight();
        }

        public static double Log(double x, double newBase)
        {
            // Based on Python sourcecode from:
            // http://en.literateprograms.org/Logarithm_Function_%28Python%29

            double partial = 0.5;

            double integer = 0;
            double fractional = 0.0;
            double epsilon = 2.22045e-16;

            if (x == 0.0) return double.NegativeInfinity;
            if ((x < 1.0) & (newBase < 1.0)) throw new ArgumentOutOfRangeException("can't compute Log");

            while (x < 1.0)
            {
                integer -= 1;
                x *= newBase;
            }

            while (x >= newBase)
            {
                integer += 1;
                x /= newBase;
            }

            x *= x;

            while (partial >= epsilon)
            {
                if (x >= newBase)
                {
                    fractional += partial;
                    x = x / newBase;
                }
                partial *= 0.5;
                x *= x;
            }

            return (integer + fractional);
        }

    }
}
