using System;
using Windows.Devices.Gpio;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;

namespace Hello_World
{
    /// <summary>
    /// This is a very simple service for blinking an LED.
    /// </summary>
    public class BlinkyService
    {
        private readonly string gpioType = "Windows.Devices.Gpio.GpioController";
        private readonly int pinNumber;
        private DispatcherTimer timer;
        private GpioPinValue pinValue;
        private GpioPin pin;

        int blinkRate;
        public int BlinkRate
        {
            get
            {
                return blinkRate;
            }
            set
            {
                blinkRate = value;
                if (timer != null)
                    timer.Interval = TimeSpan.FromMilliseconds(blinkRate);
            }
        }

        public BlinkyService(int pinNumber)
        {
            this.pinNumber = pinNumber;
        }

        /// <summary>
        /// Is blinking a light possible on this device?
        /// </summary>
        public bool CanBlink
        {
            get
            {
                //NOTE: This is where we use the adaptable code feature of UWP.
                return ApiInformation.IsTypePresent(gpioType);
            }
        }

        // This is a "fire and forget" timer.
        public void Start()
        {
            if (!InitializeGpio())
                return;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(BlinkRate);
            timer.Tick += TimerTick;

            if (pin != null)
            {
                timer.Start();
            }
        }

        private void TimerTick(object sender, object e)
        {
            if (pinValue == GpioPinValue.High)
                pinValue = GpioPinValue.Low;
            else
                pinValue = GpioPinValue.High;

            pin.Write(pinValue);
        }


        private bool InitializeGpio()
        {
            var gpio = GpioController.GetDefault();

            // No GPIO controller found
            if (gpio == null)
            {
                pin = null;
                return false;
            }

            pin = gpio.OpenPin(pinNumber);
            pinValue = GpioPinValue.High;
            pin.Write(pinValue);
            pin.SetDriveMode(GpioPinDriveMode.Output);

            return true;
        }
    }
}
