﻿using System;
using System.Net.Sockets;
using FFXIVClassic.Common;
using System.Collections.Concurrent;
using Cyotek.Collections.Generic;
using System.Net;

namespace FFXIVClassic_Lobby_Server
{
    class ClientConnection
    {
        //Connection stuff
        public Blowfish blowfish;
        public Socket socket;
        public byte[] buffer = new byte[0xffff];
        public CircularBuffer<byte> incomingStream = new CircularBuffer<byte>(1024);
        public BlockingCollection<BasePacket> SendPacketQueue = new BlockingCollection<BasePacket>(100);
        public int lastPartialSize = 0;

        //Instance Stuff
        public uint currentUserId = 0;
        public uint currentAccount;
        public string currentSessionToken;

        //Chara Creation
        public string newCharaName;
        public uint newCharaPid;
        public uint newCharaCid;
        public ushort newCharaSlot;
        public ushort newCharaWorldId;
        

        public void ProcessIncoming(int bytesIn)
        {
            if (bytesIn == 0)
                return;

            incomingStream.Put(buffer, 0, bytesIn);
        }

        public void QueuePacket(BasePacket packet)
        {
            SendPacketQueue.Add(packet);
        }

        public void FlushQueuedSendPackets()
        {
            if (!socket.Connected)
                return;

            while (SendPacketQueue.Count > 0)
            {
                BasePacket packet = SendPacketQueue.Take();
                byte[] packetBytes = packet.GetPacketBytes();
                byte[] buffer = new byte[0xffff];
                Array.Copy(packetBytes, buffer, packetBytes.Length);
                try { 
                    socket.Send(packetBytes);
                }
                catch(Exception e)
                { Program.Log.Error("Weird case, socket was d/ced: {0}", e); }
            }
        }

        public String GetAddress()
        {
            return String.Format("{0}:{1}", (socket.RemoteEndPoint as IPEndPoint).Address, (socket.RemoteEndPoint as IPEndPoint).Port);
        }        

        public void Disconnect()
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Disconnect(false);
        }
    }
}
