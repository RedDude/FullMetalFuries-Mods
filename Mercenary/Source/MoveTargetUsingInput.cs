using cs.Blit;
using Brawler2D;
using CDGEngine;
using Microsoft.Xna.Framework;

namespace FullModdedFuriesAPI.Mods.MercenaryClass.Source
{
    class MoveTargetUsingInput : LogicAction, MNetUpdateMaster
    {
        private MercenaryArtilleryAttack m_source;
        private EnemyObj.RangeState m_rangeState;
        private Vector2 m_endPt;
        private bool m_towardsOnly;
        private Vector2 m_rightJoystickDirection;

        private CharacterState lastState;
        private float m_delayCounter;
        private bool m_delayModified;
        private float m_storedDelayChange;
        private float m_maxDelayDuration;
        private float m_minDelayDuration;

        public MoveTargetUsingInput(MercenaryArtilleryAttack source, float delayInSecs)
            : base("Move Target Using Input")
        {
            this.m_source = source;
            this.m_minDelayDuration = delayInSecs;
            this.m_delayCounter = 0.0f;
            this.m_delayModified = false;
        }

        public MoveTargetUsingInput(MercenaryArtilleryAttack source, float minDelayInSecs, float maxDelayInSecs)
            : base("Move Target Using Input")
        {
            this.m_source = source;
            this.m_minDelayDuration = minDelayInSecs;
            this.m_maxDelayDuration = maxDelayInSecs;
            this.m_delayModified = false;
        }

        public MoveTargetUsingInput(MercenaryArtilleryAttack source, Vector2 randSecs)
            : base("Move Target Using Input")
        {
            this.m_source = source;
            this.m_minDelayDuration = randSecs.X;
            this.m_maxDelayDuration = randSecs.Y;
            this.m_delayModified = false;
        }


        // public MoveUsingInput(
        //     BrawlerSpriteObj source,
        //     EnemyObj.RangeState rangeState,
        //     bool ifFailedStopLogic = false,
        //     bool towardsOnly = false)
        //     : base("Move Using Input")
        // {
        //     this.m_source = source;
        //     this.m_rangeState = rangeState;
        //     this.m_towardsOnly = towardsOnly;
        // }

        public float delayCounter
        {
            get => this.m_delayCounter;
            set
            {
                this.m_delayCounter = value;
                this.m_storedDelayChange = value;
                this.m_delayModified = true;
            }
        }

        protected override void Execute()
        {
            this.sequenceType = SequenceType.Serial;
            this.m_delayCounter = this.m_delayModified ? this.m_storedDelayChange : ((double) this.m_maxDelayDuration <= (double) this.m_minDelayDuration ? this.m_minDelayDuration : RNG.get(827).RandomFloat(this.m_minDelayDuration, this.m_maxDelayDuration));
            if ((double) this.m_delayCounter > 0.0 && !this.ParentLogicSet.hasToFinish)
                return;
            this.ExecuteNext();
        }

        public override void Update(float elapsedSeconds)
        {
            this.m_delayCounter -= elapsedSeconds;

            this.UseMelee();
            if (this.m_delayCounter > this.m_minDelayDuration - 0.1f)
                return;

            this.HandleInput();
            // if ((double) this.m_delayCounter > 0.0 && !this.ParentLogicSet.hasToFinish)
            //     return;
            // this.ExecuteNext();
        }

        private void HandleInput()
        {
            this.UpdateRightJoystickDirection();
            // if (this.m_rightJoystickDirection == Vector2.Zero)
            // {
            //     this.m_source.CurrentSpeed = 0.0f;
            //     this.m_source.State = CharacterState.Idle;
            //     return;
            // }

            // this.m_source.LockFlip = true;
            var artilleryTarget = this.m_source.artilleryRedTarget;
            if (artilleryTarget == null || !artilleryTarget.Active)
                return;
            artilleryTarget.Position += new Vector2(this.m_rightJoystickDirection.X, this.m_rightJoystickDirection.Y) * 10;
            this.m_source.artilleryTargetPos = artilleryTarget.Position;
            // artilleryTarget.Heading = new Vector2(this.m_rightJoystickDirection.X, this.m_rightJoystickDirection.Y);

            // if (this.m_source.artilleryTarget.Heading.X != 0)
            //     this.m_source.Flip = this.m_source.Heading.X > 0
            //         ? SpriteEffects.None
            //         : SpriteEffects.FlipHorizontally;

            // this.m_source.CurrentSpeed = this.m_source.Speed;
            // this.m_source.State = CharacterState.Walking;

            // this.m_source.currentTarget = this.m_source;
        }

        public override void ExecuteNext()
        {
            // if (this.m_source is EnemyObj source && !flag)
            // source.ExecuteIdleLB();
            this.ParentLogicSet.ExecuteNext((LogicAction) this, false);
        }

        private void UpdateRightJoystickDirection()
        {
            var player = GameController.g_game.PlayerManager.getMainPlayerOrHost;
            if (player.useKeyboard)
                return;
            this.m_rightJoystickDirection = player.inputMap.GetLeftStick();
            if (this.m_rightJoystickDirection != Vector2.Zero)
                this.m_rightJoystickDirection.Normalize();
            this.m_rightJoystickDirection.Y = -this.m_rightJoystickDirection.Y;
            if (float.IsNaN(this.m_rightJoystickDirection.X))
                this.m_rightJoystickDirection.X = 0.0f;
            if (!float.IsNaN(this.m_rightJoystickDirection.Y))
                return;
            this.m_rightJoystickDirection.Y = 0.0f;
        }

        private void UseMelee()
        {
            var player = this.m_source.source.player;
            // if (player.useKeyboard)
            //     return;

            if (player.inputMap.Pressed(PlayerAction.SpellB))
                return;

            this.ExecuteNext();
        }
        //     ((EnemyObj) this.m_source).StopAllActions();
        //
        //
        //
        //     // var idleLogic = new LogicSet("IDLE");
        //     // LogicSet.Begin(idleLogic);
        //     // LogicSet.Add(new ChangePropertyAction(this.m_source, "State", (object) CharacterState.Idle));
        //     // // LogicSet.Add(new ChangePropertyAction(enemyObj, "CurrentSpeed", (object) 0));
        //     // LogicSet.Add(new PlayAnimationAction(m_source, "StandStart", "StandEnd", true));
        //     // LogicSet.Add(new DelayAction(0));
        //     // LogicSet.End();
        //     var logicBlock = new LogicBlock(RNG.seed);
        //
        //     if ((this.m_source is Enemy_Cat_Basic))
        //     {
        //         var logicSet = ModEntry.ModHelper.Reflection.GetField<LogicSet>(this.m_source, "m_lickLogic")
        //             .GetValue();
        //         logicBlock.AddLogicSet(logicSet);
        //     }
        //
        //     if ((this.m_source is Enemy_Artillerist_Basic))
        //     {
        //         var logicSet = ModEntry.ModHelper.Reflection
        //             .GetField<LogicSet>(this.m_source, "m_callArtilleryLargeLogic").GetValue();
        //         logicBlock.AddLogicSet(logicSet);
        //     }
        //
        //     ((EnemyObj) this.m_source).StopAllActions();
        //     ((EnemyObj) this.m_source).m_currentLB = logicBlock;
        //     //currentLB.GetValue().Execute((Dictionary<string, float>) null, 100);
        //     ((EnemyObj) this.m_source).m_currentLB.Execute((Dictionary<string, float>) null, 100);
        // }

        // private void UseDodge()
        // {
        //     var player = GameController.g_game.PlayerManager.getMainPlayerOrHost;
        //     if (player.useKeyboard)
        //         return;
        //
        //     if (!player.inputMap.Pressed(PlayerAction.SpellA, BrawlerMap.Locking.LOCK_USE))
        //         return;
        //
        //     // var currentLB = ModEntry.ModHelper.Reflection.GetField<LogicSet>((EnemyObj)this.m_source, "m_currentLB");
        //
        //     if (!(this.m_source is Enemy_Artillerist_Basic)) return;
        //     var logicSet = ModEntry.helper.Reflection.GetField<LogicSet>(this.m_source, "m_dodgeLogic").GetValue();
        //     var logicBlock2 = new LogicBlock(RNG.seed);
        //     logicBlock2.AddLogicSet(logicSet);
        //
        //     ((Enemy_Artillerist_Basic)this.m_source).StopAllActions();
        //     ((EnemyObj)this.m_source).m_currentLB = logicBlock2;
        //     //currentLB.GetValue().Execute((Dictionary<string, float>) null, 100);
        //     ((EnemyObj)this.m_source).m_currentLB.Execute((Dictionary<string, float>) null, 100);
        //
        //
        //     // var attack = new LogicSet("Dodge");
        //     // LogicSet.Begin(attack);
        //     // LogicSet.Add( (LogicAction) new RunLogicSetAction(logicSet.GetValue()), SequenceType.Serial);
        //     // // LogicSet.Add((LogicAction) new DelayAction(0.1f), SequenceType.Serial);
        //     // LogicSet.Add((LogicAction) new RunLogicSetAction(idleLogic.GetValue()), SequenceType.Serial);
        //     // LogicSet.End();
        //
        //     // attack.ForceExecute();
        //
        //     // this.m_source.dodge;
        //     // this.m_source.Attack();
        // }

        // public void PlayAnimation(string startLabel, string endLabel, bool loop = true, bool reverseLoop = false,
        //     bool resetAnimation = true)
        // {
        //     int startIndex = -1;
        //     int endIndex = -1;
        //     if (startLabel != string.Empty)
        //     {
        //         startIndex = this.m_source.GetFrameFromLabel(startLabel);
        //         endIndex = this.m_source.GetFrameFromLabel(endLabel);
        //         if (startIndex == -1)
        //             throw new Exception("Could not find label: " + startLabel);
        //         if (endIndex == -1)
        //             throw new Exception("Could not find label: " + endLabel);
        //     }
        //
        //     bool playAnimation = this.m_source.IsAnimating && this.m_source.startingIndex != startIndex &&
        //                          (this.m_source.endingIndex != endIndex && resetAnimation) &&
        //                          loop == this.m_source.IsLooping;
        //     if (!playAnimation) return;
        //
        //     if (startIndex == -1 && endIndex == -1)
        //         this.m_source.PlayAnimation(loop, reverseLoop);
        //     else
        //         this.m_source.PlayAnimation(startIndex, endIndex, loop, reverseLoop);
        // }

        public override object Clone()
        {
            return new MoveTargetUsingInput(this.m_source, this.delayCounter);
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
            this.m_source = null;
            // this.m_waypointHelper.Clear();
            // this.m_waypointHelper = (List<Vector2>) null;
            // this.m_waypointsVisited.Clear();
            // this.m_waypointsVisited = (List<Vector2>) null;
            base.Dispose();
        }
    }
}
