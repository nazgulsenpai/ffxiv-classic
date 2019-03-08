﻿using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.chara.player;
using FFXIVClassic_Map_Server.actors.director;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.dataobjects.chara;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.packets.send.events;
using FFXIVClassic_Map_Server.packets.send.actor.events;
using FFXIVClassic_Map_Server.packets.send.actor.inventory;
using FFXIVClassic_Map_Server.packets.send.events;
using FFXIVClassic_Map_Server.packets.send.player;
using FFXIVClassic_Map_Server.utils;
using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using FFXIVClassic_Map_Server.packets.receive.events;
using FFXIVClassic_Map_Server.packets.send.actor.inventory;
using FFXIVClassic_Map_Server.actors.group;
using FFXIVClassic_Map_Server.packets.send.group;
using FFXIVClassic_Map_Server.packets.WorldPackets.Send.Group;
using FFXIVClassic_Map_Server.actors.chara.ai;
using FFXIVClassic_Map_Server.actors.chara.ai.controllers;
using FFXIVClassic_Map_Server.packets.send.actor.battle;
using FFXIVClassic_Map_Server.actors.chara.ai.utils;
using FFXIVClassic_Map_Server.actors.chara.ai.state;
using FFXIVClassic_Map_Server.actors.chara.npc;
using FFXIVClassic_Map_Server.actors.chara;

namespace FFXIVClassic_Map_Server.Actors
{
    class Player : Character
    {
        public const int MAXSIZE_INVENTORY_NORMAL = 200;
        public const int MAXSIZE_INVENTORY_CURRANCY = 320;
        public const int MAXSIZE_INVENTORY_KEYITEMS = 500;
        public const int MAXSIZE_INVENTORY_LOOT = 10;
        public const int MAXSIZE_INVENTORY_MELDREQUEST = 4;
        public const int MAXSIZE_INVENTORY_BAZAAR = 10;
        public const int MAXSIZE_INVENTORY_EQUIPMENT = 35;

        public const int TIMER_TOTORAK = 0;
        public const int TIMER_DZEMAEL = 1;        
        public const int TIMER_BOWL_OF_EMBERS_HARD = 2;
        public const int TIMER_BOWL_OF_EMBERS = 3;
        public const int TIMER_THORNMARCH = 4;
        public const int TIMER_AURUMVALE = 5;
        public const int TIMER_CUTTERSCRY = 6;
        public const int TIMER_BATTLE_ALEPORT = 7;
        public const int TIMER_BATTLE_HYRSTMILL = 8;
        public const int TIMER_BATTLE_GOLDENBAZAAR = 9;
        public const int TIMER_HOWLING_EYE_HARD = 10;
        public const int TIMER_HOWLING_EYE = 11;
        public const int TIMER_CASTRUM_TOWER = 12;
        public const int TIMER_BOWL_OF_EMBERS_EXTREME = 13;
        public const int TIMER_RIVENROAD = 14;
        public const int TIMER_RIVENROAD_HARD = 15;
        public const int TIMER_BEHEST = 16;
        public const int TIMER_COMPANYBEHEST = 17;
        public const int TIMER_RETURN = 18;
        public const int TIMER_SKIRMISH = 19;

        public const int NPCLS_GONE     = 0;
        public const int NPCLS_INACTIVE = 1;
        public const int NPCLS_ACTIVE   = 2;
        public const int NPCLS_ALERT    = 3;

        public static int[] MAXEXP = {570, 700, 880, 1100, 1500, 1800, 2300, 3200, 4300, 5000,                   //Level <= 10
                                     5900, 6800, 7700, 8700, 9700, 11000, 12000, 13000, 15000, 16000,            //Level <= 20
                                     20000, 22000, 23000, 25000, 27000, 29000, 31000, 33000, 35000, 38000,       //Level <= 30
                                     45000, 47000, 50000, 53000, 56000, 59000, 62000, 65000, 68000, 71000,       //Level <= 40
                                     74000, 78000, 81000, 85000, 89000, 92000, 96000, 100000, 100000, 110000};   //Level <= 50

        //Event Related
        public uint currentEventOwner = 0;
        public string currentEventName = "";

        public Coroutine currentEventRunning;

        //Player Info
        public uint destinationZone;
        public ushort destinationSpawnType;
        public uint[] timers = new uint[20];
        public uint currentTitle;
        public uint playTime;
        public uint lastPlayTimeUpdate;
        public bool isGM = false;
        public bool isZoneChanging = true;

        //Inventory        
        private Dictionary<ushort, Inventory> inventories = new Dictionary<ushort, Inventory>();
        private Equipment equipment;

        //GC Related
        public byte gcCurrent;
        public byte gcRankLimsa;
        public byte gcRankGridania;
        public byte gcRankUldah;

        //Mount Related
        public bool hasChocobo;
        public bool hasGoobbue;
        public byte chocoboAppearance;
        public string chocoboName;
        public byte mountState = 0;        

        public uint achievementPoints;

        //Property Array Request Stuff
        private int lastPosition = 0;
        private int lastStep = 0;

        //Quest Actors (MUST MATCH playerWork.questScenario/questGuildleve)
        public Quest[] questScenario = new Quest[16];
        public uint[] questGuildleve = new uint[8];

        //Aetheryte
        public uint homepoint = 0;
        public byte homepointInn = 0;

        private List<Director> ownedDirectors = new List<Director>();
        private Director loginInitDirector = null;

        public PlayerWork playerWork = new PlayerWork();

        public Session playerSession;

        public Player(Session cp, uint actorID) : base(actorID)
        {
            playerSession = cp;
            actorName = String.Format("_pc{0:00000000}", actorID);
            className = "Player";

            moveSpeeds[0] = SetActorSpeedPacket.DEFAULT_STOP;
            moveSpeeds[1] = SetActorSpeedPacket.DEFAULT_WALK;
            moveSpeeds[2] = SetActorSpeedPacket.DEFAULT_RUN;
            moveSpeeds[3] = SetActorSpeedPacket.DEFAULT_ACTIVE;

            inventories[Inventory.NORMAL] = new Inventory(this, MAXSIZE_INVENTORY_NORMAL, Inventory.NORMAL);
            inventories[Inventory.KEYITEMS] = new Inventory(this, MAXSIZE_INVENTORY_KEYITEMS, Inventory.KEYITEMS);
            inventories[Inventory.CURRENCY] = new Inventory(this, MAXSIZE_INVENTORY_CURRANCY, Inventory.CURRENCY);
            inventories[Inventory.MELDREQUEST] = new Inventory(this, MAXSIZE_INVENTORY_MELDREQUEST, Inventory.MELDREQUEST);
            inventories[Inventory.BAZAAR] = new Inventory(this, MAXSIZE_INVENTORY_BAZAAR, Inventory.BAZAAR);
            inventories[Inventory.LOOT] = new Inventory(this, MAXSIZE_INVENTORY_LOOT, Inventory.LOOT);

            equipment = new Equipment(this, inventories[Inventory.NORMAL], MAXSIZE_INVENTORY_EQUIPMENT, Inventory.EQUIPMENT);

            //Set the Skill level caps of all FFXIV (classes)skills to 50
            for (int i = 0; i < charaWork.battleSave.skillLevelCap.Length; i++)
            {
                if (i != CLASSID_PUG &&
                    i != CLASSID_MRD &&
                    i != CLASSID_GLA &&
                    i != CLASSID_MRD &&
                    i != CLASSID_ARC &&
                    i != CLASSID_LNC &&
                    i != CLASSID_THM &&
                    i != CLASSID_CNJ &&
                    i != CLASSID_CRP &&
                    i != CLASSID_BSM &&
                    i != CLASSID_ARM &&
                    i != CLASSID_GSM &&
                    i != CLASSID_LTW &&
                    i != CLASSID_WVR &&
                    i != CLASSID_ALC &&
                    i != CLASSID_CUL &&
                    i != CLASSID_MIN &&
                    i != CLASSID_BTN &&
                    i != CLASSID_FSH)
                    charaWork.battleSave.skillLevelCap[i] = 0xFF;
                else
                    charaWork.battleSave.skillLevelCap[i] = 50;

            }

            charaWork.property[0] = 1;
            charaWork.property[1] = 1;
            charaWork.property[2] = 1;
            charaWork.property[4] = 1;

            charaWork.command[0] =  0xA0F00000 | 21001;
            charaWork.command[1] =  0xA0F00000 | 21001;

            charaWork.command[2] =  0xA0F00000 | 21002;
            charaWork.command[3] =  0xA0F00000 | 12004;
            charaWork.command[4] =  0xA0F00000 | 21005;
            charaWork.command[5] =  0xA0F00000 | 21006;
            charaWork.command[6] =  0xA0F00000 | 21007;
            charaWork.command[7] =  0xA0F00000 | 12009;
            charaWork.command[8] =  0xA0F00000 | 12010;
            charaWork.command[9] =  0xA0F00000 | 12005;
            charaWork.command[10] = 0xA0F00000 | 12007;
            charaWork.command[11] = 0xA0F00000 | 12011;
            charaWork.command[12] = 0xA0F00000 | 22012;
            charaWork.command[13] = 0xA0F00000 | 22013;
            charaWork.command[14] = 0xA0F00000 | 29497;
            charaWork.command[15] = 0xA0F00000 | 22015;            

            charaWork.commandAcquired[27150 - 26000] = true;

            playerWork.questScenarioComplete[110001 - 110001] = true;
            playerWork.questGuildleveComplete[120050 - 120001] = true;

            for (int i = 0; i < charaWork.additionalCommandAcquired.Length; i++ )
                charaWork.additionalCommandAcquired[i] = true;
            
            for (int i = 0; i < charaWork.commandCategory.Length; i++)
                charaWork.commandCategory[i] = 1;

            charaWork.battleTemp.generalParameter[3] = 1;

            charaWork.eventSave.bazaarTax = 5;
            charaWork.battleSave.potencial = 6.6f;

            charaWork.commandCategory[0] = 1;
            charaWork.commandCategory[1] = 1;

            charaWork.parameterSave.commandSlot_compatibility[0] = true;
            charaWork.parameterSave.commandSlot_compatibility[1] = true;

            charaWork.commandBorder = 0x20;

            charaWork.parameterTemp.tp = 0;

            Database.LoadPlayerCharacter(this);
            lastPlayTimeUpdate = Utils.UnixTimeStampUTC();

            this.aiContainer = new AIContainer(this, new PlayerController(this), null, new TargetFind(this));
            allegiance = CharacterTargetingAllegiance.Player;
            CalculateBaseStats();
        }
        
        public List<SubPacket> Create0x132Packets()
        {
            List<SubPacket> packets = new List<SubPacket>();
            packets.Add(_0x132Packet.BuildPacket(actorId, 0xB, "commandForced"));
            packets.Add(_0x132Packet.BuildPacket(actorId, 0xA, "commandDefault"));
            packets.Add(_0x132Packet.BuildPacket(actorId, 0x6, "commandWeak"));
            packets.Add(_0x132Packet.BuildPacket(actorId, 0x4, "commandContent"));
            packets.Add(_0x132Packet.BuildPacket(actorId, 0x6, "commandJudgeMode"));
            packets.Add(_0x132Packet.BuildPacket(actorId, 0x100, "commandRequest"));
            packets.Add(_0x132Packet.BuildPacket(actorId, 0x100, "widgetCreate"));
            packets.Add(_0x132Packet.BuildPacket(actorId, 0x100, "macroRequest"));
            return packets;
        }

        /*        
         * PLAYER ARGS:
         * Unknown - Bool 
         * Unknown - Bool
         * Is Init Director - Bool
         * Unknown - Bool
         * Unknown - Number
         * Unknown - Bool
         * Timer Array - 20 Number
        */

        public override SubPacket CreateScriptBindPacket(Player requestPlayer)
        {
            List<LuaParam> lParams;
            if (IsMyPlayer(requestPlayer.actorId))
            {
                if (loginInitDirector != null)
                    lParams = LuaUtils.CreateLuaParamList("/Chara/Player/Player_work", false, false, true, loginInitDirector, true, 0, false, timers, true);
                else
                    lParams = LuaUtils.CreateLuaParamList("/Chara/Player/Player_work", true, false, false, true, 0, false, timers, true);
            }
            else
                lParams = LuaUtils.CreateLuaParamList("/Chara/Player/Player_work", false, false, false, false, false, true);

            ActorInstantiatePacket.BuildPacket(actorId, actorName, className, lParams).DebugPrintSubPacket();


            return ActorInstantiatePacket.BuildPacket(actorId, actorName, className, lParams);
        }

        public override List<SubPacket> GetSpawnPackets(Player requestPlayer, ushort spawnType)
        {
            List<SubPacket> subpackets = new List<SubPacket>();
            subpackets.Add(CreateAddActorPacket(8));
            if (IsMyPlayer(requestPlayer.actorId))
                subpackets.AddRange(Create0x132Packets());
            subpackets.Add(CreateSpeedPacket());
            subpackets.Add(CreateSpawnPositonPacket(this, spawnType));
            subpackets.Add(CreateAppearancePacket());
            subpackets.Add(CreateNamePacket());
            subpackets.Add(_0xFPacket.BuildPacket(actorId));
            subpackets.Add(CreateStatePacket());
            subpackets.Add(CreateSubStatePacket());
            subpackets.Add(CreateInitStatusPacket());
            subpackets.Add(CreateSetActorIconPacket());
            subpackets.Add(CreateIsZoneingPacket());
            subpackets.AddRange(CreatePlayerRelatedPackets(requestPlayer.actorId));
            subpackets.Add(CreateScriptBindPacket(requestPlayer));            
            return subpackets;
        }

        public List<SubPacket> CreatePlayerRelatedPackets(uint requestingPlayerActorId)
        {
            List<SubPacket> subpackets = new List<SubPacket>();

            if (gcCurrent != 0)
                subpackets.Add(SetGrandCompanyPacket.BuildPacket(actorId, gcCurrent, gcRankLimsa, gcRankGridania, gcRankUldah));

            if (currentTitle != 0)
                subpackets.Add(SetPlayerTitlePacket.BuildPacket(actorId, currentTitle));

            if (currentJob != 0)
                subpackets.Add(SetCurrentJobPacket.BuildPacket(actorId, currentJob));

            if (IsMyPlayer(requestingPlayerActorId))
            {
                subpackets.Add(SetSpecialEventWorkPacket.BuildPacket(actorId));

                if (hasChocobo && chocoboName != null && !chocoboName.Equals(""))
                {
                    subpackets.Add(SetChocoboNamePacket.BuildPacket(actorId, chocoboName));
                    subpackets.Add(SetHasChocoboPacket.BuildPacket(actorId, hasChocobo));
                }

                if (hasGoobbue)
                    subpackets.Add(SetHasGoobbuePacket.BuildPacket(actorId, hasGoobbue));

                subpackets.Add(SetAchievementPointsPacket.BuildPacket(actorId, achievementPoints));

                subpackets.Add(Database.GetLatestAchievements(this));
                subpackets.Add(Database.GetAchievementsPacket(this));                
            }

            if (mountState == 1)
                subpackets.Add(SetCurrentMountChocoboPacket.BuildPacket(actorId, chocoboAppearance));
            else if (mountState == 2)
                subpackets.Add(SetCurrentMountGoobbuePacket.BuildPacket(actorId, 1));
          
            return subpackets;
        }

        public override List<SubPacket> GetInitPackets()
        {
            ActorPropertyPacketUtil propPacketUtil = new ActorPropertyPacketUtil("/_init", this);
                        
            propPacketUtil.AddProperty("charaWork.eventSave.bazaarTax");
            propPacketUtil.AddProperty("charaWork.battleSave.potencial");

            //Properties
            for (int i = 0; i < charaWork.property.Length; i++)
            {
                if (charaWork.property[i] != 0)                
                    propPacketUtil.AddProperty(String.Format("charaWork.property[{0}]", i));
            }
            
            //Parameters
            propPacketUtil.AddProperty("charaWork.parameterSave.hp[0]");
            propPacketUtil.AddProperty("charaWork.parameterSave.hpMax[0]");
            propPacketUtil.AddProperty("charaWork.parameterSave.mp");
            propPacketUtil.AddProperty("charaWork.parameterSave.mpMax");
            propPacketUtil.AddProperty("charaWork.parameterTemp.tp");
            propPacketUtil.AddProperty("charaWork.parameterSave.state_mainSkill[0]");
            propPacketUtil.AddProperty("charaWork.parameterSave.state_mainSkillLevel");
            
            //Status Times
            for (int i = 0; i < charaWork.statusShownTime.Length; i++)
            {
                if (charaWork.statusShownTime[i] != 0)
                    propPacketUtil.AddProperty(String.Format("charaWork.statusShownTime[{0}]", i));
            }
        
            //General Parameters
            for (int i = 3; i < charaWork.battleTemp.generalParameter.Length; i++)
            {
                if (charaWork.battleTemp.generalParameter[i] != 0)
                    propPacketUtil.AddProperty(String.Format("charaWork.battleTemp.generalParameter[{0}]", i));
            }
            
            propPacketUtil.AddProperty("charaWork.battleTemp.castGauge_speed[0]");
            propPacketUtil.AddProperty("charaWork.battleTemp.castGauge_speed[1]");

            //Battle Save Skillpoint
            propPacketUtil.AddProperty(String.Format("charaWork.battleSave.skillPoint[{0}]", charaWork.parameterSave.state_mainSkill[0] - 1));

            //Commands
            propPacketUtil.AddProperty("charaWork.commandBorder");

            for (int i = 0; i < charaWork.command.Length; i++)
            {
                if (charaWork.command[i] != 0)
                {
                    propPacketUtil.AddProperty(String.Format("charaWork.command[{0}]", i));
                    //Recast Timers
                    if(i >= charaWork.commandBorder)
                    {
                        propPacketUtil.AddProperty(String.Format("charaWork.parameterTemp.maxCommandRecastTime[{0}]", i - charaWork.commandBorder));
                        propPacketUtil.AddProperty(String.Format("charaWork.parameterSave.commandSlot_recastTime[{0}]", i - charaWork.commandBorder));
                    }
                }
            }
            
            for (int i = 0; i < charaWork.commandCategory.Length; i++)
            {
                charaWork.commandCategory[i] = 1;
                if (charaWork.commandCategory[i] != 0)
                    propPacketUtil.AddProperty(String.Format("charaWork.commandCategory[{0}]", i));
            }

            for (int i = 0; i < charaWork.commandAcquired.Length; i++)
            {
                if (charaWork.commandAcquired[i] != false)
                    propPacketUtil.AddProperty(String.Format("charaWork.commandAcquired[{0}]", i));
            }

            for (int i = 0; i < charaWork.additionalCommandAcquired.Length; i++)
            {
                if (charaWork.additionalCommandAcquired[i] != false)
                    propPacketUtil.AddProperty(String.Format("charaWork.additionalCommandAcquired[{0}]", i));
            }
            
            for (int i = 0; i < charaWork.parameterSave.commandSlot_compatibility.Length; i++)
            {
                charaWork.parameterSave.commandSlot_compatibility[i] = true;
                if (charaWork.parameterSave.commandSlot_compatibility[i])
                    propPacketUtil.AddProperty(String.Format("charaWork.parameterSave.commandSlot_compatibility[{0}]", i));
            }

            for (int i = 0; i < charaWork.parameterSave.commandSlot_recastTime.Length; i++)
            {
                if (charaWork.parameterSave.commandSlot_recastTime[i] != 0)
                    propPacketUtil.AddProperty(String.Format("charaWork.parameterSave.commandSlot_recastTime[{0}]", i));
            }

            //System
            propPacketUtil.AddProperty("charaWork.parameterTemp.forceControl_float_forClientSelf[0]");
            propPacketUtil.AddProperty("charaWork.parameterTemp.forceControl_float_forClientSelf[1]");
            propPacketUtil.AddProperty("charaWork.parameterTemp.forceControl_int16_forClientSelf[0]");
            propPacketUtil.AddProperty("charaWork.parameterTemp.forceControl_int16_forClientSelf[1]");

            charaWork.parameterTemp.otherClassAbilityCount[0] = 4;
            charaWork.parameterTemp.otherClassAbilityCount[1] = 5;
            charaWork.parameterTemp.giftCount[1] = 5;

            propPacketUtil.AddProperty("charaWork.parameterTemp.otherClassAbilityCount[0]");
            propPacketUtil.AddProperty("charaWork.parameterTemp.otherClassAbilityCount[1]");
            propPacketUtil.AddProperty("charaWork.parameterTemp.giftCount[1]");

            propPacketUtil.AddProperty("charaWork.depictionJudge");
            
            //Scenario
            for (int i = 0; i < playerWork.questScenario.Length; i++)
            {
                if (playerWork.questScenario[i] != 0)
                    propPacketUtil.AddProperty(String.Format("playerWork.questScenario[{0}]", i));
            }

            //Guildleve - Local
            for (int i = 0; i < playerWork.questGuildleve.Length; i++)
            {
                if (playerWork.questGuildleve[i] != 0)
                    propPacketUtil.AddProperty(String.Format("playerWork.questGuildleve[{0}]", i));
            }

            //Guildleve - Regional
            for (int i = 0; i < work.guildleveId.Length; i++)
            {
                if (work.guildleveId[i] != 0)
                    propPacketUtil.AddProperty(String.Format("work.guildleveId[{0}]", i));
                if (work.guildleveDone[i] != false)
                    propPacketUtil.AddProperty(String.Format("work.guildleveDone[{0}]", i));
                if (work.guildleveChecked[i] != false)
                    propPacketUtil.AddProperty(String.Format("work.guildleveChecked[{0}]", i));
            }

            //NPC Linkshell
            for (int i = 0; i < playerWork.npcLinkshellChatCalling.Length; i++)
            {
                if (playerWork.npcLinkshellChatCalling[i] != false)
                    propPacketUtil.AddProperty(String.Format("playerWork.npcLinkshellChatCalling[{0}]", i));
                if (playerWork.npcLinkshellChatExtra[i] != false)
                    propPacketUtil.AddProperty(String.Format("playerWork.npcLinkshellChatExtra[{0}]", i));
            }

            propPacketUtil.AddProperty("playerWork.restBonusExpRate");

            //Profile
            propPacketUtil.AddProperty("playerWork.tribe");
            propPacketUtil.AddProperty("playerWork.guardian");
            propPacketUtil.AddProperty("playerWork.birthdayMonth");
            propPacketUtil.AddProperty("playerWork.birthdayDay");
            propPacketUtil.AddProperty("playerWork.initialTown");
            
            return propPacketUtil.Done();
        }

        public void SendSeamlessZoneInPackets()
        {
            QueuePacket(SetMusicPacket.BuildPacket(actorId, zone.bgmDay, SetMusicPacket.EFFECT_FADEIN));
            QueuePacket(SetWeatherPacket.BuildPacket(actorId, SetWeatherPacket.WEATHER_CLEAR, 1));
        }

        public void SendZoneInPackets(WorldManager world, ushort spawnType)
        {
            QueuePacket(SetActorIsZoningPacket.BuildPacket(actorId, false));
            QueuePacket(_0x10Packet.BuildPacket(actorId, 0xFF));
            QueuePacket(SetMusicPacket.BuildPacket(actorId, zone.bgmDay, 0x01));
            QueuePacket(SetWeatherPacket.BuildPacket(actorId, SetWeatherPacket.WEATHER_CLEAR, 1));

            QueuePacket(SetMapPacket.BuildPacket(actorId, zone.regionId, zone.actorId));

            QueuePackets(GetSpawnPackets(this, spawnType));            
            //GetSpawnPackets(actorId, spawnType).DebugPrintPacket();

            #region Inventory & Equipment
            QueuePacket(InventoryBeginChangePacket.BuildPacket(actorId));
            inventories[Inventory.NORMAL].SendFullInventory();
            inventories[Inventory.CURRENCY].SendFullInventory();
            inventories[Inventory.KEYITEMS].SendFullInventory();
            inventories[Inventory.BAZAAR].SendFullInventory();
            inventories[Inventory.MELDREQUEST].SendFullInventory();
            inventories[Inventory.LOOT].SendFullInventory();
            equipment.SendFullEquipment(false);   
            playerSession.QueuePacket(InventoryEndChangePacket.BuildPacket(actorId));
            #endregion

            playerSession.QueuePacket(GetInitPackets());

            List<SubPacket> areaMasterSpawn = zone.GetSpawnPackets();
            List<SubPacket> debugSpawn = world.GetDebugActor().GetSpawnPackets();
            List<SubPacket> worldMasterSpawn = world.GetActor().GetSpawnPackets();
            
            playerSession.QueuePacket(areaMasterSpawn);
            playerSession.QueuePacket(debugSpawn);
            playerSession.QueuePacket(worldMasterSpawn);

            //Inn Packets (Dream, Cutscenes, Armoire)

            if (zone.isInn)
            {
                SetCutsceneBookPacket cutsceneBookPacket = new SetCutsceneBookPacket();
                for (int i = 0; i < 2048; i++)
                    cutsceneBookPacket.cutsceneFlags[i] = true;
                SubPacket packet = cutsceneBookPacket.BuildPacket(actorId, "<Path Companion>", 11, 1, 1);

                packet.DebugPrintSubPacket();
                QueuePacket(packet);
                QueuePacket(SetPlayerItemStoragePacket.BuildPacket(actorId));
            }

            if (zone.GetWeatherDirector() != null)
            {
                playerSession.QueuePacket(zone.GetWeatherDirector().GetSpawnPackets());
            }

            
            foreach (Director director in ownedDirectors)
            {
                QueuePackets(director.GetSpawnPackets());
                QueuePackets(director.GetInitPackets());
            }

            if (currentContentGroup != null)
                currentContentGroup.SendGroupPackets(playerSession);

            if (currentParty != null)
                currentParty.SendGroupPackets(playerSession);
        }

        private void SendRemoveInventoryPackets(List<ushort> slots)
        {
            int currentIndex = 0;

            while (true)
            {
                if (slots.Count - currentIndex >= 64)
                    QueuePacket(InventoryRemoveX64Packet.BuildPacket(actorId, slots, ref currentIndex));
                else if (slots.Count - currentIndex >= 32)
                    QueuePacket(InventoryRemoveX32Packet.BuildPacket(actorId, slots, ref currentIndex));
                else if (slots.Count - currentIndex >= 16)
                    QueuePacket(InventoryRemoveX16Packet.BuildPacket(actorId, slots, ref currentIndex));
                else if (slots.Count - currentIndex >= 8)
                    QueuePacket(InventoryRemoveX08Packet.BuildPacket(actorId, slots, ref currentIndex));
                else if (slots.Count - currentIndex == 1)
                    QueuePacket(InventoryRemoveX01Packet.BuildPacket(actorId, slots[currentIndex]));
                else
                    break;
            }

        }

        public bool IsMyPlayer(uint otherActorId)
        {
            return actorId == otherActorId;
        }        

        public void QueuePacket(SubPacket packet)

        {
            playerSession.QueuePacket(packet);
        }

        public void QueuePackets(List<SubPacket> packets)
        {
            playerSession.QueuePacket(packets);
        }

        public void SendPacket(string path)
        {
            try
            {
                BasePacket packet = new BasePacket(path);

                packet.ReplaceActorID(actorId);
                var packets = packet.GetSubpackets();
                QueuePackets(packets);
            }
            catch (Exception e)
            {
                this.SendMessage(SendMessagePacket.MESSAGE_TYPE_SYSTEM_ERROR, "[SendPacket]", "Unable to send packet.");
                this.SendMessage(SendMessagePacket.MESSAGE_TYPE_SYSTEM_ERROR, "[SendPacket]", e.Message);
            }
        }

        public void BroadcastPacket(SubPacket packet, bool sendToSelf)
        {
            if (sendToSelf)
            {
                SubPacket clonedPacket = new SubPacket(packet, actorId);
                QueuePacket(clonedPacket);
            }

            foreach (Actor a in playerSession.actorInstanceList)
            {
                if (a is Player)
                {
                    Player p = (Player)a;

                    if (p.Equals(this))
                        continue;

                    SubPacket clonedPacket = new SubPacket(packet, a.actorId);
                    p.QueuePacket(clonedPacket);
                }
            }
        }

        public void ChangeAnimation(uint animId)
        {
            Actor a = zone.FindActorInArea(currentTarget);
            if (a is Npc)
                ((Npc)a).animationId = animId;
        }

        public void SetDCFlag(bool flag)
        {
            if (flag)
            {
                BroadcastPacket(SetActorIconPacket.BuildPacket(actorId, SetActorIconPacket.DISCONNECTING), true);
            }
            else
            {
                if (isGM)
                    BroadcastPacket(SetActorIconPacket.BuildPacket(actorId, SetActorIconPacket.ISGM), true);
                else
                    BroadcastPacket(SetActorIconPacket.BuildPacket(actorId, 0), true);
            }
        }

        public void CleanupAndSave()
        {
            playerSession.LockUpdates(true);

            //Remove actor from zone and main server list
            zone.RemoveActorFromZone(this);

            //Set Destination to 0
            this.destinationZone = 0;
            this.destinationSpawnType = 0;

            //Clean up parties
            RemoveFromCurrentPartyAndCleanup();

            //Save Player
            Database.SavePlayerPlayTime(this);
            Database.SavePlayerPosition(this);
            Database.SavePlayerStatusEffects(this);
        }

        public void CleanupAndSave(uint destinationZone, ushort spawnType, float destinationX, float destinationY, float destinationZ, float destinationRot)
        {
            playerSession.LockUpdates(true);

            //Remove actor from zone and main server list
            zone.RemoveActorFromZone(this);

            //Clean up parties
            RemoveFromCurrentPartyAndCleanup();

            //Set destination
            this.destinationZone = destinationZone;
            this.destinationSpawnType = spawnType;
            this.positionX = destinationX;
            this.positionY = destinationY;
            this.positionZ = destinationZ;
            this.rotation = destinationRot;

            //Save Player
            Database.SavePlayerPlayTime(this);
            Database.SavePlayerPosition(this);
            this.statusEffects.RemoveStatusEffectsByFlags((uint)StatusEffectFlags.LoseOnZoning, true);
            Database.SavePlayerStatusEffects(this);
        }

        public Area GetZone()
        {
            return zone;
        }

        public void SendMessage(uint logType, string sender, string message)
        {
            QueuePacket(SendMessagePacket.BuildPacket(actorId, logType, sender, message));
        }

        public void Logout()
        {
            // todo: really this should be in CleanupAndSave but we might want logout/disconnect handled separately for some effects
            QueuePacket(LogoutPacket.BuildPacket(actorId));
            statusEffects.RemoveStatusEffectsByFlags((uint)StatusEffectFlags.LoseOnLogout);
            CleanupAndSave();
        }

        public void QuitGame()
        {
            QueuePacket(QuitPacket.BuildPacket(actorId));
            statusEffects.RemoveStatusEffectsByFlags((uint)StatusEffectFlags.LoseOnLogout);
            CleanupAndSave();
        }

        public uint GetPlayTime(bool doUpdate)
        {
            if (doUpdate)
            {
                uint curTime = Utils.UnixTimeStampUTC();
                playTime += curTime - lastPlayTimeUpdate;
                lastPlayTimeUpdate = curTime;
            }

            return playTime;
        }

        public void SavePlayTime()
        {
            Database.SavePlayerPlayTime(this);
        }

        public void ChangeMusic(ushort musicId)
        {
            QueuePacket(SetMusicPacket.BuildPacket(actorId, musicId, 1));
        }

        public void SendMountAppearance()
        {
            if (mountState == 1)
                BroadcastPacket(SetCurrentMountChocoboPacket.BuildPacket(actorId, chocoboAppearance), true);
            else if (mountState == 2)
                BroadcastPacket(SetCurrentMountGoobbuePacket.BuildPacket(actorId, 1), true);
        }

        public void SetMountState(byte mountState)
        {
            this.mountState = mountState;
            SendMountAppearance();
        }

        public byte GetMountState()
        {
            return mountState;
        }

        public void DoEmote(uint targettedActor, uint animId, uint descId)
        {
            BroadcastPacket(ActorDoEmotePacket.BuildPacket(actorId, targettedActor, animId, descId), true);
        }

        public void SendGameMessage(Actor sourceActor, Actor textIdOwner, ushort textId, byte log, params object[] msgParams)
        {
            if (msgParams == null || msgParams.Length == 0)
            {
                QueuePacket(GameMessagePacket.BuildPacket(Server.GetWorldManager().GetActor().actorId, sourceActor.actorId, textIdOwner.actorId, textId, log));
            }
            else
                QueuePacket(GameMessagePacket.BuildPacket(Server.GetWorldManager().GetActor().actorId, sourceActor.actorId, textIdOwner.actorId, textId, log, LuaUtils.CreateLuaParamList(msgParams)));
        }

        public void SendGameMessage(Actor textIdOwner, ushort textId, byte log, params object[] msgParams)
        {
            if (msgParams == null || msgParams.Length == 0)
                QueuePacket(GameMessagePacket.BuildPacket(Server.GetWorldManager().GetActor().actorId, textIdOwner.actorId, textId, log));
            else
                QueuePacket(GameMessagePacket.BuildPacket(Server.GetWorldManager().GetActor().actorId, textIdOwner.actorId, textId, log, LuaUtils.CreateLuaParamList(msgParams)));
        }

        public void SendGameMessageCustomSender(Actor textIdOwner, ushort textId, byte log, string customSender, params object[] msgParams)
        {
            if (msgParams == null || msgParams.Length == 0)
                QueuePacket(GameMessagePacket.BuildPacket(Server.GetWorldManager().GetActor().actorId, textIdOwner.actorId, textId, customSender, log));
            else
                QueuePacket(GameMessagePacket.BuildPacket(Server.GetWorldManager().GetActor().actorId, textIdOwner.actorId, textId, customSender, log, LuaUtils.CreateLuaParamList(msgParams)));
        }

        public void SendGameMessageDisplayIDSender(Actor textIdOwner, ushort textId, byte log, uint displayId, params object[] msgParams)
        {
            if (msgParams == null || msgParams.Length == 0)
                QueuePacket(GameMessagePacket.BuildPacket(Server.GetWorldManager().GetActor().actorId, textIdOwner.actorId, textId, displayId, log));
            else
                QueuePacket(GameMessagePacket.BuildPacket(Server.GetWorldManager().GetActor().actorId, textIdOwner.actorId, textId, displayId, log, LuaUtils.CreateLuaParamList(msgParams)));
        }

        public void BroadcastWorldMessage(ushort worldMasterId, params object[] msgParams)
        {
            //SubPacket worldMasterMessage = 
            //zone.BroadcastPacketAroundActor(this, worldMasterMessage);
        }

        public void GraphicChange(uint slot, uint graphicId)
        {
            appearanceIds[slot] = graphicId;           
        }

        public void GraphicChange(uint slot, uint weapId, uint equipId, uint variantId, uint colorId)
        {

            uint mixedVariantId;

            if (weapId == 0)
                mixedVariantId = ((variantId & 0x1F) << 5) | colorId;
            else
                mixedVariantId = variantId;

            uint graphicId =
                    (weapId & 0x3FF)  << 20 |
                    (equipId & 0x3FF) << 10 |
                    (mixedVariantId & 0x3FF);

            appearanceIds[slot] = graphicId;            
            
        }

        public void SendAppearance()
        {
            BroadcastPacket(CreateAppearancePacket(), true);
        }

        public void SendCharaExpInfo()
        {
            if (lastStep == 0)
            {
                int maxLength;
                if ((sizeof(short) * charaWork.battleSave.skillLevel.Length)-lastPosition < 0x5E)
                    maxLength = (sizeof(short) * charaWork.battleSave.skillLevel.Length) - lastPosition;
                else
                    maxLength = 0x5E;

                byte[] skillLevelBuffer = new byte[maxLength];
                Buffer.BlockCopy(charaWork.battleSave.skillLevel, 0, skillLevelBuffer, 0, skillLevelBuffer.Length);
                SetActorPropetyPacket charaInfo1 = new SetActorPropetyPacket("charaWork/exp");

                charaInfo1.SetIsArrayMode(true);
                if (maxLength == 0x5E)
                {
                    charaInfo1.AddBuffer(Utils.MurmurHash2("charaWork.battleSave.skillLevel", 0), skillLevelBuffer, 0, skillLevelBuffer.Length, 0x0);
                    lastPosition += maxLength;
                }
                else
                {
                    charaInfo1.AddBuffer(Utils.MurmurHash2("charaWork.battleSave.skillLevel", 0), skillLevelBuffer, 0, skillLevelBuffer.Length, 0x3);
                    lastPosition = 0;
                    lastStep++;
                }

                charaInfo1.AddTarget();

                QueuePacket(charaInfo1.BuildPacket(actorId));
            }
            else if (lastStep == 1)
            {
                int maxLength;
                if ((sizeof(short) * charaWork.battleSave.skillLevelCap.Length) - lastPosition < 0x5E)
                    maxLength = (sizeof(short) * charaWork.battleSave.skillLevelCap.Length) - lastPosition;
                else
                    maxLength = 0x5E;

                byte[] skillCapBuffer = new byte[maxLength];
                Buffer.BlockCopy(charaWork.battleSave.skillLevelCap, lastPosition, skillCapBuffer, 0, skillCapBuffer.Length);
                SetActorPropetyPacket charaInfo1 = new SetActorPropetyPacket("charaWork/exp");

                
                if (maxLength == 0x5E)
                {
                    charaInfo1.SetIsArrayMode(true);
                    charaInfo1.AddBuffer(Utils.MurmurHash2("charaWork.battleSave.skillLevelCap", 0), skillCapBuffer, 0, skillCapBuffer.Length, 0x1);
                    lastPosition += maxLength;
                }
                else
                {
                    charaInfo1.SetIsArrayMode(false);
                    charaInfo1.AddBuffer(Utils.MurmurHash2("charaWork.battleSave.skillLevelCap", 0), skillCapBuffer, 0, skillCapBuffer.Length, 0x3);
                    lastStep = 0;
                    lastPosition = 0;
                }

                charaInfo1.AddTarget();

                QueuePacket(charaInfo1.BuildPacket(actorId));
            }
           
        }

        public int GetHighestLevel()
        {
            int max = 0;
            foreach (short level in charaWork.battleSave.skillLevel)
            {
                if (level > max)
                    max = level;
            }
            return max;
        }

        public InventoryItem[] GetGearset(ushort classId)
        {
            return Database.GetEquipment(this, classId);
        }

        public void PrepareClassChange(byte classId)
        {            
            //If new class, init abilties and level

            SendCharaExpInfo();
        }

        public void DoClassChange(byte classId)
        {
            //load hotbars
            //Calculate stats
            //Calculate hp/mp

            //Get Potenciel ??????

            //Set HP/MP/TP PARAMS

            //Set mainskill and level

            //Set Parameters

            //Set current EXP

            //Set Hotbar Commands 1
            //Set Hotbar Commands 2
            //Set Hotbar Commands 3

            //Check if bonus point available... set

            //Set rested EXP
            charaWork.parameterSave.state_mainSkill[0] = classId;
            charaWork.parameterSave.state_mainSkillLevel = charaWork.battleSave.skillLevel[classId-1];
            playerWork.restBonusExpRate = 0.0f;
            for(int i = charaWork.commandBorder; i < charaWork.command.Length; i++)
            {
                charaWork.command[i] = 0;
                charaWork.commandCategory[i] = 0;
            }

            ActorPropertyPacketUtil propertyBuilder = new ActorPropertyPacketUtil("charaWork/stateForAll", this);

            propertyBuilder.AddProperty("charaWork.parameterSave.state_mainSkill[0]");
            propertyBuilder.AddProperty("charaWork.parameterSave.state_mainSkillLevel");
            propertyBuilder.NewTarget("playerWork/expBonus");
            propertyBuilder.AddProperty("playerWork.restBonusExpRate");
            propertyBuilder.NewTarget("charaWork/battleStateForSelf");
            propertyBuilder.AddProperty(String.Format("charaWork.battleSave.skillPoint[{0}]", classId - 1));
            Database.LoadHotbar(this);

            var time = Utils.UnixTimeStampUTC();
            for(int i = charaWork.commandBorder; i < charaWork.command.Length; i++)
            {
                if(charaWork.command[i] != 0)
                {
                    charaWork.parameterSave.commandSlot_recastTime[i - charaWork.commandBorder] = time + charaWork.parameterTemp.maxCommandRecastTime[i - charaWork.commandBorder];
                }
            }

            UpdateHotbar();

            List<SubPacket> packets = propertyBuilder.Done();

            foreach (SubPacket packet in packets)
                BroadcastPacket(packet, true);

            Database.SavePlayerCurrentClass(this);
            RecalculateStats();
        }

        public void GraphicChange(int slot, InventoryItem invItem)
        {
            if (invItem == null)            
                appearanceIds[slot] = 0;            
            else
            {
                ItemData item = Server.GetItemGamedata(invItem.itemId);

                if (item is EquipmentItem)
                {
                    EquipmentItem eqItem = (EquipmentItem)item;

                    uint mixedVariantId;
                    
                    if (eqItem.graphicsWeaponId == 0)
                        mixedVariantId = ((eqItem.graphicsVariantId & 0x1F) << 5) | eqItem.graphicsColorId;
                    else
                        mixedVariantId = eqItem.graphicsVariantId;

                    uint graphicId =
                            (eqItem.graphicsWeaponId & 0x3FF) << 20 |
                            (eqItem.graphicsEquipmentId & 0x3FF) << 10 |
                            (mixedVariantId & 0x3FF);

                    appearanceIds[slot] = graphicId;
                }

                //Handle offhand
                if (slot == MAINHAND && item is WeaponItem)
                {
                    WeaponItem wpItem = (WeaponItem)item;

                    uint graphicId =
                            (wpItem.graphicsOffhandWeaponId & 0x3FF) << 20 |
                            (wpItem.graphicsOffhandEquipmentId & 0x3FF) << 10 |
                            (wpItem.graphicsOffhandVariantId & 0x3FF);

                    appearanceIds[SetActorAppearancePacket.OFFHAND] = graphicId;
                }
            }

            Database.SavePlayerAppearance(this);
            BroadcastPacket(CreateAppearancePacket(), true);
        }

        public Inventory GetInventory(ushort type)
        {
            if (inventories.ContainsKey(type))
                return inventories[type];
            else
                return null;
        }

        public int GetCurrentGil()
        {
            if (GetInventory(Inventory.CURRENCY).HasItem(1000001))
                return GetInventory(Inventory.CURRENCY).GetItemByCatelogId(1000001).quantity;
            else
                return 0;
        }

        public Actor GetActorInInstance(uint actorId)
        {
            foreach (Actor a in playerSession.actorInstanceList)
            {
                if (a.actorId == actorId)
                    return a;
            }

            return null;
        }

        public void SetZoneChanging(bool flag)
        {
            isZoneChanging = flag;
        }

        public bool IsInZoneChange()
        {
            return isZoneChanging;
        }

        public Equipment GetEquipment()
        {
            return equipment;
        }     

        public byte GetInitialTown()
        {
            return playerWork.initialTown;
        }

        public uint GetHomePoint()
        {
            return homepoint;
        }

        public byte GetHomePointInn()
        {
            return homepointInn;
        }

        public void SetHomePoint(uint aetheryteId)
        {            
            homepoint = aetheryteId;
            Database.SavePlayerHomePoints(this);
        }

        public void SetHomePointInn(byte townId)
        {
            homepointInn = townId;
            Database.SavePlayerHomePoints(this);
        }

        public bool HasAetheryteNodeUnlocked(uint aetheryteId)
        {
            if (aetheryteId != 0)
                return true;
            else
                return false;
        }

        public int GetFreeQuestSlot()
        {
            for (int i = 0; i < questScenario.Length; i++)
            {
                if (questScenario[i] == null)
                    return i;
            }

            return -1;
        }

        public int GetFreeGuildleveSlot()
        {
            for (int i = 0; i < work.guildleveId.Length; i++)
            {
                if (work.guildleveId[i] == 0)
                    return i;
            }

            return -1;
        }

        //For Lua calls, cause MoonSharp goes retard with uint
        public void AddQuest(int id, bool isSilent = false)
        {
            AddQuest((uint)id, isSilent);
        }       
        public void CompleteQuest(int id)
        {
            CompleteQuest((uint)id);
        }
        public bool HasQuest(int id)
        {
            return HasQuest((uint)id);
        }
        public Quest GetQuest(int id)
        {
            return GetQuest((uint)id);
        }
        public bool IsQuestCompleted(int id)
        {
            return IsQuestCompleted((uint)id);
        }
        public bool CanAcceptQuest(int id)
        {
            return CanAcceptQuest((uint)id);
        }
        //For Lua calls, cause MoonSharp goes retard with uint

        public void AddGuildleve(uint id)
        {
            int freeSlot = GetFreeGuildleveSlot();

            if (freeSlot == -1)
                return;

            work.guildleveId[freeSlot] = (ushort)id;
            Database.SaveGuildleve(this, id, freeSlot);
            SendGuildleveClientUpdate(freeSlot);
        }

        public void MarkGuildleve(uint id, bool abandoned, bool completed)
        {
            if (HasGuildleve(id))
            {
                for (int i = 0; i < work.guildleveId.Length; i++)
                {
                    if (work.guildleveId[i] == id)
                    {
                        work.guildleveChecked[i] = completed;
                        work.guildleveDone[i] = abandoned;
                        Database.MarkGuildleve(this, id, abandoned, completed);
                        SendGuildleveMarkClientUpdate(i);
                    }
                }
            }
        }

        public void RemoveGuildleve(uint id)
        {
            if (HasGuildleve(id))
            {
                for (int i = 0; i < work.guildleveId.Length; i++)
                {
                    if (work.guildleveId[i] == id)
                    {
                        Database.RemoveGuildleve(this, id);
                        work.guildleveId[i] = 0;
                        SendGuildleveClientUpdate(i);
                        break;
                    }
                }
            }
        }

        public void AddQuest(uint id, bool isSilent = false)
        {
            Actor actor = Server.GetStaticActors((0xA0F00000 | id));
            AddQuest(actor.actorName, isSilent);
        }

        public void AddQuest(string name, bool isSilent = false)
        {
            Actor actor = Server.GetStaticActors(name);

            if (actor == null)
                return;

            uint id = actor.actorId;

            int freeSlot = GetFreeQuestSlot();

            if (freeSlot == -1)
                return;

            playerWork.questScenario[freeSlot] = id;
            questScenario[freeSlot] = new Quest(this, playerWork.questScenario[freeSlot], name, null, 0, 0);
            Database.SaveQuest(this, questScenario[freeSlot]);
            SendQuestClientUpdate(freeSlot);

            if (!isSilent)
            {
                SendGameMessage(Server.GetWorldManager().GetActor(), 25224, 0x20, (object)questScenario[freeSlot].GetQuestId());
                questScenario[freeSlot].NextPhase(0);
            }
        }        

        public void CompleteQuest(uint id)
        {
            Actor actor = Server.GetStaticActors((0xA0F00000 | id));
            CompleteQuest(actor.actorName);
        }

        public void CompleteQuest(string name)
        {
            Actor actor = Server.GetStaticActors(name);

            if (actor == null)
                return;

            uint id = actor.actorId;
            if (HasQuest(id))
            {
                Database.CompleteQuest(playerSession.GetActor(), id);
                SendGameMessage(Server.GetWorldManager().GetActor(), 25086, 0x20, (object)GetQuest(id).GetQuestId());
                RemoveQuest(id);
            }
        }

        //TODO: Add checks for you being in an instance or main scenario
        public void AbandonQuest(uint id)
        {
            Quest quest = GetQuest(id);
            RemoveQuestByQuestId(id);
            quest.DoAbandon();       
        }

        public void RemoveQuestByQuestId(uint id)
        {
            RemoveQuest((0xA0F00000 | id));
        }

        public void RemoveQuest(uint id)
        {
            if (HasQuest(id))
            {
                for (int i = 0; i < questScenario.Length; i++)
                {
                    if (questScenario[i] != null && questScenario[i].actorId == id)
                    {
                        Database.RemoveQuest(this, questScenario[i].actorId);
                        questScenario[i] = null;
                        playerWork.questScenario[i] = 0;
                        SendQuestClientUpdate(i);
                        break;
                    }
                }
            }
        }

        public void ReplaceQuest(uint oldId, uint newId)
        {
            if (HasQuest(oldId))
            {
                for (int i = 0; i < questScenario.Length; i++)
                {
                    if (questScenario[i] != null && questScenario[i].GetQuestId() == oldId)
                    {
                        Actor actor = Server.GetStaticActors((0xA0F00000 | newId));
                        playerWork.questScenario[i] = (0xA0F00000 | newId);
                        questScenario[i] = new Quest(this, playerWork.questScenario[i], actor.actorName, null, 0, 0);
                        Database.SaveQuest(this, questScenario[i]);
                        SendQuestClientUpdate(i);
                        break;
                    }
                }
            }
        }

        public bool CanAcceptQuest(string name)
        {
            if (!IsQuestCompleted(name) && !HasQuest(name))
                return true;
            else
                return false;
        }

        public bool CanAcceptQuest(uint id)
        {
            Actor actor = Server.GetStaticActors((0xA0F00000 | id));
            return CanAcceptQuest(actor.actorName);
        }

        public bool IsQuestCompleted(string questName)
        {
            Actor actor = Server.GetStaticActors(questName);
            return IsQuestCompleted(actor.actorId);
        }

        public bool IsQuestCompleted(uint questId)
        {
            return Database.IsQuestCompleted(this, 0xFFFFF & questId);
        }

        public Quest GetQuest(uint id)
        {
            for (int i = 0; i < questScenario.Length; i++)
            {
                if (questScenario[i] != null && questScenario[i].actorId == (0xA0F00000 | id))
                    return questScenario[i];
            }

            return null;
        }

        public Quest GetQuest(string name)
        {
            for (int i = 0; i < questScenario.Length; i++)
            {
                if (questScenario[i] != null && questScenario[i].actorName.ToLower().Equals(name.ToLower()))
                    return questScenario[i];
            }

            return null;
        }

        public bool HasQuest(string name)
        {
            for (int i = 0; i < questScenario.Length; i++)
            {
                if (questScenario[i] != null && questScenario[i].actorName.ToLower().Equals(name.ToLower()))
                    return true;
            }

            return false;
        }

        public bool HasQuest(uint id)
        {
            for (int i = 0; i < questScenario.Length; i++)
            {
                if (questScenario[i] != null && questScenario[i].actorId == (0xA0F00000 | id))
                    return true;
            }

            return false;
        }

        public bool HasGuildleve(uint id)
        {
            for (int i = 0; i < work.guildleveId.Length; i++)
            {
                if (work.guildleveId[i] == id)
                    return true;
            }

            return false;
        }

        public int GetQuestSlot(uint id)
        {
            for (int i = 0; i < questScenario.Length; i++)
            {
                if (questScenario[i] != null && questScenario[i].actorId == (0xA0F00000 | id))
                    return i;
            }

            return -1;
        }

        public void SetNpcLS(uint npcLSId, uint state)
        {            
            bool isCalling, isExtra;
            isCalling = isExtra = false;

            switch (state)
            {
                case NPCLS_INACTIVE:

                    if (playerWork.npcLinkshellChatExtra[npcLSId] == true && playerWork.npcLinkshellChatCalling[npcLSId] == false)
                        return;

                    isExtra = true;
                    break;
                case NPCLS_ACTIVE:

                    if (playerWork.npcLinkshellChatExtra[npcLSId] == false && playerWork.npcLinkshellChatCalling[npcLSId] == true)
                        return;

                    isCalling = true;
                    break;
                case NPCLS_ALERT:

                    if (playerWork.npcLinkshellChatExtra[npcLSId] == true && playerWork.npcLinkshellChatCalling[npcLSId] == true)
                        return;

                    isExtra = isCalling = true;
                    break;
            }

            playerWork.npcLinkshellChatExtra[npcLSId] = isExtra;
            playerWork.npcLinkshellChatCalling[npcLSId] = isCalling;

            Database.SaveNpcLS(this, npcLSId, isCalling, isExtra);

            ActorPropertyPacketUtil propPacketUtil = new ActorPropertyPacketUtil("playerWork/npcLinkshellChat", this);
            propPacketUtil.AddProperty(String.Format("playerWork.npcLinkshellChatExtra[{0}]", npcLSId));
            propPacketUtil.AddProperty(String.Format("playerWork.npcLinkshellChatCalling[{0}]", npcLSId));
            QueuePackets(propPacketUtil.Done());
        }

        private void SendQuestClientUpdate(int slot)
        {
            ActorPropertyPacketUtil propPacketUtil = new ActorPropertyPacketUtil("playerWork/journal", this);
            propPacketUtil.AddProperty(String.Format("playerWork.questScenario[{0}]", slot));
            QueuePackets(propPacketUtil.Done());
        }

        private void SendGuildleveClientUpdate(int slot)
        {
            ActorPropertyPacketUtil propPacketUtil = new ActorPropertyPacketUtil("work/guildleve", this);
            propPacketUtil.AddProperty(String.Format("work.guildleveId[{0}]", slot));
            QueuePackets(propPacketUtil.Done());
        }

        private void SendGuildleveMarkClientUpdate(int slot)
        {
            ActorPropertyPacketUtil propPacketUtil = new ActorPropertyPacketUtil("work/guildleve", this);
            propPacketUtil.AddProperty(String.Format("work.guildleveDone[{0}]", slot));
            propPacketUtil.AddProperty(String.Format("work.guildleveChecked[{0}]", slot));
            QueuePackets(propPacketUtil.Done());
        }

        public void SendStartCastbar(uint commandId, uint endTime)
        {
            playerWork.castCommandClient = commandId;
            playerWork.castEndClient = endTime;
            ActorPropertyPacketUtil propPacketUtil = new ActorPropertyPacketUtil("playerWork/castState", this);
            propPacketUtil.AddProperty("playerWork.castEndClient");
            propPacketUtil.AddProperty("playerWork.castCommandClient");
            QueuePackets(propPacketUtil.Done());
        }

        public void SendEndCastbar()
        {
            playerWork.castCommandClient = 0;
            playerWork.castEndClient = 0;
            ActorPropertyPacketUtil propPacketUtil = new ActorPropertyPacketUtil("playerWork/castState", this);
            propPacketUtil.AddProperty("playerWork.castCommandClient");
            QueuePackets(propPacketUtil.Done());
        }

        public void SetLoginDirector(Director director)
        {
            if (ownedDirectors.Contains(director))
                loginInitDirector = director;
        }

        public void AddDirector(Director director, bool spawnImmediatly = false)
        {            
            if (!ownedDirectors.Contains(director))
            {
                ownedDirectors.Add(director);
                director.AddMember(this);                
            }
        }

        public void SendDirectorPackets(Director director)
        {
            QueuePackets(director.GetSpawnPackets());
            QueuePackets(director.GetInitPackets());        
        }

        public void RemoveDirector(Director director)
        {
            if (ownedDirectors.Contains(director))
            {
                QueuePacket(RemoveActorPacket.BuildPacket(director.actorId));
                ownedDirectors.Remove(director);
                director.RemoveMember(this);
            }
        }
        
        public GuildleveDirector GetGuildleveDirector()
        {
            foreach (Director d in ownedDirectors)
            {
                if (d is GuildleveDirector)
                    return (GuildleveDirector)d;
            }

            return null;
        }

        public Director GetDirector(string directorName)
        {
            foreach (Director d in ownedDirectors)
            {
                if (d.GetScriptPath().Equals(directorName))                
                    return d;                
            }

            return null;
        }

        public Director GetDirector(uint id)
        {
            foreach (Director d in ownedDirectors)
            {
                if (d.actorId == id)
                    return d;
            }

            return null;
        }

        public void ExaminePlayer(Actor examinee)
        {
            Player toBeExamined;
            if (examinee is Player)
                toBeExamined = (Player)examinee;
            else
                return;

            QueuePacket(InventoryBeginChangePacket.BuildPacket(toBeExamined.actorId));
            toBeExamined.GetEquipment().SendCheckEquipmentToPlayer(this);
            QueuePacket(InventoryEndChangePacket.BuildPacket(toBeExamined.actorId));
        }

        public void SendDataPacket(params object[] parameters)
        {
            List<LuaParam> lParams = LuaUtils.CreateLuaParamList(parameters);
            SubPacket spacket = GenericDataPacket.BuildPacket(actorId, lParams);
            spacket.DebugPrintSubPacket();
            QueuePacket(spacket);
        }

        public void StartEvent(Actor owner, EventStartPacket start)
        {
            LuaEngine.GetInstance().EventStarted(this, owner, start);
        }

        public void UpdateEvent(EventUpdatePacket update)
        {
            LuaEngine.GetInstance().OnEventUpdate(this, update.luaParams);            
        } 

        public void KickEvent(Actor actor, string conditionName, params object[] parameters)
        {
            if (actor == null)
                return;

            List<LuaParam> lParams = LuaUtils.CreateLuaParamList(parameters);
            SubPacket spacket = KickEventPacket.BuildPacket(actorId, actor.actorId, conditionName, lParams);
            spacket.DebugPrintSubPacket();
            QueuePacket(spacket);
        }

        public void SetEventStatus(Actor actor, string conditionName, bool enabled, byte unknown)
        {
            QueuePacket(packets.send.actor.events.SetEventStatus.BuildPacket(actor.actorId, enabled, unknown, conditionName));
        }

        public void RunEventFunction(string functionName, params object[] parameters)
        {
            List<LuaParam> lParams = LuaUtils.CreateLuaParamList(parameters);
            SubPacket spacket = RunEventFunctionPacket.BuildPacket(actorId, currentEventOwner, currentEventName, functionName, lParams);
            spacket.DebugPrintSubPacket();
            QueuePacket(spacket);
        }

        public void EndEvent()
        {
            SubPacket p = EndEventPacket.BuildPacket(actorId, currentEventOwner, currentEventName);
            p.DebugPrintSubPacket();
            QueuePacket(p);

            currentEventOwner = 0;
            currentEventName = "";
            currentEventRunning = null;
        }
        
        public void SendInstanceUpdate()
        {
            //Server.GetWorldManager().SeamlessCheck(this);

            //Update Instance
            List<Actor> aroundMe = new List<Actor>();

            if (zone != null)
                aroundMe.AddRange(zone.GetActorsAroundActor(this, 50));
            if (zone2 != null)
                aroundMe.AddRange(zone2.GetActorsAroundActor(this, 50));
            playerSession.UpdateInstance(aroundMe);
        }

        public bool IsInParty()
        {
            return currentParty != null;
        }

        public bool IsPartyLeader()
        {
            if (IsInParty())
            {
                Party party = (Party)currentParty;
                return party.GetLeader() == actorId;
            }
            else
                return false;
        }

        public void PartyOustPlayer(uint actorId)
        {
            SubPacket oustPacket = PartyModifyPacket.BuildPacket(playerSession, 1, actorId);
            QueuePacket(oustPacket);
        }

        public void PartyOustPlayer(string name)
        {
            SubPacket oustPacket = PartyModifyPacket.BuildPacket(playerSession, 1, name);
            QueuePacket(oustPacket);
        }

        public void PartyLeave()
        {
            SubPacket leavePacket = PartyLeavePacket.BuildPacket(playerSession, false);
            QueuePacket(leavePacket);
        }

        public void PartyDisband()
        {
            SubPacket disbandPacket = PartyLeavePacket.BuildPacket(playerSession, true);
            QueuePacket(disbandPacket);
        }

        public void PartyPromote(uint actorId)
        {
            SubPacket promotePacket = PartyModifyPacket.BuildPacket(playerSession, 0, actorId);
            QueuePacket(promotePacket);
        }

        public void PartyPromote(string name)
        {
            SubPacket promotePacket = PartyModifyPacket.BuildPacket(playerSession, 0, name);
            QueuePacket(promotePacket);
        }

        //A party member list packet came, set the party
        public void SetParty(Party group)
        {
            if (group is Party && currentParty != group)
            {
                RemoveFromCurrentPartyAndCleanup();
                currentParty = group;
            }
        }

        //Removes the player from the party and cleans it up if needed
        public void RemoveFromCurrentPartyAndCleanup()
        {
            if (currentParty == null)
                return;

            Party partyGroup = (Party) currentParty;

            for (int i = 0; i < partyGroup.members.Count; i++)
            {
                if (partyGroup.members[i] == actorId)
                {
                    partyGroup.members.RemoveAt(i);
                    break;
                }
            }

            //currentParty.members.Remove(this);
            if (partyGroup.members.Count == 0)
                Server.GetWorldManager().NoMembersInParty((Party)currentParty);

            currentParty = null;
        }
        
        public void IssueChocobo(byte appearanceId, string nameResponse)
        {
            Database.IssuePlayerChocobo(this, appearanceId, nameResponse);
            hasChocobo = true;
            chocoboAppearance = appearanceId;
            chocoboName = nameResponse;
        }

        public void ChangeChocoboAppearance(byte appearanceId)
        {
            Database.ChangePlayerChocoboAppearance(this, appearanceId);
            chocoboAppearance = appearanceId;
        }

        public override void Update(DateTime tick)
        {
            aiContainer.Update(tick);
            statusEffects.Update(tick);
        }

        public override void PostUpdate(DateTime tick, List<SubPacket> packets = null)
        {
            // todo: is this correct?
            if (this.playerSession.isUpdatesLocked)
                return;

            // todo: should probably add another flag for battleTemp since all this uses reflection
            packets = new List<SubPacket>();

            // we only want the latest update for the player
            if ((updateFlags & ActorUpdateFlags.Position) != 0)
            {
                if (positionUpdates.Count > 1)
                    positionUpdates.RemoveRange(1, positionUpdates.Count - 1);
            }

            if ((updateFlags & ActorUpdateFlags.HpTpMp) != 0)
            {
                var propPacketUtil = new ActorPropertyPacketUtil("charaWork/stateAtQuicklyForAll", this);

                // todo: should this be using job as index?
                propPacketUtil.AddProperty("charaWork.parameterSave.hp[0]");
                propPacketUtil.AddProperty("charaWork.parameterSave.hpMax[0]");
                propPacketUtil.AddProperty("charaWork.parameterSave.state_mainSkill[0]");
                propPacketUtil.AddProperty("charaWork.parameterSave.state_mainSkillLevel");

                packets.AddRange(propPacketUtil.Done());
            }


            if ((updateFlags & ActorUpdateFlags.Stats) != 0)
            {
                var propPacketUtil = new ActorPropertyPacketUtil("charaWork/battleParameter", this);

                for (uint i = 0; i < 35; i++)
                {
                    if (GetMod(i) != charaWork.battleTemp.generalParameter[i])
                    {
                        charaWork.battleTemp.generalParameter[i] = (short)GetMod(i);
                        propPacketUtil.AddProperty(String.Format("charaWork.battleTemp.generalParameter[{0}]", i));
                    }
                }

                QueuePackets(propPacketUtil.Done());

            }


            base.PostUpdate(tick, packets);
        }

        public override void Die(DateTime tick, CommandResultContainer actionContainer = null)
        {
            // todo: death timer
            aiContainer.InternalDie(tick, 60);
        }

        //Update commands and recast timers for the entire hotbar
        public void UpdateHotbar()
        {
            List<ushort> slotsToUpdate = new List<ushort>();
            for (ushort i = charaWork.commandBorder; i < charaWork.commandBorder + 30; i++)
            {
                slotsToUpdate.Add(i);
            }
            UpdateHotbar(slotsToUpdate);
        }

        //Updates the hotbar and recast timers for only certain hotbar slots
        public void UpdateHotbar(List<ushort> slotsToUpdate)
        {
            UpdateHotbarCommands(slotsToUpdate);
            UpdateRecastTimers(slotsToUpdate);
        }

        //Update command ids for the passed in hotbar slots
        public void UpdateHotbarCommands(List<ushort> slotsToUpdate)
        {
            ActorPropertyPacketUtil propPacketUtil = new ActorPropertyPacketUtil("charaWork/command", this);
            foreach (ushort slot in slotsToUpdate)
            {
                propPacketUtil.AddProperty(String.Format("charaWork.command[{0}]", slot));
                propPacketUtil.AddProperty(String.Format("charaWork.commandCategory[{0}]", slot));
            }

            propPacketUtil.NewTarget("charaWork/commandDetailForSelf");
            //Enable or disable slots based on whether there is an ability in that slot
            foreach (ushort slot in slotsToUpdate)
            {
                charaWork.parameterSave.commandSlot_compatibility[slot - charaWork.commandBorder] = charaWork.command[slot] != 0;
                propPacketUtil.AddProperty(String.Format("charaWork.parameterSave.commandSlot_compatibility[{0}]", slot - charaWork.commandBorder));
            }

            QueuePackets(propPacketUtil.Done());
            //QueuePackets(compatibiltyUtil.Done());
        }

        //Update recast timers for the passed in hotbar slots
        public void UpdateRecastTimers(List<ushort> slotsToUpdate)
        {
            ActorPropertyPacketUtil recastPacketUtil = new ActorPropertyPacketUtil("charaWork/commandDetailForSelf", this);

            foreach (ushort slot in slotsToUpdate)
            {
                recastPacketUtil.AddProperty(String.Format("charaWork.parameterTemp.maxCommandRecastTime[{0}]", slot - charaWork.commandBorder));
                recastPacketUtil.AddProperty(String.Format("charaWork.parameterSave.commandSlot_recastTime[{0}]", slot - charaWork.commandBorder));
            }

            QueuePackets(recastPacketUtil.Done());
        }

        //Find the first open slot in classId's hotbar and equip an ability there.
        public void EquipAbilityInFirstOpenSlot(byte classId, uint commandId, bool printMessage = true)
        {
            //Find first open slot on class's hotbar slot, then call EquipAbility with that slot.
            ushort hotbarSlot = 0;

            //If the class we're equipping for is the current class, we can just look at charawork.command
            if(classId == charaWork.parameterSave.state_mainSkill[0])
                hotbarSlot = FindFirstCommandSlotById(0);
            //Otherwise, we need to check the database.
            else
                hotbarSlot = (ushort) (Database.FindFirstCommandSlot(this, classId) + charaWork.commandBorder);

            EquipAbility(classId, commandId, hotbarSlot, printMessage);
        }

        //Add commandId to classId's hotbar at hotbarSlot.
        //If classId is not the current class, do it in the database
        //hotbarSlot starts at 32
        public void EquipAbility(byte classId, uint commandId, ushort hotbarSlot, bool printMessage = true)
        {
            var ability = Server.GetWorldManager().GetBattleCommand(commandId);
            uint trueCommandId = 0xA0F00000 + commandId;
            ushort lowHotbarSlot = (ushort)(hotbarSlot - charaWork.commandBorder);
            ushort maxRecastTime = (ushort)(ability != null ? ability.maxRecastTimeSeconds : 5);
            uint recastEnd = Utils.UnixTimeStampUTC() + maxRecastTime;
            List<ushort> slotsToUpdate = new List<ushort>();
            
            Database.EquipAbility(this, classId, (ushort) (hotbarSlot - charaWork.commandBorder), commandId, recastEnd);
            //If the class we're equipping for is the current class (need to find out if state_mainSkill is supposed to change when you're a job)
            //then equip the ability in charawork.commands and save in databse, otherwise just save in database
            if (classId == GetCurrentClassOrJob())
            {
                charaWork.command[hotbarSlot] = trueCommandId;
                charaWork.commandCategory[hotbarSlot] = 1;
                charaWork.parameterTemp.maxCommandRecastTime[lowHotbarSlot] = maxRecastTime;
                charaWork.parameterSave.commandSlot_recastTime[lowHotbarSlot] = recastEnd;

                slotsToUpdate.Add(hotbarSlot);
                UpdateHotbar(slotsToUpdate);
            }


            if(printMessage)
                SendGameMessage(Server.GetWorldManager().GetActor(), 30603, 0x20, 0, commandId);
        }

        //Doesn't take a classId because the only way to swap abilities is through the ability equip widget oe /eaction, which only apply to current class
        //hotbarSlot 1 and 2 are 32-indexed.
        public void SwapAbilities(ushort hotbarSlot1, ushort hotbarSlot2)
        {
            //0 indexed hotbar slots for saving to database and recast timers
            uint lowHotbarSlot1 = (ushort)(hotbarSlot1 - charaWork.commandBorder);
            uint lowHotbarSlot2 = (ushort)(hotbarSlot2 - charaWork.commandBorder);
            
            //Store information about first command
            uint commandId = charaWork.command[hotbarSlot1];
            uint recastEnd = charaWork.parameterSave.commandSlot_recastTime[lowHotbarSlot1];
            ushort recastMax = charaWork.parameterTemp.maxCommandRecastTime[lowHotbarSlot1];

            //Move second command's info to first hotbar slot
            charaWork.command[hotbarSlot1] = charaWork.command[hotbarSlot2];
            charaWork.parameterTemp.maxCommandRecastTime[lowHotbarSlot1] = charaWork.parameterTemp.maxCommandRecastTime[lowHotbarSlot2];
            charaWork.parameterSave.commandSlot_recastTime[lowHotbarSlot1] = charaWork.parameterSave.commandSlot_recastTime[lowHotbarSlot2];

            //Move first command's info to second slot
            charaWork.command[hotbarSlot2] = commandId;
            charaWork.parameterTemp.maxCommandRecastTime[lowHotbarSlot2] = recastMax;
            charaWork.parameterSave.commandSlot_recastTime[lowHotbarSlot2] = recastEnd;

            //Save changes to both slots
            Database.EquipAbility(this, GetCurrentClassOrJob(), (ushort)(lowHotbarSlot1), 0xA0F00000 ^ charaWork.command[hotbarSlot1], charaWork.parameterSave.commandSlot_recastTime[lowHotbarSlot1]);
            Database.EquipAbility(this, GetCurrentClassOrJob(), (ushort)(lowHotbarSlot2), 0xA0F00000 ^ charaWork.command[hotbarSlot2], charaWork.parameterSave.commandSlot_recastTime[lowHotbarSlot2]);

            //Update slots on client
            List<ushort> slotsToUpdate = new List<ushort>();
            slotsToUpdate.Add(hotbarSlot1);
            slotsToUpdate.Add(hotbarSlot2);
            UpdateHotbar(slotsToUpdate);
        }

        public void UnequipAbility(ushort hotbarSlot, bool printMessage = true)
        {
            List<ushort> slotsToUpdate = new List<ushort>();
            ushort trueHotbarSlot = (ushort)(hotbarSlot + charaWork.commandBorder - 1);
            uint commandId = charaWork.command[trueHotbarSlot];
            Database.UnequipAbility(this, (ushort)(trueHotbarSlot - charaWork.commandBorder));
            charaWork.command[trueHotbarSlot] = 0;
            slotsToUpdate.Add(trueHotbarSlot);

            if(printMessage)
                SendGameMessage(Server.GetWorldManager().GetActor(), 30604, 0x20, 0, 0xA0F00000 ^ commandId);

            UpdateHotbar(slotsToUpdate);
        }

        //Finds the first hotbar slot with a given commandId.
        //If the returned value is outside the hotbar, it indicates it wasn't found.
        public ushort FindFirstCommandSlotById(uint commandId)
        {
            if(commandId != 0)
                commandId |= 0xA0F00000;

            ushort firstSlot = (ushort)(charaWork.commandBorder + 30);

            for (ushort i = charaWork.commandBorder; i < charaWork.commandBorder + 30; i++)
            {
                if (charaWork.command[i] == commandId)
                {
                    firstSlot = i;
                    break;
                }
            }

            return firstSlot;
        }
        
        private void UpdateHotbarTimer(uint commandId, uint recastTimeMs)
        {
            ushort slot = FindFirstCommandSlotById(commandId);
            charaWork.parameterSave.commandSlot_recastTime[slot - charaWork.commandBorder] = Utils.UnixTimeStampUTC(DateTime.Now.AddMilliseconds(recastTimeMs));
            var slots = new List<ushort>();
            slots.Add(slot);
            UpdateRecastTimers(slots);
        }

        private uint GetHotbarTimer(uint commandId)
        {
            ushort slot = FindFirstCommandSlotById(commandId);
            return charaWork.parameterSave.commandSlot_recastTime[slot - charaWork.commandBorder];
        }

        public override void Cast(uint spellId, uint targetId = 0)
        {
            if (aiContainer.CanChangeState())
                aiContainer.Cast(zone.FindActorInArea<Character>(targetId == 0 ? currentTarget : targetId), spellId);
            else if (aiContainer.IsCurrentState<MagicState>())
                // You are already casting.
                SendGameMessage(Server.GetWorldManager().GetActor(), 32536, 0x20);
            else
                // Please wait a moment and try again.
                SendGameMessage(Server.GetWorldManager().GetActor(), 32535, 0x20);
        }

        public override void Ability(uint abilityId, uint targetId = 0)
        {
            if (aiContainer.CanChangeState())
                aiContainer.Ability(zone.FindActorInArea<Character>(targetId == 0 ? currentTarget : targetId), abilityId);
            else
                // Please wait a moment and try again.
                SendGameMessage(Server.GetWorldManager().GetActor(), 32535, 0x20);
        }

        public override void WeaponSkill(uint skillId, uint targetId = 0)
        {
            if (aiContainer.CanChangeState())
                aiContainer.WeaponSkill(zone.FindActorInArea<Character>(targetId == 0 ? currentTarget : targetId), skillId);
            else
                // Please wait a moment and try again.
                SendGameMessage(Server.GetWorldManager().GetActor(), 32535, 0x20);
        }

        public override bool IsValidTarget(Character target, ValidTarget validTarget)
        {
            if (target == null)
            {
                // Target does not exist.
                SendGameMessage(Server.GetWorldManager().GetActor(), 32511, 0x20);
                return false;
            }

            if (target.isMovingToSpawn)
            {
                // That command cannot be performed on the current target.
                SendGameMessage(Server.GetWorldManager().GetActor(), 32547, 0x20);
                return false;
            }

            // enemy only
            if ((validTarget & ValidTarget.Enemy) != 0)
            {
                // todo: this seems ambiguous
                if (target.isStatic)
                {
                    // That command cannot be performed on the current target.
                    SendGameMessage(Server.GetWorldManager().GetActor(), 32547, 0x20);
                    return false;
                }
                if (currentParty != null && target.currentParty == currentParty)
                {
                    // That command cannot be performed on a party member.
                    SendGameMessage(Server.GetWorldManager().GetActor(), 32548, 0x20);
                    return false;
                }
                // todo: pvp?
                if (target.allegiance == allegiance)
                {
                    // That command cannot be performed on an ally.
                    SendGameMessage(Server.GetWorldManager().GetActor(), 32549, 0x20);
                    return false;
                }

                bool partyEngaged = false;
                // todo: replace with confrontation status effect? (see how dsp does it)
                if (target.aiContainer.IsEngaged())
                {
                    if (currentParty != null)
                    {
                        if (target is BattleNpc)
                        {
                            var helpingActorId = ((BattleNpc)target).GetMobMod((uint)MobModifier.CallForHelp);
                            partyEngaged = this.actorId == helpingActorId || (((BattleNpc)target).GetMobMod((uint)MobModifier.FreeForAll) != 0);
                        }

                        if (!partyEngaged)
                        {
                            foreach (var memberId in ((Party)currentParty).members)
                            {
                                if (memberId == target.currentLockedTarget)
                                {
                                    partyEngaged = true;
                                    break;
                                }
                            }
                        }
                    }
                    else if (target.currentLockedTarget == actorId)
                    {
                        partyEngaged = true;
                    }
                }
                else
                {
                    partyEngaged = true;
                }

                if (!partyEngaged)
                {
                    // That target is already engaged.
                    SendGameMessage(Server.GetWorldManager().GetActor(), 32520, 0x20);
                    return false;
                }
            }

            if ((validTarget & ValidTarget.Ally) != 0 && target.allegiance != allegiance)
            {
                // That command cannot be performed on the current target.
                SendGameMessage(Server.GetWorldManager().GetActor(), 32547, 0x20);
                return false;
            }

            // todo: isStatic seems ambiguous?
            if ((validTarget & ValidTarget.NPC) != 0 && target.isStatic)
                return true;

            // todo: why is player always zoning?
            // cant target if zoning
            if (target is Player && ((Player)target).playerSession.isUpdatesLocked)
            {
                // That command cannot be performed on the current target.
                SendGameMessage(Server.GetWorldManager().GetActor(), 32547, 0x20);
                return false;
            }

            return true;
        }

        public override bool CanCast(Character target, BattleCommand spell)
        {
            //Might want to do these with a CommandResult instead to be consistent with the rest of command stuff
            if (GetHotbarTimer(spell.id) > Utils.UnixTimeStampUTC())
            {
                // todo: this needs confirming
                // Please wait a moment and try again.
                SendGameMessage(Server.GetWorldManager().GetActor(), 32535, 0x20, (uint)spell.id);
                return false;
            }
            if (target == null)
            {
                // Target does not exist.
                SendGameMessage(Server.GetWorldManager().GetActor(), 32511, 0x20, (uint)spell.id);
                return false;
            }
            if (Utils.XZDistance(positionX, positionZ, target.positionX, target.positionZ) > spell.range)
            {
                // The target is too far away.
                SendGameMessage(Server.GetWorldManager().GetActor(), 32539, 0x20, (uint)spell.id);
                return false;
            }
            if (Utils.XZDistance(positionX, positionZ, target.positionX, target.positionZ) < spell.minRange)
            {
                // The target is too close.
                SendGameMessage(Server.GetWorldManager().GetActor(), 32538, 0x20, (uint)spell.id);
                return false;
            }
            if (target.positionY - positionY > (spell.rangeHeight / 2))
            {
                // The target is too far above you.
                SendGameMessage(Server.GetWorldManager().GetActor(), 32540, 0x20, (uint)spell.id);
                return false;
            }
            if (positionY - target.positionY > (spell.rangeHeight / 2))
            {
                // The target is too far below you.
                SendGameMessage(Server.GetWorldManager().GetActor(), 32541, 0x20, (uint)spell.id);
                return false;
            }
            if (!IsValidTarget(target, spell.mainTarget) || !spell.IsValidMainTarget(this, target))
            {
                // error packet is set in IsValidTarget
                return false;
            }
            return true;
        }

        public override bool CanWeaponSkill(Character target, BattleCommand skill)
        {
            // todo: see worldmaster ids 32558~32557 for proper ko message and stuff
            if (GetHotbarTimer(skill.id) > Utils.UnixTimeStampUTC())
            {
                // todo: this needs confirming
                // Please wait a moment and try again.
                SendGameMessage(Server.GetWorldManager().GetActor(), 32535, 0x20, (uint)skill.id);
                return false;
            }

            if (target == null)
            {
                // Target does not exist.
                SendGameMessage(Server.GetWorldManager().GetActor(), 32511, 0x20, (uint)skill.id);
                return false;
            }

            //Original game checked height difference before horizontal distance
            if (target.positionY - positionY > (skill.rangeHeight / 2))
            {
                // The target is too far above you.
                SendGameMessage(Server.GetWorldManager().GetActor(), 32540, 0x20, (uint)skill.id);
                return false;
            }

            if (positionY - target.positionY > (skill.rangeHeight / 2))
            {
                // The target is too far below you.
                SendGameMessage(Server.GetWorldManager().GetActor(), 32541, 0x20, (uint)skill.id);
                return false;
            }

            var targetDist = Utils.XZDistance(positionX, positionZ, target.positionX, target.positionZ);

            if (targetDist > skill.range)
            {
                // The target is out of range.
                SendGameMessage(Server.GetWorldManager().GetActor(), 32537, 0x20, (uint)skill.id);
                return false;
            }

            if (targetDist < skill.minRange)
            {
                // The target is too close.
                SendGameMessage(Server.GetWorldManager().GetActor(), 32538, 0x20, (uint)skill.id);
                return false;
            }


            if (!IsValidTarget(target, skill.validTarget) || !skill.IsValidMainTarget(this, target))
            {
                // error packet is set in IsValidTarget
                return false;
            }

            return true;
        }

        public override void OnAttack(State state, CommandResult action, ref CommandResult error)
        {
            var target = state.GetTarget();

            base.OnAttack(state, action, ref error);

            // todo: switch based on main weap (also probably move this anim assignment somewhere else)
            action.animation = 0x19001000;
            if (error == null)
            {
                // melee attack animation
                //action.animation = 0x19001000;
            }
            if (target is BattleNpc)
            {
                ((BattleNpc)target).hateContainer.UpdateHate(this, action.enmity);
            }

            LuaEngine.GetInstance().OnSignal("playerAttack");
        }

        public override void OnCast(State state, CommandResult[] actions, BattleCommand spell, ref CommandResult[] errors)
        {
            // todo: update hotbar timers to skill's recast time (also needs to be done on class change or equip crap)
            base.OnCast(state, actions, spell, ref errors);
            // todo: should just make a thing that updates the one slot cause this is dumb as hell            
            UpdateHotbarTimer(spell.id, spell.recastTimeMs);
            //LuaEngine.GetInstance().OnSignal("spellUse");
        }

        public override void OnWeaponSkill(State state, CommandResult[] actions, BattleCommand skill, ref CommandResult[] errors)
        {
            // todo: update hotbar timers to skill's recast time (also needs to be done on class change or equip crap)
            base.OnWeaponSkill(state, actions, skill, ref errors);

            // todo: should just make a thing that updates the one slot cause this is dumb as hell
            UpdateHotbarTimer(skill.id, skill.recastTimeMs);
            // todo: this really shouldnt be called on each ws?
            lua.LuaEngine.CallLuaBattleFunction(this, "onWeaponSkill", this, state.GetTarget(), skill);
            LuaEngine.GetInstance().OnSignal("weaponskillUse");
        }

        public override void OnAbility(State state, CommandResult[] actions, BattleCommand ability, ref CommandResult[] errors)
        {
            base.OnAbility(state, actions, ability, ref errors);
            UpdateHotbarTimer(ability.id, ability.recastTimeMs);
            LuaEngine.GetInstance().OnSignal("abilityUse");
        }

        //Handles exp being added, does not handle figuring out exp bonus from buffs or skill/link chains or any of that
        //Returns CommandResults that can be sent to display the EXP gained number and level ups
        //exp should be a ushort single the exp graphic overflows after ~65k
        public List<CommandResult> AddExp(int exp, byte classId, byte bonusPercent = 0)
        {
            List<CommandResult> actionList = new List<CommandResult>();
            exp += (int) Math.Ceiling((exp * bonusPercent / 100.0f));

            //You earn [exp] (+[bonusPercent]%) experience points.
            //In non-english languages there are unique messages for each language, hence the use of ClassExperienceTextIds
            actionList.Add(new CommandResult(actorId, BattleUtils.ClassExperienceTextIds[classId], 0, (ushort)exp, bonusPercent));

            bool leveled = false;
            int diff = MAXEXP[GetLevel() - 1] - charaWork.battleSave.skillPoint[classId - 1];            
            //While there is enough experience to level up, keep leveling up, unlocking skills and removing experience from exp until we don't have enough to level up
            while (exp >= diff && GetLevel() < charaWork.battleSave.skillLevelCap[classId])
            {
                //Level up
                LevelUp(classId);
                leveled = true;
                //Reduce exp based on how much exp is needed to level
                exp -= diff;
                diff = MAXEXP[GetLevel() - 1];
            }

            if(leveled)
            {
                //Set exp to current class to 0 so that exp is added correctly
                charaWork.battleSave.skillPoint[classId - 1] = 0;
                //send new level
                ActorPropertyPacketUtil expPropertyPacket2 = new ActorPropertyPacketUtil("charaWork/exp", this);
                ActorPropertyPacketUtil expPropertyPacket3 = new ActorPropertyPacketUtil("charaWork/stateForAll", this);
                expPropertyPacket2.AddProperty(String.Format("charaWork.battleSave.skillLevel[{0}]", classId - 1));
                expPropertyPacket2.AddProperty("charaWork.parameterSave.state_mainSkillLevel");
                QueuePackets(expPropertyPacket2.Done());
                QueuePackets(expPropertyPacket3.Done());
                //play levelup animation (do this outside LevelUp so that it only plays once if multiple levels are earned
                //also i dunno how to do this

                Database.SetLevel(this, classId, GetLevel());
                Database.SavePlayerCurrentClass(this);
            }
            //Cap experience for level 50
            charaWork.battleSave.skillPoint[classId - 1] = Math.Min(charaWork.battleSave.skillPoint[classId - 1] + exp, MAXEXP[GetLevel() - 1]);

            ActorPropertyPacketUtil expPropertyPacket = new ActorPropertyPacketUtil("charaWork/battleStateForSelf", this);
            expPropertyPacket.AddProperty(String.Format("charaWork.battleSave.skillPoint[{0}]", classId - 1));
            
            QueuePackets(expPropertyPacket.Done());
            Database.SetExp(this, classId, charaWork.battleSave.skillPoint[classId - 1]);

            return actionList;
        }

        //Increaess level of current class and equips new abilities earned at that level
        public void LevelUp(byte classId, List<CommandResult> actionList = null)
        {
            if (charaWork.battleSave.skillLevel[classId - 1] < charaWork.battleSave.skillLevelCap[classId])
            {
                //Increase level
                charaWork.battleSave.skillLevel[classId - 1]++;
                charaWork.parameterSave.state_mainSkillLevel++;

                //33909: You gain level [level]
                if (actionList != null)
                    actionList.Add(new CommandResult(actorId, 33909, 0, (ushort) charaWork.battleSave.skillLevel[classId - 1]));

                //If there's any abilites that unlocks at this level, equip them.
                List<uint> commandIds = Server.GetWorldManager().GetBattleCommandIdByLevel(classId, GetLevel());
                foreach(uint commandId in commandIds)
                {
                    EquipAbilityInFirstOpenSlot(classId, commandId, false);
                    byte jobId = ConvertClassIdToJobId(classId);
                    if (jobId != classId)
                        EquipAbilityInFirstOpenSlot(jobId, commandId, false);

                    //33926: You learn [command].
                    if (actionList != null)
                    {
                        if(classId == GetCurrentClassOrJob() || jobId == GetCurrentClassOrJob())
                            actionList.Add(new CommandResult(actorId, 33926, commandId));
                    }
                }
            }
        }
        
        public static byte ConvertClassIdToJobId(byte classId)
        {
            byte jobId = classId;

            switch(classId)
            {
                case CLASSID_PUG:
                case CLASSID_GLA:
                case CLASSID_MRD:
                    jobId += 13;
                    break;
                case CLASSID_ARC:
                case CLASSID_LNC:
                    jobId += 11;
                    break;
                case CLASSID_THM:
                case CLASSID_CNJ:
                    jobId += 4;
                    break;
            }

            return jobId;
        }

        public void SetCurrentJob(byte jobId)
        {
            currentJob = jobId;
            BroadcastPacket(SetCurrentJobPacket.BuildPacket(actorId, jobId), true);
            Database.LoadHotbar(this);
            SendCharaExpInfo();
        }

        //Gets the id of the player's current job. If they aren't a job, gets the id of their class
        public byte GetCurrentClassOrJob()
        {
            if (currentJob != 0)
                return (byte) currentJob;
            return charaWork.parameterSave.state_mainSkill[0];
        }

        public void hpstuff(uint hp)
        {
            SetMaxHP(hp);
            SetHP(hp);            
            mpMaxBase = (ushort)hp;
            charaWork.parameterSave.mpMax = (short)hp;
            charaWork.parameterSave.mp = (short)hp;
            AddTP(3000);
            updateFlags |= ActorUpdateFlags.HpTpMp;
        }
        
        public void SetCombos(int comboId1 = 0, int comboId2 = 0)
        {
            SetCombos(new int[] { comboId1, comboId2 });
        }

        public void SetCombos(int[] comboIds)
        {
            Array.Copy(comboIds, playerWork.comboNextCommandId, 2);

            //If we're starting or continuing a combo chain, add the status effect and combo cost bonus
            if (comboIds[0] != 0)
            {
                StatusEffect comboEffect = new StatusEffect(this, Server.GetWorldManager().GetStatusEffect((uint) StatusEffectId.Combo));
                comboEffect.SetDuration(13);
                comboEffect.SetOverwritable(1);
                statusEffects.AddStatusEffect(comboEffect, this, true);
                playerWork.comboCostBonusRate = 1;
            }
            //Otherwise we're ending a combo, remove the status
            else
            {
                statusEffects.RemoveStatusEffect(statusEffects.GetStatusEffectById((uint) StatusEffectId.Combo));
                playerWork.comboCostBonusRate = 0;
            }

            ActorPropertyPacketUtil comboPropertyPacket = new ActorPropertyPacketUtil("playerWork/combo", this);
            comboPropertyPacket.AddProperty("playerWork.comboCostBonusRate");
            comboPropertyPacket.AddProperty("playerWork.comboNextCommandId[0]");
            comboPropertyPacket.AddProperty("playerWork.comboNextCommandId[1]");
            QueuePackets(comboPropertyPacket.Done());
        }

        public override void CalculateBaseStats()
        {
            base.CalculateBaseStats();
            //Add weapon property mod
            var equip = GetEquipment();
            var mainHandItem = equip.GetItemAtSlot(Equipment.SLOT_MAINHAND);
            var damageAttribute = 0;
            var attackDelay = 3000;
            var hitCount = 1;

            if (mainHandItem != null)
            {
                var mainHandWeapon = (Server.GetItemGamedata(mainHandItem.itemId) as WeaponItem);
                damageAttribute = mainHandWeapon.damageAttributeType1;
                attackDelay = (int) (mainHandWeapon.damageInterval * 1000);
                hitCount = mainHandWeapon.frequency;
            }

            var hasShield = equip.GetItemAtSlot(Equipment.SLOT_OFFHAND) != null ? 1 : 0;
            SetMod((uint)Modifier.HasShield, hasShield);

            SetMod((uint)Modifier.AttackType, damageAttribute);
            SetMod((uint)Modifier.AttackDelay, attackDelay);
            SetMod((uint)Modifier.HitCount, hitCount);

            //These stats all correlate in a 3:2 fashion
            //It seems these stats don't actually increase their respective stats. The magic stats do, however
            AddMod((uint)Modifier.Attack, (long)(GetMod(Modifier.Strength) * 0.667));
            AddMod((uint)Modifier.Accuracy, (long)(GetMod(Modifier.Dexterity) * 0.667));
            AddMod((uint)Modifier.Defense, (long)(GetMod(Modifier.Vitality) * 0.667));

            //These stats correlate in a 4:1 fashion. (Unsure if MND is accurate but it would make sense for it to be)
            AddMod((uint)Modifier.MagicAttack, (long)((float)GetMod(Modifier.Intelligence) * 0.25));

            AddMod((uint)Modifier.MagicAccuracy, (long)((float)GetMod(Modifier.Mind) * 0.25));
            AddMod((uint)Modifier.MagicHeal, (long)((float)GetMod(Modifier.Mind) * 0.25));

            AddMod((uint)Modifier.MagicEvasion, (long)((float)GetMod(Modifier.Piety) * 0.25));
            AddMod((uint)Modifier.MagicEnfeeblingPotency, (long)((float)GetMod(Modifier.Piety) * 0.25));

            //VIT correlates to HP in a 1:1 fashion
            AddMod((uint)Modifier.Hp, (long)((float)Modifier.Vitality));

            CalculateTraitMods();
        }

        public bool HasTrait(ushort id)
        {
            BattleTrait trait = Server.GetWorldManager().GetBattleTrait(id);

            return HasTrait(trait);
        }

        public bool HasTrait(BattleTrait trait)
        {
            return (trait != null) && (trait.job == GetClass()) && (trait.level <= GetLevel());
        }

        public void CalculateTraitMods()
        {
            var traitIds = Server.GetWorldManager().GetAllBattleTraitIdsForClass((byte) GetClass());

            foreach(var traitId in traitIds)
            {
                var trait = Server.GetWorldManager().GetBattleTrait(traitId);
                if(HasTrait(trait))
                {
                    AddMod(trait.modifier, trait.bonus);
                }
            }
        }

        public bool HasItemEquippedInSlot(uint itemId, ushort slot)
        {
            var equippedItem = equipment.GetItemAtSlot(slot);

            return equippedItem != null && equippedItem.itemId == itemId;
        }
    }
}
