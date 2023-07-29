using System;
using Brawler2D;
using CDGEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FullModdedFuriesAPI.Mods.MercenaryClass.Source
{
  public class MercenaryDummyLeap : BaseAttack
  {
    private Vector2 m_jumpDirection;
    private PlayerAction m_playerAction;
    private int dummyEnemyIndex;
    private int trisDummyIndex;
    private int erinDummyIndex;
    private int alexDummyIndex;
    private int megDummyIndex;

    public MercenaryDummyLeap(PlayerClassObj source, string name, PlayerAction playerAction)
      : base(source, name)
    {
      this.m_playerAction = playerAction;
      this.trisDummyIndex = ModEntry.modHelper.Database.GetCustomEquipmentIndex("Equipment_MercenaryDummy1");
      this.erinDummyIndex = ModEntry.modHelper.Database.GetCustomEquipmentIndex("Equipment_MercenaryDummy2");
      this.alexDummyIndex = ModEntry.modHelper.Database.GetCustomEquipmentIndex("Equipment_MercenaryDummy3");
      this.megDummyIndex = ModEntry.modHelper.Database.GetCustomEquipmentIndex("Equipment_MercenaryDummy4");


    }

    public override void InitializeLogic()
    {
        LogicSet.Begin(this.m_normalAttackLS);
      LogicSet.Add(new RunFunctionAction((object) this.source.player.inputMap, "SetAllLocks", new object[1]
      {
        (object) true
      }));
      LogicSet.Add(new ChangePropertyAction((object) this.source, "State", (object) CharacterState.Attacking));
      // LogicSet.Add(new ChangePropertyAction((object) this.source, "CurrentSpeed", (object) 0));
      LogicSet.Add(new ChangePropertyAction((object) this.source, "AnimationSpeed", (object) this.animSpeed));
      LogicSet.Add(new RunFunctionAction((object) this.source, "PlayVocals", new object[2]
      {
        (object) "event:/Vocalizations/Characters/Fighter/Efforts/vox_char_ftr_effort",
        (object) false
      }));
      LogicSet.Add(new ChangePropertyAction((object) this.source, "isJumpAttacking", (object) true));
      LogicSet.Add(new RunFunctionAction((object) this, "JumpAttack", new object[0]));
      LogicSet.Add(new ChangePropertyAction((object) this.source, "LockFlip", (object) true));
      LogicSet.Add(new PropertyValueReachedAction((object) this.source, "isJumpAttacking", Convert.ToSingle(false)));
      LogicSet.Add(new PlayAnimationAction((DisplayObj) this.source, "StandStart", "StandEnd", true, false, false), SequenceType.Parallel);
      LogicSet.Add(new ChangePropertyAction((object) this.source, "AnimationSpeed", (object) this.m_sourceAnimationSpeed));
      LogicSet.Add(new RunFunctionAction((object) this.source.player.inputMap, "SetAllLocks", new object[1]
      {
        (object) false
      }));
      LogicSet.Add(new ChangePropertyAction((object) this.source, "LockFlip", (object) false));
      LogicSet.Add(new ChangePropertyAction((object) this.source, "State", (object) CharacterState.Idle));
      LogicSet.End();
    }

    public void JumpAttack()
    {
        this.dummyEnemyIndex = ModEntry.modHelper.Database.GetCustomEnemyIndex("Dummy_Mercenary");

        var dummy = ModEntry.modHelper.Database.SpawnCustomEnemy(this.dummyEnemyIndex, this.source.Position);

        var equipment = this.source.GetEquippedEquipment(EquipmentSlotType.ButtonA);
        if(equipment == (EquipmentType) this.trisDummyIndex)
            dummy.ChangeSprite("Mercenary_Tris_Dummy");
        else if(equipment == (EquipmentType) this.erinDummyIndex)
            dummy.ChangeSprite("Mercenary_Erin_Dummy");
        else if(equipment == (EquipmentType) this.alexDummyIndex)
            dummy.ChangeSprite("Mercenary_Alex_Dummy");
        else if(equipment == (EquipmentType) this.megDummyIndex)
            dummy.ChangeSprite("Mercenary_Meg_Dummy");
        // if (this.source.HasEquippedEquipment(this.source.classType, this.trisDummyIndex))

        this.m_jumpDirection = this.source.leftJoystickDirection;
      if (this.m_jumpDirection == Vector2.Zero)
        this.m_jumpDirection.X = this.source.Flip != SpriteEffects.None ? -1f : 1f;
      this.m_jumpDirection = -this.m_jumpDirection;
      float angle = CDGMath.VectorToAngle(this.m_jumpDirection);
      double num1 = Math.Atan(225.0 * Math.Tan((double) angle * Math.PI / 180.0) / (225.0 * (1.0 / (double) PlayerEV.FIGHTER_MINI_ELLIPSE_RATIO)));
      if ((double) angle > 90.0)
        num1 += Math.PI;
      else if ((double) angle < -90.0)
        num1 -= Math.PI;
      float num2 = CDGMath.DistanceBetweenPts(Vector2.Zero, new Vector2((float) (225.0 * Math.Cos(num1)), (float) (225.0 * (1.0 / (double) PlayerEV.FIGHTER_MINI_ELLIPSE_RATIO) * Math.Sin(num1))));
      float num3 = num2 / 125f;
      this.source.HeadingX = this.m_jumpDirection.X;
      this.source.HeadingY = this.m_jumpDirection.Y;
      this.source.CurrentSpeed = 600f * num3;
      this.source.jumpDistance = num2 / 1.5f;
      GameController.soundManager.PlayEvent("event:/SFX/Characters/Fighter/Dunk/sfx_char_ftr_a_leap_up", (IPositionalObj) this.source, false, false);
      this.source.CalculateFakeAccelerationY();
      this.source.UpdateFlipInput();
      this.DashEffect();
    }

    public void DashEffect()
    {
      BrawlerSpriteObj layeredSprite = this.source.Game.SpriteManager.GetLayeredSprite("Dash_Effect");
      layeredSprite.PlayAnimation(false, false);
      layeredSprite.Position = this.source.shadowPosition;
      layeredSprite.AnimationSpeed = 0.05f;
      layeredSprite.Flip = SpriteEffects.None;
      if (this.source.Flip == SpriteEffects.None)
        layeredSprite.Flip = SpriteEffects.FlipHorizontally;
      layeredSprite.Scale = new Vector2(2f);
    }

    public override void CancelAttack()
    {
      this.source.player.inputMap.SetAllLocks(false);
      this.source.enableMouseAiming = false;
      this.source.targetSpriteDistance = 0;
      base.CancelAttack();
    }

    public EnemyObj SpawnEnemy(EnemyType enemyType, Vector2 position)
    {
        var enemyManager = GameController.g_game.EnemyManager;
        int enemyObjIndex = enemyManager.PreloadEnemy(enemyType, position);
        var enemyObj = enemyManager.enemyArray[enemyObjIndex];
        enemyObj.spawnAnimType = EnemySpawnAnimType.None;
        GameController.g_game.ScreenManager.arenaScreen.AddToCurrentEnemyList(enemyObj);
        EnemySpawnAnimator.SpawnEnemy(enemyObj, true);

        return enemyObj;
    }

    public EnemyObj SpawnEnemy(EnemyObj newEnemyObj, Vector2 position)
    {
        var enemyManager = GameController.g_game.EnemyManager;
        int enemyObjIndex = this.PreloadEnemy(newEnemyObj, position);
        var enemyObj = enemyManager.enemyArray[enemyObjIndex];
        enemyObj.spawnAnimType = EnemySpawnAnimType.None;
        GameController.g_game.ScreenManager.arenaScreen.AddToCurrentEnemyList(enemyObj);
        EnemySpawnAnimator.SpawnEnemy(enemyObj, true);

        return enemyObj;
    }

    public int PreloadEnemy(EnemyObj enemyObj, Vector2 position)
    {
        var enemyManager = GameController.g_game.EnemyManager;
        foreach (object obj in enemyObj.LoadContent())
            ;
        enemyObj.Initialize();
        enemyObj.isCameraShy = true;
        enemyObj.Active = false;
        enemyObj.Visible = false;
        enemyObj.Collidable = false;
        enemyObj.forceDraw = false;
        enemyObj.Position = position;
        enemyObj.isSpawning = false;
        enemyManager.AddEnemy(enemyObj);
        return enemyManager.enemyArray_count - 1;
    }
  }
}
