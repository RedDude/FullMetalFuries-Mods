using System;
using System.Reflection;
using Brawler2D;
using CDGEngine;
using cs.Blit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FullModdedFuriesAPI.Mods.MercenaryClass.Source.ControllableEnemies.ControllableEnemyData
{
    public class MGTurret_ControllableData : IEnemyControllable
    {
        private FieldInfo rageAttackTellDelay;
        private FieldInfo dashStunDefense;
        private FieldInfo dashKnockdownDefense;
        private FieldInfo rageAttackMoveSpeed;
        private FieldInfo rageAttackMoveDistance;
        private FieldInfo storedAnimSpeed;
        private FieldInfo storedStunDefense;
        private FieldInfo storedKnockdownDefense;
        private FieldInfo rageAttackExitDelay;
        private LogicSet m_rageAttackLogic;
        private LogicBlock rageAttackLogicBlock;
        private LogicSet logicSetField;
        private FieldInfo fireAmount;
        private FieldInfo firingDelay;
        private FieldInfo bulletDelay;
        private FieldInfo bulletSpeed;
        private LogicBlock fireRandLogicBlock;
        private FieldInfo fireAngle;
        private FieldInfo dummyTarget;
        private FieldInfo firingAngleRandomCap;

        private Type enemyType;

        public MGTurret_ControllableData()
        {
            this.enemyType = typeof(Enemy_MGTurret_Basic);

            this.rageAttackTellDelay =
                typeof(Enemy_Minitaur_Basic).GetField("m_rageAttackTellDelay",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            this.dashStunDefense =
                typeof(Enemy_Minitaur_Basic).GetField("m_dashStunDefense",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            this.dashKnockdownDefense =
                typeof(Enemy_Minitaur_Basic).GetField("m_dashKnockdownDefense",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            this.rageAttackMoveSpeed =
                typeof(Enemy_Minitaur_Basic).GetField("m_rageAttackMoveSpeed",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            this.rageAttackMoveDistance =
                typeof(Enemy_Minitaur_Basic).GetField("m_rageAttackMoveDistance",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            this.storedAnimSpeed =
                typeof(Enemy_Minitaur_Basic).GetField("m_storedAnimSpeed",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            this.storedStunDefense =
                typeof(Enemy_Minitaur_Basic).GetField("m_storedStunDefense",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            this.storedKnockdownDefense =
                typeof(Enemy_Minitaur_Basic).GetField("m_storedKnockdownDefense",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            this.rageAttackExitDelay =
                typeof(Enemy_Minitaur_Basic).GetField("m_rageAttackExitDelay",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            this.rageAttackExitDelay =
                typeof(Enemy_Minitaur_Basic).GetField("m_rageAttackExitDelay",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            this.fireAmount =
                typeof(Enemy_Minitaur_Basic).GetField("m_fireAmount",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            this.firingDelay =
                typeof(Enemy_Minitaur_Basic).GetField("m_firingDelay",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            this.bulletDelay =
                typeof(Enemy_Minitaur_Basic).GetField("m_bulletDelay",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            this.bulletSpeed =
                typeof(Enemy_Minitaur_Basic).GetField("m_bulletSpeed",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            this.fireAngle =
                typeof(Enemy_Minitaur_Basic).GetField("m_fireAngle",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            this.dummyTarget =
                typeof(Enemy_Minitaur_Basic).GetField("m_dummyTarget",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            this.firingAngleRandomCap =
                typeof(Enemy_Minitaur_Basic).GetField("m_firingAngleRandomCap",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            this.firingAngleRandomCap =
                typeof(Enemy_Minitaur_Basic).GetField("m_firingAngleRandomCap",
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
            throw new NotImplementedException();
        }

        public void CastSpellB(CharacterObj source, PlayerObj playerObj)
        {
            var logicSetField =
                this.enemyType.GetField("m_snipeFireLogic",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            var enemyObj = ((EnemyObj) source);
            enemyObj.StopAllActions();

            var logicSet = (LogicSet)logicSetField.GetValue(source);
            var logicBlock = new LogicBlock(RNG.seed);
            logicBlock.AddLogicSet(logicSet);
            enemyObj.m_currentLB = logicBlock;
            enemyObj.m_currentLB.Execute(null, 100);
        }

        public void CastSpellA(CharacterObj character, PlayerObj playerObj)
        {
            var source = (Enemy_MGTurret_Basic) character;

            // if (this.m_rageAttackLogic != null)
            // {
            //     this.m_rageAttackLogic.Dispose();
            //     this.rageAttackLogicBlock.Dispose();
            // }
            //
            // {
            //     float currentRageAttackTellDelay = (float) this.rageAttackTellDelay.GetValue(source) / 2f;
            //     var currentRageAttackMoveSpeed = this.rageAttackMoveSpeed.GetValue(source);
            //
            //     this.rageAttackLogicBlock = new LogicBlock(RNG.seed);
            //     this.m_rageAttackLogic = new LogicSet(this.GetType() + ":m_rageAttackLogic");
            //     LogicSet.Begin(this.m_rageAttackLogic);
            //     LogicSet.Add(new RunFunctionAction(this, "TurnToFace", new object[] {source}), SequenceType.Serial);
            //     // LogicSet.Add(new ChangePropertyAction(source, "LockFlip", true));
            //     LogicSet.Add(
            //         new ChangePropertyAction(source, "State", CharacterState.Attacking));
            //     // LogicSet.Add(new ChangePropertyAction(source, "pulseIndicator", true));
            //     LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed", 0));
            //     LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed", 0.05f));
            //
            //     LogicSet.Add(new DelayAction(currentRageAttackTellDelay));
            //     LogicSet.Add(new PlayEventAction(GameController.soundManager,
            //         "event:/SFX/Enemies/Mobs/Minitaur/sfx_enemy_minitaur_dash_prep", source));
            //     LogicSet.Add(new PlayAnimationAction(source, "Attack1TellStart", "Attack1TellEnd"));
            //     LogicSet.Add(new DelayAction(currentRageAttackTellDelay / 2f));
            //     // LogicSet.Add(new FaceDirectionAction(source, TargetType.DummyTarget));
            //     // LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed", 0));
            //     LogicSet.Add(new PlayEventAction(GameController.soundManager,
            //         "event:/SFX/Enemies/Mobs/Hoplite/Sword/sfx_enemy_hoplite_swing_blink", source));
            //     LogicSet.Add(new ChangeColourAction(source, Color.Red, 0.1f));
            //
            //     // LogicSet.Add(new ChangePropertyAction(source, "LockFlip", true));
            //     LogicSet.Add(new DelayAction(currentRageAttackTellDelay / 2f));
            //     // LogicSet.Add(
            //     // new ChangePropertyAction(source, "pulseIndicator", false));
            //     LogicSet.Add(new PlayEventAction(GameController.soundManager,
            //         "event:/SFX/Enemies/Mobs/Minitaur/sfx_enemy_minitaur_dash", source));
            //     LogicSet.Add(new ChangePropertyAction(source, "stunDefense",
            //         this.dashStunDefense.GetValue(source)));
            //     LogicSet.Add(new ChangePropertyAction(source, "knockdownDefense",
            //         this.dashKnockdownDefense.GetValue(source)));
            //     LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed",
            //         0.03333334f));
            //     LogicSet.Add(
            //         new PlayAnimationAction(source, "Attack1ActionStart", "Attack1ActionEnd", true),
            //         SequenceType.Parallel);
            //     LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed",
            //         currentRageAttackMoveSpeed));
            //     LogicSet.Add(
            //         new ChangePropertyAction(source, "forceKnockdown", true));
            //     LogicSet.Add(new ChangePropertyAction(source, "knockdownSpeedMod",
            //         0.935f));
            //     LogicSet.Add(new ChangePropertyAction(source, "knockdownDistanceMod",
            //         1.1f));
            //     LogicSet.Add(new DelayAction(((float) this.rageAttackMoveDistance.GetValue(source)) /
            //                                  (float) currentRageAttackMoveSpeed));
            //     LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed", 0));
            //     LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed",
            //         this.storedAnimSpeed.GetValue(source)));
            //     LogicSet.Add(new PlayAnimationAction(source, "Attack1ExitStart", "Attack1ExitEnd"));
            //     // LogicSet.Add(new ChangePropertyAction(source, "LockFlip", false));
            //     LogicSet.Add(new ChangePropertyAction(source, "stunDefense",
            //         this.storedStunDefense.GetValue(source)));
            //     LogicSet.Add(new ChangePropertyAction(source, "knockdownDefense",
            //         this.storedKnockdownDefense.GetValue(source)));
            //     LogicSet.Add(new ChangePropertyAction(source, "State",
            //         CharacterState.Idle));
            //     LogicSet.Add(
            //         new ChangePropertyAction(source, "forceKnockdown", false));
            //     LogicSet.Add(new PlayAnimationAction(source, "StandStart", "StandEnd",
            //         true));
            //     LogicSet.Add(new DelayAction((float) this.rageAttackExitDelay.GetValue(source)));
            //     // LogicSet.Add(new CallCooldownAction(source));
            //     LogicSet.End();
            //     //
            //     this.rageAttackLogicBlock.AddLogicSet(this.m_rageAttackLogic);
            // }

            var m_fireSprayLogic =
                this.enemyType.GetField("m_fireSprayLogic",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            var logicSet = (LogicSet)m_fireSprayLogic.GetValue(source);
            var logicBlock = new LogicBlock(RNG.seed);
            logicBlock.AddLogicSet(logicSet);

            var enemyObj = ((EnemyObj) source);
            enemyObj.StopAllActions();
            // enemyObj.UpdateDummyTarget();
            enemyObj.m_currentLB = logicBlock;
            enemyObj.m_currentLB.Execute(null, 100);
            // this.rageAttackLogicBlock.Execute(null, 100);
        }

        public void CastSpellR(CharacterObj source, PlayerObj playerObj)
        {
            var m_fireSprayLogic =
                this.enemyType.GetField("m_focusFireLogic",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            var fireSprayLogic = (LogicSet)m_fireSprayLogic.GetValue(source);

            var enemyObj = ((EnemyObj) source);
            enemyObj.StopAllActions();
            // enemyObj.UpdateDummyTarget();

            var logicSet = (LogicSet)m_fireSprayLogic.GetValue(source);
            var logicBlock = new LogicBlock(RNG.seed);
            logicBlock.AddLogicSet(logicSet);
            enemyObj.m_currentLB = logicBlock;
            enemyObj.m_currentLB.Execute(null, 100);
        }

        public void Attack(CharacterObj source, PlayerObj playerObj)
        {
            source.ApplyStatusEffect(source, StatusEffect.Dizzy, 2);
            // var logicSetField =
            //     this.enemyType.GetField("logicSetField",
            //         BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            // var fireSprayLogic = (LogicSet)logicSetField.GetValue(source);
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

        public void UpdateDummyTarget(EnemyObj e)
        {
            var enemyObj = (EnemyObj) e;
            var playerObj = EnemyControllableManager.logicBlockStates[enemyObj].player;
            float angle = this.GetActionAngle(playerObj.currentPlayerClass.storedLeftStickDirection,
                playerObj.currentPlayerClass.Flip);
            float num1 = MathHelper.WrapAngle(angle - e.Orientation);
            float max = 9999;
            float num2 = MathHelper.Clamp(num1, -max, max);
            float Orientation = MathHelper.WrapAngle(e.Orientation + num2);
            float HeadingX = (float) Math.Cos(Orientation);
            float HeadingY = (float) Math.Sin(Orientation);

            var dummy = (DummyCharacterObj) this.dummyTarget.GetValue(e);
            dummy.Position = new Vector2(e.Position.X + HeadingX * 2, e.Position.Y + HeadingY * 2);
            dummy.shadowOffset = new Vector2(e.Position.X + HeadingX * 2, e.Position.Y + HeadingY * 2);
            dummy.Flip = e.Flip;
        }

        public void TurnToFace(GameObj obj)
        {
            var enemyObj = (EnemyObj) obj;
            var playerObj = EnemyControllableManager.logicBlockStates[enemyObj].player;
            float angle = this.GetActionAngle(playerObj.currentPlayerClass.storedLeftStickDirection,
                playerObj.currentPlayerClass.Flip);
            float num1 = MathHelper.WrapAngle(angle - obj.Orientation);
            float max = 9999;
            float num2 = MathHelper.Clamp(num1, -max, max);
            obj.Orientation = MathHelper.WrapAngle(obj.Orientation + num2);
            obj.HeadingX = (float) Math.Cos(obj.Orientation);
            obj.HeadingY = (float) Math.Sin(obj.Orientation);

            // obj.Flip = obj.HeadingX > 0 && obj.Flip == SpriteEffects.FlipHorizontally
            //     ? SpriteEffects.None
            //     : SpriteEffects.FlipHorizontally;
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

        public void FireBullet(EnemyObj source, int angleOffset, float speed, bool spreadAngle)
        {
            var enemyObj = (EnemyObj) source;
            var playerObj = EnemyControllableManager.logicBlockStates[enemyObj].player;
            float angle = this.GetActionAngle(playerObj.currentPlayerClass.storedLeftStickDirection,
                playerObj.currentPlayerClass.Flip);
            float wrapAngle = MathHelper.WrapAngle(angle - enemyObj.Orientation);
            float max = 9999;
            float clamp = MathHelper.Clamp(wrapAngle, -max, max);
            float Orientation = MathHelper.WrapAngle(source.Orientation + clamp);
            // obj.HeadingX = (float) Math.Cos(obj.Orientation);
            // obj.HeadingY = (float) Math.Sin(obj.Orientation);

            var projectileObj = GameController.g_game.ProjectileManager.FireProjectile(new ProjectileData()
            {
                collidesWithCollHullTerrainBox = true,
                Damage = source.damage,
                DestroyOnCollision = true,
                Lifespan = 10f,
                Scale = new Vector2(2f, 2f),
                SourceAnchor = new Vector2(70f, -5f),
                Speed = new Vector2(speed, speed),
                // Angle = new Vector2((float) Math.Cos(Orientation), (float) Math.Sin(Orientation)),
                AngleOffset = angleOffset,
                SpriteName = "Projectile_Minitaur_Bullet",
                canStun = true,
                hasShadow = true,
                shadowOffset = new Vector2(0.0f, source.shadowYOffset),
                TurnSpeed = 0.0f
            }, source, targetObj: null);
            projectileObj.FollowOrientation = true;
            projectileObj.canReflect = true;
            projectileObj.destroyedByProjectiles = true;
            projectileObj.Layer = source.Layer + 1f / 1000f;
            projectileObj.CollisionID = 2;
            projectileObj.Orientation = Orientation;
            projectileObj.HeadingX = MathHelper.Clamp((float) Math.Cos(Orientation),
                source.Flip == SpriteEffects.FlipHorizontally ? -1 : -0.1f,
                source.Flip == SpriteEffects.FlipHorizontally ? 0.1f : 1);

            projectileObj.HeadingY = (float) Math.Sin(Orientation);

            // float degrees = Microsoft.Xna.Framework.MathHelper.ToDegrees(projectileObj.Orientation);
            // float num1 = degrees - angleOffset;
            // if (source.Flip == SpriteEffects.FlipHorizontally)
            // {
            //     float num2 = degrees + angleOffset;
            //     num1 = (double) num2 < 0.0 ? -180f - num2 : 180f - num2;
            // }
            //
            // float fireAngleRandomCap = (float) this.firingAngleRandomCap.GetValue(source);
            // float num3 = Math.Abs(num1) - fireAngleRandomCap;
            // float num4 = fireAngleRandomCap;
            // if (spreadAngle)
            // {
            //     num3 = Math.Abs(num1) - fireAngleRandomCap;
            //     num4 = fireAngleRandomCap;
            // }
            //
            // if (source.Flip == SpriteEffects.None)
            // {
            //     if (num1 > (double) num4)
            //         degrees -= num3;
            //     else if (num1 < -(double) num4)
            //         degrees += num3;
            // }
            // else if (num1 > (double) num4)
            //     degrees += num3;
            // else if (num1 < -(double) num4)
            //     degrees -= num3;
            //
            // projectileObj.Orientation = Microsoft.Xna.Framework.MathHelper.ToRadians(degrees);
            // projectileObj.HeadingX = (float) Math.Cos(projectileObj.Orientation);
            // projectileObj.HeadingY = (float) Math.Sin(projectileObj.Orientation);
            // this.m_shotSFX =
            //     GameController.soundManager.PlayEvent("event:/SFX/Enemies/Mobs/Minitaur/sfx_enemy_minitaur_shot",
            //         (IPositionalObj) this);
        }

        public void FireRandBullet(EnemyObj source, int randAngleOffset, float speed)
        {
            randAngleOffset = Math.Abs(randAngleOffset);
            this.FireBullet(source, RNG.get(429).RandomInt(randAngleOffset * -1, randAngleOffset), speed, false);
        }

    }

}
