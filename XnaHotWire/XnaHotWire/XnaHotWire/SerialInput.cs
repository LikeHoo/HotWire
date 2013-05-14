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
                    BaudRate = 9600,
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
            System.Console.WriteLine(_lastMessage);
        }

        private void SendData(String message)
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

        public int GetPositionX()
        {
            //todo: convert _lastMessage


            return _valueX;
        }

        public int GetPositionY()
        {
            //todo: convert _lastMessage


            return _valueY;
        }

        public void ControlLed(int value)
        {
            SendData(value.ToString());
        }
    }
}
