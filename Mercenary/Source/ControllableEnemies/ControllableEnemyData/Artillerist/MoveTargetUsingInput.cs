using cs.Blit;
using Brawler2D;
using CDGEngine;
using FullModdedFuriesAPI.Mods.MercenaryClass.Source.ControllableEnemies.ControllableEnemyData;
using Microsoft.Xna.Framework;

namespace FullModdedFuriesAPI.Mods.MercenaryClass.Source
{
    class ArtilleristControllableMoveTargetUsingInput : LogicAction, MNetUpdateMaster
    {
        private DisplayObj m_source;
        private readonly PlayerObj m_player;
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

        public ArtilleristControllableMoveTargetUsingInput(DisplayObj source, PlayerObj player, float delayInSecs)
            : base("Move Target Using Input")
        {
            this.m_source = source;
            this.m_player = player;
            this.m_minDelayDuration = delayInSecs;
            this.m_delayCounter = 0.0f;
            this.m_delayModified = false;
        }

        // public ArtilleristControllableMoveTargetUsingInput(Artillerist_ControllableData source, float minDelayInSecs, float maxDelayInSecs)
        //     : base("Move Target Using Input")
        // {
        //     this.m_source = source;
        //     this.m_minDelayDuration = minDelayInSecs;
        //     this.m_maxDelayDuration = maxDelayInSecs;
        //     this.m_delayModified = false;
        // }
        //
        // public ArtilleristControllableMoveTargetUsingInput(Artillerist_ControllableData source, Vector2 randSecs)
        //     : base("Move Target Using Input")
        // {
        //     this.m_source = source;
        //     this.m_minDelayDuration = randSecs.X;
        //     this.m_maxDelayDuration = randSecs.Y;
        //     this.m_delayModified = false;
        // }
        //

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
            var artilleryTarget = this.m_source;
            if (artilleryTarget == null || !artilleryTarget.Active)
                return;
            artilleryTarget.Position += new Vector2(this.m_rightJoystickDirection.X, this.m_rightJoystickDirection.Y) * 10;
            this.m_source.Position = artilleryTarget.Position;
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
            var player = this.m_player;
            // if (player.useKeyboard)
            //     return;

            if (player.inputMap.Pressed(PlayerAction.SpellB))
                return;

            this.ExecuteNext();
        }

        public override object Clone()
        {
            return new ArtilleristControllableMoveTargetUsingInput(this.m_source, this.m_player, this.delayCounter);
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
