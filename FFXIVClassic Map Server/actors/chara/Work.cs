﻿namespace FFXIVClassic_Map_Server.Actors.Chara
{
    class Work
    {
        public ushort[] guildleveId = new ushort[16];
        public bool[] guildleveDone = new bool[16];
        public bool[] guildleveChecked = new bool[16];

        public bool betacheck = false;        

        public bool[] event_achieve_aetheryte = new bool[128];
    }
}
