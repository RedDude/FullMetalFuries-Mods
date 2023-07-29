using System;
using System.Collections.Generic;
using Brawler2D;
using cs.Blit;
using Microsoft.Xna.Framework.Input;

namespace FullModdedFuriesAPI.Mods.FriendlyFire
{
    /// <summary>The main entry point for the mod.</summary>
    public class ModEntry : Mod
    {
        private ModConfig Config;
        private static bool IsFriendlyFire = true;

        public static IModHelper helper;

        public static Dictionary<PlayerObj, bool> installed = new Dictionary<PlayerObj, bool>();
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            ModEntry.helper = helper;
            this.Config = this.Helper.ReadConfig<ModConfig>();
            if (!this.Config.Enabled) return;

            this.Helper.Events.GameLoop.UpdateTicked += (sender, args) =>
            {
                // if (!BlitNet.Lobby.IsMaster) return;
                //
                var playerManager = GameController.g_game.PlayerManager;
                if (playerManager == null) return;

                for (int index = 0; index < playerManager.activePlayerArray_count; ++index)
                {
                    var activePlayer = playerManager.activePlayerArray[index];
                    if (!(activePlayer.currentPlayerClass is PlayerClassObj_Sniper sniper)) continue;
                    if (sniper.isAiming || sniper.isGrapnelAiming)
                        SniperBaseAttackHandler.FindPlayersTarget(sniper);
                }
            };

            this.Helper.Events.GameLoop.UpdateTicking += (sender, args) =>
            {
                // if (!BlitNet.Lobby.IsMaster) return;
                //
                // var playerManager = GameController.g_game.PlayerManager;
                // if (playerManager == null) return;
                //
                // for (int index = 0; index < playerManager.activePlayerArray_count; ++index)
                // {
                //     var activePlayer = playerManager.activePlayerArray[index];
                //     if (activePlayer.currentPlayerClass is PlayerClassObj_Sniper sniper)
                //     {
                //         if (sniper.isAiming || sniper.isGrapnelAiming)
                //         SniperBaseAttackHandler.FindPlayersTarget(sniper);
                //     }
                // }

                HandleProjectile();
                HandlePlayers();
                // HandleEnemies();

                if (GameController.g_game.InputManager.KeyJustPressed(Keys.D1))
                {
                    HandleInstallEngAttackPlayers();
                }
                if (GameController.g_game.InputManager.KeyJustPressed(Keys.D2))
                {
                    HandleFixEngTurrets();
                }

            };
        }

        private static void HandleProjectile()
        {
            var projectileManager = GameController.g_game.ProjectileManager;
            if (projectileManager == null) return;
            if (projectileManager.isPaused || projectileManager.IsDisposed) return;

            var list = projectileManager.activeObjList;
            foreach (var activeObj in list)
            {
                // if (activeObj is FloatingTurretProjectileObj turretProjectileObj)
                // {
                //     HandleFloatingTurret(turretProjectileObj);
                //     continue;
                // }
                if (activeObj.masteryEquipSlot == null) continue;
                activeObj.CollisionID = WhenFriendlyChangeCollisionId(2, 10);

                if (activeObj.Tag == "RiskBomb")
                {
                    activeObj.Tag = "RiskBombFriendly";
                    activeObj.forceKnockdown = true;
                }

                if(activeObj.spriteName == "PlayerSniper_MineExplosion")
                    activeObj.forceKnockdown = true;

            }
        }

        private static void HandlePlayers()
        {
            var playerManager = GameController.g_game.PlayerManager;
            if (playerManager == null) return;

            for (int index = 0; index < playerManager.activePlayerArray_count; ++index)
            {
                var activePlayer = playerManager.activePlayerArray[index];
                var player = activePlayer.currentPlayerClass;
                if (player.IsKilled) continue;
                player.CollisionID = WhenFriendlyChangeCollisionId(2, activePlayer.playerIndex);
            }
        }

        private static void HandleEnemies()
        {
            var enemyManager = GameController.g_game.EnemyManager;
            if (enemyManager == null) return;
            if (!enemyManager.areEnemiesAlive) return;

            var enemyArray = enemyManager.enemyArray;
            for (int i = 0; i < enemyManager.enemyArray_count; i++)
            {
                var enemy = enemyArray[i];
                if (enemy.IsKilled || !enemy.Active)
                    continue;

                enemy.CollisionID = WhenFriendlyChangeCollisionId(2, 10);
            }
        }

        private static void HandleInstallEngAttackPlayers()
        {
            var playerManager = GameController.g_game.PlayerManager;
            if (playerManager == null) return;

            for (int index = 0; index < playerManager.activePlayerArray_count; ++index)
            {
                var activePlayer = playerManager.activePlayerArray[index];
                var playerClassObj = (PlayerClassObj_Engineer) activePlayer.GetClass(ClassType.Engineer);
                var engineerBaseAttackHandler = new EngineerBaseAttackHandler();
                engineerBaseAttackHandler.InstallToPlayer(playerClassObj);

                var playerClassObjSniper = (PlayerClassObj_Sniper) activePlayer.GetClass(ClassType.Sniper);
                var sniperBaseAttackHandler = new SniperBaseAttackHandler();
                sniperBaseAttackHandler.InstallToPlayer(playerClassObjSniper);
            }
        }

        private static void HandleFixEngTurrets()
        {
            var playerManager = GameController.g_game.PlayerManager;
            if (playerManager == null) return;

            for (int index = 0; index < playerManager.activePlayerArray_count; ++index)
            {
                var activePlayer = playerManager.activePlayerArray[index];
                var playerClassObj = (PlayerClassObj_Engineer) activePlayer.GetClass(ClassType.Engineer);
                var turretField = ModEntry.helper.Reflection.GetField<List<FloatingTurretProjectileObj>>(playerClassObj, "m_turretList");
                var turrets = turretField.GetValue();
                var newTurrets = new List<FloatingTurretProjectileObj>(turrets.Count);

                for (int i = 0; i < turrets.Count; i++)
                {
                    var newTurret = new FloatingTurretFriendlyFireProjectileObj(playerClassObj);
                    newTurret.Initialize("Engineer_AttackTurret");
                    newTurrets.Add(newTurret);

                    var floatingTurretProjectileObj = turrets[i];
                    floatingTurretProjectileObj.Dispose();
                }
                turretField.SetValue(newTurrets);
            }
        }


        private static void HandleFloatingTurret(FloatingTurretProjectileObj floatingTurretProjectileObj)
        {
        }

//  private void HandleFriendlyFire(IDamageObj damageObj, float dmgOverride, bool baseHit)
//     {
//       pullByPlayer = false;
//       underTurretAttack = false;
//       if (!Mod.Instance.IsFriendlyFire)
//         return;
//
//       Vector2 pushAngle = new Vector2(0.0f, 0.0f);
//       var pushAmount = 0.0f;
//
//       int knockdownDefense = (int) 10; //this.player.knockdownDefense;
//       int stunDefense = (int) 0; //this.stunDefense;
//       int stunStr = damageObj.stunStr;
//       int knockdownStr = damageObj.knockdownStr;
//
//       if (this.m_stunCount >= 19.0)
//       {
//         GameController.soundManager.PlayEvent("event:/SFX/Characters/Generic/Hits/sfx_char_gen_hit_stun_crit", (IPositionalObj) this, false, false);
//         BrawlerSpriteObj fgSprite = m_game.SpriteManager.GetFGSprite("AirCrit_BG");
//         fgSprite.TextureColor = Color.Blue;
//         fgSprite.Position = this.m_position;
//         fgSprite.Scale = Vector2.Zero;
//         fgSprite.Rotation = RNG.get(307).RandomFloat(-15f, 15f);
//         fgSprite.disableAutoKill = true;
//         Tween.To((object) fgSprite, 0.25f, new Easing(Back.EaseOut), (object) "ScaleX", (object) 0.75f, (object) "ScaleY", (object) 0.75f);
//         Tween.To((object) fgSprite, 0.25f, new Easing(Back.EaseIn), (object) "delay", (object) 0.5f, (object) "ScaleX", (object) 0, (object) "ScaleY", (object) 0);
//         Tween.AddEndHandlerToLastTween((object) m_game.SpriteManager, "DestroyObj", (object) SpriteEnum.SpriteFG, (object) fgSprite);
//         BrawlerTextObj brawlerTextObj = m_game.TextManager.DrawText(fgSprite.Position, "LOC_ID_STATUS_TEXTS_STUN_CRIT", 1f, 1f);
//         brawlerTextObj.AlignText(TextAlign.Centre, WidthAlign.Centre, HeightAlign.Centre);
//         brawlerTextObj.ChangeFont("LoveYa15");
//         brawlerTextObj.FontSize = 14f;
//         brawlerTextObj.outlineColour = new Color(56, 51, 45);
//         brawlerTextObj.outlineWidth = 2;
//         brawlerTextObj.Rotation = fgSprite.Rotation;
//         brawlerTextObj.Scale = Vector2.Zero;
//         Tween.To((object) brawlerTextObj, 0.25f, new Easing(Back.EaseOut), (object) "ScaleX", (object) 0.75f, (object) "ScaleY", (object) 0.75f);
//         Tween.To((object) brawlerTextObj, 0.25f, new Easing(Back.EaseIn), (object) "delay", (object) 0.5f, (object) "ScaleX", (object) 0, (object) "ScaleY", (object) 0);
//
//         knockdownStr = 11;
//       }
//
//       var canKnockdown = knockdownStr > knockdownDefense;
//       var canStunLikeEnemy = stunStr > stunDefense;
//
//       if (damageObj is ProjectileObj projectile)
//       {
//         if (projectile.knockdownStr > 5)
//         {
//           canKnockdown = true;
//         }
//         if (projectile.SourceObj is PlayerClassObj sourcePlayer && sourcePlayer.currentAttack.pushEnemyAmount > 0)
//         {
//           pullByPlayer = true;
//           pushAngle = CDGMath.VectorBetweenPts(this.AbsPosition, sourcePlayer.Position);
//           pushAmount = (float) sourcePlayer.currentAttack.pushEnemyAmount;
//         }
//
//         if (projectile is FloatingTurretProjectileObj)
//         {
//           pullByPlayer = true;
//           underTurretAttack = true;
//           pushAmount = (float) ((PlayerClassObj_Engineer) projectile.SourceObj).turretAttack.pushEnemyAmount;
//         }
//
//         if (projectile.Tag == "CraterProj")
//         {
//           pullByPlayer = true;
//           pushAmount = PlayerEV.SNIPER_GRAVITYBOMB_PUSH_AMOUNT;
//           pushAngle = CDGMath.VectorBetweenPts(this.AbsPosition, projectile.Position);
//         }
//
//         HitCharacterPush(projectile, canStunLikeEnemy, canKnockdown, pushAmount, pushAngle);
//         return;
//       }
//
//       if (damageObj is PlayerClassObj playerObjAttacker)
//       {
//         if (playerObjAttacker.currentAttack.pushEnemyAmount > 0)
//         {
//           pullByPlayer = true;
//           pushAngle = CDGMath.VectorBetweenPts(this.AbsPosition, playerObjAttacker.Position);
//           pushAmount = (float) playerObjAttacker.currentAttack.pushEnemyAmount;
//         }
//         // Bitfield b = new Bitfield(19);
//         // b.set<EnemyObj.B>(EnemyObj.B.SniperVisibility, this.State != CharacterState.KnockedDown && this.State != CharacterState.Stunned && (double) this.CurrentHealth > 0.0);
//         // b.set<EnemyObj.B>(EnemyObj.B.alreadyDead, (double) this.CurrentHealth <= 0.0);
//         // b.set<EnemyObj.B>(EnemyObj.B.isBurnDamage, this.burnScope);
//         // b.set<EnemyObj.B>(EnemyObj.B.hasPlayerSource, playerClassObj != null);
//         // b.set<EnemyObj.B>(EnemyObj.B.inAir, this.inAir);
//         // b.set<EnemyObj.B>(EnemyObj.B.stunBreak, (double) this.m_stunCount >= 19.0);
//         // b.set<EnemyObj.B>(EnemyObj.B.IsGrapnelProjectile, damageObj is GrapnelProjectileObj);
//         // b.set<EnemyObj.B>(EnemyObj.B.forceHit, forceHit);
//         // b.set<EnemyObj.B>(EnemyObj.B.damageOverrideNegOne, (double) dmgOverride == -1.0);
//         // b.set<EnemyObj.B>(EnemyObj.B.comboAttack, playerClassObj != null && playerClassObj.currentAttack.isComboAttack);
//         // b.set<EnemyObj.B>(EnemyObj.B.Invincible, this.hasStatusEffect(StatusEffect.Invincible));
//         // b.set<EnemyObj.B>(EnemyObj.B.AirStun, this.hasStatusEffect(StatusEffect.AirStun));
//         // b.set<EnemyObj.B>(EnemyObj.B.isBurned, this.hasStatusEffect(StatusEffect.Burn));
//         // b.set<EnemyObj.B>(EnemyObj.B.dmgGTdmgOverride, playerClassObj != null && (double) playerClassObj.damage > (double) dmgOverride);
//         // b.set<EnemyObj.B>(EnemyObj.B.projIsNull, projectileObj == null);
//         // b.set<EnemyObj.B>(EnemyObj.B.IsGrapnelTargetNull, grapnelProjectileObj != null && grapnelProjectileObj.grapnelTarget == null);
//         // b.set<EnemyObj.B>(EnemyObj.B.IsExplosion, projectileObj != null && !projectileObj.hasShadow);
//
//         if (playerObjAttacker is PlayerClassObj_Engineer && playerObjAttacker.currentAttack is EngineerRollAttack && playerObjAttacker.HasEquippedEquipment(EquipmentType.ENG_SlowRoll)
//             || playerObjAttacker is PlayerClassObj_Tank && knockdownStr > 9
//             || playerObjAttacker is PlayerClassObj_Sniper && knockdownStr > 0
//             || playerObjAttacker is PlayerClassObj_Fighter fighter && fighter.isLastAttack)
//           knockdownDefense = -10;
//
//         if (this.hasStatusEffect(StatusEffect.Unstoppable))
//         {
//           knockdownDefense += 4;
//           stunDefense += 4;
//         }
//
//         canKnockdown = knockdownStr > knockdownDefense;
//         canStunLikeEnemy = stunStr > stunDefense;
//
// //            if (mastery != EquipmentSlotType.ButtonR)
//         //            playerClassObj2.player.SetEquipSlotMasteryPoints(playerClassObj2, mastery, 1, true);
//         //if (b.get<EnemyObj.B>(EnemyObj.B.projIsNull) && b.get<EnemyObj.B>(EnemyObj.B.comboAttack) && b.get<EnemyObj.B>(EnemyObj.B.damageOverrideNegOne))
//         if (playerObjAttacker.currentAttack.isComboAttack)
//           playerObjAttacker.continueCombo = true;
//         switch (playerObjAttacker.classType)
//         {
//           case ClassType.Engineer:
//             // layeredSprite1.ChangeSprite("Effect_EngineerAttack_Sprite");
//             break;
//           case ClassType.Fighter:
//             PlayerClassObj_Fighter playerClassObjFighter = playerObjAttacker as PlayerClassObj_Fighter;
//             //layeredSprite1.ChangeSprite("Effect_FighterAttack_Sprite");
//             if (playerClassObjFighter.isSpinAttacking &&
//                 playerClassObjFighter.HasEquippedEquipment(EquipmentType.FTR_ChargeSpin))
//               playerClassObjFighter.AddToSpinDuration(0.9f);
//             //if (!b.get<EnemyObj.B>(EnemyObj.B.isBurnDamage) && (playerClassObjFighter.currentAttack is FighterFlameMeleeAttack || playerClassObjFighter.currentAttack is FighterMeleeAttack) && (playerClassObjFighter.isLastAttack && !playerClassObjFighter.hitEnemyWithLastAttack))
//             if (burnSource == null &&
//                 (playerClassObjFighter.currentAttack is FighterFlameMeleeAttack ||
//                  playerClassObjFighter.currentAttack is FighterMeleeAttack) &&
//                 (playerClassObjFighter.isLastAttack && !playerClassObjFighter.hitEnemyWithLastAttack))
//             {
//               float skillAmount = playerClassObjFighter.player.GetSkillAmount(SkillType.Fight_Hammer_CD);
//               playerClassObjFighter.ReduceCooldowns(skillAmount);
//               if (playerClassObjFighter.HasEquippedEquipment(EquipmentType.FTR_VampAttack))
//               {
//                 float num = GetTotalDamageDealt(damageObj, dmgOverride) * 0.25f;
//                 playerClassObjFighter.CurrentHealth += num;
//                 playerClassObjFighter.RunHealthCollectAnimation((int) num);
//               }
//
//               playerClassObjFighter.hitEnemyWithLastAttack = true;
//               break;
//             }
//
//             break;
//           case ClassType.Sniper:
//             PlayerClassObj_Sniper playerClassObjSniper = playerObjAttacker as PlayerClassObj_Sniper;
//             playerObjAttacker.ChangeSprite("Effect_SniperAttack_Sprite");
//             //if (!b.get<EnemyObj.B>(EnemyObj.B.projIsNull))
//           {
//             float skillAmount = playerClassObjSniper.player.GetSkillAmount(SkillType.Snp_Grap_Stun);
//
//             //if ((double) skillAmount > 0.0 && ((GrapnelProjectileObj)damageObj).grapnelTarget != null)
//             //b.get<EnemyObj.B>(EnemyObj.B.IsGrapnelProjectile))
//             //{
//               //this.ApplyStatusEffect((CharacterObj) playerClassObjSniper, StatusEffect.ArmorShred, skillAmount, true);
//             //  break;
//             //}
//
//             break;
//           }
//             break;
//           case ClassType.Tank:
//             PlayerClassObj_Tank playerClassObjTank = playerObjAttacker as PlayerClassObj_Tank;
//             if (playerClassObjTank.shoutRegenCount < 5 && playerClassObjTank.currentAttack is TankShoutAttack)
//             {
//               ++playerClassObjTank.shoutRegenCount;
//               float shoutRegen = 1f;
//               if (playerClassObjTank.HasEquippedEquipment(EquipmentType.TNK_TowerShield))
//                 shoutRegen = 0.0f;
//               float shoutRegen2 = playerClassObjTank.player.GetSkillAmount(SkillType.Tank_Shout_Regen) *
//                                   shoutRegen * playerClassObjTank.shieldMaxHealth;
//               playerClassObjTank.shieldCurrentHealth += shoutRegen2;
//             }
//
//             //if (!this.pierceThrough)
//             //{
//             //playerClassObjTank.ForceRunCancel();
//             //  break;
//             //}
//             break;
//         }
//
//         //baseHit = false;
//       }
//
//       // b.set<EnemyObj.B>(EnemyObj.B.canKnockdown, num > knockdownDefense);
//       // b.set<EnemyObj.B>(EnemyObj.B.canStun, stunStr > stunDefense);
//       // float totalDamage = (double) dmgOverride == -1.0 || applyModsToDmgOverride ? this.GetTotalDamageDealt(damageObj, dmgOverride) : dmgOverride;
//       // EquipmentSlotType mastery = EquipmentSlotType.ButtonR;
//       // if ((double) dmgOverride == -1.0 && !this.isNotEnemy && ((double) totalDamage > 0.0 && this.givesMastery))
//       // {
//       //   var result = Mod.Instance.enemyCharm
//       //     ? projectileObj == null && playerClassObj != null
//       //     : projectileObj == null;
//       //
//       //   if (result)
//       //   {
//       //     switch (playerClassObj.classType)
//       //     {
//       //       case ClassType.Engineer:
//       //         mastery = !(playerClassObj.currentAttack is EngineerBaseAttackBig) ? EquipmentSlotType.ButtonX : EquipmentSlotType.ButtonY;
//       //         break;
//       //       case ClassType.Fighter:
//       //         mastery = !((PlayerClassObj_Fighter) playerClassObj).isCounterAttacking || !playerClassObj.HasEquippedEquipment(EquipmentType.FTR_RiskCounter) ? (((PlayerClassObj_Fighter) playerClassObj).isSpinAttacking ? EquipmentSlotType.ButtonB : EquipmentSlotType.ButtonX) : EquipmentSlotType.ButtonY;
//       //         break;
//       //       case ClassType.Sniper:
//       //         mastery = !((PlayerClassObj_Sniper) playerClassObj).isEmpowered ? EquipmentSlotType.ButtonX : EquipmentSlotType.ButtonB;
//       //         break;
//       //       case ClassType.Tank:
//       //         mastery = !(playerClassObj.currentAttack is TankShieldRun) ? EquipmentSlotType.ButtonX : EquipmentSlotType.ButtonA;
//       //         break;
//       //     }
//       //   }
//       //   else {
//       //     var result2 = Mod.Instance.enemyCharm
//       //       ? projectileObj != null && projectileObj.masteryEquipSlot.HasValue && playerClassObj != null
//       //       : projectileObj.masteryEquipSlot.HasValue && playerClassObj != null;
//       //
//       //     if (result2)
//       //       mastery = projectileObj.masteryEquipSlot.Value;
//       //   }
//       // }
//       // if (GlobalEV.ENABLE_ONLINE_CHEAT)
//       //   totalDamage *= 7f;
//       // Net.Master.HitEnemy(this.n_id, damageObj, b, mastery, totalDamage, impactPt, pushAngle, pushAmount);
//       // EnemyObj.HitCharacterSlave(this, damageObj, b, mastery, totalDamage, impactPt, pushAngle, pushAmount);
//       // this.SyncLandingSpotIfNeeded();
//       // if (projectileObj == null || !projectileObj.destroysOnCollision)
//       //   return;
//       // projectileObj.RunDestroyAnimation();
//       //if (this.player.currentClassType != ClassType.Tank)
//       //{
//         HitCharacterPush(this, canStunLikeEnemy, canKnockdown, pushAmount, pushAngle);
//       //}
//     }

        public static int WhenFriendlyChangeCollisionId(int colId, int playerId)
        {
            return IsFriendlyFire ? colId + playerId * 10 : colId;
        }
    }
}
