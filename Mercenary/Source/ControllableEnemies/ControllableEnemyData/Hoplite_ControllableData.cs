using System;
using System.Reflection;
using Brawler2D;
using CDGEngine;
using cs.Blit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tweener;

namespace FullModdedFuriesAPI.Mods.MercenaryClass.Source.ControllableEnemies.ControllableEnemyData
{
    public class Hoplite_ControllableData : IEnemyControllable
    {
        private Type enemyType;

        private LogicSet castBLogicSet;
        private LogicBlock castBBlock;

        public int spellAngle;
        public float spellDelay = 0.3f;
        public float spellSpeed = 750f;
        public float spellDuration = 3.5f;

        private FieldInfo shotSFXField;

        private LogicSet dodgeLogic;
        private LogicBlock dodgeLogicBlock;
        private FieldInfo storedAnimSpeedField;

        private string soundEventKey;
        private LogicSet attackLogic;
        private LogicBlock attackLogicBlock = new LogicBlock(RNG.seed);


        public Hoplite_ControllableData()
        {
            this.enemyType = typeof(Enemy_Hoplite_Basic);

            this.shotSFXField =
                this.enemyType.GetField("m_shotSFX",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

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

        public void CastSpellA(CharacterObj source, PlayerObj playerObj)
        {
            float dodgeAnimationSpeed = 30f;
            float storedAnimSpeed = (float) this.storedAnimSpeedField.GetValue(source);
            var orientation = this.TurnToFaceVector((EnemyObj) source);
            orientation = new Vector2(orientation.X, -orientation.Y);

            if (this.dodgeLogic != null)
            {
                this.dodgeLogic?.Dispose();
                this.dodgeLogicBlock?.Dispose();
            }

            this.dodgeLogic = new LogicSet("DODGE_MB");
            LogicSet.Begin(this.dodgeLogic);
            LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed", (float) (1.0 / dodgeAnimationSpeed)));
            LogicSet.Add(new ChangePropertyAction(source, "LockFlip", (object) true), SequenceType.Serial);
            // LogicSet.Add((LogicAction) new ChangePropertyAction(source, "LockFlip", (object) true));
            // LogicSet.Add(new RunFunctionAction(source, "DrawFadeUpTextFast", new object[2]
            // {
            //     "LOC_ID_STATUS_TEXTS_COUNTER",
            //     Color.Red
            // }));
            LogicSet.Add(new RunFunctionAction(source, "PlayVocals", new object[2]
            {
                "event:/Vocalizations/Enemies/Mobs/Hoplite/vox_enemy_hoplite_hit",
                false
            }));
            LogicSet.Add(new PlayEventAction(GameController.soundManager, "event:/SFX/Enemies/Mobs/Hoplite/Roll/sfx_enemy_hoplite_dodge", source));
            LogicSet.Add(new RunFunctionAction(source, "Dodge", new object[]{orientation}));
            LogicSet.Add(new PropertyValueReachedAction(source, "isDodging", Convert.ToSingle(false)));
            LogicSet.Add(new ChangePropertyAction(source, "State", CharacterState.Idle));
            LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed", storedAnimSpeed));
            LogicSet.Add(new ChangePropertyAction(source, "LockFlip", false));
            LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed", 0.0f));
            LogicSet.Add(new PlayAnimationAction(source, "StandStart", "StandEnd", true));
            // LogicSet.Add((LogicAction) new AddLSCooldownAction((EnemyObj) source, this.m_rollCD, this.m_dodgeMBLogic));
            this.dodgeLogic.Tag = "Tag_CD_Roll";
            LogicSet.End();


            EnemyControllableManager.RunEnemyAttack((EnemyObj) source, ref this.dodgeLogic, ref this.dodgeLogicBlock);
        }

        public void CastSpellB(CharacterObj source, PlayerObj playerObj)
        {
            float spellLoop = 2f;
            float attackPostPause = 0.1f;//1f;
            float storedAnimSpeed = (float) this.storedAnimSpeedField.GetValue(source);

            if (this.castBLogicSet != null)
            {
                this.castBLogicSet?.Dispose();
                this.castBBlock?.Dispose();
            }

            this.castBLogicSet = new LogicSet(this.GetType().ToString() + ":m_arrowLogic");
            LogicSet.Begin(this.castBLogicSet);
            // LogicSet.Add(new RunLogicSetAction(this.m_enterArrowRange));
            // LogicSet.Add((LogicAction) new ChangePropertyAction(source, "pulseIndicator", (object) true), SequenceType.Serial);
            LogicSet.Add(new RunFunctionAction(this, "TurnToFace", new object[] {source}), SequenceType.Serial);
            LogicSet.Add(new ChangePropertyAction(source, "LockFlip", (object) true), SequenceType.Serial);
            LogicSet.Add(new ChangePropertyAction(source, "State", CharacterState.Attacking));
            LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed", 0));
            LogicSet.Add(new PlayEventAction(GameController.soundManager,
                "event:/SFX/Enemies/Mobs/Hoplite/Gun/sfx_enemy_hoplite_shot_prep", source));
            LogicSet.Add(new PlayAnimationAction(source, "Attack3TellStart", "Attack3TellEnd"));
            // LogicSet.Add((LogicAction) new ChangePropertyAction(source, "pulseIndicator", (object) false), SequenceType.Serial);
            // LogicSet.Add((LogicAction) new FaceDirectionAction((CharacterObj) this, TargetType.CurrentTarget), SequenceType.Serial);
            // LogicSet.Add((LogicAction) new ChangePropertyAction(source, "LockFlip", (object) true), SequenceType.Serial);
            LogicSet.Add(new ChangeColourAction(source, Color.Red, 0.1f));
            LogicSet.Add(new DelayAction(this.spellDelay));
            for (int index = 0; index < spellLoop; ++index)
            {
                LogicSet.Add(new RunFunctionAction(this, "FireArrow", new object[]
                {
                    source,
                    this.spellAngle
                }));
                LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed", 0.05f));
                LogicSet.Add(
                    new PlayAnimationAction(source, "Attack3ActionStart", "Attack3ActionEnd", false, false, true),
                    SequenceType.Parallel);
                LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed", storedAnimSpeed));
                LogicSet.Add(new DelayAction(0.1f));
            }

            LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed", storedAnimSpeed));
            LogicSet.Add(new DelayAction(attackPostPause));
            LogicSet.Add((LogicAction) new ChangePropertyAction(source, "LockFlip", (object) false), SequenceType.Serial);
            LogicSet.Add(new ChangePropertyAction(source, "State", CharacterState.Idle));
            // LogicSet.Add((LogicAction) new AddLSCooldownAction((EnemyObj) source, this.m_spellCD, this.m_arrowLogic), SequenceType.Serial);
            this.castBLogicSet.Tag = "Tag_CD_Arrow";
            LogicSet.Add(new CallCooldownAction((EnemyObj) source));
            LogicSet.End();

            // var logicSetField =
            //     this.enemyType.GetField("m_arrowLogic",
            //         BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            EnemyControllableManager.RunEnemyAttack((EnemyObj) source, ref this.castBLogicSet, ref this.castBBlock);
        }

        public void Attack(CharacterObj source, PlayerObj playerObj)
        {
            float attackStartDelay = 0.1f; //1
            float attackAnimationSpeed = 10f;
            float attackSpeed = 550f;
            float attackDistance = 70f;
            float attackExitDelay = 0.4f;// 0.25f;
            float attackPostPause = 1f;
            float attackTellDelay = 0.05f;// 0.125f;

            float dashAttackSpeed = 665f;
            float dashAttackTellDelay = 0.4f;

            float storedAnimSpeed = (float) this.storedAnimSpeedField.GetValue(source);

            if (this.attackLogic != null)
            {
                this.attackLogic?.Dispose();
                this.attackLogicBlock?.Dispose();
            }

          this.attackLogic = new LogicSet(this.GetType().ToString() + ":controllableAttackLogic");
          LogicSet.Begin(this.attackLogic);
          // LogicSet.Add((LogicAction) new RunLogicSetAction(this.m_enterAttackRange));
          // LogicSet.Add((LogicAction) new ChangePropertyAction(source, "pulseIndicator", (object) true));
          // LogicSet.Add(new DelayAction(attackStartDelay));

          //Original
          // LogicSet.Add(new RunFunctionAction(this, "TurnToFace", new object[] {source}), SequenceType.Serial);
          LogicSet.Add(new ChangePropertyAction(source, "LockFlip", (object) true), SequenceType.Serial);
          LogicSet.Add(new ChangePropertyAction(source, "State", CharacterState.Attacking));
          LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed", 0));
          LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed", (float) (1.0 / attackAnimationSpeed)));
          LogicSet.Add(new PlayEventAction(GameController.soundManager, "event:/SFX/Enemies/Mobs/Hoplite/Sword/sfx_enemy_hoplite_swing_prep", source));
          LogicSet.Add(new PlayAnimationAction(source, "Attack1TellStart", "Attack1TellEnd"));
          LogicSet.Add(new PlayEventAction(GameController.soundManager, "event:/SFX/Enemies/Mobs/Hoplite/Sword/sfx_enemy_hoplite_swing_blink", source));
          LogicSet.Add(new ChangeColourAction(source, Color.Red, 0.1f));
          LogicSet.Add(new DelayAction(attackTellDelay));
          LogicSet.Add(new ChangePropertyAction(source, "pulseIndicator", false));
          LogicSet.Add(new PlayAnimationAction(source, source.GetFrameFromLabel("Attack1ActionStart"), source.GetFrameFromLabel("Attack1ActionEnd") - 3));
          LogicSet.Add(new PlayEventAction(GameController.soundManager, "event:/SFX/Enemies/Mobs/Hoplite/Sword/sfx_enemy_hoplite_swing", source));
          LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed", attackSpeed));
          LogicSet.Add(new PlayAnimationAction(source, source.GetFrameFromLabel("Attack1ActionEnd") - 2, source.GetFrameFromLabel("Attack1ActionEnd")), SequenceType.Parallel);
          LogicSet.Add(new DelayAction(attackDistance / attackSpeed));
          LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed", 0));
          LogicSet.Add(new DelayAction(attackExitDelay));
          LogicSet.Add(new PlayAnimationAction(source, "StandStart", "StandEnd", true));
          LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed", storedAnimSpeed));
          LogicSet.Add((LogicAction) new ChangePropertyAction(source, "LockFlip", (object) false));
          LogicSet.Add(new ChangePropertyAction(source, "State", CharacterState.Idle));

            //Inverted
          // LogicSet.Add(new RunFunctionAction(this, "TurnToFace", new object[] {source}), SequenceType.Serial);
          // LogicSet.Add(new ChangePropertyAction(source, "LockFlip", (object) true), SequenceType.Serial);
          // LogicSet.Add(new ChangePropertyAction(source, "State", CharacterState.Attacking));
          // LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed", 0));
          // LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed", (float) (1.0 / attackAnimationSpeed)));
          // LogicSet.Add(new PlayEventAction(GameController.soundManager, "event:/SFX/Enemies/Mobs/Hoplite/Sword/sfx_enemy_hoplite_swing_prep", source));
          // LogicSet.Add(new PlayAnimationAction(source, "Attack1TellStart", "Attack1TellEnd"));
          // LogicSet.Add(new PlayEventAction(GameController.soundManager, "event:/SFX/Enemies/Mobs/Hoplite/Sword/sfx_enemy_hoplite_swing_blink", source));
          // LogicSet.Add(new ChangeColourAction(source, Color.Red, 0.1f));
          //   LogicSet.Add(new ChangePropertyAction(source, "pulseIndicator", false));
          // LogicSet.Add(new PlayAnimationAction(source, source.GetFrameFromLabel("Attack1ActionStart"), source.GetFrameFromLabel("Attack1ActionEnd") - 3));
          // LogicSet.Add(new PlayEventAction(GameController.soundManager, "event:/SFX/Enemies/Mobs/Hoplite/Sword/sfx_enemy_hoplite_swing", source));
          // LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed", attackSpeed));
          // LogicSet.Add(new PlayAnimationAction(source, source.GetFrameFromLabel("Attack1ActionEnd") - 2, source.GetFrameFromLabel("Attack1ActionEnd")), SequenceType.Parallel);
          // LogicSet.Add(new DelayAction(attackDistance / attackSpeed));
          // LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed", 0));
          // LogicSet.Add(new DelayAction(attackExitDelay));
          // LogicSet.Add(new PlayAnimationAction(source, "StandStart", "StandEnd", true));
          // LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed", storedAnimSpeed));
          // LogicSet.Add((LogicAction) new ChangePropertyAction(source, "LockFlip", (object) false));
          // LogicSet.Add(new ChangePropertyAction(source, "State", CharacterState.Idle));

          // LogicSet.Add((LogicAction) new CallCooldownAction((EnemyObj) source));
          LogicSet.End();

            var enemyObj = ((EnemyObj) source);
            enemyObj.StopAllActions();

            var logicSet = this.attackLogic;
            var logicBlock = new LogicBlock(RNG.seed);
            logicBlock.AddLogicSet(logicSet);
            enemyObj.m_currentLB = logicBlock;
            enemyObj.m_currentLB.Execute(null, 100);

            EnemyControllableManager.RunEnemyAttack((EnemyObj) source, ref this.attackLogic, ref this.attackLogicBlock);
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

        public void FireArrow(EnemyObj source, int angle)
        {
            // var orientation = this.TurnToFaceVector((EnemyObj) source);
            // orientation = new Vector2(orientation.X, -orientation.Y);

            this.soundEventKey = "event:/SFX/Enemies/Mobs/Hoplite/Gun/sfx_enemy_hoplite_shot";
            this.shotSFXField.SetValue(source, GameController.soundManager.PlayEvent(this.soundEventKey, source));
            var projectileObj = GameController.g_game.ProjectileManager.FireProjectile(new ProjectileData()
            {
                collidesWithCollHullTerrainBox = true,
                Damage = source.damage,
                DestroyOnCollision = true,
                Lifespan = this.spellDuration,
                Scale = new Vector2(2f, 2f),
                SourceAnchor = new Vector2(70f, -15f),
                Speed = new Vector2(this.spellSpeed, this.spellSpeed),
                SpriteName = "Projectile_Minitaur_Bullet",
                AngleOffset = angle,
                canStun = true,
                hasShadow = true,
                shadowOffset = new Vector2(0.0f, 50f),
                followOrientation = true
            }, source);
            projectileObj.CollisionID = 1;
            projectileObj.TextureColor = PlayerClassObj_Mercenary.MERCENARY_COLOUR;
            projectileObj.canReflect = true;
            projectileObj.destroyedByProjectiles = true;
            projectileObj.Layer = source.Layer + 1f / 1000f;
        }

        public void CastSpellR(CharacterObj source, PlayerObj playerObj)
        {
            // var logicSetField =
            //     this.enemyType.GetField("m_dodgeMBLogic",
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
