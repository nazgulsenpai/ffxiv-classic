﻿using System;
using System.IO;
using System.Text;

namespace FFXIVClassic_Map_Server.packets.receive.social
{
    class AddRemoveSocialPacket
    {
        public bool invalidPacket = false;
        public string name;

        public AddRemoveSocialPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        name = Encoding.ASCII.GetString(binReader.ReadBytes(0x20));
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}
