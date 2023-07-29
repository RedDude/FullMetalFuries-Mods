using System;
using System.Reflection;
using Brawler2D;
using CDGEngine;
using cs.Blit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FullModdedFuriesAPI.Mods.MercenaryClass.Source.ControllableEnemies.ControllableEnemyData
{
    public class Minitaur_ControllableData : IEnemyControllable
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
        private LogicSet rageAttackLogic;
        private LogicBlock rageAttackLogicBlock;
        private LogicSet fireRandLogic;
        private FieldInfo fireAmount;
        private FieldInfo firingDelay;
        private FieldInfo bulletDelay;
        private FieldInfo bulletSpeed;
        private LogicBlock fireRandLogicBlock;
        private FieldInfo fireAngle;
        private FieldInfo dummyTarget;
        private FieldInfo firingAngleRandomCap;
        private FieldInfo shotSFXField;

        public Minitaur_ControllableData()
        {
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

            this.shotSFXField =
                typeof(Enemy_Minitaur_Basic).GetField("m_shotSFX",
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

        public void CastSpellB(CharacterObj character, PlayerObj playerObj)
        {
            throw new NotImplementedException();
        }

        public void CastSpellA(CharacterObj character, PlayerObj playerObj)
        {
            var source = (Enemy_Minitaur_Basic) character;

            if (this.rageAttackLogic != null)
            {
                this.rageAttackLogic.Dispose();
                this.rageAttackLogicBlock.Dispose();
            }

            // float currentRageAttackTellDelay = (float) this.rageAttackTellDelay.GetValue(source) / 2f;
            var currentRageAttackMoveSpeed = this.rageAttackMoveSpeed.GetValue(source);
            float exitDelay = (float) this.rageAttackExitDelay.GetValue(source);

            float currentRageAttackTellDelay = 0.0f; //0.4f
            // exitDelay = 0.01f;
            this.rageAttackLogicBlock = new LogicBlock(RNG.seed);
            this.rageAttackLogic = new LogicSet(this.GetType() + ":m_rageAttackLogic");
            LogicSet.Begin(this.rageAttackLogic);
            LogicSet.Add(new RunFunctionAction(this, "TurnToFace", new object[] {source}), SequenceType.Serial);
            LogicSet.Add(new ChangePropertyAction(source, "LockFlip", true));
            LogicSet.Add(
                new ChangePropertyAction(source, "State", CharacterState.Attacking));
            // LogicSet.Add(new ChangePropertyAction(source, "pulseIndicator", true));
            LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed", 0));
            LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed", 0.05f));

            LogicSet.Add(new DelayAction(currentRageAttackTellDelay));
            LogicSet.Add(new PlayEventAction(GameController.soundManager,
                "event:/SFX/Enemies/Mobs/Minitaur/sfx_enemy_minitaur_dash_prep", source));
            LogicSet.Add(new PlayAnimationAction(source, "Attack1TellStart", "Attack1TellEnd"));
            LogicSet.Add(new DelayAction(currentRageAttackTellDelay / 2f));
            // LogicSet.Add(new FaceDirectionAction(source, TargetType.DummyTarget));
            // LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed", 0));
            LogicSet.Add(new PlayEventAction(GameController.soundManager,
                "event:/SFX/Enemies/Mobs/Hoplite/Sword/sfx_enemy_hoplite_swing_blink", source));
            LogicSet.Add(new ChangeColourAction(source, Color.Red, 0.1f));

            // LogicSet.Add(new ChangePropertyAction(source, "LockFlip", true));
            LogicSet.Add(new DelayAction(currentRageAttackTellDelay / 2f));
            // LogicSet.Add(
            // new ChangePropertyAction(source, "pulseIndicator", false));
            LogicSet.Add(new PlayEventAction(GameController.soundManager,
                "event:/SFX/Enemies/Mobs/Minitaur/sfx_enemy_minitaur_dash", source));
            LogicSet.Add(new ChangePropertyAction(source, "stunDefense",
                this.dashStunDefense.GetValue(source)));
            LogicSet.Add(new ChangePropertyAction(source, "knockdownDefense",
                this.dashKnockdownDefense.GetValue(source)));
            LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed",
                0.03333334f));
            LogicSet.Add(
                new PlayAnimationAction(source, "Attack1ActionStart", "Attack1ActionEnd", true),
                SequenceType.Parallel);
            LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed",
                currentRageAttackMoveSpeed));
            LogicSet.Add(
                new ChangePropertyAction(source, "forceKnockdown", true));
            LogicSet.Add(new ChangePropertyAction(source, "knockdownSpeedMod",
                0.935f));
            LogicSet.Add(new ChangePropertyAction(source, "knockdownDistanceMod",
                1.1f));
            LogicSet.Add(new DelayAction(((float) this.rageAttackMoveDistance.GetValue(source)) /
                                         (float) currentRageAttackMoveSpeed));
            LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed", 0));
            LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed",
                this.storedAnimSpeed.GetValue(source)));
            LogicSet.Add(new PlayAnimationAction(source, "Attack1ExitStart", "Attack1ExitEnd"));
            // LogicSet.Add(new ChangePropertyAction(source, "LockFlip", false));
            LogicSet.Add(new ChangePropertyAction(source, "stunDefense",
                this.storedStunDefense.GetValue(source)));
            LogicSet.Add(new ChangePropertyAction(source, "knockdownDefense",
                this.storedKnockdownDefense.GetValue(source)));
            LogicSet.Add(new ChangePropertyAction(source, "State",
                CharacterState.Idle));
            LogicSet.Add(
                new ChangePropertyAction(source, "forceKnockdown", false));
            LogicSet.Add(new PlayAnimationAction(source, "StandStart", "StandEnd",
                true));
            LogicSet.Add(new DelayAction(exitDelay));
            // LogicSet.Add(new CallCooldownAction(source));
            LogicSet.End();
            //

            EnemyControllableManager.RunEnemyAttack(source, ref this.rageAttackLogic,
                ref this.rageAttackLogicBlock);
        }

        public void CastSpellR(CharacterObj character, PlayerObj playerObj)
        {
        }

        public void Attack(CharacterObj character, PlayerObj playerObj)
        {
            var source = (Enemy_Minitaur_Basic) character;
            float bulletDelayTime = (float) this.bulletDelay.GetValue(source);
            if (this.fireRandLogic != null)
            {
                this.fireRandLogic.Dispose();
                this.fireRandLogicBlock.Dispose();
            }

            this.fireRandLogicBlock = new LogicBlock(RNG.seed);
            this.fireRandLogic = new LogicSet(this.GetType().ToString() + ":m_fireRandLogic");
            LogicSet.Begin(this.fireRandLogic);
            LogicSet.Add(new ChangePropertyAction(source, "LockFlip", true));
            LogicSet.Add(
                new ChangePropertyAction(source, "State", CharacterState.Attacking));
            // LogicSet.Add(new RunFunctionAction(this, "TurnToFace", new object[]{source}));
            // LogicSet.Add(new ChangePropertyAction(source, "pulseIndicator", true));
            LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed", 0));
            // LogicSet.Add(new DelayAction(0.1f, 0.3f));
            LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed", 0.1f));
            LogicSet.Add(new PlayEventAction(GameController.soundManager,
                "event:/SFX/Enemies/Mobs/Minitaur/sfx_enemy_minitaur_shot_prep", source));
            LogicSet.Add(
                new PlayAnimationAction(source, "Attack2TellStart", "Attack2TellEnd"));
            LogicSet.Add(new DelayAction((float) this.firingDelay.GetValue(source) / 2));
            // LogicSet.Add(new ChangePropertyAction(source, "pulseIndicator", false));
            LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed", 0.03333334f));
            LogicSet.Add(new PlayEventAction(GameController.soundManager,
                "event:/SFX/Enemies/Mobs/Hoplite/Sword/sfx_enemy_hoplite_swing_blink", source));
            LogicSet.Add(new ChangeColourAction(source, Color.Red, 0.1f));
            // LogicSet.Add(new ChangePropertyAction(source, "LockFlip", true));
            int amount = (int) this.fireAmount.GetValue(source);
            for (int index = 0; index < amount; ++index)
            {
                LogicSet.Add(new DelayAction(bulletDelayTime));
                if (index % 2 == 0)
                    LogicSet.Add(new PlayAnimationAction(source, "Attack2LoopBStart",
                        "Attack2LoopBEnd", true));
                else
                    LogicSet.Add(new PlayAnimationAction(source, "Attack2LoopAStart",
                        "Attack2LoopAEnd", true));
                LogicSet.Add(new RunFunctionAction(this, "FireRandBullet", new []
                {
                    source,
                    this.fireAngle.GetValue(source),
                    this.bulletSpeed.GetValue(source)
                }));
                LogicSet.Add(new DelayAction(bulletDelayTime));
            }

            LogicSet.Add(new PlayAnimationAction(source, "StandStart", "StandEnd", true));
            LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed",
                this.storedAnimSpeed.GetValue(source)));
            // LogicSet.Add((LogicAction) new DelayAction(this.m_firingPostPause));
            LogicSet.Add(new ChangePropertyAction(source, "State", CharacterState.Idle));
            // LogicSet.Add(new ChangePropertyAction(source, "LockFlip", false));
            LogicSet.Add(new CallCooldownAction(source));
            LogicSet.End();

            EnemyControllableManager.RunEnemyAttack(source, ref this.fireRandLogic, ref this.fireRandLogicBlock);
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
            var enemyObj = source;
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
            projectileObj.TextureColor = PlayerClassObj_Mercenary.MERCENARY_COLOUR;
            projectileObj.FollowOrientation = true;
            projectileObj.canReflect = true;
            projectileObj.destroyedByProjectiles = true;
            projectileObj.Layer = source.Layer + 1f / 1000f;
            projectileObj.CollisionID = 3;
            projectileObj.Orientation = Orientation;
            projectileObj.HeadingX = MathHelper.Clamp((float) Math.Cos(Orientation),
                source.Flip == SpriteEffects.FlipHorizontally ? -1 : -0.1f,
                source.Flip == SpriteEffects.FlipHorizontally ? 0.1f : 1);

            projectileObj.HeadingY = (float) Math.Sin(Orientation);
            this.shotSFXField.SetValue(source, GameController.soundManager.PlayEvent("event:/SFX/Enemies/Mobs/Minitaur/sfx_enemy_minitaur_shot",
                source));

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

        }



        public void FireRandBullet(EnemyObj source, int randAngleOffset, float speed)
        {
            randAngleOffset = Math.Abs(randAngleOffset);
            this.FireBullet(source, RNG.get(429).RandomInt(randAngleOffset * -1, randAngleOffset), speed, false);
        }

    }

}
