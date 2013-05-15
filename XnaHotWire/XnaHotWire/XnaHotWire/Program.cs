using System;
using System.Linq;
using System.Xml.Linq;

namespace XnaHotWire
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            XDocument doc = XDocument.Load("configuration.xml");

            XElement serialPortElement = (from xml in doc.Descendants("SerialPort")
                                          select xml).First();

            XElement inverXElement = (from xml in doc.Descendants("InvertX")
                                          select xml).FirstOrDefault();

            XElement inverYElement = (from xml in doc.Descendants("InvertY")
                                      select xml).FirstOrDefault();
            
            
            string comPortName = serialPortElement.Value;

            //SerialInput
            SerialInput serialInput = new SerialInput(comPortName);
            serialInput.SendData("g 8000\r\n");
            
            serialInput.InvertX = inverXElement != null && bool.Parse(inverXElement.Value);
            serialInput.InvertY = inverYElement != null && bool.Parse(inverYElement.Value);

            using (HotWire game = new HotWire(serialInput))
            {
                game.Run();
             
            }
        }
    }
#endif
}

