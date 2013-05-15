using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace XnaHotWire
{
    class SerialInput
    {
        private int _valueX;
        private int _valueY;
        private readonly SerialPort _serialport;
        private String _lastMessage;

        public SerialInput(string comPort)
        {
            try
            {
                _serialport = new SerialPort(comPort)
                {
                    BaudRate = 19200,
                    Parity = Parity.None,
                    StopBits = StopBits.One,
                    DataBits = 8,
                    Handshake = Handshake.None
                };

                _serialport.DataReceived += SerialportOnDataReceived;
                _serialport.Open();
            }
            catch (Exception e)
            {
                throw new Exception("Exception: SerialPort\n" + e.Message);
            }
        }

        ~SerialInput()
        {
            _serialport.Close();
        }

        private void SerialportOnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            _lastMessage = sp.ReadExisting();
        }

        public void SendData(String message)
        {
            try
            {
                //_serialport.Write(data, 0, data.Length);
                _serialport.Write(message);
            }
            catch (Exception e)
            {
                throw new Exception("Exception: SerialPort\n" + e.Message);
            }
        }

        public float GetPositionX()
        {
            //todo: convert _lastMessage
            if (_lastMessage != null && _lastMessage.Length == 4 && !_lastMessage.Contains(":"))
            {
                Char[] x = _lastMessage.ToCharArray();
                string strX = "" + x[0] + x[1];
                try
                {
                    _valueX = Int32.Parse(strX, System.Globalization.NumberStyles.HexNumber);
                }
                catch (System.FormatException)
                {


                }

            }

            return (float)(_valueX-128)/64;
        }

        public float GetPositionY()
        {
            if (_lastMessage != null && _lastMessage.Length == 4 && !_lastMessage.Contains(":"))
            {

                Char[] y = _lastMessage.ToCharArray();
                string strY = "" + y[2] + y[3];

                try
                {
                    _valueY = Int32.Parse(strY, System.Globalization.NumberStyles.HexNumber);
                }
                catch (System.FormatException)
                {
                }
            }
            return (float)(_valueY-128)/64;
        }

        public void ControlLed(int value)
        {
            SendData(value.ToString());
        }
    }
}
