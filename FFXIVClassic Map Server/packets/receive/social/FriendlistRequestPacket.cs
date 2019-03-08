﻿using System;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.receive.social
{
    class FriendlistRequestPacket
    {
        public bool invalidPacket = false;
        public uint num1;
        public uint num2;

        public FriendlistRequestPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        num1 = binReader.ReadUInt32();
                        num2 = binReader.ReadUInt32();
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}
