using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PictureThief.Interfaces
{
    public class ConsoleLogger
    {
        private static Dictionary<Color, ConsoleColor> ColorMap = new Dictionary<Color, ConsoleColor>();

        private static Logger instance = null;
        public static Logger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Logger.Instance;

                    if (!instance.inited)
                    {
                        Logger.Instance.Init(WriteLog, Color.Red, Color.Green, Color.Yellow);
                    }
                }

                return Logger.Instance;
            }
        }

        static ConsoleLogger()
        {
            foreach (string name in Enum.GetNames(typeof(ConsoleColor)))
            {
                try
                {
                    ColorMap.Add(Color.FromName(name), (ConsoleColor)(Enum.Parse(typeof(ConsoleColor), name)));
                }
                catch (Exception ex) { }
            }
        }

        private static ConsoleColor GetConsoleColor(Color color)
        {
            if (ColorMap.ContainsKey(color))
            {
                return ColorMap[color];
            }

            return ConsoleColor.White;
        }

        private static void WriteLog(Object color, string log)
        {
            try
            {
                Color newColor = (Color)color;
                Console.ForegroundColor = GetConsoleColor(newColor);
            }
            catch (Exception)
            { }
            Console.WriteLine(log);
        }

    }
}
