﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.chara.ai;
using FFXIVClassic_Map_Server.actors.chara.ai.controllers;
using FFXIVClassic_Map_Server.actors.group;
using FFXIVClassic_Map_Server.packets.send.actor;

// port of dsp's ai code https://github.com/DarkstarProject/darkstar/blob/master/src/map/ai/

namespace FFXIVClassic_Map_Server.actors.chara.ai
{
    // https://github.com/Windower/POLUtils/blob/master/PlayOnline.FFXI/Enums.cs
    [Flags]
    public enum ValidTarget : ushort
    {
        None = 0x00,
        Self = 0x01,
        Player = 0x02,
        PartyMember = 0x04,
        Ally = 0x08,
        NPC = 0x10,
        Enemy = 0x20,
        Unknown = 0x40,
        Object = 0x60,
        CorpseOnly = 0x80,
        Corpse = 0x9D // CorpseOnly + NPC + Ally + Partymember + Self
    }

    /// <summary> Targeting from/to different entity types </summary>
    enum TargetFindCharacterType : byte
    {
        None,
        /// <summary> Player can target all <see cref="Player">s in party </summary>
        PlayerToPlayer,
        /// <summary> Player can target all <see cref="BattleNpc"/>s (excluding player owned <see cref="Pet"/>s) </summary>
        PlayerToBattleNpc,
        /// <summary> BattleNpc can target other <see cref="BattleNpc"/>s </summary>
        BattleNpcToBattleNpc,
        /// <summary> BattleNpc can target <see cref="Player"/>s and their <see cref="Pet"/>s </summary>
        BattleNpcToPlayer,
    }

    /// <summary> Type of AOE region to create </summary>
    enum TargetFindAOEType : byte
    {
        None,
        /// <summary> Really a cylinder, uses maxDistance parameter in SetAOEType </summary>
        Circle,
        /// <summary> Create a cone with param in radians </summary>
        Cone,
        /// <summary> Box using self/target coords and </summary>
        Box
    }

    /// <summary> Set AOE around self or target </summary>
    enum TargetFindAOETarget : byte
    {
        /// <summary> Set AOE's origin at target's position </summary>
        Target,
        /// <summary> Set AOE's origin to own position. </summary>
        Self
    }

    /// <summary> Target finding helper class </summary>
    class TargetFind
    {
        private Character owner;
        private Character masterTarget;         // if target is a pet, this is the owner
        private TargetFindCharacterType findType;
        private ValidTarget validTarget;
        private TargetFindAOETarget aoeTarget;
        private TargetFindAOEType aoeType;
        private Vector3 aoeTargetPosition;      //This is the center of circle an cone AOEs and the position where line aoes come out
        private float aoeTargetRotation;        //This is the direction the aoe target is facing
        private float maxDistance;              //Radius for circle and cone AOEs, length for line AOEs
        private float minDistance;              //Minimum distance to that target must be to be able to be hit
        private float width;                    //Width of line AOEs
        private float height;                   //All AoEs are boxes or cylinders. Height is usually 10y regardless of maxDistance, but some commands have different values. Height is total height, so targets can be at most half this distance away on Y axis
        private float aoeRotateAngle;           //This is the angle that cones and line aoes are rotated about aoeTargetPosition for skills that come out of a side other than the front
        private float coneAngle;                //The angle of the cone itself in Pi Radians
        private float param;
        private List<Character> targets;

        public TargetFind(Character owner)
        {
            this.owner = owner;
            Reset();
        }

        public void Reset()
        {
            this.findType = TargetFindCharacterType.None;
            this.validTarget = ValidTarget.Enemy;
            this.aoeType = TargetFindAOEType.None;
            this.aoeTarget = TargetFindAOETarget.Target;
            this.aoeTargetPosition = null;
            this.aoeTargetRotation = 0;
            this.maxDistance = 0.0f;
            this.minDistance = 0.0f;
            this.width = 0.0f;
            this.height = 0.0f;
            this.aoeRotateAngle = 0.0f;
            this.coneAngle = 0.0f;
            this.param = 0.0f;
            this.targets = new List<Character>();
        }

        public List<T> GetTargets<T>() where T : Character
        {
            return new List<T>(targets.OfType<T>());
        }

        public List<Character> GetTargets()
        {
            return targets;
        }

        /// <summary>
        /// Call this before <see cref="FindWithinArea"/> <para/>
        /// </summary>
        /// <param name="maxDistance">
        /// <see cref="TargetFindAOEType.Circle"/> - radius of circle <para/>
        /// <see cref="TargetFindAOEType.Cone"/> - height of cone <para/>
        /// <see cref="TargetFindAOEType.Box"/> - width of box / 2 (todo: set box length not just between user and target)
        /// </param>
        /// <param name="param"> param in degrees of cone (todo: probably use radians and forget converting at runtime) </param>
        public void SetAOEType(ValidTarget validTarget, TargetFindAOEType aoeType, TargetFindAOETarget aoeTarget, float maxDistance, float minDistance, float height, float aoeRotate, float coneAngle, float param = 0.0f)
        {
            this.validTarget = validTarget;
            this.aoeType = aoeType;
            this.maxDistance = maxDistance;
            this.minDistance = minDistance;
            this.param = param;
            this.height = height;
            this.aoeRotateAngle = aoeRotate;
            this.coneAngle = coneAngle;
        }

        /// <summary>
        /// Call this to prepare Box AOE
        /// </summary>
        /// <param name="validTarget"></param>
        /// <param name="aoeTarget"></param>
        /// <param name="length"></param>
        /// <param name="width"></param>
        public void SetAOEBox(ValidTarget validTarget, TargetFindAOETarget aoeTarget, float length, float width, float aoeRotateAngle)
        {
            this.validTarget = validTarget;
            this.aoeType = TargetFindAOEType.Box;
            this.aoeTarget = aoeTarget;
            this.aoeRotateAngle = aoeRotateAngle;
            float x = owner.positionX - (float)Math.Cos(owner.rotation + (float)(Math.PI / 2)) * (length);
            float z = owner.positionZ + (float)Math.Sin(owner.rotation + (float)(Math.PI / 2)) * (length);
            this.maxDistance = length;
            this.width = width;
        }

        /// <summary>
        /// Find and try to add a single target to target list
        /// </summary>
        public void FindTarget(Character target, ValidTarget flags)
        {
            validTarget = flags;
            // todo: maybe this should only be set if successfully added?
            AddTarget(target, false);
        }

        /// <summary>
        /// <para> Call SetAOEType before calling this </para>
        /// Find targets within area set by <see cref="SetAOEType"/>
        /// </summary>
        public void FindWithinArea(Character target, ValidTarget flags, TargetFindAOETarget aoeTarget)
        {
            targets.Clear();
            validTarget = flags;
            // are we creating aoe circles around target or self
            if (aoeTarget == TargetFindAOETarget.Self)
            {
                this.aoeTargetPosition = owner.GetPosAsVector3();
                this.aoeTargetRotation = owner.rotation + (float) (aoeRotateAngle * Math.PI);
            }
            else
            {
                this.aoeTargetPosition = target.GetPosAsVector3();
                this.aoeTargetRotation = target.rotation + (float) (aoeRotateAngle * Math.PI);
            }

            masterTarget = TryGetMasterTarget(target) ?? target;

            // todo: this is stupid
            bool withPet = (flags & ValidTarget.Ally) != 0 || masterTarget.allegiance != owner.allegiance;

            if (masterTarget != null && CanTarget(masterTarget))
                targets.Add(masterTarget);

            if (aoeType != TargetFindAOEType.None)
            {
                AddAllInRange(target, withPet);
            }

            //if (targets.Count > 8)
                //targets.RemoveRange(8, targets.Count - 8);

            //Curaga starts with lowest health players, so the targets are definitely sorted at least for some abilities
            //Other aoe abilities might be sorted by distance?
            //Protect is random
            targets.Sort(delegate (Character a, Character b) { return a.GetHP().CompareTo(b.GetHP()); });
        }

        /// <summary>
        /// Find targets within a box using owner's coordinates and target's coordinates as length
        /// with corners being `maxDistance` yalms to either side of self and target
        /// </summary>
        private bool IsWithinBox(Character target, bool withPet)
        {
            Vector3 vec = target.GetPosAsVector3() - aoeTargetPosition;
            Vector3 relativePos = new Vector3();

            //Get target's position relative to owner's position where owner's front is facing positive z axis
            relativePos.X = (float)(vec.X * Math.Cos(aoeTargetRotation) - vec.Z * Math.Sin(aoeTargetRotation));
            relativePos.Z = (float)(vec.X * Math.Sin(aoeTargetRotation) + vec.Z * Math.Cos(aoeTargetRotation));

            float halfHeight = height / 2;
            float halfWidth = width / 2;

            Vector3 closeBottomLeft = new Vector3(-halfWidth, -halfHeight, minDistance);
            Vector3 farTopRight = new Vector3(halfWidth, halfHeight, maxDistance);

            return relativePos.IsWithinBox(closeBottomLeft, farTopRight);
        }

        private bool IsWithinCone(Character target, bool withPet)
        {
            double distance = Utils.XZDistance(aoeTargetPosition, target.GetPosAsVector3());

            //Make sure target is within the correct range first
            if (!IsWithinCircle(target))
                return false;

            //This might not be 100% right or the most optimal way to do this
            //Get between taget's position and our position
            return target.GetPosAsVector3().IsWithinCone(aoeTargetPosition, aoeTargetRotation, coneAngle);
        }

        private void AddTarget(Character target, bool withPet)
        {
            if (CanTarget(target))
            {
                // todo: add pets too
                targets.Add(target);
            }
        }

        private void AddAllInParty(Character target, bool withPet)
        {
            var party = target.currentParty as Party;
            if (party != null)
            {
                foreach (var actorId in party.members)
                {
                    AddTarget(owner.zone.FindActorInArea<Character>(actorId), withPet);
                }
            }
        }

        private void AddAllInAlliance(Character target, bool withPet)
        {
            // todo:
            AddAllInParty(target, withPet);
        }

        private void AddAllBattleNpcs(Character target, bool withPet)
        {
            int dist = (int)maxDistance;
            var actors = owner.zone.GetActorsAroundActor<BattleNpc>(target, dist);

            foreach (BattleNpc actor in actors)
            {
                AddTarget(actor, false);
            }
        }

        private void AddAllInZone(Character target, bool withPet)
        {
            var actors = owner.zone.GetAllActors<Character>();
            foreach (Character actor in actors)
            {
                AddTarget(actor, withPet);
            }
        }

        private void AddAllInRange(Character target, bool withPet)
        {
            int dist = (int)maxDistance;
            var actors = owner.zone.GetActorsAroundActor<Character>(target, dist);

            foreach (Character actor in actors)
            {
                AddTarget(actor, false);
            }

        }

        private void AddAllInHateList()
        {
            if (!(owner is BattleNpc))
            {
                Program.Log.Error($"TargetFind.AddAllInHateList() owner [{owner.actorId}] {owner.customDisplayName} {owner.actorName} is not a BattleNpc");
            }
            else
            {
                foreach (var hateEntry in ((BattleNpc)owner).hateContainer.GetHateList())
                {
                    AddTarget(hateEntry.Value.actor, false);
                }
            }
        }

        public bool CanTarget(Character target, bool withPet = false, bool retarget = false, bool ignoreAOE = false)
        {
            // already targeted, dont target again
            if (target == null || !retarget && targets.Contains(target))
                return false;

            //This skill can't be used on self and target is self, return false
            if ((validTarget & ValidTarget.Self) == 0 && target == owner)
                return false;

            //This skill can't be used on NPCs and target is an NPC, return false
            if ((validTarget & ValidTarget.NPC) == 0 && target.isStatic)
                return false;

            //This skill can't be used on corpses and target is dead, return false
            if ((validTarget & ValidTarget.Corpse) == 0 && target.IsDead())
                return false;

            //This skill must be used on Allies and target is not an ally, return false
            if ((validTarget & ValidTarget.Ally) != 0 && target.allegiance != owner.allegiance)
                return false;


            //This skill can't be used on players and target is a player, return false
            //Do we need a player flag? Ally/Enemy flags probably serve the same purpose
            //if ((validTarget & ValidTarget.Player) == 0 && target is Player)
                //return false;


            //This skill must be used on enemies an target is not an enemy
            if ((validTarget & ValidTarget.Enemy) != 0 && target.allegiance == owner.allegiance)
                return false;

            //This skill must be used on a party member and target is not in owner's party, return false
            if ((validTarget & ValidTarget.PartyMember) != 0 && target.currentParty != owner.currentParty)
                return false;

            //This skill must be used on a corpse and target is alive, return false
            if ((validTarget & ValidTarget.CorpseOnly) != 0 && target.IsAlive())
                return false;

            // todo: why is player always zoning?
            // cant target if zoning
            if (target is Player && ((Player)target).playerSession.isUpdatesLocked)
            {
                owner.aiContainer.ChangeTarget(null);
                return false;
            }

            if (/*target.isZoning || owner.isZoning || */target.zone != owner.zone)
                return false;

            if (validTarget == ValidTarget.Self && aoeType == TargetFindAOEType.None && owner != target)
                return false;

            // this is fuckin retarded, think of a better way l8r
            if (!ignoreAOE)
            {
                // hit everything within zone or within aoe region
                if (param == -1.0f || aoeType == TargetFindAOEType.Circle && !IsWithinCircle(target))
                    return false;

                if (aoeType == TargetFindAOEType.Cone && !IsWithinCone(target, withPet))
                    return false;

                if (aoeType == TargetFindAOEType.Box && !IsWithinBox(target, withPet))
                    return false;

                if (aoeType == TargetFindAOEType.None && targets.Count != 0)
                    return false;
            }
            return true;
        }

        private bool IsWithinCircle(Character target)
        {
            //Check if XZ is in circle and that y difference isn't larger than half height
            return target.GetPosAsVector3().IsWithinCircle(aoeTargetPosition, maxDistance, minDistance) && Math.Abs(owner.positionY - target.positionY) <= (height / 2);
        }

        private bool IsPlayer(Character target)
        {
            if (target is Player)
                return true;

            // treat player owned pets as players too
            return TryGetMasterTarget(target) is Player;
        }

        private Character TryGetMasterTarget(Character target)
        {
            // if character is a player owned pet, treat as a player
            if (target.aiContainer != null)
            {
                var controller = target.aiContainer.GetController<PetController>();
                if (controller != null)
                    return controller.GetPetMaster();
            }
            return null;
        }

        private bool IsBattleNpcOwner(Character target)
        {
            // i know i copied this from dsp but what even
            if (!(owner is Player) || target is Player)
                return true;

            // todo: check hate list
            if (owner is BattleNpc && ((BattleNpc)owner).hateContainer.GetMostHatedTarget() != target)
            {
                return false;
            }
            return false;
        }

        public Character GetValidTarget(Character target, ValidTarget findFlags)
        {
            if (target == null || target is Player && ((Player)target).playerSession.isUpdatesLocked)
                return null;

            if ((findFlags & ValidTarget.Ally) != 0)
            {
                return owner.pet;
            }

            // todo: this is beyond retarded
            var oldFlags = this.validTarget;
            this.validTarget = findFlags;
            if (CanTarget(target, false, true))
            {
                this.validTarget = oldFlags;
                return target;
            }
            this.validTarget = oldFlags;

            return null;
        }
    }
}
