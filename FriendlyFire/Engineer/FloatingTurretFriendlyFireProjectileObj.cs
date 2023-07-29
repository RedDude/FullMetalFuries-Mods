using CDGEngine;
using FMOD_.Studio;
using Microsoft.Xna.Framework;
using SpriteSystem2;
using System;
using System.Collections.Generic;
using FullModdedFuriesAPI;
using FullModdedFuriesAPI.Mods.FriendlyFire;
using Tweener;

namespace Brawler2D
{
    public class FloatingTurretFriendlyFireProjectileObj : FloatingTurretProjectileObj
    {
        private PlayerClassObj_Engineer m_engineerObj;
        private CharacterObj m_target;

        private List<CharacterObj> m_targetsToHit;
        private float m_fireDelayCounter;

        private EventInstance m_shotSFX;
        private EventInstance m_turretActiveSFX;
        private CharacterObj m_lastTargetHit;

        private IReflectedField<bool> triggeredField;
        private IReflectedField<float> timesHitTargetField;
        private IReflectedMethod updateAttackEllipse;

        public FloatingTurretFriendlyFireProjectileObj(PlayerClassObj_Engineer source) : base(source)
        {
            this.m_engineerObj = source;
            this.m_targetsToHit = new List<CharacterObj>();
            this.isPixelSprite = true;
            this.canStun = true;
            this.masteryEquipSlot = new EquipmentSlotType?(EquipmentSlotType.ButtonB);
            this.lockLayerDuration = 0.0f;

            this.triggeredField = ModEntry.helper.Reflection.GetField<bool>(this, "m_triggered");
            this.timesHitTargetField = ModEntry.helper.Reflection.GetField<float>(this, "m_timesHitTarget");
            this.updateAttackEllipse = ModEntry.helper.Reflection.GetMethod(this, "UpdateAttackEllipse");
        }

        public override void Update(float elapsedSeconds)
        {
            bool triggered = this.triggeredField.GetValue();
            if (!this.m_isDying && triggered)
            {
                if ((double) this.m_fireDelayCounter > 0.0)
                    this.m_fireDelayCounter -= elapsedSeconds;

                this.CheckEnemyEllipseCollision();
                // if (Mod.Instance.IsFriendlyFire)
                {
                    var lastTargets = new List<CharacterObj>(this.m_targetsToHit);
                    var lastTarget = this.m_target;
                    this.CheckPlayerEllipseCollision();
                    this.m_targetsToHit.AddRange(lastTargets);
                    if (lastTarget != null)
                        this.m_target = lastTarget;
                }

                if (this.m_engineerObj.HasEquippedEquipment(EquipmentType.ENG_MultiDrone) ||
                    this.m_engineerObj.HasEquippedEquipment(EquipmentType.ENG_AirDrone))
                {
                    if ((double) this.m_fireDelayCounter <= 0.0 && this.m_targetsToHit.Count > 0)
                    {
                        this.m_shotSFX = GameController.soundManager.PlayEvent(
                            "event:/SFX/Characters/Engineer/Turret/sfx_char_eng_b_turret_normal_fire",
                            (IPositionalObj) this, false, false);
                        this.PlayAnimation("AttackStart", "AttackEnd", false, false);
                        float num = 1f;
                        if (this.m_engineerObj.HasEquippedEquipment(EquipmentType.ENG_MultiDrone))
                            num = 1f;
                        if (this.m_engineerObj.HasEquippedEquipment(EquipmentType.ENG_AirDrone))
                            num = 0.75f;
                        this.m_fireDelayCounter = 0.2f * num;
                        // if (Mod.Instance.IsFriendlyFire)
                        {
                            foreach (CharacterObj obj in this.m_targetsToHit)
                            {
                                if (obj is EnemyObj enemyObj)
                                    enemyObj.HitCharacter((IDamageObj) this, enemyObj.AbsPosition, -1f, false, true);
                                if (obj is PlayerClassObj playerObj)
                                {
                                    if (obj != this.SourceObj)
                                    {
                                        var lastPosition = playerObj.Position;
                                        playerObj.SetInvincible(0, false);
                                        playerObj.HitCharacter((IDamageObj) this, playerObj.AbsPosition, 2f, false,
                                            true);
                                        playerObj.Position = lastPosition;
                                    }

                                    ;
                                }

                            }
                        }
                        // else
                        // {
                        //   foreach (EnemyObj enemyObj in this.m_targetsToHit)
                        //     enemyObj.HitCharacter((IDamageObj) this, enemyObj.AbsPosition, -1f, false, true);
                        // }

                    }
                }
                else if (this.m_target != null && (double) this.m_fireDelayCounter <= 0.0)
                {
                    this.m_shotSFX = GameController.soundManager.PlayEvent(
                        "event:/SFX/Characters/Engineer/Turret/sfx_char_eng_b_turret_normal_fire",
                        (IPositionalObj) this, false, false);
                    this.PlayAnimation("AttackStart", "AttackEnd", false, false);
                    if (this.m_engineerObj.HasEquippedEquipment(EquipmentType.ENG_StunDrone))
                    {
                        if (this.m_lastTargetHit == this.m_target)
                            this.timesHitTargetField.SetValue(this.timesHitTargetField.GetValue() + 1);
                        else
                            this.timesHitTargetField.SetValue(0.0f);
                        this.m_lastTargetHit = (CharacterObj) this.m_target;
                    }

                    if (this.m_target is EnemyObj target)
                        target.HitCharacter((IDamageObj) this, target.AbsPosition, -1f, false, true);

                    // if(Mod.Instance.IsFriendlyFire)
                    if (this.m_target is PlayerClassObj playerTarget)
                    {
                        if (playerTarget != this.SourceObj)
                        {
                            var lastPosition = playerTarget.Position;
                            playerTarget.HitCharacter((IDamageObj) this, playerTarget.AbsPosition, this.damage / 2, false, true);
                            playerTarget.SetInvincible(0, false);
                            playerTarget.Position = lastPosition;
                        }
                    }


                    float num = 1f;
                    if (this.m_engineerObj.HasEquippedEquipment(EquipmentType.ENG_StunDrone))
                        num = 0.25f;
                    this.m_fireDelayCounter = 0.2f * num;
                }

                if (!this.IsAnimating)
                    this.PlayAnimation("StandStart", "StandEnd", true, false);
                base.Update(elapsedSeconds);
            }
            else
                this.UpdateHeading(elapsedSeconds);

            if (this.m_isDying)
                return;

            this.updateAttackEllipse.Invoke();
        }

        private void CheckEnemyEllipseCollision()
        {
            bool flag = this.m_engineerObj.HasEquippedEquipment(EquipmentType.ENG_AirDrone);
            this.m_target = (CharacterObj) null;
            this.m_targetsToHit.Clear();
            ArenaScreen arenaScreen = this.m_engineerObj.Game.ScreenManager.arenaScreen;
            int globalEnemyArrayCount = arenaScreen.currentAndGlobalEnemyArray_count;
            for (int index1 = 0; index1 < globalEnemyArrayCount; ++index1)
            {
                EnemyObj currentAndGlobalEnemy = arenaScreen.currentAndGlobalEnemyArray[index1];
                Enemy_TimeBomb_Basic enemyTimeBombBasic = currentAndGlobalEnemy as Enemy_TimeBomb_Basic;
                if (currentAndGlobalEnemy.isTargetable && currentAndGlobalEnemy.Active &&
                    (!currentAndGlobalEnemy.IsKilled && !currentAndGlobalEnemy.isSpawning) &&
                    (currentAndGlobalEnemy.State != CharacterState.Dying &&
                     currentAndGlobalEnemy.State != CharacterState.Dodging) &&
                    (!currentAndGlobalEnemy.inAir && !flag &&
                        currentAndGlobalEnemy.State != CharacterState.KnockedDown || currentAndGlobalEnemy.inAir &&
                        flag &&
                        ((double) currentAndGlobalEnemy.AnchorY * (double) currentAndGlobalEnemy.ScaleY > 65.0 &&
                         (double) currentAndGlobalEnemy.AnchorY * (double) currentAndGlobalEnemy.ScaleY < 275.0)))
                {
                    Hitbox[] hitboxesArray = currentAndGlobalEnemy.HitboxesArray;
                    int hitboxesCount = currentAndGlobalEnemy.HitboxesCount;
                    for (int index2 = 0; index2 < hitboxesCount; ++index2)
                    {
                        Hitbox hitbox = hitboxesArray[index2];
                        if ((hitbox.Type == HitboxType.Terrain || hitbox.Type == HitboxType.Body &&
                                (enemyTimeBombBasic != null || currentAndGlobalEnemy.objType == 92)) &&
                            this.attackEllipse.Intersects(hitbox.ToRect()))
                        {
                            if (!this.m_targetsToHit.Contains((CharacterObj) currentAndGlobalEnemy))
                            {
                                this.m_targetsToHit.Add((CharacterObj) currentAndGlobalEnemy);
                                break;
                            }

                            break;
                        }
                    }
                }
            }

            EnemyObj enemyObj1 = (EnemyObj) null;
            float num1 = float.MaxValue;
            foreach (EnemyObj enemyObj2 in this.m_targetsToHit)
            {
                float num2 = CDGMath.DistanceBetweenPts(this.AbsPosition, enemyObj2.AbsPosition);
                if ((double) num2 < (double) num1)
                {
                    num1 = num2;
                    enemyObj1 = enemyObj2;
                }
            }

            this.m_target = (CharacterObj) enemyObj1;
        }

        private void CheckPlayerEllipseCollision()
        {
            this.m_target = (CharacterObj) null;
            this.m_targetsToHit.Clear();
            PlayerManager playerManager = this.m_engineerObj.Game.PlayerManager;
            int playerArrayCount = playerManager.activePlayerArray_count;
            for (int index = 0; index < playerArrayCount; ++index)
            {
                PlayerClassObj currentPlayerClass = playerManager.activePlayerArray[index].currentPlayerClass;
                if (!currentPlayerClass.IsKilled && currentPlayerClass.State != CharacterState.KnockedDown &&
                    (!currentPlayerClass.inAir && currentPlayerClass.State != CharacterState.Dodging) &&
                    (this.attackEllipse.Intersects(currentPlayerClass.collisionEllipse) &&
                     !this.m_targetsToHit.Contains((CharacterObj) currentPlayerClass)))
                    this.m_targetsToHit.Add((CharacterObj) currentPlayerClass);
            }

            PlayerClassObj playerClassObj1 = (PlayerClassObj) null;
            float num1 = float.MaxValue;
            foreach (PlayerClassObj playerClassObj2 in this.m_targetsToHit)
            {
                float num2 = CDGMath.DistanceBetweenPts(this.AbsPosition, playerClassObj2.AbsPosition);
                if ((double) num2 < (double) num1)
                {
                    num1 = num2;
                    playerClassObj1 = playerClassObj2;
                }
            }

            this.m_target = (CharacterObj) playerClassObj1;
        }

        // private void UpdateAttackEllipse()
        // {
        //     this.m_attackEllipse.x = this.X;
        //     this.m_attackEllipse.y = this.Y + this.shadowYOffset;
        //     this.attackRadiusSprite.X = this.m_attackEllipse.x;
        //     this.attackRadiusSprite.Y = this.m_attackEllipse.y;
        //     if (this.m_engineerObj.Game.ScreenManager.CurrentScreenType != ScreenType.Arena)
        //         return;
        //     (this.m_engineerObj.Game.ScreenManager.CurrentScreen as ArenaScreen).AddToDrawList((GameObj) this.attackRadiusSprite);
        // }

    }
}
