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
    public class Grenadier_ControllableData : IEnemyControllable
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

        public Grenadier_ControllableData()
        {
            var type = typeof(Enemy_Grenadier_Basic);
            this.grenadeAttackDelayField =
                type.GetField("m_grenadeAttackDelay",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            this.grenadeAmountField =
                type.GetField("m_grenadeAmount",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            this.grenadeSpeedField =
                type.GetField("m_grenadeSpeed",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            this.grenadeDistanceRandomField =
                type.GetField("m_grenadeDistanceRandom",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            this.grenadeDistanceField =
                type.GetField("m_grenadeDistance",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            this.grenadeDistanceField =
                type.GetField("m_grenadeDistance",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            this.grenadeReticleWidthField =
                type.GetField("m_grenadeReticleWidth",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            // mine

            this.mineAttackDelayField =
                type.GetField("m_mineAttackDelay",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            this.mineAmountField =
                type.GetField("m_mineAmount",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);


            this.mineSpeedField =
                type.GetField("m_mineSpeed",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            this.mineDistanceRandomField =
                type.GetField("m_mineDistanceRandom",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);


            this.mineExplosionSizeField =
                type.GetField("m_mineExplosionSize",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);


            this.mineTriggerDelayField =
                type.GetField("m_mineTriggerDelay",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            this.mineTriggerRadiusXField =
                type.GetField("m_mineTriggerRadiusX",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            this.mineLifeSpanField =
                type.GetField("m_mineLifeSpan",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);


            this.storedAnimSpeedField =
                type.GetField("m_storedAnimSpeed",
                    BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);



            // this.mineReticleWidthField =
                // type.GetField("m_mineReticleWidth",
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
            if (this.mineAttackLogic != null)
            {
                this.mineAttackLogic?.Dispose();
                this.mineAttackLogicBlock?.Dispose();
            }

            float mineAttackDelay = (float) this.mineAttackDelayField.GetValue(source);
            float mineAmount = (int) this.mineAmountField.GetValue(source);


            this.mineAttackLogic = new LogicSet(this.GetType() + ":m_mineAttackLogic");
            LogicSet.Begin(this.mineAttackLogic);
            // LogicSet.Add(new ChangePropertyAction(source, "pulseIndicator", true));
            LogicSet.Add(new RunFunctionAction(this, "TurnToFace", new object[] {source}), SequenceType.Serial);
            LogicSet.Add(new ChangePropertyAction(source, "State", CharacterState.Attacking));
            LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed", 0));
            LogicSet.Add(new PlayAnimationAction(source, "Attack1TellStart", "Attack1TellEnd", false, false, true));
            LogicSet.Add(new DelayAction(mineAttackDelay / 2));
            LogicSet.Add(new PlayAnimationAction(source, source.GetFrameFromLabel("Attack1ActionStart"),
                source.GetFrameFromLabel("Attack1ActionEnd") - 2));
            // LogicSet.Add(new FaceDirectionAction(source, TargetType.CurrentTarget));
            // LogicSet.Add(new ChangePropertyAction(source, "LockFlip", true));
            LogicSet.Add(new PlayEventAction(GameController.soundManager,
                "event:/SFX/Enemies/Mobs/Grenadier/sfx_enemy_gren_throw", source));
            for (int index = 0; index < mineAmount; ++index)
                LogicSet.Add(new RunFunctionAction(this, "ThrowMine", new object[] {source}));
            // LogicSet.Add(new ChangePropertyAction(source, "pulseIndicator", false));
            LogicSet.Add(new PlayAnimationAction(source, source.GetFrameFromLabel("Attack1ActionEnd") - 1,
                source.GetFrameFromLabel("Attack1ActionEnd")));
            // LogicSet.Add(new ChangePropertyAction(source, "LockFlip", false));
            this.mineAttackLogic.Tag = "Tag_CD_Mine_UNBREAKABLE";
            LogicSet.Add(new ChangePropertyAction(source, "State", CharacterState.Idle));
            // LogicSet.Add(new CallCooldownAction((EnemyObj) source));
            LogicSet.End();

            EnemyControllableManager.RunEnemyAttack((EnemyObj) source, ref this.mineAttackLogic,
                ref this.mineAttackLogicBlock);
        }

        public void CastSpellA(CharacterObj source, PlayerObj playerObj)
        {
            var grenadier = (Enemy_Grenadier_Basic) source;
            var orientation = this.TurnToFaceVector(grenadier);
            orientation = new Vector2(orientation.X, -orientation.Y);
            float dodgeAnimationSpeed = 30f;

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
            LogicSet.Add(new ChangePropertyAction(source, "AnimationSpeed", this.storedAnimSpeedField.GetValue(source)));
            LogicSet.Add(new ChangePropertyAction(source, "LockFlip", false));
            LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed", 0.0f));
            LogicSet.Add(new PlayAnimationAction(source, "StandStart", "StandEnd", true));
            // LogicSet.Add((LogicAction) new AddLSCooldownAction((EnemyObj) source, this.m_rollCD, this.m_dodgeMBLogic));
            this.dodgeLogic.Tag = "Tag_CD_Roll";
            LogicSet.End();

            EnemyControllableManager.RunEnemyAttack((EnemyObj) source, ref this.dodgeLogic, ref this.dodgeLogicBlock);
        }

        public void Attack(CharacterObj source, PlayerObj playerObj)
        {
            if (this.grenadeAttackLogic != null)
            {
                this.grenadeAttackLogic?.Dispose();
                this.grenadeAttackBlock?.Dispose();
            }

            float grenadeAttackDelay = (float) this.grenadeAttackDelayField.GetValue(source);
            float grenadeAmount = (int) this.grenadeAmountField.GetValue(source);

            this.grenadeAttackLogic = new LogicSet(this.GetType() + ":m_grenadeAttackLogic");
            LogicSet.Begin(this.grenadeAttackLogic);
            // LogicSet.Add(new ChangePropertyAction(source, "pulseIndicator", true));
            LogicSet.Add(new RunFunctionAction(this, "TurnToFace", new object[] {source}), SequenceType.Serial);
            LogicSet.Add(new ChangePropertyAction(source, "State", CharacterState.Attacking));
            LogicSet.Add(new ChangePropertyAction(source, "CurrentSpeed", 0));
            LogicSet.Add(new PlayAnimationAction(source, "Attack1TellStart", "Attack1TellEnd", false, false, true));

            LogicSet.Add(new DelayAction(grenadeAttackDelay / 2));
            LogicSet.Add(new PlayAnimationAction(source, source.GetFrameFromLabel("Attack1ActionStart"),
                source.GetFrameFromLabel("Attack1ActionEnd") - 2));
            // LogicSet.Add(new FaceDirectionAction(source, TargetType.CurrentTarget));
            // LogicSet.Add(new ChangePropertyAction(source, "LockFlip", true));
            LogicSet.Add(new PlayEventAction(GameController.soundManager,
                "event:/SFX/Enemies/Mobs/Grenadier/sfx_enemy_gren_throw", source));
            for (int index = 0; index < grenadeAmount; ++index)
                LogicSet.Add(new RunFunctionAction(this, "ThrowGrenade", new object[] {source}));
            // LogicSet.Add(new ChangePropertyAction(source, "pulseIndicator", false));
            LogicSet.Add(new PlayAnimationAction(source, source.GetFrameFromLabel("Attack1ActionEnd") - 1,
                source.GetFrameFromLabel("Attack1ActionEnd")));
            // LogicSet.Add(new ChangePropertyAction(source, "LockFlip", false));
            LogicSet.Add(new ChangePropertyAction(source, "State", CharacterState.Idle));
            this.grenadeAttackLogic.Tag = "Tag_CD_Grenade_UNBREAKABLE";
            // LogicSet.Add(new CallCooldownAction(source));
            LogicSet.End();

            EnemyControllableManager.RunEnemyAttack((EnemyObj) source, ref this.grenadeAttackLogic, ref this.grenadeAttackBlock);
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

        public void ThrowGrenade(Enemy_Grenadier_Basic source)
        {
            float orientation = this.TurnToFace(source);
            // obj.HeadingX = (float) Math.Cos(obj.Orientation);
            // obj.HeadingY = (float) Math.Sin(obj.Orientation);


            ProjectileObj projectileObj = GameController.g_game.ProjectileManager.FireProjectile(new ProjectileData
            {
                SpriteName = "Projectile_Grenade_Sprite",
                canStun = true,
                Scale = new Vector2(2f, 2f),
                Damage = source.damage,
                checkForSameTypes = true,
                collidesWithCharacterTerrainBox = false,
                followOrientation = false,
                hasShadow = true,
                shadowOffset = new Vector2(0.0f, 5f),
                followFlipOrientation = true
            }, source);
            // projectileObj.Heading = this.Heading;
            projectileObj.HeadingX = (float) Math.Cos(orientation);
            projectileObj.HeadingY = (float) Math.Sin(orientation);

            if (projectileObj.Heading != Vector2.Zero)
                projectileObj.Heading.Normalize();
            projectileObj.CurrentSpeed = (float) this.grenadeSpeedField.GetValue(source);
            projectileObj.fakeWeighted = true;

            var distance = (Vector2) this.grenadeDistanceRandomField.GetValue(source);
            float grenadeSpeed = (float) this.grenadeSpeedField.GetValue(source);
            float grenadeDistanceValue = RNG.get(365).RandomFloat(distance.X, distance.Y);
            this.grenadeDistanceField.SetValue(source, grenadeDistanceValue);
            projectileObj.jumpDistance = grenadeDistanceValue;
            projectileObj.CalculateFakeAccelerationY();
            projectileObj.LifeSpan = grenadeDistanceValue / grenadeSpeed;
            var vector2 = source.Position + projectileObj.Heading * grenadeDistanceValue;
            var currentZone = GameController.g_game.ScreenManager.arenaScreen.currentZone;
            var navMesh = currentZone.navMesh;
            if (currentZone.hookProjectilesToNavMesh && navMesh != null &&
                (!currentZone.hookProjectilesToNavMesh || !navMesh.BufferedContains(vector2)))
                return;
            Tween.RunFunction((float) (grenadeDistanceValue / (double) grenadeSpeed), source, "CreateGrenadeExplosion",
                (object) vector2);
            float grenadeReticleWidth = (float) this.grenadeReticleWidthField.GetValue(source);
            this.DisplayReticle(source, grenadeDistanceValue, grenadeReticleWidth, grenadeSpeed);
        }

        public void DisplayReticle(EnemyObj source, float grenadeDistance, float grenadeReticleWidth, float grenadeSpeed)
        {
            BrawlerSpriteObj layeredSprite = GameController.g_game.SpriteManager.GetLayeredSprite("Artillery_Target2White_Sprite");
            layeredSprite.Layer = 0.0f;
            layeredSprite.isPixelSprite = false;
            layeredSprite.lockLayer = true;
            layeredSprite.PlayAnimation(true, false);
            layeredSprite.Opacity = 0.0f;
            layeredSprite.TextureColor = PlayerClassObj_Mercenary.MERCENARY_COLOUR;

            Vector2 heading = source.Heading;
            if (heading != Vector2.Zero)
                heading.Normalize();
            layeredSprite.Position = source.Position + heading * grenadeDistance;
            layeredSprite.PlayAnimation(true, false);
            // float grenadeReticleWidth = this.m_grenadeReticleWidth;
            layeredSprite.Scale = new Vector2(grenadeReticleWidth / layeredSprite.SpriteBounds.Width, grenadeReticleWidth / 2.65f / layeredSprite.SpriteBounds.Height);
            Tween.To((object) layeredSprite, 0.1f, new Easing(Tween.EaseNone), (object) "Opacity", (object) 1f);
            Tween.RunFunction(grenadeDistance / grenadeSpeed, (object) layeredSprite, "StopAnimation");
        }

        public void ThrowMine(Enemy_Grenadier_Basic source)
        {
            source.Heading = this.TurnToFaceVector(source);
            ProjectileData projData = new ProjectileData
            {
                SpriteName = "Projectile_Mine_Sprite",
                canStun = true,
                Scale = new Vector2(2.5f, 2.5f),
                Damage = source.damage,
                checkForSameTypes = true,
                collidesWithCharacterTerrainBox = false,
                followOrientation = false,
                hasShadow = true,
                shadowOffset = new Vector2(0.0f, 5f),
                followFlipOrientation = false
            };
            var mineDistanceRandom = (Vector2)this.mineDistanceRandomField.GetValue(source);
            float num1 = RNG.get(373).RandomFloat(mineDistanceRandom.X, mineDistanceRandom.Y);
            ArenaZone currentZone = GameController.g_game.ScreenManager.arenaScreen.currentZone;
            NavMeshObj navMesh = currentZone.navMesh;
            int num2 = 0;
            bool flag;
            for (flag = false; !flag && num2 < 10; ++num2)
            {
                Vector2 vector2 = source.Position + source.Heading * num1;
                if (!currentZone.hookProjectilesToNavMesh || navMesh == null ||
                    currentZone.hookProjectilesToNavMesh && navMesh.BufferedContains(vector2))
                    flag = true;
                else
                    num1 = RNG.get(374).RandomFloat(mineDistanceRandom.X, mineDistanceRandom.Y);
            }

            if (flag)
            {
                float mineExplosionSize = (float)this.mineExplosionSizeField.GetValue(source);
                float mineTriggerDelay = (float)this.mineTriggerDelayField.GetValue(source);
                float mineTriggerRadiusX = (float)this.mineTriggerRadiusXField.GetValue(source);

                var projectileObj =
                    GameController.g_game.ProjectileManager.FireProjectile(projData, source);
                projectileObj.Heading = source.Heading;
                projectileObj.AnimationSpeed = 0.05f;
                projectileObj.CurrentSpeed = (float)this.mineSpeedField.GetValue(source);
                projectileObj.fakeWeighted = true;
                projectileObj.jumpDistance = num1;
                projectileObj.TextureColor = PlayerClassObj_Mercenary.MERCENARY_COLOUR;
                projectileObj.CalculateFakeAccelerationY();
                projectileObj.LifeSpan = (float) this.mineLifeSpanField.GetValue(source);
                projectileObj.projectileType = ProjectileType.MineRadius_TriggerEnemy;
                projectileObj.PlayAnimation("InAirStart", "InAirEnd", true);
                projectileObj.SetMineProjData(new ProjectileData
                {
                    SpriteName = "Projectile_Grenade_Explosion",
                    canStun = true,
                    forceKnockdown = true,
                    Scale = new Vector2(mineExplosionSize, mineExplosionSize),
                    Damage = source.damage,
                    checkForSameTypes = true,
                    destroyOnAnimationEnd = true,
                    collidesWithCharacterTerrainBox = true
                }, mineTriggerDelay, mineTriggerRadiusX);

                EnemyControllableManager.changeColorFeedback.Add(projectileObj);
            }
            else
                Console.WriteLine("Could not drop mine.  Could not find suitable spot on navmesh.");
        }

        public void CastSpellR(CharacterObj character, PlayerObj playerObj)
        {
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
