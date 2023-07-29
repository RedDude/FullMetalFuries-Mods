using System;
using System.Collections.Generic;
using Brawler2D;
using CDGEngine;
using cs.Blit;
using DevMenu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace QuickStart
{
    class MoveUsingInput : LogicAction, MNetUpdateMaster
    {
        private CharacterObj m_source;
        private EnemyObj.RangeState m_rangeState;
        private Vector2 m_endPt;
        private bool m_towardsOnly;
        private Vector2 m_rightJoystickDirection;


        private CharacterState lastState;
        public MoveUsingInput(
            CharacterObj source,
            // Vector2 xOffsets,
            // Vector2 yOffsets,
            float totalAttemptDuration,
            bool ifFailedStopLogic = false)
            : base("Move Using Input")
        {
            this.m_source = source;
            // this.m_xOffsets = xOffsets;
            // this.m_yOffsets = yOffsets;
        }

        public MoveUsingInput(
            CharacterObj source,
            EnemyObj.RangeState rangeState,
            bool ifFailedStopLogic = false,
            bool towardsOnly = false)
            : base("Move Using Input")
        {
            this.m_source = source;
            this.m_rangeState = rangeState;
            this.m_towardsOnly = towardsOnly;
        }

        protected override void Execute()
        {
            //     this.UseDodge();
            // Vector2 shadowPosition = this.m_target.shadowPosition;
            // CDGEllipse collisionEllipse = this.m_target.collisionEllipse;
            // this.m_attemptDurationCounter = this.m_attemptDuration;
            // this.m_endPt = this.FindAcceptableEndPos();
            // this.seekPosition = this.FindNextWaypoint();
        }

        public override void Update(float elapsedSeconds)
        {
            this.HandleInput();
            this.UseMelee();
            this.UseDodge();

            // if (this.m_rightJoystickDirection == Vector2.Zero)
            // {
            //     this.m_source.State = CharacterState.Idle;
            // }

            ((EnemyObj) this.m_source).displayArrowIndicator = false;

            if (this.m_source.State == CharacterState.Idle && this.lastState != CharacterState.Idle)
                this.PlayAnimation("StandStart", "StandEnd");

            if (this.m_source.State == CharacterState.Walking && this.lastState != CharacterState.Walking)
                this.PlayAnimation("RunStart", "RunEnd");

            this.lastState = this.m_source.State;
            base.Update(elapsedSeconds);
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
            this.m_rightJoystickDirection = player.inputMap.GetRightStick();
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
            var player = GameController.g_game.PlayerManager.getMainPlayerOrHost;
            if (player.useKeyboard)
                return;

            if (!player.inputMap.JustPressed(PlayerAction.Attack, BrawlerMap.Locking.LOCK_USE))
                return;

            ((EnemyObj)this.m_source).StopAllActions();



            // var idleLogic = new LogicSet("IDLE");
            // LogicSet.Begin(idleLogic);
            // LogicSet.Add(new ChangePropertyAction(this.m_source, "State", (object) CharacterState.Idle));
            // // LogicSet.Add(new ChangePropertyAction(enemyObj, "CurrentSpeed", (object) 0));
            // LogicSet.Add(new PlayAnimationAction(m_source, "StandStart", "StandEnd", true));
            // LogicSet.Add(new DelayAction(0));
            // LogicSet.End();
            var logicBlock = new LogicBlock(RNG.seed);

            if ((this.m_source is Enemy_Cat_Basic))
            {
                var logicSet = ModEntry.ModHelper.Reflection.GetField<LogicSet>(this.m_source, "m_lickLogic").GetValue();
                logicBlock.AddLogicSet(logicSet);
            }

            if ((this.m_source is Enemy_Artillerist_Basic))
            {
                var logicSet = ModEntry.ModHelper.Reflection.GetField<LogicSet>(this.m_source, "m_callArtilleryLargeLogic").GetValue();
                logicBlock.AddLogicSet(logicSet);
            }

            ((EnemyObj)this.m_source).StopAllActions();
            ((EnemyObj)this.m_source).m_currentLB = logicBlock;
            //currentLB.GetValue().Execute((Dictionary<string, float>) null, 100);
            ((EnemyObj)this.m_source).m_currentLB.Execute((Dictionary<string, float>) null, 100);
        }

        private void UseDodge()
        {
            var player = GameController.g_game.PlayerManager.getMainPlayerOrHost;
            if (player.useKeyboard)
                return;

            if (!player.inputMap.Pressed(PlayerAction.SpellA, BrawlerMap.Locking.LOCK_USE))
                return;

            // var currentLB = ModEntry.ModHelper.Reflection.GetField<LogicSet>((EnemyObj)this.m_source, "m_currentLB");

            if (!(this.m_source is Enemy_Artillerist_Basic)) return;
            var logicSet = ModEntry.ModHelper.Reflection.GetField<LogicSet>(this.m_source, "m_dodgeLogic").GetValue();
            var logicBlock2 = new LogicBlock(RNG.seed);
            logicBlock2.AddLogicSet(logicSet);

            ((Enemy_Artillerist_Basic)this.m_source).StopAllActions();
            ((EnemyObj)this.m_source).m_currentLB = logicBlock2;
            //currentLB.GetValue().Execute((Dictionary<string, float>) null, 100);
            ((EnemyObj)this.m_source).m_currentLB.Execute((Dictionary<string, float>) null, 100);


            // var attack = new LogicSet("Dodge");
            // LogicSet.Begin(attack);
            // LogicSet.Add( (LogicAction) new RunLogicSetAction(logicSet.GetValue()), SequenceType.Serial);
            // // LogicSet.Add((LogicAction) new DelayAction(0.1f), SequenceType.Serial);
            // LogicSet.Add((LogicAction) new RunLogicSetAction(idleLogic.GetValue()), SequenceType.Serial);
            // LogicSet.End();

            // attack.ForceExecute();

            // this.m_source.dodge;
            // this.m_source.Attack();
        }


        private void HandleInput()
        {
            if(this.m_source.State == CharacterState.Dodging)
                return;

            this.UpdateRightJoystickDirection();
            if (this.m_rightJoystickDirection == Vector2.Zero)
            {
                this.m_source.CurrentSpeed = 0.0f;
                this.m_source.State = CharacterState.Idle;
                return;
            }

            this.m_source.LockFlip = true;
            this.m_source.Heading = new Vector2(this.m_rightJoystickDirection.X, this.m_rightJoystickDirection.Y);
            if(this.m_source.Heading.X != 0)
                this.m_source.Flip = this.m_source.Heading.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            this.m_source.CurrentSpeed = this.m_source.Speed;
            this.m_source.State = CharacterState.Walking;

            this.m_source.currentTarget = this.m_source;
        }

        public void PlayAnimation(string startLabel, string endLabel, bool loop = true, bool reverseLoop = false, bool resetAnimation = true)
        {
            int startIndex = -1;
            int endIndex = -1;
            if (startLabel != string.Empty)
            {
                startIndex = this.m_source.GetFrameFromLabel(startLabel);
                 endIndex = this.m_source.GetFrameFromLabel(endLabel);
                if (startIndex == -1)
                    throw new Exception("Could not find label: " + startLabel);
                if (endIndex == -1)
                    throw new Exception("Could not find label: " + endLabel);
            }
            bool playAnimation = this.m_source.IsAnimating && this.m_source.startingIndex != startIndex && (this.m_source.endingIndex != endIndex && resetAnimation) && loop == this.m_source.IsLooping;
            if (!playAnimation) return;

            if (startIndex == -1 && endIndex == -1)
                this.m_source.PlayAnimation(loop, reverseLoop);
            else
                this.m_source.PlayAnimation(startIndex, endIndex, loop, reverseLoop);
        }

        public override object Clone()
        {
            return new MoveUsingInput(this.m_source, this.m_rangeState,
                this.m_towardsOnly);
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
            this.m_source = (CharacterObj) null;
            // this.m_waypointHelper.Clear();
            // this.m_waypointHelper = (List<Vector2>) null;
            // this.m_waypointsVisited.Clear();
            // this.m_waypointsVisited = (List<Vector2>) null;
            base.Dispose();
        }
    }
}
