using MITM.Wakfu.Extensions;
using MITM.Wakfu.Network;
using MITM.Wakfu.Utils;
using Rebirth.Common.IO;
using System;
using System.Text;

namespace MITM.Wakfu
{
    class Program
    {
        public static ServerAuth ServerLogin { get; set; }
        public static ServerGame ServerGame { get; set; }
        public static bool Hexa { get; set; }

        static void Main(string[] args)
        {
            LogManager.Init();
            LogManager.GetLoggerClass().Infos("Voulez vous voir les datas en Hexa ? [y/n]");
            do
            {
                var key = Console.ReadLine();
                if (key == "y")
                {
                    Hexa = true;
                    break;
                }
                else if (key == "n")
                    break;
                else
                    LogManager.GetLoggerClass().Infos("Commande incorrect merci de choisir entre 'y' ou 'n' !");
            } while (true);
            LogManager.GetLoggerClass().Infos("Lancement du MITM !");
            ServerLogin = new ServerAuth(5558);
            ServerGame = new ServerGame(5556);
            Console.Read();
        }
    }
}