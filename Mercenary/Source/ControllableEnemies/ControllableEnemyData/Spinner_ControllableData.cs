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
    public class Spinner_ControllableData : IEnemyControllable
    {
        private LogicSet attackLogic;
        private LogicBlock attackLogicBlock;

        private LogicSet dodgeLogic;
        private LogicBlock dodgeLogicBlock;

        private FieldInfo storedAnimSpeedField;
        private Type enemyType;
        private FieldInfo storedKnockdownDefenseField;
        private FieldInfo storedTurnSpeedField;
        private FieldInfo storedStunDefenseField;

        public Spinner_ControllableData()
        {
            this.enemyType = typeof(Enemy_Spinner_Basic);

            this.storedAnimSpeedField =
                this.enemyType.GetField("m_storedAnimSpeed",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            this.storedTurnSpeedField =
                this.enemyType.GetField("m_storedTurnSpeed",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            this.storedStunDefenseField =
                this.enemyType.GetField("m_storedStunDefense",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            this.storedKnockdownDefenseField =
                this.enemyType.GetField("m_storedKnockdownDefense",
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
            //     this.enemyType.GetField("m_orbAttackLogic",
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

        public void CastSpellA(CharacterObj source, PlayerObj playerObj)
        {
            var attackStartDelay = new Vector2(0.1f, 0.5f);
            float attackTellDelay = 0.75f;
            float spinAttackAnimSpeed = 22f;
            float spinDuration = 3.5f;
            float spinMoveSpeed = 140f;
            float spinAttackEndDelay = 0.5f;
            float spinTurnSpeed = 55f;
            int spinAttackStunDefense = 6;
            int spinAttackKnockdownDefense = 6;
            bool bouncesOffWalls = true;

            float storedAnimSpeed = (float) this.storedAnimSpeedField.GetValue(source);
            float storedStunDefense = (byte) this.storedStunDefenseField.GetValue(source);
            float storedKnockdownDefense = (byte) this.storedKnockdownDefenseField.GetValue(source);
            float storedTurnSpeed = (float) this.storedTurnSpeedField.GetValue(source);

            if (this.dodgeLogic != null)
            {
                this.dodgeLogic.Dispose();
                this.dodgeLogicBlock.Dispose();
            }

            this.dodgeLogicBlock = new LogicBlock(RNG.seed);
            this.dodgeLogic = new LogicSet(source.GetType() + ":attackLogic");

            LogicSet.Begin(this.dodgeLogic);
            // LogicSet.Add(new ChangePropertyAction(source, "pulseIndicator", true));
            LogicSet.Add(new DelayAction(attackStartDelay));
            LogicSet.Add(new ChangePropertyAction(source, "State", CharacterState.Attacking));
            LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed", 0));
            LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed",
                (float) (1.0 / spinAttackAnimSpeed)));
            LogicSet.Add(new FaceDirectionAction(source, TargetType.CurrentTarget));
            LogicSet.Add(new ChangePropertyAction(source, "LockFlip", true));
            LogicSet.Add(new PlayEventAction(GameController.soundManager, "event:/SFX/Enemies/Mobs/Spinner/sfx_enemy_spinner_prep", source, true));
            LogicSet.Add(new PlayAnimationAction(source, "Attack1TellStart", "Attack1PreloopEnd"));
            LogicSet.Add(new ChangeColourAction(source, Color.Red, 0.1f));
            LogicSet.Add(new DelayAction(attackTellDelay));
            LogicSet.Add(new RunFunctionAction(source, "PlaySpinEvent", new object[]{}));
            LogicSet.Add(new ChangePropertyAction(source, "stunDefense", spinAttackStunDefense));
            LogicSet.Add(new ChangePropertyAction(source, "knockdownDefense", spinAttackKnockdownDefense));
            LogicSet.Add(new ChangePropertyAction(source, "TurnSpeed", spinTurnSpeed));
            LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed", spinMoveSpeed));
            LogicSet.Add(new PlayAnimationAction(source,
                source.GetFrameFromLabel("Attack1ActionStart"), source.GetFrameFromLabel("Attack1ActionEnd"), true));
            /*if (source is Enemy_Spinner_Miniboss)
            {
                for (int index1 = 0; index1 < 25; ++index1)
                {
                    for (int index2 = 0; index2 < 1; ++index2)
                    {
                        LogicSet.Add((LogicAction) new RunFunctionAction(source, "FireBullet", new object[3]
                        {
                            fireAngle,
                            bulletSpeed,
                            (object) false
                        }));
                        LogicSet.Add((LogicAction) new PlayEventAction(GameController.soundManager,
                            "event:/SFX/Enemies/Mobs/Minitaur/sfx_enemy_minitaur_shot", source));
                    }

                    LogicSet.Add((LogicAction) new DelayAction(0.2f));
                }
            }*/

            if (bouncesOffWalls)
                LogicSet.Add(new DelayAction(spinDuration));
            else
                LogicSet.Add(new ChaseAction(source, TargetType.CurrentTarget, true, spinDuration));
            LogicSet.Add(new RunFunctionAction(source, "UpdateFacingDirection",
                new object[]{}));
            LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed", storedAnimSpeed));
            // LogicSet.Add(new CallCooldownAction((EnemyObj) source));
            LogicSet.Add(new ChangePropertyAction(source, "pulseIndicator", false));
            LogicSet.Add(new ChangePropertyAction(source, "TurnSpeed", storedTurnSpeed));
            LogicSet.Add(new ChangePropertyAction(source, "stunDefense", storedStunDefense));
            LogicSet.Add(new ChangePropertyAction(source, "knockdownDefense", storedKnockdownDefense));
            LogicSet.Add(this.Mustache(source));
            LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed", storedAnimSpeed));
            // LogicSet.Add(new ChangePropertyAction(source, "LockFlip", false));
            LogicSet.Add(new ChangePropertyAction(source, "State", CharacterState.Idle));
            // LogicSet.Add(new CallCooldownAction((EnemyObj) source));
            LogicSet.End();

            EnemyControllableManager.RunEnemyAttack((EnemyObj) source, ref this.dodgeLogic, ref this.dodgeLogicBlock);
        }

        public void Attack(CharacterObj source, PlayerObj playerObj)
        {
            int fireAngle = 180;
            float bulletSpeed = 225f;
            Vector2 shootStartDelay = new Vector2(0.1f, 0.5f);
            float shootTellDelay = 0.2f;
            float shootAttackAnimSpeed = 22f;
            float shootPreSpinDuration = 0.25f;
            float actualShootTellDelay = 0.75f;
            float shootSpinShotDelay = 0.15f;
            float shootSpinShotAmount = 12f;
            float shootSpinShotPerSpin = 2f;
            float shootSpinMoveSpeed = 0;
            float shootSpinTurnSpeed = 0;
            byte shootSpinAttackStunDefense = 6;
            byte shootSpinAttackKnockdownDefense = 6;
            bool displayTimer = true;
            float spinAttackAnimSpeed = 22f;

            float storedAnimSpeed = (float) this.storedAnimSpeedField.GetValue(source);
            float storedStunDefense = (byte) this.storedStunDefenseField.GetValue(source);
            float storedKnockdownDefense = (byte) this.storedKnockdownDefenseField.GetValue(source);
            float storedTurnSpeed = (float) this.storedTurnSpeedField.GetValue(source);

            if (this.attackLogic != null)
            {
                this.attackLogic.Dispose();
                this.attackLogicBlock.Dispose();
            }

            this.attackLogicBlock = new LogicBlock(RNG.seed);
            this.attackLogic = new LogicSet(source.GetType() + ":attackLogic");

            LogicSet.Begin(this.attackLogic);
            LogicSet.Add(new ChangePropertyAction(source, "pulseIndicator", true));
            LogicSet.Add(new DelayAction(shootStartDelay));
            LogicSet.Add(
                new ChangePropertyAction(source, "State", CharacterState.Attacking));
            LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed", 0));
            LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed",
                (float) (1.0 / shootAttackAnimSpeed)));
            LogicSet.Add(new FaceDirectionAction(source, TargetType.CurrentTarget));
            LogicSet.Add(new ChangePropertyAction(source, "LockFlip", true));
            LogicSet.Add(new RunFunctionAction(source, "RunSparkEffect", new object[1]
            {
                (float) (actualShootTellDelay + (double) shootTellDelay +
                         shootPreSpinDuration)
            }));
            LogicSet.Add(new ChangePropertyAction(source, "stunDefense",
                shootSpinAttackStunDefense));
            LogicSet.Add(new ChangePropertyAction(source, "knockdownDefense",
                shootSpinAttackKnockdownDefense));
            LogicSet.Add(new PlayEventAction(GameController.soundManager,
                "event:/SFX/Enemies/Mobs/Spinner/sfx_enemy_spinner_prep", source, true));
            LogicSet.Add(new PlayAnimationAction(source, "Attack1TellStart",
                "Attack1PreloopEnd"));
            if (displayTimer)
                LogicSet.Add(new RunFunctionAction(source, "SetCooldownDisplay", new object[1]
                {
                    (float) (actualShootTellDelay + (double) shootTellDelay +
                             shootPreSpinDuration)
                }));
            LogicSet.Add(new DelayAction(0.25f));
            for (int index = 0; index < 2; ++index)
            {
                LogicSet.Add(new PlayAnimationAction(source, "Attack1PreloopEnd",
                    "Attack1TellEnd", resetAnimation: true));
                LogicSet.Add(new DelayAction(0.1f));
                LogicSet.Add(new PlayAnimationAction(source, "Attack1TellEnd",
                    "Attack1PreloopEnd", resetAnimation: true));
                LogicSet.Add(new DelayAction(0.1f));
            }

            LogicSet.Add(new ChangeColourAction(source, Color.Red, 0.1f));
            LogicSet.Add(new DelayAction(0.1f));
            LogicSet.Add(new ChangeColourAction(source, Color.Red, 0.1f));
            LogicSet.Add(new DelayAction(0.1f));
            LogicSet.Add(new ChangeColourAction(source, Color.Red, 0.1f));
            LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed",
                (float) (1.0 / spinAttackAnimSpeed)));
            LogicSet.Add(new DelayAction(shootTellDelay));
            LogicSet.Add(new RunFunctionAction(source, "StopSparkEffect",
                new object[]{}));
            LogicSet.Add(new RunFunctionAction(source, "PlaySpinEvent",
                new object[]{}));
            LogicSet.Add(new ChangePropertyAction(source, "TurnSpeed", shootSpinTurnSpeed));
            LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed", shootSpinMoveSpeed));
            LogicSet.Add(new PlayAnimationAction(source,
                source.GetFrameFromLabel("Attack1ActionStart"), source.GetFrameFromLabel("Attack1ActionEnd"), true));
            // if (this is Enemy_Spinner_Miniboss)
            //     LogicSet.Add(new DelayAction(shootPreSpinDuration));
            // else
            //     LogicSet.Add(new ChaseAction(source, TargetType.CurrentTarget, true,
            //         shootPreSpinDuration));
            for (int index1 = 0; (double) index1 < (double) shootSpinShotAmount; ++index1)
            {
                for (int index2 = 0; (double) index2 < (double) shootSpinShotPerSpin; ++index2)
                {
                    LogicSet.Add(new RunFunctionAction(source, "FireBullet", new object[3]
                    {
                        fireAngle,
                        bulletSpeed,
                        false
                    }));
                    LogicSet.Add(new PlayEventAction(GameController.soundManager,
                        "event:/SFX/Enemies/Mobs/Minitaur/sfx_enemy_minitaur_shot", source));
                }

                LogicSet.Add(new DelayAction(shootSpinShotDelay));
            }

            LogicSet.Add(new RunFunctionAction(source, "UpdateFacingDirection",
                new object[]{}));
            LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed",
                storedAnimSpeed));
            LogicSet.Add(new ChangePropertyAction(source, "pulseIndicator", false));
            LogicSet.Add(new ChangePropertyAction(source, "TurnSpeed",
                storedTurnSpeed));
            LogicSet.Add(new ChangePropertyAction(source, "stunDefense",
                storedStunDefense));
            LogicSet.Add(new ChangePropertyAction(source, "knockdownDefense",
                storedKnockdownDefense));
            LogicSet.Add(this.Mustache(source));
            LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed",
                storedAnimSpeed));
            // LogicSet.Add(new ChangePropertyAction(source, "LockFlip", false));
            LogicSet.Add(new ChangePropertyAction(source, "State", CharacterState.Idle));
            // LogicSet.Add(new CallCooldownAction((EnemyObj) source));
            LogicSet.End();

            EnemyControllableManager.RunEnemyAttack((EnemyObj) source, ref this.attackLogic, ref this.attackLogicBlock);
        }

        public IfElseAction Mustache(DisplayObj source)
        {
            float spinAttackEndDelay = 0.5f;
            LogicSet ls1 = new LogicSet(this.GetType().ToString() + ":endAttackWithMoustache");
            LogicSet.Begin(ls1);
            LogicSet.Add(new RunFunctionAction(this, "StopSpinEvent", new object[1]
            {
                STOP_MODE.ALLOWFADEOUT
            }));
            LogicSet.Add(new PlayEventAction(GameController.soundManager,
                "event:/SFX/Enemies/Mobs/Spinner/sfx_enemy_spinner_spin_exit", source));
            LogicSet.Add(new PlayAnimationAction(source, source.GetFrameFromLabel("Attack1ExitMoustacheStart"),
                source.GetFrameFromLabel("Attack1ExitMoustacheStart") + 1));
            LogicSet.Add(new ChangePropertyAction(this, "CurrentSpeed", 0));
            LogicSet.Add(new DelayAction(spinAttackEndDelay));
            LogicSet.Add(new PlayAnimationAction(source, source.GetFrameFromLabel("Attack1ExitMoustacheStart") + 2,
                source.GetFrameFromLabel("Attack1ExitMoustacheEnd")));
            LogicSet.End();

            LogicSet ls2 = new LogicSet(this.GetType().ToString() + ":endAttackWithoutMoustache");
            LogicSet.Begin(ls2);
            LogicSet.Add(new RunFunctionAction(this, "StopSpinEvent", new object[1]
            {
                STOP_MODE.ALLOWFADEOUT
            }));
            LogicSet.Add(new PlayEventAction(GameController.soundManager,
                "event:/SFX/Enemies/Mobs/Spinner/sfx_enemy_spinner_spin_exit", source));
            LogicSet.Add(new PlayAnimationAction(source, source.GetFrameFromLabel("Attack1ExitStart"),
                source.GetFrameFromLabel("Attack1ExitStart") + 1));
            LogicSet.Add(new ChangePropertyAction(this, "CurrentSpeed", 0));
            LogicSet.Add(new DelayAction(spinAttackEndDelay));
            LogicSet.Add(new PlayAnimationAction(source, source.GetFrameFromLabel("Attack1ExitStart") + 2,
                source.GetFrameFromLabel("Attack1ExitEnd")));
            LogicSet.End();

            return new IfElseAction(source, "RunMoustacheCheck", new LogicSet[2]
            {
                ls1,
                ls2
            });
        }

        public void CastSpellR(CharacterObj source, PlayerObj playerObj)
        {
            // var logicSetField =
            //     this.enemyType.GetField("m_shootLogic",
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
            EnemyObj enemyObj = source;
            PlayerObj playerObj = EnemyControllableManager.logicBlockStates[enemyObj].player;
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
            Vector2 normalized = direction;
            normalized.Normalize();
            Vector2 actionDirection = normalized;
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
        //     currentSpeed = dodgeSpeed;
        //     dodgeDistanceCounter = 0.0f;
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
        //     float angle = source.GetActionAngle(playerObj.currentPlayerClass.storedLeftStickDirection,
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
