using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PictureThief.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            PictureThief.Interfaces.ConsoleLogger.Instance.Info("Start");
            //PictureThief.Impl.PictureThiefManager.Instance.Start("MyWardrobe");
            PictureThief.Impl.PictureThiefManager.Instance.Start("Reiss");
            Console.Read();
        }
    }
}
