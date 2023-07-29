using System;
using System.Collections.Generic;
using System.Reflection;
using Brawler2D;
using CDGEngine;
using cs.Blit;
using Microsoft.Xna.Framework;
using Tweener;
using Tweener.Ease;

namespace FullModdedFuriesAPI.Mods.FriendlyFire
{
    public class FriendlyFireManager
    {
        public bool isEnable = true;

        public static Dictionary<PlayerClassObj, IDamageObj> pullByPlayer =
            new Dictionary<PlayerClassObj, IDamageObj>();

        private Assembly Assembly;
        private Type EngineerRollAttackType;

        public FriendlyFireManager()
        {
            this.Assembly = typeof(GameController).Assembly;
            this.EngineerRollAttackType = this.Assembly.GetType("Brawler2D.EngineerRollAttack");
            var friendlyFireManagerPatch = new FriendlyFireManagerPatch(GameController.g_game, ModEntry.helper, this);
            friendlyFireManagerPatch.Patch();
        }

        // public void Update()
        // {
        //     var playerManager = GameController.g_game.PlayerManager;
        //     int playerArrayCount = playerManager.activePlayerArray_count;
        //     for (int index1 = 0; index1 < playerArrayCount; ++index1)
        //     {
        //         var currentPlayerClass = playerManager.activePlayerArray[index1].currentPlayerClass;
        //         if (currentPlayerClass == null) continue;
        //         float stunCount = ModEntry.helper.Reflection.GetField<float>(currentPlayerClass, "m_stunCount").GetValue();
        //     }
        // }
        // 2693 PlayerObj
        public void HandleFriendlyFire(PlayerClassObj beingAttacked, IDamageObj damageObj, float dmgOverride,
            bool baseHit)
        {
            // pullByPlayer = false;
            // underTurretAttack = false;
            if (!this.isEnable)
                return;

            GameController game = GameController.g_game;
            // PlayerClassObj playerClass = (PlayerClassObj)damageObj;
            Vector2 pushAngle = new Vector2(0.0f, 0.0f);
            float pushAmount = 0.0f;

            int knockdownDefense = (int) 10; //this.player.knockdownDefense;
            int stunDefense = (int) 0; //this.stunDefense;
            int stunStr = damageObj.stunStr;
            int knockdownStr = damageObj.knockdownStr;

            float stunCount = ModEntry.helper.Reflection.GetField<float>(beingAttacked, "m_stunCount").GetValue();
            if (stunCount >= 19.0)
                knockdownStr = StunCharacter(beingAttacked, game);

            bool canKnockdown = knockdownStr > knockdownDefense;
            bool canStunLikeEnemy = stunStr > stunDefense;

            if (damageObj is ProjectileObj projectile)
            {
                this.HandleProjectile(beingAttacked, projectile, canKnockdown, pushAngle, pushAmount, canStunLikeEnemy, stunCount);
                return;
            }

            if (damageObj is PlayerClassObj playerObjAttacker)
            {
                if (playerObjAttacker.currentAttack.pushEnemyAmount > 0)
                {
                    pullByPlayer.Add(beingAttacked, playerObjAttacker);
                    pushAngle = CDGMath.VectorBetweenPts(beingAttacked.AbsPosition, playerObjAttacker.Position);
                    pushAmount = (float) playerObjAttacker.currentAttack.pushEnemyAmount;
                }
                // Bitfield b = new Bitfield(19);
                // b.set<EnemyObj.B>(EnemyObj.B.SniperVisibility, this.State != CharacterState.KnockedDown && this.State != CharacterState.Stunned && (double) this.CurrentHealth > 0.0);
                // b.set<EnemyObj.B>(EnemyObj.B.alreadyDead, (double) this.CurrentHealth <= 0.0);
                // b.set<EnemyObj.B>(EnemyObj.B.isBurnDamage, this.burnScope);
                // b.set<EnemyObj.B>(EnemyObj.B.hasPlayerSource, playerClassObj != null);
                // b.set<EnemyObj.B>(EnemyObj.B.inAir, this.inAir);
                // b.set<EnemyObj.B>(EnemyObj.B.stunBreak, (double) this.m_stunCount >= 19.0);
                // b.set<EnemyObj.B>(EnemyObj.B.IsGrapnelProjectile, damageObj is GrapnelProjectileObj);
                // b.set<EnemyObj.B>(EnemyObj.B.forceHit, forceHit);
                // b.set<EnemyObj.B>(EnemyObj.B.damageOverrideNegOne, (double) dmgOverride == -1.0);
                // b.set<EnemyObj.B>(EnemyObj.B.comboAttack, playerClassObj != null && playerClassObj.currentAttack.isComboAttack);
                // b.set<EnemyObj.B>(EnemyObj.B.Invincible, this.hasStatusEffect(StatusEffect.Invincible));
                // b.set<EnemyObj.B>(EnemyObj.B.AirStun, this.hasStatusEffect(StatusEffect.AirStun));
                // b.set<EnemyObj.B>(EnemyObj.B.isBurned, this.hasStatusEffect(StatusEffect.Burn));
                // b.set<EnemyObj.B>(EnemyObj.B.dmgGTdmgOverride, playerClassObj != null && (double) playerClassObj.damage > (double) dmgOverride);
                // b.set<EnemyObj.B>(EnemyObj.B.projIsNull, projectileObj == null);
                // b.set<EnemyObj.B>(EnemyObj.B.IsGrapnelTargetNull, grapnelProjectileObj != null && grapnelProjectileObj.grapnelTarget == null);
                // b.set<EnemyObj.B>(EnemyObj.B.IsExplosion, projectileObj != null && !projectileObj.hasShadow);

                if (playerObjAttacker is PlayerClassObj_Engineer &&
                    playerObjAttacker.currentAttack.GetType() == this.EngineerRollAttackType &&
                    playerObjAttacker.HasEquippedEquipment(EquipmentType.ENG_SlowRoll)
                    || playerObjAttacker is PlayerClassObj_Tank && knockdownStr > 9
                    || playerObjAttacker is PlayerClassObj_Sniper && knockdownStr > 0
                    || playerObjAttacker is PlayerClassObj_Fighter fighter && fighter.isLastAttack)
                    knockdownDefense = -10;

                if (beingAttacked.hasStatusEffect(StatusEffect.Unstoppable))
                {
                    knockdownDefense += 4;
                    stunDefense += 4;
                }

                canKnockdown = knockdownStr > knockdownDefense;
                canStunLikeEnemy = stunStr > stunDefense;

//            if (mastery != EquipmentSlotType.ButtonR)
                //            playerClassObj2.player.SetEquipSlotMasteryPoints(playerClassObj2, mastery, 1, true);
                //if (b.get<EnemyObj.B>(EnemyObj.B.projIsNull) && b.get<EnemyObj.B>(EnemyObj.B.comboAttack) && b.get<EnemyObj.B>(EnemyObj.B.damageOverrideNegOne))
                if (playerObjAttacker.currentAttack.isComboAttack)
                    playerObjAttacker.continueCombo = true;
                switch (playerObjAttacker.classType)
                {
                    case ClassType.Engineer:
                        // layeredSprite1.ChangeSprite("Effect_EngineerAttack_Sprite");
                        break;
                    case ClassType.Fighter:
                        PlayerClassObj_Fighter playerClassObjFighter = playerObjAttacker as PlayerClassObj_Fighter;
                        //layeredSprite1.ChangeSprite("Effect_FighterAttack_Sprite");
                        if (playerClassObjFighter.isSpinAttacking &&
                            playerClassObjFighter.HasEquippedEquipment(EquipmentType.FTR_ChargeSpin))
                            playerClassObjFighter.AddToSpinDuration(0.9f);
                        //if (!b.get<EnemyObj.B>(EnemyObj.B.isBurnDamage) && (playerClassObjFighter.currentAttack is FighterFlameMeleeAttack || playerClassObjFighter.currentAttack is FighterMeleeAttack) && (playerClassObjFighter.isLastAttack && !playerClassObjFighter.hitEnemyWithLastAttack))
                        if (beingAttacked.burnSource == null &&
                            (playerClassObjFighter.currentAttack is FighterFlameMeleeAttack ||
                             playerClassObjFighter.currentAttack is FighterMeleeAttack) &&
                            (playerClassObjFighter.isLastAttack && !playerClassObjFighter.hitEnemyWithLastAttack))
                        {
                            float skillAmount = playerClassObjFighter.player.GetSkillAmount(SkillType.Fight_Hammer_CD);
                            playerClassObjFighter.ReduceCooldowns(skillAmount);
                            if (playerClassObjFighter.HasEquippedEquipment(EquipmentType.FTR_VampAttack))
                            {
                                float num = beingAttacked.GetTotalDamageDealt(damageObj, dmgOverride) * 0.25f;
                                playerClassObjFighter.CurrentHealth += num;
                                playerClassObjFighter.RunHealthCollectAnimation((int) num);
                            }

                            playerClassObjFighter.hitEnemyWithLastAttack = true;
                            break;
                        }

                        break;
                    case ClassType.Sniper:
                        PlayerClassObj_Sniper playerClassObjSniper = playerObjAttacker as PlayerClassObj_Sniper;
                        playerObjAttacker.ChangeSprite("Effect_SniperAttack_Sprite");
                        //if (!b.get<EnemyObj.B>(EnemyObj.B.projIsNull))
                    {
                        float skillAmount = playerClassObjSniper.player.GetSkillAmount(SkillType.Snp_Grap_Stun);

                        //if ((double) skillAmount > 0.0 && ((GrapnelProjectileObj)damageObj).grapnelTarget != null)
                        //b.get<EnemyObj.B>(EnemyObj.B.IsGrapnelProjectile))
                        //{
                        //this.ApplyStatusEffect((CharacterObj) playerClassObjSniper, StatusEffect.ArmorShred, skillAmount, true);
                        //  break;
                        //}

                        break;
                    }
                        break;
                    case ClassType.Tank:
                        PlayerClassObj_Tank playerClassObjTank = playerObjAttacker as PlayerClassObj_Tank;
                        if (playerClassObjTank.shoutRegenCount < 5 &&
                            playerClassObjTank.currentAttack is TankShoutAttack)
                        {
                            ++playerClassObjTank.shoutRegenCount;
                            float shoutRegen = 1f;
                            if (playerClassObjTank.HasEquippedEquipment(EquipmentType.TNK_TowerShield))
                                shoutRegen = 0.0f;
                            float shoutRegen2 = playerClassObjTank.player.GetSkillAmount(SkillType.Tank_Shout_Regen) *
                                                shoutRegen * playerClassObjTank.shieldMaxHealth;
                            playerClassObjTank.shieldCurrentHealth += shoutRegen2;
                        }

                        //if (!this.pierceThrough)
                        //{
                        //playerClassObjTank.ForceRunCancel();
                        //  break;
                        //}
                        break;
                }

                //baseHit = false;
            }

            // b.set<EnemyObj.B>(EnemyObj.B.canKnockdown, num > knockdownDefense);
            // b.set<EnemyObj.B>(EnemyObj.B.canStun, stunStr > stunDefense);
            // float totalDamage = (double) dmgOverride == -1.0 || applyModsToDmgOverride ? this.GetTotalDamageDealt(damageObj, dmgOverride) : dmgOverride;
            // EquipmentSlotType mastery = EquipmentSlotType.ButtonR;
            // if ((double) dmgOverride == -1.0 && !this.isNotEnemy && ((double) totalDamage > 0.0 && this.givesMastery))
            // {
            //   var result = Mod.Instance.enemyCharm
            //     ? projectileObj == null && playerClassObj != null
            //     : projectileObj == null;
            //
            //   if (result)
            //   {
            //     switch (playerClassObj.classType)
            //     {
            //       case ClassType.Engineer:
            //         mastery = !(playerClassObj.currentAttack is EngineerBaseAttackBig) ? EquipmentSlotType.ButtonX : EquipmentSlotType.ButtonY;
            //         break;
            //       case ClassType.Fighter:
            //         mastery = !((PlayerClassObj_Fighter) playerClassObj).isCounterAttacking || !playerClassObj.HasEquippedEquipment(EquipmentType.FTR_RiskCounter) ? (((PlayerClassObj_Fighter) playerClassObj).isSpinAttacking ? EquipmentSlotType.ButtonB : EquipmentSlotType.ButtonX) : EquipmentSlotType.ButtonY;
            //         break;
            //       case ClassType.Sniper:
            //         mastery = !((PlayerClassObj_Sniper) playerClassObj).isEmpowered ? EquipmentSlotType.ButtonX : EquipmentSlotType.ButtonB;
            //         break;
            //       case ClassType.Tank:
            //         mastery = !(playerClassObj.currentAttack is TankShieldRun) ? EquipmentSlotType.ButtonX : EquipmentSlotType.ButtonA;
            //         break;
            //     }
            //   }
            //   else {
            //     var result2 = Mod.Instance.enemyCharm
            //       ? projectileObj != null && projectileObj.masteryEquipSlot.HasValue && playerClassObj != null
            //       : projectileObj.masteryEquipSlot.HasValue && playerClassObj != null;
            //
            //     if (result2)
            //       mastery = projectileObj.masteryEquipSlot.Value;
            //   }
            // }
            // if (GlobalEV.ENABLE_ONLINE_CHEAT)
            //   totalDamage *= 7f;
            // Net.Master.HitEnemy(this.n_id, damageObj, b, mastery, totalDamage, impactPt, pushAngle, pushAmount);
            // EnemyObj.HitCharacterSlave(this, damageObj, b, mastery, totalDamage, impactPt, pushAngle, pushAmount);
            // this.SyncLandingSpotIfNeeded();
            // if (projectileObj == null || !projectileObj.destroysOnCollision)
            //   return;
            // projectileObj.RunDestroyAnimation();
            //if (this.player.currentClassType != ClassType.Tank)
            //{
            this.HitCharacterPush(beingAttacked,
                damageObj,
                canStunLikeEnemy,
                canKnockdown,
                pushAmount,
                pushAngle,
                stunCount);
            //}
        }

        private static int StunCharacter(PlayerClassObj beingAttacked, GameController game)
        {
            int knockdownStr;
            GameController.soundManager.PlayEvent("event:/SFX/Characters/Generic/Hits/sfx_char_gen_hit_stun_crit",
                (IPositionalObj) beingAttacked, false, false);
            BrawlerSpriteObj fgSprite = game.SpriteManager.GetFGSprite("AirCrit_BG");
            fgSprite.TextureColor = Color.Blue;
            fgSprite.Position = beingAttacked.m_position;
            fgSprite.Scale = Vector2.Zero;
            fgSprite.Rotation = RNG.get(307).RandomFloat(-15f, 15f);
            fgSprite.disableAutoKill = true;
            Tween.To((object) fgSprite, 0.25f, new Easing(Back.EaseOut), (object) "ScaleX", (object) 0.75f,
                (object) "ScaleY", (object) 0.75f);
            Tween.To((object) fgSprite, 0.25f, new Easing(Back.EaseIn), (object) "delay", (object) 0.5f,
                (object) "ScaleX", (object) 0, (object) "ScaleY", (object) 0);
            Tween.AddEndHandlerToLastTween((object) game.SpriteManager, "DestroyObj", (object) SpriteEnum.SpriteFG,
                (object) fgSprite);
            BrawlerTextObj brawlerTextObj =
                game.TextManager.DrawText(fgSprite.Position, "LOC_ID_STATUS_TEXTS_STUN_CRIT", 1f, 1f);
            brawlerTextObj.AlignText(TextAlign.Centre, WidthAlign.Centre, HeightAlign.Centre);
            brawlerTextObj.ChangeFont("LoveYa15");
            brawlerTextObj.FontSize = 14f;
            brawlerTextObj.outlineColour = new Color(56, 51, 45);
            brawlerTextObj.outlineWidth = 2;
            brawlerTextObj.Rotation = fgSprite.Rotation;
            brawlerTextObj.Scale = Vector2.Zero;
            Tween.To((object) brawlerTextObj, 0.25f, new Easing(Back.EaseOut), (object) "ScaleX", (object) 0.75f,
                (object) "ScaleY", (object) 0.75f);
            Tween.To((object) brawlerTextObj, 0.25f, new Easing(Back.EaseIn), (object) "delay", (object) 0.5f,
                (object) "ScaleX", (object) 0, (object) "ScaleY", (object) 0);

            knockdownStr = 11;
            return knockdownStr;
        }

        private void HandleProjectile(PlayerClassObj beingAttacked, ProjectileObj projectile, bool canKnockdown,
            Vector2 pushAngle, float pushAmount, bool canStunLikeEnemy, float stunCount)
        {
            if (projectile.knockdownStr > 5)
            {
                canKnockdown = true;
            }

            if (projectile.SourceObj is PlayerClassObj sourcePlayer &&
                sourcePlayer.currentAttack.pushEnemyAmount > 0)
            {
                pullByPlayer.Add(beingAttacked, projectile);
                pushAngle = CDGMath.VectorBetweenPts(beingAttacked.AbsPosition, sourcePlayer.Position);
                pushAmount = (float) sourcePlayer.currentAttack.pushEnemyAmount;
            }

            if (projectile is FloatingTurretProjectileObj)
            {
                pullByPlayer.Add(beingAttacked, projectile);
                // this.underTurretAttack = true;
                pushAmount = 0; //(float) ((PlayerClassObj_Engineer) projectile.SourceObj).turretAttack.pushEnemyAmount;
            }

            if (projectile.Tag == "CraterProj")
            {
                pullByPlayer.Add(beingAttacked, projectile);
                pushAmount = PlayerEV.SNIPER_GRAVITYBOMB_PUSH_AMOUNT;
                pushAngle = CDGMath.VectorBetweenPts(beingAttacked.AbsPosition, projectile.Position);
            }

            this.HitCharacterPush(beingAttacked,
                projectile,
                canStunLikeEnemy,
                canKnockdown,
                pushAmount,
                pushAngle,
                stunCount);
        }

        public void HitCharacterPush(PlayerClassObj beingAttacked, IDamageObj obj, bool canStun, bool canKnockdown,
            float pushAmount, Vector2 pushAngle, float stun)
        {
            if (canKnockdown)
            {
                beingAttacked.StopAllActions();
                this.Knockdown(beingAttacked, obj);
                return;
            }

            if (!beingAttacked.inAir && canStun)
            {
                beingAttacked.StopAllActions();
                if (obj is PlayerClassObj playerObjAttacker)
                    this.StunEnemy(beingAttacked, stun, playerObjAttacker);
                beingAttacked.PushTowardsVector(pushAngle,
                    (float) -((double) pushAmount * (double) beingAttacked.PushMod), 0.1f);
            }
        }

        public void StunEnemy(PlayerClassObj playerClassObj, float stunCount, PlayerClassObj classObj = null)
        {
            var mStunCount = ModEntry.helper.Reflection.GetField<float>(playerClassObj, "m_stunCount");
            var mstunDurationCounter = ModEntry.helper.Reflection.GetField<float>(playerClassObj, "m_stunDurationCounter");
            mstunDurationCounter.SetValue(0.75f);
            if (classObj == null)
            {
                mStunCount.SetValue(stunCount + 1);
                return;
            }

            switch (classObj.classType)
            {
                case ClassType.Engineer:
                    mStunCount.SetValue(stunCount + 1);
                    break;
                case ClassType.Sniper:
                    mStunCount.SetValue(stunCount + 10);
                    break;
                default:
                    mStunCount.SetValue(stunCount + 2);
                    break;
            }
        }

        public virtual void Knockdown(PlayerClassObj playerClassObj, IDamageObj attacker)
        {
            playerClassObj.RemoveStatusEffect(StatusEffect.Dizzy, true);
            playerClassObj.RemoveStatusEffect(StatusEffect.Stop, true);
            playerClassObj.LockFlip = true;
            playerClassObj.CurrentSpeed = 0.0f;
            playerClassObj.State = CharacterState.KnockedDown;
            float num1 = 1f;
            float num2 = 1f;
            if (attacker != null)
            {
                Vector2 vector2 = new Vector2(
                    playerClassObj.X - playerClassObj.AnchorX * playerClassObj.ScaleX - attacker.X,
                    playerClassObj.Y - playerClassObj.AnchorY * playerClassObj.ScaleY - attacker.Y);
                if (vector2 != Vector2.Zero)
                    vector2 = Vector2.Normalize(vector2);
                if ((double) attacker.knockdownDistanceMod < 0.0)
                {
                    vector2.X = -vector2.X;
                    vector2.Y = -vector2.Y;
                }

                playerClassObj.HeadingX = vector2.X;
                playerClassObj.HeadingY = vector2.Y;
                num1 = attacker.knockdownSpeedMod;
                num2 = attacker.knockdownDistanceMod;
            }
            else
            {
                playerClassObj.HeadingX = (float) RNG.get(320).RandomPlusMinus();
                playerClassObj.HeadingY = 0.0f;
            }

            if (playerClassObj.objType == 117)
            {
                playerClassObj.CurrentSpeed = 750f;
                playerClassObj.jumpDistance = 1250f;
            }
            else
            {
                playerClassObj.CurrentSpeed = 375f * num1;
                playerClassObj.jumpDistance = 250f * Math.Abs(num2);
            }

            playerClassObj.CalculateFakeAccelerationY();
            //    this.originaljumpDistance = this.jumpDistance;

            //  ++this.m_airComboCount;
            //StatsHelper.MaxAirCritCombo((int) this.m_airComboCount - 1);
            //if (this.m_airComboCount >= (byte) 6)
            // {
            //   PlayerManager playerManager = this.Game.PlayerManager;
            //   int playerArrayCount = playerManager.activePlayerArray_count;
            //   for (int index = 0; index < playerArrayCount; ++index)
            //     playerManager.activePlayerArray[index].COMBO_AIR_CRIT = true;
            // }
            GameController.soundManager.PlayEvent("event:/SFX/Characters/Generic/Hits/sfx_char_gen_hit_knockup_airborn",
                (IPositionalObj) playerClassObj, false, false);
        }
    }
}
