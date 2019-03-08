﻿using FFXIVClassic.Common;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace FFXIVClassic_Lobby_Server
{
    class ConfigConstants
    {
        public static String OPTIONS_BINDIP;
        public static String OPTIONS_PORT;
        public static bool   OPTIONS_TIMESTAMP = false;

        public static String DATABASE_HOST;
        public static String DATABASE_PORT;
        public static String DATABASE_NAME;
        public static String DATABASE_USERNAME;
        public static String DATABASE_PASSWORD;

        public static bool Load()
        {
            Program.Log.Info("Loading lobby_config.ini file");

            if (!File.Exists("./lobby_config.ini"))
            {
                Program.Log.Error("FILE NOT FOUND!");
                Program.Log.Error("Loading defaults...");
            }

            INIFile configIni = new INIFile("./lobby_config.ini");

            ConfigConstants.OPTIONS_BINDIP =        configIni.GetValue("General", "server_ip", "127.0.0.1");
            ConfigConstants.OPTIONS_PORT =          configIni.GetValue("General", "server_port", "54994");
            ConfigConstants.OPTIONS_TIMESTAMP =     configIni.GetValue("General", "showtimestamp", "true").ToLower().Equals("true");

            ConfigConstants.DATABASE_HOST =         configIni.GetValue("Database", "host", "");
            ConfigConstants.DATABASE_PORT =         configIni.GetValue("Database", "port", "");
            ConfigConstants.DATABASE_NAME =         configIni.GetValue("Database", "database", "");
            ConfigConstants.DATABASE_USERNAME =     configIni.GetValue("Database", "username", "");
            ConfigConstants.DATABASE_PASSWORD =     configIni.GetValue("Database", "password", "");

            return true;
        }
        public static void ApplyLaunchArgs(string[] launchArgs)
        {
            var args = (from arg in launchArgs select arg.ToLower().Trim().TrimStart('-')).ToList();

            for (var i = 0; i + 1 < args.Count; i += 2)
            {
                var arg = args[i];
                var val = args[i + 1];
                var legit = false;

                if (arg == "ip")
                {
                    IPAddress ip;
                    if (IPAddress.TryParse(val, out ip) && (legit = true))
                        OPTIONS_BINDIP = val;
                }
                else if (arg == "port")
                {
                    UInt16 port;
                    if (UInt16.TryParse(val, out port) && (legit = true))
                        OPTIONS_PORT = val;
                }
                else if (arg == "user" && (legit = true))
                {
                    DATABASE_USERNAME = val;
                }
                else if (arg == "p" && (legit = true))
                {
                    DATABASE_PASSWORD = val;
                }
                else if (arg == "db" && (legit = true))
                {
                    DATABASE_NAME = val;
                }
                else if (arg == "host" && (legit = true))
                {
                    DATABASE_HOST = val;
                }
                if (!legit)
                {
                    Program.Log.Error("Invalid parameter <{0}> for argument: <--{1}> or argument doesnt exist!", val, arg);
                }
            }
        }
    }
}
