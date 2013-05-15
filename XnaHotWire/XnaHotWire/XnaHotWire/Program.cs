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

            string comPortName = serialPortElement.Value;

            using (HotWire game = new HotWire(comPortName))
            {
                game.Run();
            }
        }
    }
#endif
}

