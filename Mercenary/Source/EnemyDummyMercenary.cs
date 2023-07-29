using System;
using Brawler2D;
using CDGEngine;
using cs.Blit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FullModdedFuriesAPI.Mods.MercenaryClass.Source
{
  public class EnemyDummyMercenary : EnemyObj, IDodgableObj
  {
    protected float m_dodgeSpeed = 400f;
    protected float m_dodgeDistance = 175f;
    protected float m_attackTellDelay = 0.1f;
    protected Vector2 m_attackStartDelay = new Vector2(0.0f, 0.1f);
    protected float m_attackAnimationSpeed = 10f;
    protected float m_attackPostPause = 0.15f;
    protected float m_attackExitDelay = 0.25f;
    protected Vector2 m_spellDistanceX = new Vector2(125f, 275f);
    protected Vector2 m_spellDistanceY = new Vector2(-20f, 20f);
    protected float m_spellDelay = 0.4f;
    protected float m_spellSpeed = 500f;
    protected float m_spellDuration = 2.5f;
    protected int m_spellLoop = 1;
    protected float m_spellCD = 2.5f;
    protected Vector2 m_dashAttackRangeX = new Vector2(130f, 175f);
    protected Vector2 m_dashAttackRangeY = new Vector2(-30f, 30f);
    protected Vector2 m_dashAttackStartDelay = new Vector2(0.0f, 0.1f);
    protected float m_dashAttackAnimationSpeed = 12f;
    protected float m_dashAttackDistance = 165f;
    protected float m_dashAttackSpeed = 600f;
    protected float m_dashAttackTellDelay = 0.5f;
    protected float m_dashAttackExitDelay = 0.3f;
    private float m_hitReturnDuration = 3f;
    protected LogicBlock m_meleeLB;
    protected LogicBlock m_projectileLB;
    protected LogicBlock m_wanderLB;
    protected float m_dodgeDistanceCounter;
    protected float m_attackSpeed;
    protected float m_attackDistance;
    protected int m_spellAngle;
    protected LogicSet m_enterArrowRange;
    protected LogicSet m_attackLogic;
    protected LogicSet m_arrowLogic;
    protected LogicSet m_dashAttackLogic;
    protected LogicSet m_enterDashAttackRange;
    private Vector2 m_startingPos;
    private float m_hitTimer;
    protected LogicBlock m_returnToStartLB;

    public override bool LockFlip
    {
      get => true;
      set => base.LockFlip = value;
    }

    public bool isDodging { get; set; }

    public EnemyDummyMercenary(GameController game)
      : base(game, nameof (Enemy_Dummy_Basic))
    {
      this.Name = "Dummy_Mercenary";
      this.m_objType = 164;
      this.showHealthBar = false;
      this.displayArrowIndicator = false;
      this.givesMastery = false;

    }

    protected override void InitializeData()
    {
      base.InitializeData();
      this.shadowYOffset = 26f;
      this.shadowWidth = 38f;
      this.m_attackRangeX = new Vector2(70f, 90f);
      this.m_attackRangeY = new Vector2(-35f, 35f);
      this.m_dodgeAnimationSpeed = 30f;
      this.m_idleDuration = new Vector2(99999f, 99999f);
      this.TextureColor = Color.Blue;
      this.ChangeSprite("Mercenary_Tris_Dummy");
    }

    protected override void InitializeLogic()
    {
      base.InitializeLogic();
      this.m_enterDashAttackRange = new LogicSet(this.GetType().ToString() + ":m_enterDashAttackRange");
      LogicSet.Begin(this.m_enterDashAttackRange);
      // LogicSet.Add(new PlayAnimationAction((DisplayObj) this, "RunStart", "RunEnd", true, false, false), SequenceType.Serial);
      // LogicSet.Add(new ChangePropertyAction((object) this, "CurrentSpeed", (object) this.Speed), SequenceType.Serial);
      // LogicSet.Add(new GoToLocationAction((CharacterObj) this, TargetType.CurrentTarget, this.m_dashAttackRangeX, this.m_dashAttackRangeY, this.m_attemptDuration, this.m_rng, true, true, true, true), SequenceType.Serial);
      LogicSet.Add(new ChangePropertyAction((object) this, "State", (object) CharacterState.Idle), SequenceType.Serial);
      LogicSet.End();
      this.m_enterArrowRange = new LogicSet(this.GetType().ToString() + ":m_enterArrowRange");
      LogicSet.Begin(this.m_enterArrowRange);
      // LogicSet.Add(new PlayAnimationAction((DisplayObj) this, "RunStart", "RunEnd", true, false, false), SequenceType.Serial);
      // LogicSet.Add(new ChangePropertyAction((object) this, "CurrentSpeed", (object) this.Speed), SequenceType.Serial);
      // LogicSet.Add(new GoToLocationAction((CharacterObj) this, TargetType.CurrentTarget, this.m_spellDistanceX, this.m_spellDistanceY, this.m_attemptDuration, this.m_rng, true, true, true, true), SequenceType.Serial);
      LogicSet.Add(new ChangePropertyAction((object) this, "State", (object) CharacterState.Idle), SequenceType.Serial);
      LogicSet.End();
      this.m_attackLogic = new LogicSet(this.GetType().ToString() + ":m_attackLogic");
      LogicSet.Begin(this.m_attackLogic);
      LogicSet.Add(new DelayAction(this.m_attackStartDelay), SequenceType.Serial);
      LogicSet.Add(new ChangePropertyAction((object) this, "State", (object) CharacterState.Attacking), SequenceType.Serial);
      LogicSet.Add(new ChangePropertyAction((object) this, "CurrentSpeed", (object) 0), SequenceType.Serial);
      LogicSet.Add(new ChangePropertyAction((object) this, "AnimationSpeed", (object) (float) (1.0 / (double) this.m_attackAnimationSpeed)), SequenceType.Serial);
      LogicSet.Add(new FaceDirectionAction((CharacterObj) this, TargetType.CurrentTarget), SequenceType.Serial);
      LogicSet.Add(new ChangePropertyAction((object) this, "LockFlip", (object) true), SequenceType.Serial);
      LogicSet.Add(new PlayAnimationAction((DisplayObj) this, "Attack1TellStart", "Attack1TellEnd", false, false, false), SequenceType.Serial);
      LogicSet.Add(new ChangeColourAction((CharacterObj) this, Color.Red, 0.1f, -1f), SequenceType.Serial);
      LogicSet.Add(new DelayAction(this.m_attackTellDelay), SequenceType.Serial);
      LogicSet.Add(new PlayAnimationAction((DisplayObj) this, this.GetFrameFromLabel("Attack1ActionStart"), this.GetFrameFromLabel("Attack1ActionEnd") - 3, false, false, false), SequenceType.Serial);
      LogicSet.Add(new ChangePropertyAction((object) this, "CurrentSpeed", (object) this.m_attackSpeed), SequenceType.Serial);
      LogicSet.Add(new PlayAnimationAction((DisplayObj) this, this.GetFrameFromLabel("Attack1ActionEnd") - 2, this.GetFrameFromLabel("Attack1ActionEnd"), false, false, false), SequenceType.Parallel);
      LogicSet.Add(new ChangePropertyAction((object) this, "CurrentSpeed", (object) 0), SequenceType.Serial);
      LogicSet.Add(new DelayAction(this.m_attackExitDelay), SequenceType.Serial);
      LogicSet.Add(new PlayAnimationAction((DisplayObj) this, "StandStart", "StandEnd", true, false, false), SequenceType.Serial);
      LogicSet.Add(new ChangePropertyAction((object) this, "AnimationSpeed", (object) this.m_storedAnimSpeed), SequenceType.Serial);
      LogicSet.Add(new ChangePropertyAction((object) this, "LockFlip", (object) false), SequenceType.Serial);
      LogicSet.Add(new ChangePropertyAction((object) this, "State", (object) CharacterState.Idle), SequenceType.Serial);
      LogicSet.Add(new CallCooldownAction((EnemyObj) this), SequenceType.Serial);
      LogicSet.End();
      this.m_arrowLogic = new LogicSet(this.GetType().ToString() + ":m_arrowLogic");
      LogicSet.Begin(this.m_arrowLogic);
      LogicSet.Add(new RunLogicSetAction(this.m_enterArrowRange), SequenceType.Serial);
      LogicSet.Add(new ChangePropertyAction((object) this, "State", (object) CharacterState.Attacking), SequenceType.Serial);
      LogicSet.Add(new ChangePropertyAction((object) this, "CurrentSpeed", (object) 0), SequenceType.Serial);
      LogicSet.Add(new PlayAnimationAction((DisplayObj) this, "Attack3TellStart", "Attack3TellEnd", false, false, false), SequenceType.Serial);
      LogicSet.Add(new ChangeColourAction((CharacterObj) this, Color.Red, 0.1f, -1f), SequenceType.Serial);
      LogicSet.Add(new DelayAction(this.m_spellDelay), SequenceType.Serial);

      LogicSet.Add(new ChangePropertyAction((object) this, "AnimationSpeed", (object) this.m_storedAnimSpeed), SequenceType.Serial);
      LogicSet.Add(new DelayAction(this.m_attackPostPause), SequenceType.Serial);
      LogicSet.Add(new ChangePropertyAction((object) this, "State", (object) CharacterState.Idle), SequenceType.Serial);
      LogicSet.Add(new AddLSCooldownAction((EnemyObj) this, this.m_spellCD, this.m_arrowLogic), SequenceType.Serial);
      this.m_arrowLogic.Tag = "Tag_CD_Arrow";
      LogicSet.Add(new CallCooldownAction((EnemyObj) this), SequenceType.Serial);
      LogicSet.End();
      this.m_dashAttackLogic = new LogicSet(this.GetType().ToString() + ":m_dashAttackLogic");
      LogicSet.Begin(this.m_dashAttackLogic);
      LogicSet.Add(new RunLogicSetAction(this.m_enterDashAttackRange), SequenceType.Serial);
      LogicSet.Add(new DelayAction(this.m_dashAttackStartDelay), SequenceType.Serial);
      LogicSet.Add(new ChangePropertyAction((object) this, "State", (object) CharacterState.Attacking), SequenceType.Serial);
      LogicSet.Add(new ChangePropertyAction((object) this, "CurrentSpeed", (object) 0), SequenceType.Serial);
      LogicSet.Add(new ChangePropertyAction((object) this, "AnimationSpeed", (object) (float) (1.0 / (double) this.m_dashAttackAnimationSpeed)), SequenceType.Serial);
      LogicSet.Add(new PlayAnimationAction((DisplayObj) this, "Attack2TellStart", "Attack2TellEnd", false, false, false), SequenceType.Serial);
      LogicSet.Add(new DelayAction(this.m_dashAttackTellDelay / 2f), SequenceType.Serial);
      LogicSet.Add(new ChangePropertyAction((object) this, "LockFlip", (object) true), SequenceType.Serial);
      LogicSet.Add(new FaceDirectionAction((CharacterObj) this, TargetType.CurrentTarget), SequenceType.Serial);
      LogicSet.Add(new ChangeColourAction((CharacterObj) this, Color.Red, 0.1f, -1f), SequenceType.Serial);
      LogicSet.Add(new DelayAction(this.m_dashAttackTellDelay / 2f), SequenceType.Serial);
      LogicSet.Add(new PlayAnimationAction((DisplayObj) this, "Attack2ActionStart", "Attack2ActionEnd", false, false, false), SequenceType.Parallel);
      LogicSet.Add(new ChangePropertyAction((object) this, "CurrentSpeed", (object) this.m_dashAttackSpeed), SequenceType.Serial);
      LogicSet.Add(new DelayAction(this.m_dashAttackDistance / this.m_dashAttackSpeed), SequenceType.Serial);
      LogicSet.Add(new ChangePropertyAction((object) this, "CurrentSpeed", (object) 0), SequenceType.Serial);
      LogicSet.Add(new ChangePropertyAction((object) this, "AnimationSpeed", (object) this.m_storedAnimSpeed), SequenceType.Serial);
      LogicSet.Add(new PlayAnimationAction((DisplayObj) this, "Attack2ActionEnd", "Attack2ActionEnd", false, false, false), SequenceType.Serial);
      LogicSet.Add(new DelayAction(this.m_dashAttackExitDelay), SequenceType.Serial);
      LogicSet.Add(new PlayAnimationAction((DisplayObj) this, "Attack2ExitStart", "Attack2ExitEnd", false, false, false), SequenceType.Serial);
      LogicSet.Add(new ChangePropertyAction((object) this, "LockFlip", (object) false), SequenceType.Serial);
      LogicSet.Add(new ChangePropertyAction((object) this, "State", (object) CharacterState.Idle), SequenceType.Serial);
      LogicSet.Add(new CallCooldownAction((EnemyObj) this), SequenceType.Serial);
      LogicSet.End();
      LogicBlock logicBlock1 = new LogicBlock(this.m_rng);
      logicBlock1.AddLogicSet(this.m_getUpLogic, this.m_getUpDodgeLogic);
      this.SetKnockdownResponseLB(logicBlock1, 100, 0);
      LogicBlock logicBlock2 = new LogicBlock(this.m_rng);
      logicBlock2.AddLogicSet(this.m_idleLogic, this.m_walkToMeleeLogic, this.m_walkToProjectileLogic, this.m_dodgeLogic);
      this.SetCooldownResponseLB(logicBlock2, 100, 0, 0, 0);
      this.m_wanderLB = new LogicBlock(this.m_rng);
      this.m_wanderLB.AddLogicSet(this.m_idleLogic, this.m_walkToMeleeLogic, this.m_walkToProjectileLogic, this.m_attackLogic);
      this.SetWanderRangeLogic(this.m_wanderLB, 100, 0, 0, 0);
      this.m_projectileLB = new LogicBlock(this.m_rng);
      this.m_projectileLB.AddLogicSet(this.m_idleLogic, this.m_attackLogic, this.m_dashAttackLogic, this.m_arrowLogic);
      this.SetProjectileRangeLogic(this.m_projectileLB, 100, 0, 0, 0);
      this.m_meleeLB = new LogicBlock(this.m_rng);
      this.m_meleeLB.AddLogicSet(this.m_idleLogic, this.m_attackLogic, this.m_dashAttackLogic, this.m_arrowLogic);
      this.SetMeleeRangeLogic(this.m_meleeLB, 100, 0, 0, 0);

      LogicSet ls = new LogicSet(this.GetType().ToString() + ":returnToStartLS");
      LogicSet.Begin(ls);
      LogicSet.Add(new PlayAnimationAction((DisplayObj) this, "RunStart", "RunEnd", true, false, false), SequenceType.Serial);
      LogicSet.Add(new ChangePropertyAction((object) this, "CurrentSpeed", (object) this.Speed), SequenceType.Serial);
      LogicSet.Add(new RunFunctionAction((object) this, "ForceFlip", new object[0]), SequenceType.Serial);
      // LogicSet.Add(new GoToLocationAction((CharacterObj) this, TargetType.CurrentTarget, Vector2.Zero, Vector2.Zero, 999f, this.m_rng, true, true, false, false), SequenceType.Serial);
      LogicSet.Add(new ChangePropertyAction((object) this, "State", (object) CharacterState.Idle), SequenceType.Serial);
      LogicSet.End();
      this.m_returnToStartLB = new LogicBlock(this.m_rng);
      this.m_returnToStartLB.AddLogicSet(ls);
    }

    public override void OnSpawn()
    {
      if (this.m_startingPos == Vector2.Zero)
        this.m_startingPos = this.AbsPosition;
      this.m_dummyTarget.Position = this.m_startingPos;
      this.TurnSpeed = 1E+08f;
      base.OnSpawn();
      this.currentTarget = (CharacterObj) this.m_dummyTarget;
    }

    public void Dodge()
    {
      this.Dodge(new Vector2?());
    }

    public virtual void Dodge(Vector2? heading)
    {
      Vector2 vector2 = new Vector2(RNG.get(363).RandomFloat(-1f, 1f), RNG.get(364).RandomFloat(-1f, 1f));
      this.Collidable = true;
      if (heading.HasValue)
        vector2 = heading.Value;
      this.State = CharacterState.Dodging;
      vector2.X = this.Flip != SpriteEffects.None ? -1f : 1f;
      if (vector2 != Vector2.Zero)
        vector2.Normalize();
      this.HeadingX = vector2.X;
      this.HeadingY = -vector2.Y;
      this.m_currentSpeed = this.m_dodgeSpeed;
      this.m_dodgeDistanceCounter = 0.0f;
      this.isDodging = true;
      this.PlayAnimation("DodgeStart", "DodgeEnd", true, false);
      if ((double) this.HeadingX < 0.0)
        this.Flip = SpriteEffects.FlipHorizontally;
      else
        this.Flip = SpriteEffects.None;
    }

    public void ForceFlip()
    {
      if (!BlitNet.Lobby.IsMaster)
        return;
      this.Flip = SpriteEffects.None;
      if ((double) this.currentTarget.X >= (double) this.X)
        return;
      this.Flip = SpriteEffects.FlipHorizontally;
    }

    public override void Update(float elapsedSeconds)
    {
      base.Update(elapsedSeconds);
      switch (this.State)
      {
        case CharacterState.Idle:
          if ((double) this.m_hitTimer <= 0.0)
            break;
          this.m_hitTimer -= elapsedSeconds;
          if ((double) this.m_hitTimer > 0.0)
            break;
          this.ExecuteLB(this.m_returnToStartLB, 100);
          break;
        case CharacterState.Dodging:
          this.m_dodgeDistanceCounter += Math.Abs((this.Heading * this.CurrentSpeed * elapsedSeconds).Length());
          if ((double) this.m_dodgeDistanceCounter < (double) this.m_dodgeDistance)
            break;
          this.isDodging = false;
          break;
      }
    }

    public override void HitCharacter(
      IDamageObj damageObj,
      Vector2 impactPt,
      float dmgOverride = -1f,
      bool applyModsToDmgOverride = false,
      bool forceHit = false)
    {
      this.Flip = SpriteEffects.None;
      if ((double) damageObj.X < (double) this.X)
        this.Flip = SpriteEffects.FlipHorizontally;
      base.HitCharacter(damageObj, impactPt, dmgOverride, applyModsToDmgOverride, forceHit);
      this.m_hitTimer = this.m_hitReturnDuration;
    }

    public override void RunDeathAnimation(GameObj attacker)
    {
      this.RemoveAllStatusEffects(true);
      this.StopAllActions();
      this.ExecuteLB(this.m_deathAnimationLB, 100);
      this.State = CharacterState.Dying;
      this.LockFlip = true;
      this.CurrentSpeed = 0.0f;
      this.Collidable = false;
      this.Acceleration = Vector2.Zero;
      this.Opacity = 0.5f;
    }

    public override void Reset(bool resetDeathState, bool stopAllTweens)
    {
      this.isDodging = false;
      base.Reset(resetDeathState, stopAllTweens);
    }

    public override void Dispose()
    {
      if (this.IsDisposed)
        return;
      if (this.m_meleeLB != null)
        this.m_meleeLB.Dispose();
      this.m_meleeLB = (LogicBlock) null;
      if (this.m_projectileLB != null)
        this.m_projectileLB.Dispose();
      this.m_projectileLB = (LogicBlock) null;
      if (this.m_wanderLB != null)
        this.m_wanderLB.Dispose();
      this.m_wanderLB = (LogicBlock) null;
      if (this.m_enterArrowRange != null)
        this.m_enterArrowRange.Dispose();
      this.m_enterArrowRange = (LogicSet) null;
      if (this.m_attackLogic != null)
        this.m_attackLogic.Dispose();
      this.m_attackLogic = (LogicSet) null;
      if (this.m_arrowLogic != null)
        this.m_arrowLogic.Dispose();
      this.m_arrowLogic = (LogicSet) null;
      if (this.m_dashAttackLogic != null)
        this.m_dashAttackLogic.Dispose();
      this.m_dashAttackLogic = (LogicSet) null;
      if (this.m_enterDashAttackRange != null)
        this.m_enterDashAttackRange.Dispose();
      this.m_enterDashAttackRange = (LogicSet) null;
      if (this.m_returnToStartLB != null)
        this.m_returnToStartLB.Dispose();
      this.m_returnToStartLB = (LogicBlock) null;
      base.Dispose();
    }
  }
}
