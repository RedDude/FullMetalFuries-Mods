using System;
using System.Collections.Generic;
using System.Reflection;
using Brawler2D;
using CDGEngine;
using cs.Blit;
using FMOD_.Studio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteSystem2;
using Tweener;
using Tweener.Ease;

namespace FullModdedFuriesAPI.Mods.FriendlyFire
{
    public class SniperBaseAttackHandler
    {
        public List<PlayerClassObj> m_playersToHit = new List<PlayerClassObj>();

        public void InstallToPlayer(PlayerClassObj_Sniper source) //, FieldInfo currentShotSFX)
        {
            var baseAttack = ModEntry.helper.Reflection.GetField<BaseAttack>(source, "m_normalShot").GetValue();
            var attack = ModEntry.helper.Reflection.GetField<LogicSet>(baseAttack, "m_normalAttackLS").GetValue();

            var empowerBaseAttack = ModEntry.helper.Reflection.GetField<BaseAttack>(source, "m_empowerShot").GetValue();
            var empowerAttack = ModEntry.helper.Reflection.GetField<LogicSet>(empowerBaseAttack, "m_normalAttackLS").GetValue();


            this.AddReplacePlayerAction(source, baseAttack, attack);
            this.AddReplacePlayerAction(source, empowerBaseAttack, empowerAttack);
        }

        private void AddReplacePlayerAction(PlayerClassObj_Sniper source, BaseAttack attack, LogicSet attackLs)
        {
            var action = attackLs.firstNode;
            while (action.NextLogicAction != null)
            {
                var previousAction = action;
                action = action.NextLogicAction;
                if (action is not RunFunctionAction runFunctionAction) continue;

                string functionName = ModEntry.helper.Reflection
                    .GetField<string>(runFunctionAction, "m_functionName")
                    .GetValue();

                if (functionName != "FindObjectToHit") continue;

                var run = new RunFunctionAction(this, "FindObjectToHitFriendlyFire", new object[2] {source, attack})
                {
                    ParentLogicSet = action.ParentLogicSet,
                    PreviousLogicAction = action.PreviousLogicAction,
                    NextLogicAction = action.NextLogicAction
                };
                previousAction.NextLogicAction = run;
                break;
            }
        }

        public static void FindPlayersTarget(PlayerClassObj_Sniper source)
        {
            var playersObj = new List<GameObj>();
            var game = GameController.g_game;
            var playerManager = game.PlayerManager;
            for (int i = 0; i < playerManager.activePlayerArray_count; i++)
            {
                playersObj.Add(playerManager.activePlayerArray[i].currentPlayerObj);
            }
            var pt = source.storedLeftStickDirection;
            if (pt == Vector2.Zero)
            {
                pt = new Vector2(1f, 0.0f);
                if (source.Flip == SpriteEffects.FlipHorizontally)
                    pt = new Vector2(-1f, 0.0f);
            }
            pt.Normalize();
            float angle = CDGMath.VectorToAngle(pt);
            var mLaserLength = ModEntry.helper.Reflection.GetField<float>(source, "m_laserLength");
            var mGrapnelProjectileObj = ModEntry.helper.Reflection.GetField<GrapnelProjectileObj>(source, "m_grapnelProjectileObj");

            float laserLength = mLaserLength.GetValue();
            var hitboxList = ModEntry.helper.Reflection.GetField<List<CDGRect>>(source, "m_hitboxList").GetValue();

            var lineStartPt = source.Position - pt * 100f;
            var lineEndPt = source.Position + pt * laserLength;

            foreach (var objHit in playersObj)
            {
                FindPlayerTarget(source, objHit, angle, lineStartPt, lineEndPt, hitboxList);
            }
            var pt1 = Vector2.Zero;
            if (source.targetedObjList.Count > 0)
            {
                if (source.HasEquippedEquipment(EquipmentType.SNP_PierceShot) && source.isAiming && !source.isEmpowered)
                {
                    for (int index = 0; index < source.targetedObjList.Count; ++index)
                    {
                        if (!(source.targetedObjList[index] is BrawlerCollHullObj)) continue;
                        pt1 = source.targetedPointList[index];
                        break;
                    }
                }
                else
                {
                    for (int index = 0; index < source.targetedObjList.Count; ++index)
                    {
                        if (source.targetedObjList[index] is PlayerClassObj objsToHit &&
                            (!objsToHit.Collidable)) continue;
                        pt1 = source.targetedPointList[index];
                        break;
                    }
                }
            }
            if (pt1 != Vector2.Zero)
                mLaserLength.SetValue(Math.Abs(CDGMath.DistanceBetweenPts(pt1, source.Position)));
            mGrapnelProjectileObj.GetValue().grapnelCutoff = mLaserLength.GetValue();
        }


        private static void FindPlayerTarget(PlayerClassObj_Sniper sniper, GameObj objHit,
            float angle, Vector2 lineStartPt, Vector2 lineEndPt, List<CDGRect> hitboxList)
        {
            // if (!Mod.Instance.IsFriendlyFire)
            // {
            // return false;
            // }

            if (!(objHit is PlayerClassObj playerObj)) return;
            if (playerObj == sniper || !playerObj.Collidable ||
                (sniper.HasEquippedEquipment(EquipmentType.SNP_PassHook) && sniper.isGrapnelAiming) ||
                (!playerObj.Active || playerObj.IsKilled || (playerObj.State == CharacterState.Dying)) ||
                (playerObj.State == CharacterState.Dodging ||
                 (!sniper.isGrapnelAiming && (playerObj.State == CharacterState.KnockedDown || playerObj.inAir))) ||
                (sniper.isGrapnelAiming && playerObj.State == CharacterState.KnockedDown && !playerObj.inAir))
                return;
            float distance = CDGMath.AngleBetweenPts(sniper.shadowPosition, playerObj.shadowPosition);
            if (playerObj.Flip == SpriteEffects.FlipHorizontally)
            {
                if (distance > 0.0 && angle < 0.0)
                    distance -= 360f;
                else if (distance < 0.0 && angle > 0.0)
                    distance += 360f;
            }

            var hitboxesArray = playerObj.HitboxesArray;
            int hitboxesCount = playerObj.HitboxesCount;
            for (int index1 = 0; index1 < hitboxesCount; ++index1)
            {
                var hitbox = hitboxesArray[index1];
                if (hitbox.Type != HitboxType.Body) continue;
                var rect = new CDGRect(hitbox.X, hitbox.Y, hitbox.Width, hitbox.Height);
                if (sniper.isGrapnelAiming && playerObj.inAir)
                    rect.Y -= playerObj.AnchorY * playerObj.ScaleY;
                bool flag2 = distance > angle - 90.0 && distance < angle + 90.0;
                if (!playerObj.inAir && !flag2) continue;
                var cdgRectIntersectPt = CDGMath.LineToCDGRectIntersectPt(lineStartPt, lineEndPt, rect,
                    playerObj.Rotation, Vector2.Zero);
                if (cdgRectIntersectPt.X == 3.40282347E+38f) continue;
                if (sniper.targetedObjList.Count < 1)
                {
                    sniper.targetedObjList.Add(playerObj);
                    hitboxList.Add(rect);
                    sniper.targetedPointList.Add(cdgRectIntersectPt);
                    break;
                }

                bool flag3 = false;
                for (int index2 = 0; index2 < sniper.targetedPointList.Count; ++index2)
                {
                    float num2 = CDGMath.DistanceBetweenPts(sniper.AbsPosition,
                        sniper.targetedPointList[index2]);
                    if (CDGMath.DistanceBetweenPts(sniper.AbsPosition, cdgRectIntersectPt) <=
                        (double) num2)
                    {
                        sniper.targetedObjList.Insert(index2, playerObj);
                        hitboxList.Insert(index2, rect);
                        sniper.targetedPointList.Insert(index2, cdgRectIntersectPt);
                        flag3 = true;
                        break;
                    }
                }

                if (!flag3)
                {
                    sniper.targetedObjList.Add(playerObj);
                    hitboxList.Add(rect);
                    sniper.targetedPointList.Add(cdgRectIntersectPt);
                    break;
                }

                break;
            }
        }

        public void FindObjectToHitFriendlyFire(PlayerClassObj msource, BaseAttack attack)
        {
            // var baseAttack = ModEntry.helper.Reflection.GetMethod(attack, "FindObjectToHit");
            // baseAttack.Invoke();
            //
            // return;
            var source = msource as PlayerClassObj_Sniper;
            FindPlayersTarget(source);
            var direction = (msource as PlayerClassObj_Sniper).storedLeftStickDirection;
            if (direction == Vector2.Zero)
            {
                direction = new Vector2(1f, 0.0f);
                if (source.Flip == SpriteEffects.FlipHorizontally)
                    direction = new Vector2(-1f, 0.0f);
            }

            var storedDirection = ModEntry.helper.Reflection
                .GetField<Vector2>(attack, "m_storedDirection");
            storedDirection
                .SetValue(direction);

            var targetedObjList = source.targetedObjList;
            if (targetedObjList.Count < 1)
                source.consecutiveHits = 0;
            var pt2 = Vector2.Zero;
            if (targetedObjList.Count > 0)
            {
                if (source.HasEquippedEquipment(EquipmentType.SNP_PierceShot) && source.isAiming && !source.isEmpowered)
                {
                    for (int index = 0; index < targetedObjList.Count; ++index)
                    {
                        if (targetedObjList[index] is BrawlerCollHullObj)
                        {
                            pt2 = source.targetedPointList[index];
                            break;
                        }
                    }
                }
                else
                    pt2 = source.targetedPointList[0];
            }

            bool flag1 = true;
            bool flag2 = false;

            var mStunStr = ModEntry.helper.Reflection
                .GetField<int>(attack, "m_knockdownStr");

            var mKnockdownStr = ModEntry.helper.Reflection
                .GetField<int>(attack, "m_stunStr");

            var createExplosion = ModEntry.helper.Reflection
                .GetMethod(attack, "CreateExplosion");

            int knockdownStr = mKnockdownStr.GetValue();
            int stunStr = mStunStr.GetValue();
            if (source.HasEquippedEquipment(EquipmentType.SNP_TripleEmpower) && source.isEmpowered)
            {
                mKnockdownStr.SetValue(0);
                mStunStr.SetValue(0);
                source.givenStatusEffect = StatusEffect.Dizzy;
                source.givenStatusEffectDuration = 5f;
            }

            if (source.HasEquippedEquipment(EquipmentType.SNP_FastShot) && !source.isEmpowered)
            {
                mKnockdownStr.SetValue(1);
                mStunStr.SetValue(3);
                source.givenStatusEffect = StatusEffect.Burn;
                source.givenStatusEffectDuration = 3f;
            }
            else if (source.HasEquippedEquipment(EquipmentType.SNP_PowerShot) && !source.isEmpowered)
            {
                mKnockdownStr.SetValue(9);
                mStunStr.SetValue(9);
            }

            for (int index = 0; index < targetedObjList.Count; ++index)
            {
                var gameObj = targetedObjList[index];
                var enemyObj = gameObj as EnemyObj;
                var playerObj = gameObj as PlayerClassObj;
                var brawlerCollHullObj = gameObj as BrawlerCollHullObj;
                if (playerObj != null)
                {
                    if (source.isEmpowered)
                    {
                        if (source.HasEquippedEquipment(EquipmentType.SNP_KillEmpower) ||
                            source.HasEquippedEquipment(EquipmentType.SNP_TripleEmpower))
                        {
                            var absPosition = gameObj.AbsPosition;
                            absPosition.Y -= gameObj.AnchorY * gameObj.ScaleY;
                            playerObj.HitCharacter(source, absPosition, -1f, false, true);
                            flag2 = true;
                        }

                        if (index == 0 && !source.HasEquippedEquipment(EquipmentType.SNP_KillEmpower) &&
                            !source.HasEquippedEquipment(EquipmentType.SNP_TripleEmpower))
                        {
                            flag2 = true;
                            createExplosion.Invoke(new object[] { source.targetedPointList[index], gameObj });
                        }
                    }
                    else if (flag1 && playerObj.inAir)
                    {
                        var absPosition = playerObj.AbsPosition;
                        absPosition.Y -= playerObj.AnchorY * playerObj.ScaleY;
                        playerObj.HitCharacter(source, absPosition, -1f, false, true);
                        if (!playerObj.hasStatusEffect(StatusEffect.Invincible) || !playerObj.isInvincible)
                            GameController.soundManager.PlayEvent(
                                "event:/SFX/Characters/Sniper/Rifle/Shot/sfx_char_sni_x_rifle_shot_air_bonus",
                                source, false, false);
                        flag2 = true;
                    }
                    else
                    {
                        flag2 = true;
                        playerObj.HitCharacter(source, source.targetedPointList[index], -1f, false,
                            true);
                    }
                }
                else if (enemyObj != null)
                {
                    if (source.isEmpowered)
                    {
                        if (source.HasEquippedEquipment(EquipmentType.SNP_KillEmpower) ||
                            source.HasEquippedEquipment(EquipmentType.SNP_TripleEmpower))
                        {
                            var absPosition = gameObj.AbsPosition;
                            absPosition.Y -= gameObj.AnchorY * gameObj.ScaleY;
                            enemyObj.HitCharacter(source, absPosition, -1f, false, true);
                            flag2 = true;
                        }

                        if (index == 0 && !source.HasEquippedEquipment(EquipmentType.SNP_KillEmpower) &&
                            !source.HasEquippedEquipment(EquipmentType.SNP_TripleEmpower))
                        {
                            flag2 = true;

                            createExplosion.Invoke(new object[] { source.targetedPointList[index], gameObj });
                        }
                    }
                    else if (flag1 && enemyObj.inAir)
                    {
                        var absPosition = enemyObj.AbsPosition;
                        absPosition.Y -= enemyObj.AnchorY * enemyObj.ScaleY;
                        enemyObj.HitCharacter(source, absPosition, -1f, false, true);
                        if (!enemyObj.hasStatusEffect(StatusEffect.Invincible))
                            GameController.soundManager.PlayEvent(
                                "event:/SFX/Characters/Sniper/Rifle/Shot/sfx_char_sni_x_rifle_shot_air_bonus",
                                source, false, false);
                        flag2 = true;
                    }
                    else
                    {
                        flag2 = true;
                        enemyObj.HitCharacter(source, source.targetedPointList[index], -1f, false,
                            true);
                    }
                }
                else
                {
                    var layeredSprite =
                        source.Game.SpriteManager.GetLayeredSprite("Effect_SniperAttack_Sprite");
                    layeredSprite.Position = source.targetedPointList[index];
                    layeredSprite.Layer = source.Layer + 1f / 1000f;
                    layeredSprite.lockLayer = true;
                    layeredSprite.AnimationSpeed = 0.03333334f;
                    layeredSprite.Scale = new Vector2(2f, 2f);
                    layeredSprite.PlayAnimation(false, false);
                    if (source.isEmpowered && !source.HasEquippedEquipment(EquipmentType.SNP_KillEmpower) &&
                        !source.HasEquippedEquipment(EquipmentType.SNP_TripleEmpower))
                        createExplosion.Invoke(new object[] { source.targetedPointList[index], gameObj });
                }

                if (source.HasEquippedEquipment(EquipmentType.SNP_PierceShot) && !source.isEmpowered)
                {
                    if (brawlerCollHullObj != null)
                        break;
                }
                else if (enemyObj == null || enemyObj != null && !enemyObj.pierceThrough)
                    break;
            }

            if (flag2)
                ++source.consecutiveHits;
            else
                source.consecutiveHits = 0;
            source.givenStatusEffect = StatusEffect.None;
            mKnockdownStr.SetValue(knockdownStr);
            mStunStr.SetValue(stunStr);
            var leftStickDirection = (source as PlayerClassObj_Sniper).storedLeftStickDirection;
            if (leftStickDirection == Vector2.Zero)
                leftStickDirection.X = source.Flip != SpriteEffects.None ? -1f : 1f;
            float angle = CDGMath.VectorToAngle(leftStickDirection);
            var layeredSprite1 = source.Game.SpriteManager.GetLayeredSprite("Sniper_Smoke_Effect_01");
            float num = CDGMath.DistanceBetweenPts(source.AbsPosition, pt2);
            if (num > 1000.0)
                num = 1000f;
            layeredSprite1.ScaleX = num / layeredSprite1.Bounds.Width;
            layeredSprite1.ScaleY = 10f / layeredSprite1.Bounds.Height;
            var vector2_2 = CDGMath.RotatedPoint(new Vector2(source.X + 65f, source.Y),
                source.Position, angle);
            layeredSprite1.Position = vector2_2;
            layeredSprite1.Rotation = angle;
            layeredSprite1.disableAutoKill = true;
            layeredSprite1.Layer = source.Layer;
            if (source.isEmpowered)
            {
                layeredSprite1.TextureColor = PlayerEV.SNIPER_EMPOWERED_SHOT_COLOR;
                if (source.HasEquippedEquipment(EquipmentType.SNP_BombEmpower))
                    layeredSprite1.TextureColor = PlayerEV.SNIPER_BOMBEMPOWER_SHOT_COLOR;
                if (source.HasEquippedEquipment(EquipmentType.SNP_KillEmpower))
                    layeredSprite1.TextureColor = PlayerEV.SNIPER_KILLEMPOWER_SHOT_COLOR;
                if (source.HasEquippedEquipment(EquipmentType.SNP_TripleEmpower))
                    layeredSprite1.TextureColor = PlayerEV.SNIPER_TRIPLESHOT_SHOT_COLOR;
            }

            layeredSprite1.Opacity = 0.7f;
            Tween.To(layeredSprite1, 1f, new Easing(Quad.EaseOut), (object) "Opacity", (object) 0);
            Tween.By(layeredSprite1, 1f, new Easing(Quad.EaseOut), (object) "X",
                (object) RNG.get(141).RandomFloat(-10f, -5f));
            Tween.By(layeredSprite1, 1f, new Easing(Quad.EaseOut), (object) "ScaleY",
                (object) RNG.get(142).RandomFloat(0.1f, 0.3f), (object) "ScaleX",
                (object) RNG.get(143).RandomFloat(0.1f, 0.3f));
            Tween.RunFunction(1f, source.Game.SpriteManager, "DestroyObj",
                (object) SpriteEnum.SpriteLayered, (object) layeredSprite1);
            if (source.HasEquippedEquipment(EquipmentType.SNP_PowerShot) && !source.isEmpowered)
                source.PushTowardsVector(-storedDirection.GetValue(), 300f, 0.15f);
            source.isEmpowered = false;
        }
    }
}
