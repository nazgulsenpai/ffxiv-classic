﻿using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.lua;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FFXIVClassic_Map_Server.Actors
{
    class Quest : Actor
    {
        private Player owner;
        private uint currentPhase = 0;
        private uint questFlags = 0;
        private Dictionary<string, Object> questData = new Dictionary<string, object>();

        public Quest(uint actorID, string name)
            : base(actorID)
        {
            actorName = name;            
        }

        public Quest(Player owner, uint actorID, string name, string questDataJson, uint questFlags, uint currentPhase)
            : base(actorID)
        {
            this.owner = owner;
            actorName = name;            
            this.questFlags = questFlags;

            if (questDataJson != null)
                this.questData = JsonConvert.DeserializeObject<Dictionary<string, Object>>(questDataJson);
            else
                questData = null;

            if (questData == null)
                questData = new Dictionary<string, object>();

            this.currentPhase = currentPhase;
        }
       
        public void SetQuestData(string dataName, object data)
        {            
                questData[dataName] = data;

            //Inform update
        }

        public uint GetQuestId()
        {
            return actorId & 0xFFFFF;
        }

        public object GetQuestData(string dataName)
        {
            if (questData.ContainsKey(dataName))
                return questData[dataName];
            else
                return null;
        }

        public void ClearQuestData()
        {
            questData.Clear();
        }       

        public void ClearQuestFlags()
        {
            questFlags = 0;
        }

        public void SetQuestFlag(int bitIndex, bool value)
        {
            if (bitIndex >= 32)
            {
                Program.Log.Error("Tried to access bit flag >= 32 for questId: {0}", actorId);
                return;
            }
            
            int mask = 1 << bitIndex;

            if (value)
                questFlags |= (uint)(1 << bitIndex);
            else
                questFlags &= (uint)~(1 << bitIndex);

            DoCompletionCheck();
        }

        public bool GetQuestFlag(int bitIndex)
        {
            if (bitIndex >= 32)
            {
                Program.Log.Error("Tried to access bit flag >= 32 for questId: {0}", actorId);
                return false;
            }
            else
            return (questFlags & (1 << bitIndex)) == (1 << bitIndex);
        }

        public uint GetPhase()
        {
            return currentPhase;
        }

        public void NextPhase(uint phaseNumber)
        {
            currentPhase = phaseNumber;
            owner.SendGameMessage(Server.GetWorldManager().GetActor(), 25116, 0x20, (object)GetQuestId());
            SaveData();

            DoCompletionCheck();
        }

        public uint GetQuestFlags()
        {
            return questFlags;
        }

        public string GetSerializedQuestData()
        {
            return JsonConvert.SerializeObject(questData, Formatting.Indented);
        }

        public void SaveData()
        {
            Database.SaveQuest(owner, this);
        }

        public void DoCompletionCheck()
        {
            List<LuaParam> returned = LuaEngine.GetInstance().CallLuaFunctionForReturn(owner, this, "isObjectivesComplete", true);
            if (returned != null && returned.Count >= 1 && returned[0].typeID == 3)
            {
                owner.SendDataPacket("attention", Server.GetWorldManager().GetActor(), "", 25225, (object)GetQuestId());
                owner.SendGameMessage(Server.GetWorldManager().GetActor(), 25225, 0x20, (object)GetQuestId());	
            }
        }

        public void DoAbandon()
        {
            LuaEngine.GetInstance().CallLuaFunctionForReturn(owner, this, "onAbandonQuest", true);
            owner.SendGameMessage(owner, Server.GetWorldManager().GetActor(), 25236, 0x20, (object)GetQuestId());
        }

    }
}
