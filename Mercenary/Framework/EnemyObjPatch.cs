using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using CDGEngine;
using cs.Blit;
using FMOD_;
using FullModdedFuriesAPI;
using FullModdedFuriesAPI.Mods.MercenaryClass;
using FullModdedFuriesAPI.Mods.MercenaryClass.Framework;
using HarmonyLib;
using Microsoft.Xna.Framework;
using SpriteSystem2;

namespace Brawler2D
{
    [HarmonyPatch]
    public class EnemyObjPatch
    {
        private GameController game;
        private static IModHelper Helper;
        private static FieldInfo stunCountField;
        private static FieldInfo burnScopeField;

        public EnemyObjPatch(GameController game, IModHelper helper)
        {
            this.game = game;
            Helper = helper;
        }

        public void Patch()
        {
            var harmony = new Harmony("EnemyObjPatch");
            harmony.Patch(
            original: AccessTools.Method(typeof(EnemyObj), "CollisionResponse"),
            prefix: this.GetHarmonyMethod(nameof(this.Before_CollisionResponse))
            );

            stunCountField =
                typeof(EnemyObj).GetField("m_stunCount",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            burnScopeField =
                typeof(EnemyObj).GetField("burnScope",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
        }

        public static bool Before_CollisionResponse(
            GameObj otherObj,
            Vector2 mtd,
            Hitbox thisBox,
            Hitbox otherBox,
            EnemyObj __instance)
        {
            var enemyObj = __instance;
            if (otherObj is ItemDropObj || otherObj is DoppleGangerObj ||
                otherObj is IJumpableObj jumpableObj && jumpableObj.inAir ||
                otherObj is BrawlerCollHullObj brawlerCollHullObj && brawlerCollHullObj.onlyPlayersCollide)
                return true;
            if (enemyObj.inAir && otherBox.Type == HitboxType.Terrain &&
                (otherObj is IJumpOverObj jumpOverObj && jumpOverObj.jumpable) && enemyObj.landingSpotClear)
                return true;
            // if (otherObj is PlayerClassObj_Engineer classObjEngineer && !(this is IHazardObj))
            // {
            //   if (otherBox.Type == HitboxType.Weapon && classObjEngineer.State != CharacterState.Dodging)
            //     return;
            //   if (otherBox.Type == HitboxType.Weapon && classObjEngineer.State == CharacterState.Dodging && classObjEngineer.HasEquippedEquipment(EquipmentType.ENG_SlowRoll))
            //   {
            //     float dmgOverride = !classObjEngineer.HasEquippedEquipment(EquipmentType.ENG_SlowRoll) ? classObjEngineer.Intelligence * 0.6f : classObjEngineer.Intelligence * 0.6f;
            //     enemyObj.HitCharacter((IDamageObj) classObjEngineer, enemyObj.Position, dmgOverride, true);
            //   }
            // }
            if (otherObj is CharacterObj && otherBox.Type == HitboxType.Terrain && !otherObj.rigidBody
                // || otherObj is EnemyObj && !otherObj.rigidBody)
                )
                return true;
            if (otherObj is ProjectileObj projectileObj)
            {
                if (projectileObj.SourceObj == enemyObj && !projectileObj.canHitSource ||
                    projectileObj.collidesWithCharacterTerrainBox && otherBox.Type == HitboxType.Weapon ||
                    projectileObj.canHitAir && enemyObj.inAir)
                    return true;
                // if (projectileObj.collidesWithCharacterTerrainBox && otherBox.Type == HitboxType.Terrain && projectileObj.SourceObj is PlayerClassObj)
                // {
                //   Rectangle rect1 = thisBox.ToRect();
                //   Rectangle rect2 = otherBox.ToRect();
                //   Point point = Rectangle.Intersect(rect1, rect2).Center;
                //   if (point == Point.Zero)
                //     point = new Point(rect1.Center.X + (rect2.Center.X - rect1.Center.X), rect1.Center.Y + (rect2.Center.Y - rect1.Center.Y));
                //   Vector2 impactPt = new Vector2((float) point.X, (float) point.Y);
                //   enemyObj.HitCharacter((IDamageObj) projectileObj, impactPt);
                // }
                // if (otherBox.Type == HitboxType.Terrain)
                //   return;
            }

            if (enemyObj.isSpawning || otherBox.Type != HitboxType.Weapon) return true;

            var enemyOtherObj = otherObj as EnemyObj;
            if (enemyOtherObj == null)
                if (otherObj is ProjectileObj projectileOtherObj)
                    enemyOtherObj = projectileOtherObj.SourceObj as EnemyObj;

            if (otherObj is not IDamageObj damageObj || enemyOtherObj == null ||
                enemyOtherObj.State == CharacterState.Dodging) return true;

            var rect1 = thisBox.ToRect();
            var rect2 = otherBox.ToRect();
            var point = Rectangle.Intersect(rect1, rect2).Center;
            if (point == Point.Zero)
                point = new Point(rect1.Center.X + (rect2.Center.X - rect1.Center.X),
                    rect1.Center.Y + (rect2.Center.Y - rect1.Center.Y));
            var impactPt = new Vector2(point.X, point.Y);

            // enemyObj.HitCharacter(damageObj, impactPt);

            // var position = enemyObj.Position;
            // var heading = enemyObj.Heading;
            // enemyObj.CollisionResponse(otherObj, mtd, thisBox, otherBox);

            HitCharacterMaster(__instance, (IDamageObj) otherObj, impactPt);
            return false;

            // if (enemyObj.State != CharacterState.KnockedDown || otherBox.Type != HitboxType.Terrain || enemyObj.alreadyRebounded)
            //   return;
            // float num1 = 0.0f;
            // if ((double) mtd.X != 0.0)
            // {
            //   enemyObj.HeadingX = -enemyObj.HeadingX;
            //   if ((double) enemyObj.HeadingX > 0.0)
            //     enemyObj.X += 5f;
            //   else
            //     enemyObj.X -= 5f;
            //   if (enemyObj.inAir && (double) Math.Abs(heading.X) > 0.100000001490116)
            //   {
            //     num1 = (enemyObj.Position.X - enemyObj.thisHitOriginalPos.X) / heading.X;
            //     enemyObj.Y = enemyObj.thisHitOriginalPos.Y + heading.Y * num1;
            //   }
            // }
            // if ((double) mtd.Y != 0.0)
            // {
            //   enemyObj.HeadingY = -enemyObj.HeadingY;
            //   if ((double) enemyObj.HeadingY > 0.0)
            //     enemyObj.Y += 5f;
            //   else
            //     enemyObj.Y -= 5f;
            //   if (enemyObj.inAir && (double) Math.Abs(heading.Y) > 0.100000001490116)
            //   {
            //     num1 = (enemyObj.Position.Y - enemyObj.thisHitOriginalPos.Y) / heading.Y;
            //     enemyObj.X = enemyObj.thisHitOriginalPos.X + heading.X * num1;
            //   }
            // }
            // enemyObj.inAirInitialSyncOffset = position + enemyObj.inAirOffset - enemyObj.Position;
            // enemyObj.inAirOffset = enemyObj.inAirInitialSyncOffset;
            // if (enemyObj.inAir)
            //   enemyObj.jumpDistance = enemyObj.originaljumpDistance - Math.Abs((enemyObj.Heading * num1).Length());
            // float num2 = 0.65f;
            // enemyObj.CurrentSpeed *= num2;
            // enemyObj.jumpDistance *= num2;
            // if ((double) enemyObj.CurrentSpeed != 0.0)
            // {
            //   enemyObj.inAirSmoothingAccTime = 0.0f;
            //   enemyObj.inAirSmoothingTime = (float) ((double) enemyObj.jumpDistance / (double) enemyObj.CurrentSpeed * 0.850000023841858);
            // }
            // enemyObj.thisHitOriginalPos = enemyObj.Position;
            // enemyObj.originaljumpDistance = enemyObj.jumpDistance;
            // enemyObj.CalculateFakeAccelerationY();
            // enemyObj.currentExpectedLandingSpot = enemyObj.Position + enemyObj.Heading * enemyObj.jumpDistance;
            // Vector2 vector2 = enemyObj.shadowPosition + enemyObj.Heading * enemyObj.jumpDistance;
            // enemyObj.alreadyRebounded = true;
        }

        public static void HitCharacterMaster(
            EnemyObj enemyObj,
            IDamageObj damageObj,
            Vector2 impactPt,
            float dmgOverride = -1f,
            bool applyModsToDmgOverride = false,
            bool forceHit = false)
        {
            ProjectileObj projectileObj = null;
            PlayerClassObj playerClassObj = null;
            var pushAngle = new Vector2(0.0f, 0.0f);
            if (!enemyObj.AcceptHit(damageObj, forceHit))
                return;
            if (!enemyObj.AcceptHitPermission(playerClassObj))
            {
                if (projectileObj == null || !projectileObj.destroysOnCollision)
                    return;
                projectileObj.RunDestroyAnimation();
            }
            else
            {
                switch (enemyObj.objType)
                {
                    case 23:
                    case 24:
                    case 25:
                    case 26:
                        var shieldKnightBasic = (Enemy_ShieldKnight_Basic) enemyObj;
                        if (shieldKnightBasic.isBlocking)
                        {
                            float pushEnemyAmount = 0.0f;
                            float pushPlayerAmount = 0.0f;
                            if (playerClassObj != null)
                            {
                                if (playerClassObj.currentAttack.isComboAttack)
                                    playerClassObj.continueCombo = true;
                                pushAngle = CDGMath.VectorBetweenPts(enemyObj.AbsPosition, playerClassObj.Position);
                                var classObjEngineer = playerClassObj as PlayerClassObj_Engineer;
                                pushEnemyAmount = playerClassObj.currentAttack.pushEnemyAmount;
                                if (classObjEngineer != null && projectileObj != null &&
                                    projectileObj is FloatingTurretProjectileObj)
                                    pushEnemyAmount = classObjEngineer.turretAttack.pushEnemyAmount;
                                if (playerClassObj.currentAttack.isActive && playerClassObj.currentAttack.isMeleeAttack)
                                    pushPlayerAmount = shieldKnightBasic.m_blockPushBackAmount;
                            }

                            shieldKnightBasic.SKHitCharacter(playerClassObj, impactPt, pushAngle, pushEnemyAmount,
                                pushPlayerAmount);
                            if (projectileObj == null || !projectileObj.destroysOnCollision)
                                return;
                            projectileObj.RunDestroyAnimation();
                            return;
                        }

                        enemyObj.RemoveStatusEffect(StatusEffect.Haste, true);
                        break;
                    case 115:
                        if (!((Enemy_Prometheus_Computer) enemyObj).computerActive)
                        {
                            if (projectileObj == null || !projectileObj.destroysOnCollision)
                                return;
                            projectileObj.RunDestroyAnimation();
                            return;
                        }

                        break;
                }

                var b = new Bitfield(19);
                var grapnelProjectileObj = projectileObj as GrapnelProjectileObj;
                float pushAmount = 0.0f;
                if (playerClassObj != null)
                {
                    pushAngle = CDGMath.VectorBetweenPts(enemyObj.AbsPosition, playerClassObj.Position);
                    pushAmount = playerClassObj.currentAttack.pushEnemyAmount;
                }

                bool isStunBreak = ((int)stunCountField.GetValue(enemyObj)) >= 19.0;

                b.set(EnemyObj.B.SniperVisibility,
                    enemyObj.State != CharacterState.KnockedDown && enemyObj.State != CharacterState.Stunned &&
                    enemyObj.CurrentHealth > 0.0);
                b.set(EnemyObj.B.alreadyDead, enemyObj.CurrentHealth <= 0.0);
                b.set(EnemyObj.B.isBurnDamage, (bool) burnScopeField.GetValue(enemyObj));
                b.set(EnemyObj.B.hasPlayerSource, playerClassObj != null);
                b.set(EnemyObj.B.inAir, enemyObj.inAir);
                b.set(EnemyObj.B.stunBreak, (bool) isStunBreak);
                b.set(EnemyObj.B.IsGrapnelProjectile, damageObj is GrapnelProjectileObj);
                b.set(EnemyObj.B.forceHit, forceHit);
                b.set(EnemyObj.B.damageOverrideNegOne, dmgOverride == -1.0);
                b.set(EnemyObj.B.comboAttack,
                    playerClassObj != null && playerClassObj.currentAttack.isComboAttack);
                b.set(EnemyObj.B.Invincible, enemyObj.hasStatusEffect(StatusEffect.Invincible));
                b.set(EnemyObj.B.AirStun, enemyObj.hasStatusEffect(StatusEffect.AirStun));
                b.set(EnemyObj.B.isBurned, enemyObj.hasStatusEffect(StatusEffect.Burn));
                b.set(EnemyObj.B.dmgGTdmgOverride,
                    playerClassObj != null && playerClassObj.damage > (double) dmgOverride);
                b.set(EnemyObj.B.projIsNull, projectileObj == null);
                b.set(EnemyObj.B.IsGrapnelTargetNull,
                    grapnelProjectileObj != null && grapnelProjectileObj.grapnelTarget == null);
                b.set(EnemyObj.B.IsExplosion, projectileObj != null && !projectileObj.hasShadow);
                int stunStr = damageObj.stunStr;
                int knockdownStr = damageObj.knockdownStr;

                if (isStunBreak)
                    knockdownStr = 19;
                int knockdownDefense = enemyObj.knockdownDefense;
                int stunDefense = enemyObj.stunDefense;
                if (enemyObj.hasStatusEffect(StatusEffect.Unstoppable))
                {
                    knockdownDefense += 4;
                    stunDefense += 4;
                }

                b.set(EnemyObj.B.canKnockdown, knockdownStr > knockdownDefense);
                b.set(EnemyObj.B.canStun, stunStr > stunDefense);
                float totalDamage = (double) dmgOverride == -1.0 || applyModsToDmgOverride
                    ? enemyObj.GetTotalDamageDealt(damageObj, dmgOverride)
                    : dmgOverride;
                var mastery = EquipmentSlotType.ButtonR;
                // if ((double) dmgOverride == -1.0 && !enemyObj.isNotEnemy && ((double) totalDamage > 0.0 && enemyObj.givesMastery))
                // {
                //   if (projectileObj == null)
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
                //   else if (projectileObj.masteryEquipSlot.HasValue && playerClassObj != null)
                //     mastery = projectileObj.masteryEquipSlot.Value;
                // }
                // if (GlobalEV.ENABLE_ONLINE_CHEAT)
                // totalDamage *= 7f;
                Net.Master.HitEnemy(enemyObj.n_id, damageObj, b, mastery, totalDamage, impactPt, pushAngle, pushAmount);
                EnemyObj.HitCharacterSlave(enemyObj, damageObj, b, mastery, totalDamage, impactPt, pushAngle,
                    pushAmount);
                enemyObj.SyncLandingSpotIfNeeded();
                if (projectileObj == null || !projectileObj.destroysOnCollision)
                    return;
                projectileObj.RunDestroyAnimation();
            }
        }

        protected HarmonyMethod GetHarmonyMethod(string name, int? priority = null)
    {
        var method = new HarmonyMethod(
            AccessTools.Method(this.GetType(), name)
            ?? throw new InvalidOperationException($"Can't find patcher method {name}.")
        );

        if (priority.HasValue)
            method.priority = priority.Value;

        return method;
    }

    }
}
