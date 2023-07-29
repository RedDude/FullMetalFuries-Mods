using System;
using System.Collections.Generic;
using Brawler2D;
using CDGEngine;
using cs.Blit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FullModdedFuriesAPI.Mods.MercenaryClass.Source.ControllableEnemies
{
    class EnemyControllableLogicSet : LogicAction, MNetUpdateMaster
    {
        private CharacterObj m_source;
        private readonly PlayerObj m_player;
        private readonly List<EnemyObj> m_activeControlledEnemies;
        private EnemyObj.RangeState m_rangeState;
        private Vector2 m_endPt;
        private bool m_towardsOnly;
        private Vector2 m_joystickDirection;

        public bool useLeftStick = true;

        private CharacterState lastState;

        public EnemyControllableLogicSet(
            CharacterObj source,
            PlayerObj player,
            List<EnemyObj> activeControlledEnemies,
            float totalAttemptDuration,
            bool ifFailedStopLogic = false)
            : base("EnemyControllable")
        {
            this.m_source = source;
            this.m_player = player;
            this.m_activeControlledEnemies = activeControlledEnemies;
        }

        public EnemyControllableLogicSet(
            CharacterObj source,
            PlayerObj player,
            List<EnemyObj> activeControlledEnemies,
            EnemyObj.RangeState rangeState,
            bool ifFailedStopLogic = false,
            bool towardsOnly = false)
            : base("Move Using Input")
        {
            this.m_source = source;
            this.m_player = player;
            this.m_activeControlledEnemies = activeControlledEnemies;
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
            var enemyObj = (EnemyObj) this.m_source;
            if (!EnemyControllableManager._activeControlledEnemies.Contains(enemyObj))
                this.ExecuteNext();

            enemyObj.hudOffset = new Vector2(enemyObj.AbsBounds.Height, enemyObj.hudOffset.X);

            this.HandleInput();
            this.UseMelee();
            this.UseDodge();
            this.UseSkillB();
            this.UseSkillR();

            enemyObj.displayArrowIndicator = false;

            if (this.m_source.State == CharacterState.Idle && this.lastState != CharacterState.Idle)
            {
                this.m_source.PlayAnimation("StandStart", "StandEnd", true);
            }


            if (this.m_source.State == CharacterState.Walking && this.lastState != CharacterState.Walking)
            {
                this.m_source.PlayAnimation("RunStart", "RunEnd", true);
            }


            this.lastState = this.m_source.State;
            base.Update(elapsedSeconds);
        }

        public override void ExecuteNext()
        {
            // if (this.m_source is EnemyObj source && !flag)
            // source.ExecuteIdleLB();
            this.ParentLogicSet.ExecuteNext(this);
        }

        private void UpdateJoystickDirection()
        {
            this.m_joystickDirection = this.useLeftStick ? this.m_player.inputMap.GetLeftStick() : this.m_player.inputMap.GetRightStick();
            if (this.m_joystickDirection != Vector2.Zero)
                this.m_joystickDirection.Normalize();
            this.m_joystickDirection.Y = -this.m_joystickDirection.Y;
            if (float.IsNaN(this.m_joystickDirection.X))
                this.m_joystickDirection.X = 0.0f;
            if (!float.IsNaN(this.m_joystickDirection.Y))
                return;
            this.m_joystickDirection.Y = 0.0f;
        }

        private void UseMelee()
        {
            var player = this.m_player; //GameController.g_game.PlayerManager.getMainPlayerOrHost;
            // if (player.useKeyboard)
            //     return;

            if (!player.inputMap.JustPressed(PlayerAction.Attack))
                return;

            float actionAngle = this.GetActionAngle();

            ((EnemyObj) this.m_source).StopAllActions();


            // var idleLogic = new LogicSet("IDLE");
            // LogicSet.Begin(idleLogic);
            // LogicSet.Add(new ChangePropertyAction(this.m_source, "State", (object) CharacterState.Idle));
            // // LogicSet.Add(new ChangePropertyAction(enemyObj, "CurrentSpeed", (object) 0));
            // LogicSet.Add(new PlayAnimationAction(m_source, "StandStart", "StandEnd", true));
            // LogicSet.Add(new DelayAction(0));
            // LogicSet.End();
            var logicBlock = new LogicBlock(RNG.seed);

            EnemyControllableManager.ControllableData.TryGetValue(this.m_source.GetType(), out var enemyControllable);
            if (enemyControllable != null)
            {
                enemyControllable.Attack(this.m_source, this.m_player);
                return;
            }

            if ((this.m_source is Enemy_Cat_Basic))
            {
                var logicSet = ModEntry.modHelper.Reflection.GetField<LogicSet>(this.m_source, "m_lickLogic")
                    .GetValue();
                logicBlock.AddLogicSet(logicSet);
            }

            if ((this.m_source is Enemy_Artillerist_Basic))
            {
                var logicSet = ModEntry.modHelper.Reflection
                    .GetField<LogicSet>(this.m_source, "m_callArtilleryLargeLogic").GetValue();
                logicBlock.AddLogicSet(logicSet);
            }

            ((EnemyObj) this.m_source).StopAllActions();
            ((EnemyObj) this.m_source).m_currentLB = logicBlock;
            //currentLB.GetValue().Execute((Dictionary<string, float>) null, 100);
            ((EnemyObj) this.m_source).m_currentLB.Execute(null, 100);
        }

        private float GetActionAngle()
        {
            var normalized = this.m_player.currentPlayerClass.storedLeftStickDirection;
            normalized.Normalize();
            var actionDirection = normalized;
            if (actionDirection == Vector2.Zero)
                actionDirection.X = this.m_source.Flip != SpriteEffects.None ? -1f : 1f;
            float angle = CDGMath.VectorToAngle(normalized);
            double actionAngle = Math.Atan(byte.MaxValue * Math.Tan(angle * Math.PI / 180.0) /
                                           (byte.MaxValue * 1.0));
            if (angle > 90.0)
                actionAngle += Math.PI;
            else if (angle < -90.0)
                actionAngle -= Math.PI;
            return (float)actionAngle;
        }

        private void UseDodge()
        {
            var player = this.m_player;
            // if (player.useKeyboard)
            //     return;

            if (!player.inputMap.Pressed(PlayerAction.SpellA))
                return;

            float actionAngle = this.GetActionAngle();

            // var currentLB = ModEntry.ModHelper.Reflection.GetField<LogicSet>((EnemyObj)this.m_source, "m_currentLB");

            EnemyControllableManager.ControllableData.TryGetValue(this.m_source.GetType(), out var enemyControllable);
            if (enemyControllable != null)
            {
                enemyControllable.CastSpellA(this.m_source, this.m_player);
                return;
            }

            var logicBlock = new LogicBlock(RNG.seed);

            if ((this.m_source is Enemy_Artillerist_Basic))
            {
                var logicSet = ModEntry.modHelper.Reflection.GetField<LogicSet>(this.m_source, "m_dodgeLogic").GetValue();
                logicBlock.AddLogicSet(logicSet);
            }


            var enemyObj = ((EnemyObj) this.m_source);
            enemyObj.StopAllActions();
            enemyObj.m_currentLB = logicBlock;
            //currentLB.GetValue().Execute((Dictionary<string, float>) null, 100);
            enemyObj.m_currentLB.Execute(null, 100);


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

        private void UseSkillB()
        {
            var player = this.m_player;
            // if (player.useKeyboard)
            //     return;

            if (!player.inputMap.Pressed(PlayerAction.SpellB))
                return;

            EnemyControllableManager.ControllableData.TryGetValue(this.m_source.GetType(), out var enemyControllable);
            enemyControllable?.CastSpellB(this.m_source, this.m_player);
        }

        private void UseSkillR()
        {
            var player = this.m_player;
            // if (player.useKeyboard)
            //     return;

            if (!player.inputMap.Pressed(PlayerAction.SpecialR))
                return;

            EnemyControllableManager.ControllableData.TryGetValue(this.m_source.GetType(), out var enemyControllable);
            enemyControllable?.CastSpellR(this.m_source, this.m_player);
        }

        private static void SetFaceDirectionAngle(LogicSet logicSet, float actionAngle)
        {
            var node = logicSet.firstNode.NextLogicAction;
            while (node.n_next)
            {
                if (node is not FaceDirectionAction faceDirectionAction) continue;
                ModEntry.modHelper.Reflection
                    .GetField<float>(faceDirectionAction, "m_angle").SetValue(actionAngle);
                break;
            }
        }


        private void HandleInput()
        {
            if (this.m_source.State == CharacterState.Dodging)
                return;

            this.UpdateJoystickDirection();
            if (this.m_joystickDirection == Vector2.Zero)
            {
                this.m_source.CurrentSpeed = 0.0f;
                this.m_source.State = CharacterState.Idle;
                return;
            }

            this.m_source.LockFlip = true;
            this.m_source.Heading = new Vector2(this.m_joystickDirection.X, this.m_joystickDirection.Y);
            if (this.m_source.Heading.X != 0)
                this.m_source.Flip = this.m_source.Heading.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            this.m_source.CurrentSpeed = this.m_source.Speed;
            this.m_source.State = CharacterState.Walking;

            this.m_source.currentTarget = this.m_source;
        }

        public void PlayAnimation(string startLabel, string endLabel, bool loop = true, bool reverseLoop = false,
            bool resetAnimation = true)
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

            bool playAnimation = this.m_source.IsAnimating && this.m_source.startingIndex != startIndex &&
                                 (this.m_source.endingIndex != endIndex && resetAnimation) &&
                                 loop == this.m_source.IsLooping;
            if (!playAnimation) return;

            if (startIndex == -1 && endIndex == -1)
                this.m_source.PlayAnimation(loop, reverseLoop);
            else
                this.m_source.PlayAnimation(startIndex, endIndex, loop, reverseLoop);
        }

        public override object Clone()
        {
            return new EnemyControllableLogicSet(this.m_source, this.m_player, this.m_activeControlledEnemies, this.m_rangeState,
                this.m_towardsOnly);
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
