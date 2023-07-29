﻿using System;
using System.Reflection;
using Brawler2D;
using CDGEngine;
using cs.Blit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tweener;

namespace FullModdedFuriesAPI.Mods.MercenaryClass.Source.ControllableEnemies.ControllableEnemyData
{
    public class WallCannon_ControllableData : IEnemyControllable
    {
        private FieldInfo grenadeAttackDelayField;
        private FieldInfo grenadeAmountField;

        private FieldInfo grenadeDistanceField;
        private FieldInfo grenadeDistanceRandomField;
        private FieldInfo grenadeSpeedField;

        private FieldInfo mineDistanceRandomField;
        private FieldInfo mineSpeedField;
        private FieldInfo mineExplosionSizeField;
        private FieldInfo mineTriggerDelayField;
        private FieldInfo mineTriggerRadiusXField;
        private FieldInfo mineLifeSpanField;

        private LogicSet grenadeAttackLogic;
        private LogicBlock grenadeAttackBlock;

        private LogicSet mineAttackLogic;
        private LogicBlock mineAttackLogicBlock;
        private FieldInfo mineAttackDelayField;
        private FieldInfo mineAmountField;
        private LogicSet dodgeLogic;
        private LogicBlock dodgeLogicBlock;
        private FieldInfo grenadeReticleWidthField;
        private FieldInfo mineReticleWidthField;
        private FieldInfo storedAnimSpeedField;
        private Type enemyType;

        public WallCannon_ControllableData()
        {
            this.enemyType = typeof(Enemy_WallCannon_Basic);

            this.grenadeAttackDelayField =
                this.enemyType.GetField("m_grenadeAttackDelay",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            this.grenadeAmountField =
                this.enemyType.GetField("m_grenadeAmount",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            this.grenadeSpeedField =
                this.enemyType.GetField("m_grenadeSpeed",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            this.grenadeDistanceRandomField =
                this.enemyType.GetField("m_grenadeDistanceRandom",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            this.grenadeDistanceField =
                this.enemyType.GetField("m_grenadeDistance",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            this.grenadeDistanceField =
                this.enemyType.GetField("m_grenadeDistance",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            this.grenadeReticleWidthField =
                this.enemyType.GetField("m_grenadeReticleWidth",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            // mine

            this.mineAttackDelayField =
                this.enemyType.GetField("m_mineAttackDelay",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            this.mineAmountField =
                this.enemyType.GetField("m_mineAmount",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);


            this.mineSpeedField =
                this.enemyType.GetField("m_mineSpeed",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            this.mineDistanceRandomField =
                this.enemyType.GetField("m_mineDistanceRandom",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);


            this.mineExplosionSizeField =
                this.enemyType.GetField("m_mineExplosionSize",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);


            this.mineTriggerDelayField =
                this.enemyType.GetField("m_mineTriggerDelay",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            this.mineTriggerRadiusXField =
                this.enemyType.GetField("m_mineTriggerRadiusX",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            this.mineLifeSpanField =
                this.enemyType.GetField("m_mineLifeSpan",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);


            this.storedAnimSpeedField =
                this.enemyType.GetField("m_storedAnimSpeed",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);



            // this.mineReticleWidthField =
                // this.enemyType.GetField("m_mineReticleWidth",
                    // BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

        }



        public bool CheckCanControl(CharacterObj character)
        {
            return true;
        }

        public float InterfaceOffset(CharacterObj character)
        {
            return 0;
        }

        public float SitOffset(CharacterObj character)
        {
            return +5;
        }

        public void CastSpellY(CharacterObj character, PlayerObj playerObj)
        {
        }

        public void CastSpellB(CharacterObj source, PlayerObj playerObj)
        {
            var logicSetField =
                this.enemyType.GetField("m_rotatingLogic",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            var enemyObj = ((EnemyObj) source);
            enemyObj.StopAllActions();

            var logicSet = (LogicSet)logicSetField.GetValue(source);
            var logicBlock = new LogicBlock(RNG.seed);
            logicBlock.AddLogicSet(logicSet);
            enemyObj.m_currentLB = logicBlock;
            enemyObj.m_currentLB.Execute(null, 100);
        }

        public void CastSpellA(CharacterObj source, PlayerObj playerObj)
        {
            var logicSetField =
                this.enemyType.GetField("m_cardinalLogic",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            var enemyObj = ((EnemyObj) source);
            enemyObj.StopAllActions();

            var logicSet = (LogicSet)logicSetField.GetValue(source);
            var logicBlock = new LogicBlock(RNG.seed);
            logicBlock.AddLogicSet(logicSet);
            enemyObj.m_currentLB = logicBlock;
            enemyObj.m_currentLB.Execute(null, 100);
        }

        public void Attack(CharacterObj source, PlayerObj playerObj)
        {
            source.ApplyStatusEffect(source, StatusEffect.Dizzy, 2);
            // var logicSetField =
            //     this.enemyType.GetField("m_fireChaseLogic",
            //         BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            //
            // var enemyObj = ((EnemyObj) source);
            // enemyObj.StopAllActions();
            //
            // var logicSet = (LogicSet)logicSetField.GetValue(source);
            // var logicBlock = new LogicBlock(RNG.seed);
            // logicBlock.AddLogicSet(logicSet);
            // enemyObj.m_currentLB = logicBlock;
            // enemyObj.m_currentLB.Execute(null, 100);
        }


        public void CastSpellR(CharacterObj source, PlayerObj playerObj)
        {
            // var logicSetField =
            //     this.enemyType.GetField("m_attack1Logic",
            //         BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            //
            // var enemyObj = ((EnemyObj) source);
            // enemyObj.StopAllActions();
            //
            // var logicSet = (LogicSet)logicSetField.GetValue(source);
            // var logicBlock = new LogicBlock(RNG.seed);
            // logicBlock.AddLogicSet(logicSet);
            // enemyObj.m_currentLB = logicBlock;
            // enemyObj.m_currentLB.Execute(null, 100);
        }
        public float TurnToFace(EnemyObj source)
        {
            var enemyObj = source;
            var playerObj = EnemyControllableManager.logicBlockStates[enemyObj].player;
            float angle = this.GetActionAngle(playerObj.currentPlayerClass.storedLeftStickDirection,
                playerObj.currentPlayerClass.Flip);
            float wrapAngle = MathHelper.WrapAngle(angle - enemyObj.Orientation);
            float max = 9999;
            float clamp = MathHelper.Clamp(wrapAngle, -max, max);
            float orientation = MathHelper.WrapAngle(source.Orientation + clamp);
            return orientation;
        }

        public Vector2 TurnToFaceVector(EnemyObj source)
        {
            float orientation = this.TurnToFace(source);
            return new Vector2((float) Math.Cos(orientation),  (float) Math.Sin(orientation));
        }

        private float GetActionAngle(Vector2 direction, SpriteEffects flip)
        {
            var normalized = direction;
            normalized.Normalize();
            var actionDirection = normalized;
            if (actionDirection == Vector2.Zero)
                actionDirection.X = flip != SpriteEffects.None ? -1f : 1f;
            float angle = CDGMath.VectorToAngle(normalized);
            double actionAngle = Math.Atan(byte.MaxValue * Math.Tan(angle * Math.PI / 180.0) /
                                           (byte.MaxValue * 1.0));
            if (angle > 90.0)
                actionAngle += Math.PI;
            else if (angle < -90.0)
                actionAngle -= Math.PI;
            return (float) actionAngle;
        }


        // public virtual void Dodge(Enemy_Grenadier_Basic source, Vector2? heading)
        // {
        //     Vector2 vector2 = new Vector2(RNG.get(375).RandomFloat(-1f, 1f), RNG.get(376).RandomFloat(-1f, 1f));
        //     source.Collidable = true;
        //     if (heading.HasValue)
        //         vector2 = heading.Value;
        //     source.State = CharacterState.Dodging;
        //     vector2.X = this.Flip != SpriteEffects.None ? -1f : 1f;
        //     if (vector2 != Vector2.Zero)
        //         vector2.Normalize();
        //     source.HeadingX = vector2.X;
        //     source.HeadingY = -vector2.Y;
        //     source.m_currentSpeed = this.m_dodgeSpeed;
        //     source.m_dodgeDistanceCounter = 0.0f;
        //     source.isDodging = true;
        //     source.PlayAnimation("DodgeStart", "DodgeEnd", true, false);
        //     if ((double) this.HeadingX < 0.0)
        //         source.Flip = SpriteEffects.FlipHorizontally;
        //     else
        //         source.Flip = SpriteEffects.None;
        // }


        // public void FireBullet(EnemyObj source, int angleOffset, float speed, bool spreadAngle)
        // {
        //     var enemyObj = source;
        //     var playerObj = EnemyControllableManager.logicBlockStates[enemyObj].player;
        //     float angle = this.GetActionAngle(playerObj.currentPlayerClass.storedLeftStickDirection,
        //         playerObj.currentPlayerClass.Flip);
        //     float wrapAngle = MathHelper.WrapAngle(angle - enemyObj.Orientation);
        //     float max = 9999;
        //     float clamp = MathHelper.Clamp(wrapAngle, -max, max);
        //     float Orientation = MathHelper.WrapAngle(source.Orientation + clamp);
        //     // obj.HeadingX = (float) Math.Cos(obj.Orientation);
        //     // obj.HeadingY = (float) Math.Sin(obj.Orientation);
        //
        //     var projectileObj = GameController.g_game.ProjectileManager.FireProjectile(new ProjectileData
        //     {
        //         collidesWithCollHullTerrainBox = true,
        //         Damage = source.damage,
        //         DestroyOnCollision = true,
        //         Lifespan = 10f,
        //         Scale = new Vector2(2f, 2f),
        //         SourceAnchor = new Vector2(70f, -5f),
        //         Speed = new Vector2(speed, speed),
        //         // Angle = new Vector2((float) Math.Cos(Orientation), (float) Math.Sin(Orientation)),
        //         AngleOffset = angleOffset,
        //         SpriteName = "Projectile_Minitaur_Bullet",
        //         canStun = true,
        //         hasShadow = true,
        //         shadowOffset = new Vector2(0.0f, source.shadowYOffset),
        //         TurnSpeed = 0.0f
        //     }, source, targetObj: null);
        //     projectileObj.FollowOrientation = true;
        //     projectileObj.canReflect = true;
        //     projectileObj.destroyedByProjectiles = true;
        //     projectileObj.Layer = source.Layer + 1f / 1000f;
        //     projectileObj.CollisionID = 2;
        //     projectileObj.Orientation = Orientation;
        //     projectileObj.HeadingX = MathHelper.Clamp((float) Math.Cos(Orientation),
        //         source.Flip == SpriteEffects.FlipHorizontally ? -1 : -0.1f,
        //         source.Flip == SpriteEffects.FlipHorizontally ? 0.1f : 1);
        //
        //     projectileObj.HeadingY = (float) Math.Sin(Orientation);
        //
        //     // float degrees = Microsoft.Xna.Framework.MathHelper.ToDegrees(projectileObj.Orientation);
        //     // float num1 = degrees - angleOffset;
        //     // if (source.Flip == SpriteEffects.FlipHorizontally)
        //     // {
        //     //     float num2 = degrees + angleOffset;
        //     //     num1 = (double) num2 < 0.0 ? -180f - num2 : 180f - num2;
        //     // }
        //     //
        //     // float fireAngleRandomCap = (float) this.firingAngleRandomCap.GetValue(source);
        //     // float num3 = Math.Abs(num1) - fireAngleRandomCap;
        //     // float num4 = fireAngleRandomCap;
        //     // if (spreadAngle)
        //     // {
        //     //     num3 = Math.Abs(num1) - fireAngleRandomCap;
        //     //     num4 = fireAngleRandomCap;
        //     // }
        //     //
        //     // if (source.Flip == SpriteEffects.None)
        //     // {
        //     //     if (num1 > (double) num4)
        //     //         degrees -= num3;
        //     //     else if (num1 < -(double) num4)
        //     //         degrees += num3;
        //     // }
        //     // else if (num1 > (double) num4)
        //     //     degrees += num3;
        //     // else if (num1 < -(double) num4)
        //     //     degrees -= num3;
        //     //
        //     // projectileObj.Orientation = Microsoft.Xna.Framework.MathHelper.ToRadians(degrees);
        //     // projectileObj.HeadingX = (float) Math.Cos(projectileObj.Orientation);
        //     // projectileObj.HeadingY = (float) Math.Sin(projectileObj.Orientation);
        //     // this.m_shotSFX =
        //     //     GameController.soundManager.PlayEvent("event:/SFX/Enemies/Mobs/Minitaur/sfx_enemy_minitaur_shot",
        //     //         (IPositionalObj) this);
        // }

        // public void FireRandBullet(EnemyObj source, int randAngleOffset, float speed)
        // {
        //     randAngleOffset = Math.Abs(randAngleOffset);
        //     this.FireBullet(source, RNG.get(429).RandomInt(randAngleOffset * -1, randAngleOffset), speed, false);
        // }

    }

}
