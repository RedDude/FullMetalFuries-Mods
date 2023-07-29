using Brawler2D;
using CDGEngine;
using cs.Blit;
using FMOD_.Studio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tweener;

namespace FullModdedFuriesAPI.Mods.MercenaryClass.Source
{
    public class MercenaryArtilleryAttack : BaseAttack
    {
        private Vector2 m_jumpDirection;
        private PlayerAction m_playerAction;

        private float m_artilleryStrikeLargeWidth = 225f;
        private Vector2 m_artilleryStrikeLargeSpaceWidth = new Vector2(75f, 25f);
        private float m_artilleryStrikeLargeDelay = 0.025f;
        protected LogicSet m_callArtilleryLargeLogic;
        protected float m_chatterStartDelay = 0.9f;
        protected float m_chatterEndDelay = 0.7f;
        private float m_artilleryStrikeSmallWidth = 100f;

        private Vector2 m_artilleryStrikeLineSpaceWidth = new Vector2(75f, 0.0f);
        private EventInstance m_artilleryStrikeSFX;
        private EventInstance m_artilleryStrikeWhistleSFX;

        public Vector2 artilleryTargetPos;
        protected float m_artilleryTargetLayer;
        private BrawlerSpriteObj m_speechBubble;
        protected float m_artilleryStrikeDelay = 0.1f;
        protected float m_artilleryStrikeScale = 2f;
        private LogicSet m_artilleryStrikeLS;

        public BrawlerSpriteObj artilleryRedTarget;
        private double enemyActivationRadius = 300;

        public MercenaryArtilleryAttack(PlayerClassObj source, string name, PlayerAction playerAction)
            : base(source, name)
        {
            this.m_playerAction = playerAction;
        }

        public void Update(float elapsedSeconds)
        {
            if (this.m_callArtilleryLargeLogic != null && this.m_callArtilleryLargeLogic.IsActive)
                this.m_callArtilleryLargeLogic.Update(elapsedSeconds);
        }

        public void Execute()
        {
            this.m_callArtilleryLargeLogic.Execute();
        }

        public override void InitializeLogic()
        {
            LogicSet ls1 = new LogicSet(this.GetType().ToString() + ":fireArtilleryLargeLogic");
            LogicSet.Begin(ls1);

            LogicSet.Add(new RunFunctionAction((object) this, "SpawnArtilleryReticle", new object[2]
            {
                (object) this.m_artilleryStrikeLargeWidth,
                (object) new Vector2(0.0f, 0.0f)
            }), SequenceType.Serial);

            for (int x = -1; x <= 1; ++x)
            {
                for (int y = -1; y <= 1; ++y)
                {
                    // LogicSet.Add(new RunFunctionAction((object) this, "StartArtilleryProjectileSmall", new object[1]
                    // {
                    //     (object) new Vector2(this.m_artilleryStrikeLargeSpaceWidth.X * (float) x,
                    //         this.m_artilleryStrikeLargeSpaceWidth.Y * (float) y)
                    // }), SequenceType.Serial);
                    LogicSet.Add(new DelayAction(this.m_artilleryStrikeLargeDelay), SequenceType.Serial);
                }
            }

            LogicSet.End();
            this.m_callArtilleryLargeLogic = new LogicSet(this.GetType() + ":m_callArtilleryLargeLogic");
            LogicSet.Begin(this.m_callArtilleryLargeLogic);
            // LogicSet.Add(new ChangePropertyAction((object) this, "pulseIndicator", (object) true), SequenceType.Serial);
            LogicSet.Add(new ChangePropertyAction(this.source, "State", (object) CharacterState.Attacking));
            LogicSet.Add(new ChangePropertyAction(this.source, "CurrentSpeed", (object) 0));
            // LogicSet.Add(new ChangePropertyAction(this.source, "LockFlip", (object) true));

            LogicSet.Add(
                new PlayAnimationAction((DisplayObj) this.source, "ReadyLoopStart", "ReadyLoopEnd", false), SequenceType.Parallel);
            LogicSet.Add(new RunFunctionAction((object) this, "SpawnSpeechBubble"));
            LogicSet.Add(new RunFunctionAction((object) this, "RunArtilleryStrike", ls1));
            LogicSet.Add(new DelayAction(0.1f));
            LogicSet.Add(new MoveTargetUsingInput(this, 2.0f));
            LogicSet.Add(
                new PlayAnimationAction((DisplayObj) this.source, "ReadyStart", "ReadyEnd", false), SequenceType.Parallel);
            // LogicSet.Add(new ChangePropertyAction(this, "m_artilleryTargetPos", (object) this.artilleryTarget.Position));
            // LogicSet.Add(new ChangePropertyAction((object) this, "pulseIndicator", (object) false), SequenceType.Serial);
            // LogicSet.Add(new DelayAction(this.m_chatterEndDelay), SequenceType.Serial);
            // LogicSet.Add(new DelayAction(this.m_chatterStartDelay));
            LogicSet.Add(new RunFunctionAction((object) this, "FireArtilleryOnTarget", Vector2.One));
            // LogicSet.Add(new MoreAttacksTargetUsingInput(this, 0.5f));
            LogicSet.Add(new ChangePropertyAction(this.source, "isArtilleryAttacking", (object) false));
            LogicSet.Add(new ChangePropertyAction(this.source, "State", (object) CharacterState.Idle));
            // LogicSet.Add(new ChangePropertyAction(this.source, "LockFlip", (object) false));


            // LogicSet.Add(new RunLogicSetAction(this.m_idleLogic), SequenceType.Serial);
            // LogicSet.Add(new CallCooldownAction((PlayerClassObj) this), SequenceType.Serial);
            LogicSet.End();
        }

        public void SpawnSpeechBubble()
        {
            this.source.PlayVocals("event:/SFX/Enemies/Mobs/Artillerist/sfx_enemy_artill_call", true);
            var speechBubbleLayer =
                this.source.Game.SpriteManager.GetLayeredSprite("Artillery_SpeechBubbleBasic_Sprite");
            speechBubbleLayer.target = this.m_source;
            speechBubbleLayer.isPixelSprite = true;
            speechBubbleLayer.PlayAnimation(true, false);
            speechBubbleLayer.Scale = new Vector2(2f, 2f);
            speechBubbleLayer.targetOffsetY = -50f;
            Tween.RunFunction(this.m_chatterStartDelay + this.m_chatterEndDelay, (object) speechBubbleLayer,
                "StopAnimation").ID = "ArtilleryStrike";
            speechBubbleLayer.Tag = "ArtilleryStrike";
            this.m_speechBubble = speechBubbleLayer;
        }

        public void DestroySpeechBubble()
        {
            if (this.m_speechBubble != null && this.m_speechBubble.Tag == "ArtilleryStrike" &&
                this.m_speechBubble.target == this)
            {
                this.DestroySpeechBubble();
                return;
            }

            if(this.m_speechBubble != null)
                Tween.StopAllContaining((object) this.m_speechBubble, "ArtilleryStrike", false);
            this.m_speechBubble?.StopAnimation();
            this.m_speechBubble = (BrawlerSpriteObj) null;
        }

        public void SpawnArtilleryReticle(float reticleSize, Vector2 reticleDisplace)
        {
            var target = this.source.getTarget(TargetType.ClosestEnemyObj);
            if (target == null || CDGMath.DistanceBetweenPts(this.source.AbsPosition, target.AbsPosition) >
                (double) this.enemyActivationRadius)
            {
                this.artilleryTargetPos = this.source.AbsPosition + this.source.shadowOffset;
                this.m_artilleryTargetLayer = this.source.Layer + 1f / 1000f;
            }
            else
            {
                this.artilleryTargetPos = target.AbsPosition + target.shadowOffset;
                this.m_artilleryTargetLayer = target.Layer + 1f / 1000f;
            }


            var vector2 = this.artilleryTargetPos + reticleDisplace;
            var currentZone = this.source.Game.ScreenManager.arenaScreen.currentZone;
            var navMesh = currentZone.navMesh;
            if (currentZone.hookProjectilesToNavMesh && navMesh != null &&
                (!currentZone.hookProjectilesToNavMesh || !navMesh.BufferedContains(vector2)))
                return;

            this.artilleryRedTarget = this.source.Game.SpriteManager.GetLayeredSprite("Artillery_Target2White_Sprite");
            var redTarget
                = this.artilleryRedTarget;
            redTarget.TextureColor = PlayerClassObj_Mercenary.MERCENARY_COLOUR;
            redTarget.Layer = 0.0f;
            redTarget.isPixelSprite = false;
            redTarget.lockLayer = true;
            redTarget.PlayAnimation(true, false);
            redTarget.Opacity = 0.0f;
            redTarget.Position = this.artilleryTargetPos + reticleDisplace;
            this.m_artilleryStrikeWhistleSFX = GameController.soundManager.PlayEvent(
                "event:/SFX/Enemies/Mobs/Artillerist/sfx_enemy_artill_falling", (IPositionalObj) redTarget, false,
                false);
            float num = reticleSize;
            redTarget.Scale = new Vector2(num / redTarget.SpriteBounds.Width,
                num / 2.65f / redTarget.SpriteBounds.Height);
            Tween.To((object) redTarget, 0.1f, new Easing(Tween.EaseNone), (object) "Opacity", (object) 1f);
        }

        public void FireArtilleryProjectileSmall(Vector2 reticleDisplace)
        {
            var vector2_1 = this.artilleryTargetPos + reticleDisplace;
            var currentZone = this.source.Game.ScreenManager.arenaScreen.currentZone;
            var navMesh = currentZone.navMesh;
            if (currentZone.hookProjectilesToNavMesh && navMesh != null &&
                (!currentZone.hookProjectilesToNavMesh || !navMesh.BufferedContains(vector2_1)))
                return;
            var artilleristBulletLayer =
                this.source.Game.SpriteManager.GetLayeredSprite("Projectile_ArtilleristLarge_Bullet");
            artilleristBulletLayer.PlayAnimation(true, false);
            artilleristBulletLayer.Scale = new Vector2(1f, 1f);
            artilleristBulletLayer.Layer = this.m_artilleryTargetLayer + 1f / 1000f;
            artilleristBulletLayer.lockLayer = true;
            artilleristBulletLayer.isPixelSprite = true;
            artilleristBulletLayer.TextureColor = Color.Purple;
            artilleristBulletLayer.AnimationSpeed = 0.05f;
            var vector2_2 = CDGMath.AngleToVector(-60f) * 4000f;
            artilleristBulletLayer.Position = this.artilleryTargetPos + vector2_2;
            Tween.To((object) artilleristBulletLayer, this.m_artilleryStrikeDelay, new Easing(Tween.EaseNone),
                (object) "X",
                (object) (float) ((double) this.artilleryTargetPos.X + (double) reticleDisplace.X), (object) "Y",
                (object) (float) ((double) this.artilleryTargetPos.Y + (double) reticleDisplace.Y)).ID = "DoNotStop";
            Tween.AddEndHandlerToLastTween((object) artilleristBulletLayer, "StopAnimation");
            Tween.RunFunction(this.m_artilleryStrikeDelay, (object) this, "CreateExplosion",
                (object) new Vector2(this.artilleryTargetPos.X + reticleDisplace.X,
                    this.artilleryTargetPos.Y + reticleDisplace.Y), (object) 2.5f).ID = "DoNotStop";
            var circleSprite = this.source.Game.SpriteManager.GetLayeredSprite("Circle_Sprite");
            circleSprite.Layer = 1f / 1000f;
            circleSprite.lockLayer = true;
            circleSprite.TextureColor = Color.Purple;
            circleSprite.Opacity = 0.4f;
            circleSprite.PlayAnimation(true, false);
            circleSprite.Position = this.artilleryTargetPos + reticleDisplace;
            float num = 30f;
            var vector2_3 = new Vector2(num / circleSprite.SpriteBounds.Width,
                num / 2.65f / circleSprite.SpriteBounds.Height) * 1.75f;
            circleSprite.Scale = vector2_3 * 0.1f;
            Tween.To((object) circleSprite, this.m_artilleryStrikeDelay, new Easing(Tween.EaseNone),
                (object) "ScaleX", (object) vector2_3.X, (object) "ScaleY", (object) vector2_3.Y).ID = "DoNotStop";
            Tween.AddEndHandlerToLastTween((object) circleSprite, "StopAnimation");
        }

        public void StartArtilleryProjectileSmall(Vector2 reticleDisplace)
        {
            var reticlePosition = this.artilleryTargetPos + reticleDisplace;
            var currentZone = this.source.Game.ScreenManager.arenaScreen.currentZone;
            var navMesh = currentZone.navMesh;
            if (currentZone.hookProjectilesToNavMesh && navMesh != null &&
                (!currentZone.hookProjectilesToNavMesh || !navMesh.BufferedContains(reticlePosition)))
                return;
            var artilleryBulletLayer =
                this.source.Game.SpriteManager.GetLayeredSprite("Projectile_ArtilleristLarge_Bullet");
            artilleryBulletLayer.PlayAnimation(true, false);
            artilleryBulletLayer.Scale = new Vector2(1f, 1f);
            artilleryBulletLayer.Layer = this.m_artilleryTargetLayer + 1f / 1000f;
            artilleryBulletLayer.lockLayer = true;
            artilleryBulletLayer.isPixelSprite = true;
            // artilleristBulletLayer.TextureColor = Color.Purple;
            artilleryBulletLayer.AnimationSpeed = 0.05f;
            var vector2_2 = CDGMath.AngleToVector(-60f) * 4000f;
            artilleryBulletLayer.Position = this.artilleryTargetPos + vector2_2;//reticlePosition;//
            // Tween.To((object) artilleryBulletLayer, this.m_artilleryStrikeDelay, new Easing(Tween.EaseNone));
            Tween.To((object) artilleryBulletLayer, this.m_artilleryStrikeDelay, new Easing(Tween.EaseNone), (object) "X", (object) (float) ((double) this.artilleryTargetPos.X + (double) reticleDisplace.X), (object) "Y", (object) (float) ((double) this.artilleryTargetPos.Y + (double) reticleDisplace.Y)).ID = "DoNotStop";
            Tween.AddEndHandlerToLastTween((object) artilleryBulletLayer, "StopAnimation");
        }

        public void FireArtilleryOnTargetOnPress(Vector2 reticleDisplace)
        {
            if(this.source.player.inputMap.JustPressed(PlayerAction.SpellB))
                this.FireArtilleryOnTarget(reticleDisplace);
        }

        public void FireArtilleryOnTarget(Vector2 reticleDisplace)
        {
            for (int x = -1; x <= 1; ++x)
            {
                for (int y = -1; y <= 1; ++y)
                {

                    reticleDisplace = new Vector2(this.m_artilleryStrikeLargeSpaceWidth.X * (float) x,
                        this.m_artilleryStrikeLargeSpaceWidth.Y * (float) y);

                    var vector2_1 = this.artilleryTargetPos + reticleDisplace;
                    this.StartArtilleryProjectileSmall(reticleDisplace);
                    var vector2_2 = CDGMath.AngleToVector(-60f) * 4000f;
                    // this.artilleryTarget.Position = this.m_artilleryTargetPos + vector2_2;
                    Tween.To((object) this.artilleryRedTarget, this.m_artilleryStrikeDelay, new Easing(Tween.EaseNone),
                        (object) "X",
                        (object) (float) ((double) this.artilleryTargetPos.X + (double) reticleDisplace.X), (object) "Y",
                        (object) (float) ((double) this.artilleryTargetPos.Y + (double) reticleDisplace.Y)).ID = "DoNotStop";
                    Tween.AddEndHandlerToLastTween((object) this.artilleryRedTarget, "StopAnimation");
                    Tween.RunFunction(this.m_artilleryStrikeDelay, (object) this, "CreateExplosion",
                        (object) new Vector2(this.artilleryTargetPos.X + reticleDisplace.X,
                            this.artilleryTargetPos.Y + reticleDisplace.Y), (object) 2.5f).ID = "DoNotStop";
                    var circleSprite = this.source.Game.SpriteManager.GetLayeredSprite("Circle_Sprite");
                    circleSprite.Layer = 1f / 1000f;
                    circleSprite.lockLayer = true;
                    circleSprite.TextureColor = Color.Purple;
                    circleSprite.Opacity = 0.4f;
                    circleSprite.PlayAnimation(true, false);
                    circleSprite.Position = this.artilleryTargetPos + reticleDisplace;
                    float num = 30f;
                    var vector2_3 = new Vector2(num / circleSprite.SpriteBounds.Width,
                        num / 2.65f / circleSprite.SpriteBounds.Height) * 1.75f;
                    circleSprite.Scale = vector2_3 * 0.1f;
                    Tween.To((object) circleSprite, this.m_artilleryStrikeDelay, new Easing(Tween.EaseNone),
                        (object) "ScaleX", (object) vector2_3.X, (object) "ScaleY", (object) vector2_3.Y).ID = "DoNotStop";
                    Tween.AddEndHandlerToLastTween((object) circleSprite, "StopAnimation");
                    Tween.RunFunction(this.m_chatterEndDelay + this.m_artilleryStrikeDelay, (object) this.artilleryRedTarget,
                        "StopAnimation");
                }
            }


        }

        public void TargetAndFireArtilleryProjectileSmall(Vector2 reticleDisplace)
        {
            this.StartArtilleryProjectileSmall(reticleDisplace);
            this.FireArtilleryOnTarget(reticleDisplace);
        }

        public void FireArtilleryProjectileMega(Vector2 reticleDisplace)
        {
            var vector2_1 = this.artilleryTargetPos + reticleDisplace;
            var currentZone = this.source.Game.ScreenManager.arenaScreen.currentZone;
            var navMesh = currentZone.navMesh;
            if (currentZone.hookProjectilesToNavMesh && navMesh != null &&
                (!currentZone.hookProjectilesToNavMesh || !navMesh.BufferedContains(vector2_1)))
                return;
            var artilleryBulletLayer =
                this.source.Game.SpriteManager.GetLayeredSprite("Projectile_ArtilleristLarge_Bullet");
            artilleryBulletLayer.PlayAnimation(true, false);
            artilleryBulletLayer.Scale = new Vector2(this.m_artilleryStrikeScale, this.m_artilleryStrikeScale);
            artilleryBulletLayer.Layer = this.m_artilleryTargetLayer + 1f / 1000f;
            artilleryBulletLayer.lockLayer = true;
            artilleryBulletLayer.isPixelSprite = true;
            artilleryBulletLayer.AnimationSpeed = 0.05f;
            var vector2_2 = CDGMath.AngleToVector(-60f) * 4000f;
            artilleryBulletLayer.Position = this.artilleryTargetPos + vector2_2;
            Tween.To((object) artilleryBulletLayer, this.m_artilleryStrikeDelay, new Easing(Tween.EaseNone),
                (object) "X",
                (object) (float) ((double) this.artilleryTargetPos.X + (double) reticleDisplace.X), (object) "Y",
                (object) (float) ((double) this.artilleryTargetPos.Y + (double) reticleDisplace.Y)).ID = "DoNotStop";
            Tween.AddEndHandlerToLastTween((object) artilleryBulletLayer, "StopAnimation");
            Tween.RunFunction(this.m_artilleryStrikeDelay, (object) this, "CreateExplosion",
                (object) new Vector2(this.artilleryTargetPos.X + reticleDisplace.X,
                    this.artilleryTargetPos.Y + reticleDisplace.Y), (object) 3f).ID = "DoNotStop";
            var layeredSprite2 = this.source.Game.SpriteManager.GetLayeredSprite("Circle_Sprite");
            layeredSprite2.Layer = 1f / 1000f;
            layeredSprite2.lockLayer = true;
            layeredSprite2.TextureColor = Color.Black;
            layeredSprite2.Opacity = 0.4f;
            layeredSprite2.PlayAnimation(true, false);
            layeredSprite2.Position = this.artilleryTargetPos + reticleDisplace;
            float num = 30f;
            var vector2_3 = new Vector2(num / layeredSprite2.SpriteBounds.Width,
                num / 2.65f / layeredSprite2.SpriteBounds.Height) * 1.75f;
            layeredSprite2.Scale = vector2_3 * 0.1f;
            Tween.To((object) layeredSprite2, this.m_artilleryStrikeDelay, new Easing(Tween.EaseNone),
                (object) "ScaleX", (object) vector2_3.X, (object) "ScaleY", (object) vector2_3.Y).ID = "DoNotStop";
            Tween.AddEndHandlerToLastTween((object) layeredSprite2, "StopAnimation");
        }

        public void CreateExplosion(Vector2 pos, float scale)
        {
            var currentZone = this.source.Game.ScreenManager.arenaScreen.currentZone;
            var navMesh = currentZone.navMesh;
            if (currentZone.hookProjectilesToNavMesh && navMesh != null &&
                (!currentZone.hookProjectilesToNavMesh || !navMesh.BufferedContains(pos)))
                return;
            var targetObj = this.source.Game.ProjectileManager.FireProjectile(new ProjectileData()
            {
                SpriteName = "ArtilleristTriple_MineExplosion",
                canStun = true,
                destroyOnAnimationEnd = true,
                Scale = new Vector2(scale, scale),
                Damage = this.source.damage,
                checkForSameTypes = true,
                collidesWithCharacterTerrainBox = true
            }, (CharacterObj) this.source, false, (GameObj) null);
            targetObj.AnimationSpeed = 0.04545455f;
            targetObj.Layer = this.source.Game.ScreenManager.arenaScreen.getLayerValue(pos.Y);
            targetObj.lockLayer = true;
            targetObj.Position = pos;
            targetObj.PlayAnimation(false, false);
            this.RunShatterAnimation(targetObj);
            targetObj.forceKnockdown = true;
            targetObj.knockdownSpeedMod = 0.935f;
            targetObj.knockdownDistanceMod = 1.1f;
            targetObj.CollisionID = 2; //Mod.Instance.WhenFriendlyChangeCollisionId(2, player.playerIndex);
            (this.source.Game.ScreenManager.CurrentScreen as BrawlerScreen).ShakeScreen(0.25f, new Vector2(2f, 2f));
            this.m_artilleryStrikeSFX = GameController.soundManager.PlayEvent(
                "event:/SFX/Enemies/Mobs/Artillerist/sfx_enemy_artill_strike", (IPositionalObj) targetObj, false,
                false);
        }

        public void RunArtilleryStrike(LogicSet ls)
        {
            this.m_artilleryStrikeLS = ls;
            using (new NetScope<Forced>())
            {
                using (new NetScope<IgnoreParent>())
                    this.m_artilleryStrikeLS.Execute();
            }
        }
        public void RunShatterAnimation(ProjectileObj targetObj)
        {
            var absSpriteBounds = targetObj.AbsSpriteBounds;
            for (int index = 1; index <= 6; ++index)
            {
                var debris =
                    this.source.Game.SpriteManager.GetLayeredSprite("Debris_Piece" +
                                                                    (object) RNG.get(333).RandomInt(1, 6));
                debris.Scale = new Vector2(1f);
                debris.applyBounce = true;
                debris.Layer = this.source.AbsLayer + 0.0001f;
                debris.disableAutoKill = true;
                debris.AccelerationY = (float) RNG.get(334).RandomInt(-625, -450);
                debris.AccelerationX = (float) RNG.get(335).RandomInt(-115, 115);
                debris.bounciness = 0.35f;
                debris.Position =
                    new Vector2(RNG.get(336).RandomFloat(absSpriteBounds.Left, absSpriteBounds.Right),
                        RNG.get(337).RandomFloat(absSpriteBounds.Top, absSpriteBounds.Bottom));
                debris.rotationSpeed = (float) RNG.get(338).RandomInt(100, 200);
                if (debris.Flip == SpriteEffects.None)
                {
                    debris.rotationSpeed = -debris.rotationSpeed;
                    debris.AccelerationX = -debris.AccelerationX;
                }

                debris.groundLevel = RNG.get(339).RandomFloat(absSpriteBounds.Top, absSpriteBounds.Bottom);
                Tween.To((object) debris, 0.25f, new Easing(Tween.EaseNone), (object) "delay", (object) 1,
                    (object) "Opacity", (object) 0);
                Tween.AddEndHandlerToLastTween(this.source.Game.SpriteManager, "DestroyObj",
                    (object) SpriteEnum.SpriteLayered, (object) debris);
            }
        }
    }
}
