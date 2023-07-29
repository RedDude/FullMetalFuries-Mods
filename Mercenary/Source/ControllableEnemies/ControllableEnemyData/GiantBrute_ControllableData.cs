using System;
using System.Reflection;
using Brawler2D;
using CDGEngine;
using cs.Blit;
using FMOD_.Studio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tweener;

namespace FullModdedFuriesAPI.Mods.MercenaryClass.Source.ControllableEnemies.ControllableEnemyData
{
    public class GiantBrute_ControllableData : IEnemyControllable
    {
        private LogicSet grenadeAttackLogic;
        private LogicBlock grenadeAttackBlock;

        private LogicSet attackLogic;
        private LogicBlock attackLogicBlock;

        private LogicSet dodgeLogic;
        private LogicBlock dodgeLogicBlock;

        private Type enemyType;
        private FieldInfo storedAnimSpeedField;

        public GiantBrute_ControllableData()
        {
            this.enemyType = typeof(Enemy_GiantBrute_Basic);

            this.storedAnimSpeedField =
                this.enemyType.GetField("m_storedAnimSpeed",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
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
            // var logicSetField =
            //     this.enemyType.GetField("m_swipeAttack2",
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

        public void Attack(CharacterObj source, PlayerObj playerObj)
        {
            float smashTellDelay = 0.75f;
            float smashWaveDelay = 0.1f;
            int smashashTotalAmount = 10;
            float smashShockwaveSize = 0.9f;
            float smashShockwaveScale = 0.12f;
            int smashTotalAmount = 10;
            float smashExitDelay = 0.5f;
            float smashIdleDelay = 1f;
            float smashAttackCD = 2f;

            if (this.dodgeLogic != null)
            {
                this.dodgeLogic?.Dispose();
                this.dodgeLogicBlock?.Dispose();
            }

            this.dodgeLogicBlock = new LogicBlock(RNG.seed);
            this.dodgeLogic = new LogicSet(this.GetType() + ":dodgeLogic");
            LogicSet.Begin(this.dodgeLogic);
            LogicSet.Add(new ChangePropertyAction(source, "pulseIndicator", true));
            LogicSet.Add(
                new ChangePropertyAction(source, "State", CharacterState.Attacking));
            LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed", 0));
            LogicSet.Add(new ChangePropertyAction(source, "LockFlip", true));
            LogicSet.Add(new PlayEventAction(GameController.soundManager,
                "event:/SFX/Enemies/Mobs/GiantBrute/sfx_enemy_brute_smash_prep", source));
            LogicSet.Add(
                new PlayAnimationAction(source, "Attack1TellStart", "Attack1TellEnd"));
            LogicSet.Add(new ChangeColourAction(source, Color.Red, 0.1f));
            LogicSet.Add(new DelayAction(smashTellDelay));
            LogicSet.Add(new RunFunctionAction(source, "PlayRumbleEvent",
                new object[]{}));
            LogicSet.Add(new PlayAnimationAction(source, "Attack1LoopIntroStart",
                "Attack1LoopStart"));
            LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed", 0.01666667f));
            for (int index = 0; index < smashTotalAmount; ++index)
            {
                LogicSet.Add(new RunFunctionAction(source, "CreateShockwave", new object[2]
                {
                    (float) (smashShockwaveSize + index * (double) smashShockwaveScale),
                    false
                }));
                LogicSet.Add(new PlayAnimationAction(source, "Attack1LoopStart",
                    "Attack1ExitStart", resetAnimation: true));
                LogicSet.Add(new DelayAction(smashWaveDelay));
            }

            LogicSet.Add(new ChangePropertyAction(source, "pulseIndicator", false));
            LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed",
                source.AnimationSpeed));
            LogicSet.Add(new PlayEventAction(GameController.soundManager,
                "event:/SFX/Enemies/Mobs/GiantBrute/sfx_enemy_brute_smash_ end", source));
            LogicSet.Add(
                new PlayAnimationAction(source, "Attack1ExitStart", "Attack1ExitEnd"));
            LogicSet.Add(new RunFunctionAction(source, "StopRumbleEvent", new object[1]
            {
                STOP_MODE.ALLOWFADEOUT
            }));
            LogicSet.Add(new DelayAction(smashExitDelay));
            LogicSet.Add(new ChangePropertyAction(source, "State", CharacterState.Idle));
            LogicSet.Add(new ChangePropertyAction(source, "LockFlip", false));
            LogicSet.Add(new PlayAnimationAction(source, "StandStart", "StandEnd", true));
            LogicSet.Add(new DelayAction(smashIdleDelay));
            // LogicSet.Add(new AddLSCooldownAction(source, smashAttackCD, smashGroundLS));
            this.dodgeLogic.Tag = "Tag_CD_JumpAttack";
            // LogicSet.Add(new CallCooldownAction(source));
            LogicSet.End();

            EnemyControllableManager.RunEnemyAttack((EnemyObj) source, ref this.dodgeLogic, ref this.dodgeLogicBlock);
        }

        public void CastSpellA(CharacterObj source, PlayerObj playerObj)
        {
            float jumpAttackLandTime = 1.25f;
            float jumpAttackTellDelay = 0.75f;
            float jumpAttackExitDelay = 0.35f;
            float jumpAttackIdleDelay = 0.75f;
            float jumpAttackShockwaveSize = 1.75f;
            float jumpAttackMultiJumpDelay;
            float jumpAttackAmount = 1f;

            if (this.attackLogic != null)
            {
                this.attackLogic?.Dispose();
                this.attackLogicBlock?.Dispose();
            }
            this.attackLogicBlock = new LogicBlock(RNG.seed);
            this.attackLogic = new LogicSet(this.GetType() + ":attackLogic");
            LogicSet.Begin(this.attackLogic);
            // LogicSet.Add(new ChangePropertyAction(source, "pulseIndicator", true));
            LogicSet.Add(
                new ChangePropertyAction(source, "State", CharacterState.Attacking));
            LogicSet.Add(new ChangePropertyAction(source, "LockFlip", true));
            LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed", 0));
            LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed", 0.05f));
            LogicSet.Add(new RunFunctionAction(source, "UpdateFlip",
                new object[]{}));
            LogicSet.Add(new PlayEventAction(GameController.soundManager,
                "event:/SFX/Enemies/Mobs/GiantBrute/sfx_enemy_brute_jump_prep", source));
            LogicSet.Add(new PlayAnimationAction(source, "JumpTellStart", "JumpTellEnd"));
            LogicSet.Add(new DelayAction(jumpAttackTellDelay));
            for (int index = 0; (double) index < (double) jumpAttackAmount; ++index)
            {
                LogicSet.Add(new PlayEventAction(GameController.soundManager,
                    "event:/SFX/Enemies/Mobs/GiantBrute/sfx_enemy_brute_jump_up", source));
                LogicSet.Add(new RunFunctionAction(this, "JumpAttack",
                    new object[]{source}));
                // LogicSet.Add(new NetSyncAction());
                LogicSet.Add(new RunFunctionAction(source, "consumeJumpAttackInfo",
                    new object[]{}));
                LogicSet.Add(new RunFunctionAction(source, "DisplayJumpIndicator",
                    new object[]{}));
                LogicSet.Add(new DelayAction(0.15f));
                LogicSet.Add(
                    new PropertyValueReachedAction(source, "inAir", Convert.ToSingle(false)));
                LogicSet.Add(new RunFunctionAction(source, "DestroyJumpIndicator",
                    new object[]{}));
                LogicSet.Add(new RunFunctionAction(source, "CreateShockwave", new object[2]
                {
                    jumpAttackShockwaveSize,
                    true
                }));
                LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed", 0));
                LogicSet.Add(new PlayAnimationAction(source, "JumpLandStart", "JumpLandEnd"));
                // LogicSet.Add(new DelayAction(jumpAttackMultiJumpDelay));
            }

            // LogicSet.Add(new ChangePropertyAction(source, "pulseIndicator", false));
            LogicSet.Add(new DelayAction(jumpAttackExitDelay));
            LogicSet.Add(
                new PlayAnimationAction(source, "JumpLandExitStart", "JumpLandExitEnd"));
            LogicSet.Add(new ChangePropertyAction(source, "State", CharacterState.Idle));
            LogicSet.Add(new PlayAnimationAction(source, "StandStart", "StandEnd", true));
            // LogicSet.Add(new DelayAction(jumpAttackIdleDelay));
            // LogicSet.Add(new ChangePropertyAction(source, "LockFlip", false));
            // LogicSet.Add(new CallCooldownAction((EnemyObj) source));
            LogicSet.End();

            EnemyControllableManager.RunEnemyAttack((EnemyObj) source, ref this.attackLogic, ref this.attackLogicBlock);
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

        public void JumpAttack(EnemyObj source)
        {
            if (!BlitNet.Lobby.IsMaster)
                return;
            float jumpAttackLandTime = 1.25f;

            var netPos = this.TurnToFaceVector(source);
            // Vector2 netPos = ((PlayerClassObj) this.getTarget(TargetType.CurrentTarget)).NetPos;
            var vector2 = new Vector2(netPos.X - source.X, netPos.Y - source.Y - source.shadowYOffset);
            if (vector2 != Vector2.Zero)
                vector2.Normalize();
            source.HeadingX = vector2.X;
            source.HeadingY = vector2.Y;
            if (source.HeadingX > 0.0)
                source.Flip = SpriteEffects.None;
            else if (source.HeadingX < 0.0)
                source.Flip = SpriteEffects.FlipHorizontally;
            source.jumpDistance = CDGMath.DistanceBetweenPts(source.Position, netPos * 3);
            source.CurrentSpeed = source.jumpDistance / jumpAttackLandTime;
            source.CalculateFakeAccelerationY();
            source.SendSyncAttackLandingSpot();
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
        //     source.m_currentSpeed = dodgeSpeed;
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
        //     // shotSFX =
        //     //     GameController.soundManager.PlayEvent("event:/SFX/Enemies/Mobs/Minitaur/sfx_enemy_minitaur_shot",
        //     //         source);
        // }

        // public void FireRandBullet(EnemyObj source, int randAngleOffset, float speed)
        // {
        //     randAngleOffset = Math.Abs(randAngleOffset);
        //     this.FireBullet(source, RNG.get(429).RandomInt(randAngleOffset * -1, randAngleOffset), speed, false);
        // }

    }

}
