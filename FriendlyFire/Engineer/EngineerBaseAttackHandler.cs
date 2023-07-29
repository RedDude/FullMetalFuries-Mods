using System;
using System.Collections.Generic;
using System.Reflection;
using Brawler2D;
using CDGEngine;
using FMOD_.Studio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteSystem2;
using Tweener;
using Tweener.Ease;

namespace FullModdedFuriesAPI.Mods.FriendlyFire
{
    public class EngineerBaseAttackHandler
    {
        public List<PlayerClassObj> m_playersToHit = new List<PlayerClassObj>();

        public void InstallToPlayer(PlayerClassObj_Engineer source) //, FieldInfo currentShotSFX)
        {
            var attacks = ModEntry.helper.Reflection.GetField<List<BaseAttack>>(source, "m_attackList").GetValue();
            var attack = ModEntry.helper.Reflection.GetField<LogicSet>(attacks[0], "m_normalAttackLS").GetValue();

            var bigAttack = ModEntry.helper.Reflection.GetField<BaseAttack>(source, "m_bigAttack").GetValue();
            var bigAttackLogic = ModEntry.helper.Reflection.GetField<LogicSet>(bigAttack, "m_normalAttackLS").GetValue();

            this.AddFindPlayerAction(source, attack);
            this.AddFindPlayerAction(source, bigAttackLogic, true);

            // attack.numActions;
            // while (attack.numActions)
            // {
            //
            // }
            // attack.NextLogicAction
            // attack.
            // normalAttackLS.
            // source.attack
        }

        private void AddFindPlayerAction(PlayerClassObj_Engineer source, LogicSet attack, bool isBigAttack = false)
        {
            int i = 0;
            var action = attack.firstNode;
            while (action.NextLogicAction != null)
            {
                action = action.NextLogicAction;
                if (action is RunFunctionAction runFunctionAction)
                {
                    string functionName = ModEntry.helper.Reflection.GetField<string>(runFunctionAction, "m_functionName")
                        .GetValue();
                    if (functionName == "FindEnemyToHit")
                    {
                        attack.AddActionAt(new RunFunctionAction(this, "FindPlayerToHit", new object[3]
                        {
                            source,
                            isBigAttack,
                            true
                        }), i);
                        break;
                    }
                }

                i++;
            }
        }

        public List<PlayerClassObj> FindPlayerToHit(PlayerClassObj_Engineer source, bool isBigAttack, bool hitPlayer)//, FieldInfo currentShotSFX)
        {
            string soundName = "event:/SFX/Characters/Engineer/Pistol/sfx_char_eng_x_pistol_fire_normal";
            if (source.HasEquippedEquipment(EquipmentType.ENG_StrongAttack))
                soundName = "event:/SFX/Characters/Engineer/Pistol/sfx_char_eng_x_deagle_fire_normal";
            else if (source.HasEquippedEquipment(EquipmentType.ENG_MultiAttack))
                soundName = "event:/SFX/Characters/Engineer/Pistol/sfx_char_eng_x_shotgun_fire_normal";
            if (source.currentAmmo == 1)
                soundName += "_lastshot";
            // currentShotSFX.SetValue((EventInstance)GameController.soundManager.PlayEvent(soundName, (IPositionalObj) source, false, false));

            // if (source.currentAmmo > 4)
            // {
            //     int num1 = (int) this.m_currentShotSFX.setParameterValue("clip_empty", 0.0f);
            // }
            // else
            // {
            //     int num2 = (int) this.m_currentShotSFX.setParameterValue("clip_empty", 1f);
            // }

            --source.currentAmmo;
            if (source.currentAmmo <= 0)
                source.StartReloadCooldown();
            this.m_playersToHit.Clear();
            var arenaScreen = source.Game.ScreenManager.arenaScreen;
            var attackEllipse = source.attackEllipse;
            var pt = (source as PlayerClassObj_Engineer).storedLeftStickDirection;
            if (source.player.useKeyboard)
                pt = source.forcedMousePos;
            if (pt == Vector2.Zero)
                pt.X = source.Flip != SpriteEffects.None ? -1f : 1f;
            float num3 = 52f;
            if (source.HasEquippedEquipment(EquipmentType.ENG_MultiAttack))
                num3 = 75f;
            if (source.HasEquippedEquipment(EquipmentType.ENG_StrongAttack))
                num3 = 3f;
            if (source.HasEquippedEquipment(EquipmentType.ENG_SemiAttack))
                num3 = 9f;
            float angle = CDGMath.VectorToAngle(pt);
            float num4 = (float) Math.Sqrt((double) attackEllipse.radiusX * (double) attackEllipse.radiusX +
                                           (double) attackEllipse.radiusY * (double) attackEllipse.radiusY);
            var vector2_1 = CDGMath.AngleToVector(angle - num3) * num4;
            var vector2_2 = CDGMath.AngleToVector(angle + num3) * num4;
            var trianglePt2 = vector2_1 + source.shadowPosition;
            var trianglePt3 = vector2_2 + source.shadowPosition;
            bool flag1 = isBigAttack;
            var gamePlayerManager = source.Game.PlayerManager;
            int activePlayerArrayCount = gamePlayerManager.activePlayerArray_count;
            for (int index1 = 0; index1 < activePlayerArrayCount; ++index1)
            {
                var currentPlayerClass = gamePlayerManager.activePlayerArray[index1].currentPlayerClass;
                if (currentPlayerClass == source)
                    continue;

                if ((flag1 || !currentPlayerClass.inAir) &&
                    (currentPlayerClass.Active && !currentPlayerClass.IsKilled &&
                     (currentPlayerClass.State != CharacterState.Dying)) &&
                    currentPlayerClass.State != CharacterState.Dodging)
                {
                    Hitbox[] hitboxesArray = currentPlayerClass.HitboxesArray;
                    int hitboxesCount = currentPlayerClass.HitboxesCount;
                    for (int index2 = 0; index2 < hitboxesCount; ++index2)
                    {
                        Hitbox hitbox = hitboxesArray[index2];
                        if (hitbox.Type == HitboxType.Body)
                        {
                            bool flag2 = true;
                            if (currentPlayerClass.inAir &&
                                (double) currentPlayerClass.AnchorY * (double) currentPlayerClass.ScaleY > 150.0)
                                flag2 = false;
                            var rect = new CDGRect(hitbox.X, hitbox.Y, hitbox.Width, hitbox.Height);
                            float num5 = CDGMath.AngleBetweenPts(source.shadowPosition, rect.Center);
                            if (source.Flip == SpriteEffects.FlipHorizontally)
                            {
                                if ((double) num5 > 0.0 && (double) angle < 0.0)
                                    num5 -= 360f;
                                else if ((double) num5 < 0.0 && (double) angle > 0.0)
                                    num5 += 360f;
                            }

                            if (flag2 && attackEllipse.Intersects(rect) &&
                                (CDGMath.CDGRectTriangleIntersects(rect, source.shadowPosition, trianglePt2,
                                        trianglePt3) || (double) num5 > (double) angle - (double) num3 &&
                                    (double) num5 < (double) angle + (double) num3))
                            {
                                if (!this.m_playersToHit.Contains(currentPlayerClass))
                                {
                                    this.m_playersToHit.Add(currentPlayerClass);
                                    break;
                                }

                                break;
                            }

                            rect.Y -= currentPlayerClass.AnchorY * currentPlayerClass.ScaleY;
                            float num6 = CDGMath.AngleBetweenPts(source.shadowPosition, rect.Center);
                            if (source.Flip == SpriteEffects.FlipHorizontally)
                            {
                                if ((double) num6 > 0.0 && (double) angle < 0.0)
                                    num6 -= 360f;
                                else if ((double) num6 < 0.0 && (double) angle > 0.0)
                                    num6 += 360f;
                            }

                            if (attackEllipse.Intersects(rect) &&
                                (CDGMath.CDGRectTriangleIntersects(rect, source.shadowPosition, trianglePt2,
                                        trianglePt3) || (double) num6 > (double) angle - (double) num3 &&
                                    (double) num6 < (double) angle + (double) num3))
                            {
                                if (!this.m_playersToHit.Contains(currentPlayerClass))
                                {
                                    this.m_playersToHit.Add(currentPlayerClass);
                                    break;
                                }

                                break;
                            }
                        }
                    }
                }
            }

            PlayerClassObj playerObj1 = null;
            PlayerClassObj playerObj2 = null;
            float num7 = float.MaxValue;
            float num8 = float.MaxValue;
            foreach (PlayerClassObj playerObj3 in this.m_playersToHit)
            {
                float num5 = CDGMath.DistanceBetweenPts(source.AbsPosition, playerObj3.AbsPosition);
                if ((double) num5 < (double) num7)
                {
                    num7 = num5;
                    playerObj1 = playerObj3;
                }

                bool flag2 = false;
                if (source.Flip == SpriteEffects.None && (double) playerObj3.X > (double) source.X ||
                    source.Flip == SpriteEffects.FlipHorizontally && (double) playerObj3.X < (double) source.X)
                    flag2 = true;
                if ((double) num5 < (double) num8 && flag2)
                {
                    num8 = num5;
                    playerObj2 = playerObj3;
                }
            }

            if (playerObj2 != null)
                playerObj1 = playerObj2;
            if (playerObj1 != null)
            {
                if ((double) source.X < (double) playerObj1.X && source.Flip == SpriteEffects.FlipHorizontally)
                    source.Flip = SpriteEffects.None;
                else if ((double) source.X > (double) playerObj1.X && source.Flip == SpriteEffects.None)
                    source.Flip = SpriteEffects.FlipHorizontally;

                // if (this is EngineerBaseAttackBig)
                //     source.canHitDowned = true;
                //
                if (hitPlayer)
                    playerObj1.HitCharacter((IDamageObj) source,
                        playerObj1.AbsPosition - playerObj1.Anchor * playerObj1.Scale, -1f, false, true);
                if (source.HasEquippedEquipment(EquipmentType.ENG_MultiAttack) && !isBigAttack)
                {
                    foreach (var playerObj3 in this.m_playersToHit)
                    {
                        if (playerObj3 != playerObj1)
                            playerObj3.HitCharacter((IDamageObj) source,
                                playerObj1.AbsPosition - playerObj1.Anchor * playerObj1.Scale, -1f, false, true);
                    }
                }

                source.canHitDowned = false;
            }
            else
            {
                var position = new Vector2(source.X + 30f, source.Y - 35f);
                if (source.Flip == SpriteEffects.FlipHorizontally)
                    position.X = source.X - 30f;
                var brawlerTextObj = source.Game.TextManager.DrawText(position, "LOC_ID_STATUS_TEXTS_MISS", 0.55f, 1f);
                brawlerTextObj.ChangeFont("Banger35");
                brawlerTextObj.TextureColor = Color.White;
                brawlerTextObj.TextAlign = TextAlign.Centre;
                brawlerTextObj.HeightAnchorAlign = HeightAlign.Centre;
                brawlerTextObj.Scale = Vector2.Zero;
                brawlerTextObj.FontSize = 10f;
                brawlerTextObj.outlineWidth = 1;
                brawlerTextObj.outlineColour = new Color(1, 46, 3);
                Tween.To((object) brawlerTextObj, 0.1f, new Easing(Quint.EaseOut), (object) "ScaleX", (object) 1,
                    (object) "ScaleY", (object) 1);
                Tween.To((object) brawlerTextObj, 0.1f, new Easing(Quint.EaseIn), (object) "delay", (object) 0.2f,
                    (object) "ScaleX", (object) 0, (object) "ScaleY", (object) 0);
            }

            if (!isBigAttack && (source.HasEquippedEquipment(EquipmentType.ENG_StrongAttack) ||
                                 source.HasEquippedEquipment(EquipmentType.ENG_SemiAttack) ||
                                 source.HasEquippedEquipment(EquipmentType.ENG_MultiAttack)))
            {
                var zero = Vector2.Zero;
                var leftStickDirection  = (source as PlayerClassObj_Engineer).storedLeftStickDirection;
                var vector = playerObj1 == null ? leftStickDirection : playerObj1.Position - source.Position;
                if (!isBigAttack)
                {
                    float num5 = 125f;
                    if (source.HasEquippedEquipment(EquipmentType.ENG_SemiAttack))
                    {
                        num5 = 40f;
                        source.LockFlip = false;
                    }

                    if (source.HasEquippedEquipment(EquipmentType.ENG_MultiAttack))
                    {
                        num5 = 75f;
                        source.LockFlip = false;
                    }

                    source.PushTowardsVector(vector, -num5, 0.2f);
                }
            }

            return this.m_playersToHit;
        }
    }
}
