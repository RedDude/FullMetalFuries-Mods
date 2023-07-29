using CDGEngine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Tweener;

namespace Brawler2D
{
  public class SpeechBubble : EnemyObj
  {
    protected float m_signTextDisplayHeight = 2f;
    protected float m_signOpacityDefault = 0.85f;
    protected float m_signOpacityOnBlock = 0.85f;

    private PlayerActionTextObj m_textObj;
    private float m_textDisplayCounter;
    public bool m_displayVisible;
    private BrawlerContainerObj m_signSprite;
    public bool m_isAppearing;
    private Vector2 m_storedSignScale;
    private float m_firstFrameTop;

    public BrawlerContainerObj target;
    public string text;

    public bool triggerOnHit;

    public float displayDuration = 1f;
    public float displayDurationOnHit = 0.5f;
    public Func<bool, ContainerObj, string> textCallback;
    public Action<bool, ContainerObj> completeCallback;

    public bool hasHit;
    public ContainerObj triggerSource;

    private ContextSensitiveObj contextSensitiveObj;
    private float collisionEllipseWidthSetter;

    public SpeechBubble(GameController game, float collisionEllipseWidthSetter = 0f)
      : base(game, "")
    {
      this.Name = "Signpost_Basic";
      this.m_objType = 113;
      this.killCountable = false;
      this.isNotEnemy = true;
      this.showHealthBar = false;
      this.spawnLimitCountable = false;
      this.dropsGold = false;
      this.collisionEllipseWidthSetter = collisionEllipseWidthSetter;
      this.m_hitEvent = "event:/SFX/Props/Other/sfx_prop_signpost_hit_spin";
    }


    public override IEnumerable<object> LoadContent()
    {
        this.m_textObj = new PlayerActionTextObj("LoveYa15")
        {
            FontSize = 8f,
            WordWrap = 437,
            TextureColor = Color.Black,
            TextAlign = TextAlign.Centre,
            WidthAnchorAlign = WidthAlign.Centre
        };

        this.m_signSprite = new BrawlerContainerObj("TextBox_Paper_Container")
        {
            Visible = false,
            disableAutoKill = true,
            lockLayer = true,
            Layer = 1f,
            Scale = new Vector2(0.65f, 0.1f)
        };
        this.m_storedSignScale = this.m_signSprite.Scale;

        foreach (object obj in base.LoadContent())
            yield return null;
    }

    protected override void InitializeData()
    {
      base.InitializeData();

      this.shadowWidth = 0;
      this.collisionEllipseWidth = this.collisionEllipseWidthSetter;
      this.shadowYOffset = 0;
      // this.shadowWidth = 0;
      // this.collisionEllipseWidth = this.collisionEllipseWidthSetter;
      // this.shadowYOffset = 0;
      this.StopAnimation();
      this.killCountable = false;
      this.LockFlip = true;

      // this.m_firstFrameTop = this.AbsBounds.Top;
    }

    protected override void InitializeLogic()
    {
      base.InitializeLogic();
    }

    public override void HitCharacter(
      IDamageObj damageObj,
      Vector2 impactPt,
      float dmgOverride = -1f,
      bool applyModsToDmgOverride = false,
      bool forceHit = false)
    {
        if(!this.triggerOnHit)
            return;

      if (!this.AcceptHit(damageObj, forceHit) || !this.NetAcceptHit(damageObj))
        return;
      this.AddToHitList(damageObj);
      if (this.m_displayVisible)
        return;

      PlayerClassObj playerClass = null;
      if (!(damageObj is PlayerClassObj) && damageObj is ProjectileObj projectileObj)
        playerClass = projectileObj.SourceObj as PlayerClassObj;
      // if (playerClass == null)
      //   Console.WriteLine("cannot display sign.  Object that hit sign was not player.  It was " + (object) damageObj);
      this.hasHit = true;
      this.triggerSource = playerClass;
      if (playerClass == null)
      {
          this.triggerSource = playerClass;
          if (damageObj is ContainerObj containerObj)
              this.triggerSource = containerObj;
          else
              this.triggerSource = null;
      }
      this.NoDisplaySign(impactPt, this.m_isAppearing);
    }

    public void SetContext(BrawlerContainerObj target, string text = null)
    {
        this.contextSensitiveObj = new ContextSensitiveObj {methodObject1 = this, method1Name = "SayContext"};
        this.target = target;
        this.text = text;
    }

    public void SayContext()
    {
        this.hasHit = false;
        this.triggerSource = this.contextSensitiveObj.contextPlayer.currentPlayerClass;
        this.DisplaySign();
        // this.NoDisplaySign(this.target.Position, this.m_isAppearing);
    }


    public void Say(BrawlerContainerObj target, string text, ContainerObj triggerSource = null)
    {
        if (this.m_displayVisible)
            return;

        this.target = target;
        this.text = text;
        this.hasHit = false;

        this.triggerSource =  triggerSource;

        // this.m_textDisplayCounter = this.displayDuration;
        this.DisplaySign();

        // this.NoDisplaySign(target.Position, this.m_isAppearing);
    }

    public void NoDisplaySign(Vector2 impactPt, bool isAppearing)
    {
      Net.Master.NoDisplaySign(this, null, impactPt, isAppearing);
      GameController.soundManager.PlayEvent(this.m_hitEvent, (IPositionalObj) this, false, false);
      BrawlerSpriteObj layeredSprite = this.Game.SpriteManager.GetLayeredSprite("PlayerHitEffect_Sprite");
      layeredSprite.Position = impactPt;
      layeredSprite.Layer = this.Layer + 1f / 1000f;
      layeredSprite.lockLayer = true;
      layeredSprite.Scale = new Vector2(2f, 2f);
      layeredSprite.PlayAnimation(false, false);
      // this.PlayAnimationThenFunction("HitStart", "HitEnd", (object) this, "GoToFrame", (object) 1);
      this.m_hitSFX = GameController.soundManager.PlayEvent("event:/SFX/Characters/Generic/Hits/sfx_char_gen_hit_melee_1", (IPositionalObj) this, false, false);
      // GameController.soundManager.PlayEvent("event:/SFX/Props/Other/sfx_prop_signpost_spin", (IPositionalObj) this, false, false);

      if (isAppearing)
      // if (playerClass == null || isAppearing)
        return;
      this.DisplaySign();
    }

    public void DisplaySign()
    {
        if ( this.m_isAppearing)
            // if (playerClass == null || isAppearing)
            return;

      this.m_displayVisible = true;
      this.m_isAppearing = true;
      this.m_signSprite.Scale = this.m_storedSignScale * 1f / GameController.g_game.Camera.Zoom;
      var absBounds = this.m_signSprite.AbsBounds;

      this.m_game.SpriteManager.AddNonResourcePooledObj(SpriteEnum.ContainerFG, this.m_signSprite);
      this.m_signSprite.X = this.X;
      this.m_signSprite.Opacity = 0.0f;
      this.m_textObj.Scale = Vector2.One;
      this.m_game.TextManager.AddNonResourcePooledTextObj(TextEnum.Text, this.m_textObj);
      this.m_textObj.player = GameController.g_game.PlayerManager.getMainPlayerOrHost; //null;//player;
      this.m_textObj.Text = this.text ?? this.textCallback.Invoke(this.hasHit, this.triggerSource);//ModEntry.modHelper.Translation.Get(this.Tag, (BrawlerTextObj) this.m_textObj);//LocaleBuilder.getString(this.Tag, (BrawlerTextObj) this.m_textObj));
      this.m_textDisplayCounter = this.hasHit ? this.displayDurationOnHit : this.displayDuration;
      this.m_textObj.Opacity = 0.0f;
      this.m_textObj.X = this.m_signSprite.X;
      this.m_signSprite.Scale = new Vector2(this.m_textObj.AbsBounds.X, this.m_textObj.AbsBounds.Y);
      Tween.To(this.m_textObj, 0.25f, new Easing(Tween.EaseNone), (object) "Opacity", (object) this.m_signOpacityDefault);
      Tween.To(this.m_signSprite, 0.25f, new Easing(Tween.EaseNone), (object) "Opacity", (object) this.m_signOpacityDefault);
      Tween.AddEndHandlerToLastTween(this, "DisableEffect", (object) this.m_signSprite);
      // this.m_isAppearing = false;
      // DisableEffect(BrawlerContainerObj obj)

      this.m_signSprite.Y = this.target.AbsBounds.Top - absBounds.Height / 2f + this.m_signTextDisplayHeight;

      // var fgContainer = this.m_game.SpriteManager.GetFGContainer("TextBox_Paper_Container");
      // float num = this.Bounds.Width / fgContainer.Bounds.Width;
      // fgContainer.Scale = new Vector2(num, num);
      // fgContainer.Position = this.Position - new Vector2(0.0f, fgContainer.Bounds.Height);
      // fgContainer.disableAutoKill = true;
      // fgContainer.Opacity = 0.0f;
      // Tween.To(fgContainer, 0.15f, new Easing(Tween.EaseNone), (object) "ScaleX", (object) this.m_signSprite.ScaleX, (object) "ScaleY", (object) this.m_signSprite.ScaleY, (object) "X", (object) this.m_signSprite.X, (object) "Y", (object) this.m_signSprite.Y, (object) "Opacity", (object) 0.7f);
      // Tween.AddEndHandlerToLastTween(this, "DisableEffect", (object) fgContainer);
    }

    public void DisableEffect(BrawlerContainerObj obj)
    {
      // obj.disableAutoKill = false;
      // obj.Active = false;
      this.m_isAppearing = false;
    }

    public void CallComplete(bool hit, ContainerObj source)
    {
        this.completeCallback?.Invoke(hit, source);
    }


    public void HideSign()
    {
      if (!this.m_displayVisible || this.m_isAppearing)
        return;


      this.m_isAppearing = true;
      this.m_displayVisible = false;
      Tween.To(this.m_textObj, 0.25f, new Easing(Tween.EaseNone), (object) "Opacity", (object) 0.0f);
      Tween.AddEndHandlerToLastTween(this.m_game.TextManager, "DestroyNonResourcePooledTextObj", (object) TextEnum.Text, (object) this.m_textObj);
      Tween.To(this.m_signSprite, 0.25f, new Easing(Tween.EaseNone), (object) "Opacity", (object) 0.0f);
      Tween.AddEndHandlerToLastTween(this.m_game.SpriteManager, "DestroyNonResourcePooledObj", (object) SpriteEnum.ContainerFG, (object) this.m_signSprite);

      this.completeCallback?.Invoke(this.hasHit, this.triggerSource);
      this.hasHit = false;

      // BrawlerContainerObj fgContainer = this.m_game.SpriteManager.GetFGContainer("TextBox_Paper_Container");
      // float num1 = this.m_signSprite.Bounds.Width / fgContainer.Bounds.Width;
      // float num2 = this.Bounds.Width / fgContainer.Bounds.Width;
      // fgContainer.Scale = new Vector2(num1, num1);
      // fgContainer.Position = this.m_signSprite.Position;
      // fgContainer.disableAutoKill = true;
      // fgContainer.Opacity = 0.7f;
      // Tween.To(fgContainer, 0.2f, new Easing(Tween.EaseNone), (object) "ScaleX", (object) num2, (object) "ScaleY", (object) num2, (object) "X", (object) this.X, (object) "Y", (object) this.Y, (object) "Opacity", (object) 0.0f);
      // Tween.AddEndHandlerToLastTween(this, "DisableEffect", (object) fgContainer);
      this.m_isAppearing = false;
    }

    public override void Update(float elapsedSeconds)
    {
      if (this.m_textDisplayCounter > 0.0)
      {
          this.Position = this.target.Position;
          this.m_textDisplayCounter -= elapsedSeconds;
      }

      if (this.m_displayVisible && this.m_textDisplayCounter <= 0.0)
          this.HideSign();

      // if (this.m_displayVisible && !this.m_isAppearing) // && (this.triggerOnHit || this.contextSensitiveObj != null))
        this.CheckBoundsCollision();

      CDGRect absBounds = this.m_signSprite.AbsBounds;
      Vector2 vector2 = Vector2.One * (this.m_signSprite.Scale / this.m_storedSignScale);
      if (this.m_textObj.Scale != vector2)
        this.m_textObj.Scale = vector2;

      this.m_signSprite.X = this.X;
      this.m_signSprite.Y = this.target.AbsBounds.Top - absBounds.Height / 2f + this.m_signTextDisplayHeight;
      // this.m_textObj.Y = this.m_signSprite.Y + 1f * this.m_signSprite.ScaleX;
      this.m_textObj.Y = this.m_signSprite.Y + this.m_signSprite.ScaleY / 2 - 10;
      this.m_textObj.X = this.m_signSprite.X;

      this.UpdateHitList(elapsedSeconds);
    }

    public void CheckBoundsCollision()
    {
      this.m_signSprite.Opacity = this.m_signOpacityDefault;
      this.m_textObj.Opacity = this.m_signOpacityDefault;
      CDGRect absBounds = this.m_signSprite.AbsBounds;
      CDGRect ownerBounds = this.AbsBounds;
      int playerArrayCount = Net.pm.activePlayerArray_count;
      for (int index = 0; index < playerArrayCount; ++index)
      {
          if (Net.pm.activePlayerArray[index].currentPlayerClass.AbsBounds.Intersects(ownerBounds))
          {
              if (this.contextSensitiveObj != null)
              {
                  this.contextSensitiveObj.contextPlayer = Net.pm.activePlayerArray[index];
                  Net.pm.activePlayerArray[index].contextObj = this.contextSensitiveObj;
                  this.contextSensitiveObj.contextPlayer.displayInputIcon = true;
              }
              else
              {
                  this.contextSensitiveObj.contextPlayer.displayInputIcon = false;
                  Net.pm.activePlayerArray[index].contextObj = null;
              }
          }

          if (Net.pm.activePlayerArray[index].currentPlayerClass.AbsBounds.Intersects(absBounds))
        {
          this.m_signSprite.Opacity = this.m_signOpacityOnBlock;
          this.m_textObj.Opacity = this.m_signOpacityOnBlock;

          return;
        }
      }
      if (Net.sm.CurrentScreen == null || !(Net.sm.CurrentScreen is ArenaScreen))
        return;
      ArenaScreen currentScreen = Net.sm.CurrentScreen as ArenaScreen;
      int globalEnemyArrayCount = currentScreen.currentAndGlobalEnemyArray_count;
      for (int index = 0; index < globalEnemyArrayCount; ++index)
      {
        EnemyObj currentAndGlobalEnemy = currentScreen.currentAndGlobalEnemyArray[index];
        if (currentAndGlobalEnemy == this || !currentAndGlobalEnemy.AbsBounds.Intersects(absBounds)) continue;
        this.m_signSprite.Opacity = this.m_signOpacityOnBlock;
        this.m_textObj.Opacity = this.m_signOpacityOnBlock;
        break;
      }
    }

    public override void Draw(Camera2D camera, float elapsedSeconds)
    {
      this.m_signSprite.Scale = this.m_storedSignScale * 1f / camera.Zoom;
      base.Draw(camera, elapsedSeconds);
    }

    public override void Dispose()
    {
      if (this.IsDisposed)
        return;
      this.m_textObj = null;
      this.m_signSprite = null;
      this.contextSensitiveObj = null;
      base.Dispose();
    }
  }
}
