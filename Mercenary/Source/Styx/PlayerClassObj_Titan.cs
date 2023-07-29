using System;
using System.Collections.Generic;
using Brawler2D;
using CDGEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tweener;
using Tweener.Ease;

namespace FullModdedFuriesAPI.Mods.TitanClass.Source
{
    public class PlayerClassObj_Titan : PlayerClassObj
    {
        public static ClassType TitanClassType = (ClassType) 0;

        private FighterSpinAttack m_spinAttack;
        private bool m_spinAttacking;
        private bool m_isCounterDefending;
        private bool m_isCounterAttacking;
        private float m_counterAttackDistanceCounter;
        public FighterCounterAttack m_counterAttack;
        private FighterCounterDefend m_counterDefend;
        private bool m_counterAttackComplete;
        private float m_counterCDCounter;
        private FighterRiskCounter m_riskCounterAttack;
        private FighterJumpAttack m_jumpAttack;
        private FighterPullLeapAttack m_pullJumpAttack;
        private bool m_isJumpAttacking;
        private List<BaseAttack> m_heavyAttackList;
        private List<BaseAttack> m_flameAttackList;
        private bool m_hitEnemyWithLastAttack;
        private float m_chargeSpinCount;
        private bool m_startChargeSpinCount;

        public static Color
            Titan_COLOUR = new Color(255, 111, 142); //new Color(147, 76, 116);//new Color(168, 67, 71); //;


        private IModHelper helper;
        private readonly SkillType Tec_Titan_Index;
        private readonly SkillType Atk_Titan_Index;
        private readonly SkillType Hp_Titan_Index;

        public void CounterAttack()
        {
            Net.Master.Cast((byte) this.player.playerIndex, PlayerActions.CounterAttack);
            this.StopAllActions();
            this.m_counterCDCounter = 1f;
            var vector2 = new Vector2(this.leftJoystickDirection.X, this.leftJoystickDirection.Y);
            if (vector2 == Vector2.Zero)
                vector2.X = this.Flip != SpriteEffects.None ? -1f : 1f;
            this.Heading = vector2;
            this.m_counterAttackDistanceCounter = 0.0f;
            if ((double) this.HeadingX < 0.0)
                this.Flip = SpriteEffects.FlipHorizontally;
            else
                this.Flip = SpriteEffects.None;
            if (this.HasEquippedEquipment(EquipmentType.FTR_RiskCounter))
                this.m_currentAttack = (BaseAttack) this.m_counterDefend;
            else
                this.m_currentAttack = (BaseAttack) this.m_counterDefend;
            using (new NetScope<Forced>())
                this.m_currentAttack.Execute();
        }

        public void BSpinAttack()
        {
            Net.Master.Cast((byte) this.player.playerIndex, PlayerActions.BSpinAttack);
            this.StopAllActions();
            this.isSpinAttacking = true;
            this.m_currentAttack = (BaseAttack) this.m_spinAttack;
            using (new NetScope<Forced>())
                this.m_currentAttack.Execute();
        }

        public void BNoSpinAttack()
        {
            Net.Master.Cast((byte) this.player.playerIndex, PlayerActions.BNoSpinAttack);
            this.StopAllActions();
            this.m_currentAttack = (BaseAttack) this.m_spinAttack;
            using (new NetScope<Forced>())
                this.m_currentAttack.Execute();
        }

        public void AJumpAttack()
        {
            Net.Master.Cast((byte) this.player.playerIndex, PlayerActions.AJumpAttack);
            this.StopAllActions();
            // if (this.player.HasEquippedEquipment(ClassType.Fighter, EquipmentType.FTR_MiniLeap))
            // this.m_currentAttack = (BaseAttack) this.m_dummyAttack;
            // else if (this.player.HasEquippedEquipment(ClassType.Fighter, EquipmentType.FTR_PullLeap))
            // {
            //   this.m_currentAttack = (BaseAttack) this.m_pullJumpAttack;
            //   this.SetSpellACounter(3.375f);
            // }
            // else
            //   this.m_currentAttack = (BaseAttack) this.m_jumpAttack;
            using (new NetScope<Forced>())
                this.m_currentAttack.Execute();
            var arenaScreen = this.Game.ScreenManager.arenaScreen;
            if (arenaScreen == null || !arenaScreen.currentZone.isArena)
                return;
            this.player.SetCurrentEquipSlotMasteryPoints(EquipmentSlotType.ButtonA, 1, true);
        }

        public void FighterAttack()
        {
            Net.Master.Cast((byte) this.player.playerIndex, PlayerActions.FighterAttack);
            if (this.targetSpriteDistance <= 0)
                return;
            this.State = CharacterState.Idle;
            this.targetSpriteDistance = 0;
        }

        public float chargeSpinCount
        {
            get { return this.m_chargeSpinCount; }
        }

        public override float AnimationSpeed
        {
            get => this.playingInstrument ? PlayerEV.FIGHTER_INSTRUMENT_ANIM_SPEED : base.AnimationSpeed;
            set => base.AnimationSpeed = value;
        }

        public override float CurrentSpeed
        {
            get => this.playingInstrument ? PlayerEV.PLAYER_INSTRUMENT_MOVE_SPEED : base.CurrentSpeed;
            set => base.CurrentSpeed = value;
        }

        public override bool canHitAir
        {
            get => this.isSpinAttacking || base.canHitAir;
            set => base.canHitAir = value;
        }

        protected override List<BaseAttack> attackList
        {
            get
            {
                if (this.HasEquippedEquipment(EquipmentType.FTR_HeavyAttack))
                    return this.m_heavyAttackList;
                return this.HasEquippedEquipment(EquipmentType.FTR_FlameAttack)
                    ? this.m_flameAttackList
                    : base.attackList;
            }
        }

        public bool hitEnemyWithLastAttack
        {
            get => this.m_hitEnemyWithLastAttack;
            set => this.m_hitEnemyWithLastAttack = value;
        }

        public bool isLastAttack => this.attackList.Count - 1 == (int) this.NextComboAttackNum;

        public override float IntelligenceGlobalMods
        {
            get
            {
                float num = 1f;
                if (this.isSpinAttacking)
                {
                    if (this.HasEquippedEquipment(EquipmentType.FTR_FastSpin))
                        num = 0.75f;
                    if (this.HasEquippedEquipment(EquipmentType.FTR_PullSpin))
                        num = 1f;
                    if (this.HasEquippedEquipment(EquipmentType.FTR_ChargeSpin))
                        num = 1.25f;
                }

                return base.IntelligenceGlobalMods * num;
            }
        }

        public override float IntelligenceFlatMods =>
            base.IntelligenceFlatMods + (int) this.player.GetSkillAmount(this.Tec_Titan_Index);

        public override float StrengthFlatMods =>
            base.StrengthFlatMods + (int) this.player.GetSkillAmount(this.Atk_Titan_Index);

        public override float MaxHealthFlatMods => base.MaxHealthFlatMods + this.player.GetSkillAmount(this.Hp_Titan_Index);

        public override int stunStr
        {
            get
            {
                float num = 1f;
                if (this.isSpinAttacking && this.HasEquippedEquipment(EquipmentType.FTR_FastSpin))
                    num = 0.0f;
                return (int) ((double) base.stunStr * (double) num);
            }
        }

        public bool isJumpAttacking
        {
            get => this.m_isJumpAttacking;
            set => this.m_isJumpAttacking = value;
        }

        public bool counterAttackComplete
        {
            get => this.m_counterAttackComplete;
            set => this.m_counterAttackComplete = value;
        }

        public bool isCounterAttacking
        {
            get => this.m_isCounterAttacking;
            set => this.m_isCounterAttacking = value;
        }

        public bool isCounterDefending
        {
            get => this.m_isCounterDefending;
            set => this.m_isCounterDefending = value;
        }

        public bool isSpinAttacking
        {
            get => this.m_spinAttacking;
            set
            {
                if (value != this.m_spinAttacking)
                    this.useCachedTec = false;
                this.m_spinAttacking = value;
            }
        }

        public override float Speed
        {
            get
            {
                float num = 1f;
                if (this.isSpinAttacking)
                {
                    num *= 0.95f;
                    if (this.HasEquippedEquipment(EquipmentType.FTR_FastSpin))
                        num *= 2f;
                    if (this.HasEquippedEquipment(EquipmentType.FTR_PullSpin))
                        num *= 0.425f;
                    if (this.HasEquippedEquipment(EquipmentType.FTR_ChargeSpin))
                        num *= 0.65f;
                }
                else if (this.isCounterAttacking)
                    num *= 4.25f;

                return base.Speed * num;
            }
            set => base.Speed = value;
        }

        public PlayerClassObj_Titan(GameController game, PlayerObj player, IModHelper helper)
            : base(game, player, "PlayerTitan_Character")
        {
            this.helper = helper;

            this.Name = "Player_Titan";
            this.m_objType = 5;
            this.m_classType = (ClassType) this.helper.Database.GetCustomClasses()["Titan"];
            TitanClassType = this.m_classType;
            this.m_heavyAttackList = new List<BaseAttack>();
            this.m_flameAttackList = new List<BaseAttack>();
            //this.AnimationSpeed = 1f;

            this.Tec_Titan_Index = (SkillType) this.helper.Database.GetCustomSkillIndex("TEC_Titan");
            this.Atk_Titan_Index = (SkillType) this.helper.Database.GetCustomSkillIndex("ATK_Titan");
            this.Hp_Titan_Index = (SkillType) this.helper.Database.GetCustomSkillIndex("HP_Titan");
        }

        public override IEnumerable<object> LoadContent()
        {
            this.m_walkFrameSound = new FrameSound((DisplayObj) this, new int[2]
            {
                17,
                21
            }, "event:/SFX/Characters/Generic/Movement/sfx_char_gen_move_run", false);
            this.m_waterFrameSound = new FrameSound((DisplayObj) this, new int[2]
            {
                17,
                21
            }, "event:/SFX/Characters/Generic/Movement/sfx_char_gen_move_run_wet", false);
            this.m_waterFrameFunction = new FrameFunction((DisplayObj) this, new int[2]
            {
                17,
                21
            }, (object) this, "RunWaterRippleEffect", new object[0]);
            this.m_instrument1Name =
                "event:/Music/interactive instruments/mus_interactiveinstrument_guitar/mus_interactiveinstrument_guitar_a";
            this.m_instrument2Name =
                "event:/Music/interactive instruments/mus_interactiveinstrument_guitar/mus_interactiveinstrument_guitar_b";
            this.m_instrument3Name =
                "event:/Music/interactive instruments/mus_interactiveinstrument_guitar/mus_interactiveinstrument_guitar_csharp";
            this.m_instrument4Name =
                "event:/Music/interactive instruments/mus_interactiveinstrument_guitar/mus_interactiveinstrument_guitar_d";
            this.m_instrument5Name =
                "event:/Music/interactive instruments/mus_interactiveinstrument_guitar/mus_interactiveinstrument_guitar_e";
            this.m_instrument6Name =
                "event:/Music/interactive instruments/mus_interactiveinstrument_guitar/mus_interactiveinstrument_guitar_fsharp";
            this.m_instrument7Name =
                "event:/Music/interactive instruments/mus_interactiveinstrument_guitar/mus_interactiveinstrument_guitar_fsharphigh";
            this.m_instrument8Name =
                "event:/Music/interactive instruments/mus_interactiveinstrument_guitar/mus_interactiveinstrument_guitar_gsharp";

            foreach (var obj in base.LoadContent())
                yield return null;
        }

        public override void Initialize()
        {
            this.classColour = Titan_COLOUR;
            base.Initialize();
            this.shadowYOffset = 32f;
            this.targetSpriteEllipseRatio = PlayerEV.FIGHTER_LEAP_ELLIPSE_RATIO;
        }

        protected override void InitializeAttackLogic()
        {
            this.m_attackList.Clear();
            this.m_flameAttackList.Clear();
            int num = 0;
            foreach (string attackString in this.ClassData.attackStringList)
            {
                string soundName = "event:/SFX/Characters/Fighter/Hammer/sfx_char_ftr_x_attack_1";
                switch (num)
                {
                    case 0:
                    case 1:
                        soundName = "event:/SFX/Characters/Fighter/Hammer/sfx_char_ftr_x_attack_1";
                        break;
                    case 2:
                        soundName = "event:/SFX/Characters/Fighter/Hammer/sfx_char_ftr_x_attack_2";
                        break;
                    case 3:
                        soundName = "event:/SFX/Characters/Fighter/Hammer/sfx_char_ftr_x_attack_3";
                        break;
                    case 4:
                        soundName = "event:/SFX/Characters/Fighter/Hammer/sfx_char_ftr_x_attack_4";
                        break;
                }

                var fighterMeleeAttack =
                    new FighterMeleeAttack((PlayerClassObj) this, attackString, soundName);
                fighterMeleeAttack.InitializeLogic();
                this.m_attackList.Add((BaseAttack) fighterMeleeAttack);
                ++num;
                if (num < this.ClassData.attackStringList.Count)
                    this.m_flameAttackList.Add((BaseAttack) fighterMeleeAttack);
            }

            var flameMeleeAttack = new FighterFlameMeleeAttack((PlayerClassObj) this,
                "event:/SFX/Characters/Fighter/Hammer/sfx_char_ftr_x_attack_4_molten");
            flameMeleeAttack.InitializeLogic();
            this.m_flameAttackList.Add((BaseAttack) flameMeleeAttack);
            var fighterMeleeAttack1 = new FighterMeleeAttack((PlayerClassObj) this, "Hammer_Attack5",
                "event:/SFX/Characters/Fighter/Hammer/sfx_char_ftr_x_attack_4");
            fighterMeleeAttack1.InitializeLogic();
            this.m_heavyAttackList.Add((BaseAttack) fighterMeleeAttack1);
            this.currentAttack = this.attackList[0];
            this.m_spinAttack = new FighterSpinAttack((PlayerClassObj) this, "Hammer_Spin", PlayerAction.SpellB);
            this.m_spinAttack.InitializeLogic();
            this.m_counterAttack = new FighterCounterAttack((PlayerClassObj) this, "Hammer_Counter");
            this.m_counterAttack.InitializeLogic();
            this.m_counterDefend = new FighterCounterDefend((PlayerClassObj) this, "Hammer_Counter");
            this.m_counterDefend.InitializeLogic();
            this.m_riskCounterAttack = new FighterRiskCounter((PlayerClassObj) this, "Hammer_Counter");
            this.m_riskCounterAttack.InitializeLogic();
            this.m_jumpAttack = new FighterJumpAttack((PlayerClassObj) this, "Hammer_Jump", PlayerAction.SpellA);
            this.m_jumpAttack.InitializeLogic();
            //
            // this.m_dummyAttack = new TitanDummyLeap((PlayerClassObj) this, "Hammer_Jump", PlayerAction.SpellA);
            // this.m_dummyAttack.InitializeLogic();
            //
            // this.m_artilleryAttack = new TitanArtilleryAttack(this,
            //     "Hammer_Jump", // "Titan_Artillery",
            //     PlayerAction.SpellY);
            // this.m_artilleryAttack.InitializeLogic();

            this.m_pullJumpAttack =
                new FighterPullLeapAttack((PlayerClassObj) this, "Hammer_Jump", PlayerAction.SpellA);
            this.m_pullJumpAttack.InitializeLogic();

            this.InitializeLogic();
        }

        public override void Attack()
        {
            return;
            // if ((double) this.player.GetSkillAmount(SkillType.Fight_Skill_X) < 1.0 && !GlobalEV.UNLOCK_ALL_SKILLS)
            //   return;
            // // this.FighterAttack();
            // target = getTarget(TargetType.RandomEnemyObj);
            // m_callArtilleryLargeLogic.Execute();
            //base.Attack();
        }

        public override void CastSpellY()
        {
            // return;
            if ((double) this.player.GetSkillAmount(SkillType.Fight_Skill_Y) < 1.0 && !GlobalEV.UNLOCK_ALL_SKILLS)
                return;

            if ((double) this.spellBCDCounter > 0.0 && !this.isSpinAttacking)
                this.RunCDAnimation(true);

            this.target = this.getTarget(TargetType.RandomEnemyObj);
            // this.m_artilleryAttack.Execute();

            // using (new NetScope<Forced>())
                // this.m_currentAttack.Execute();
            this.player.SetCurrentEquipSlotMasteryPoints(EquipmentSlotType.ButtonY, 1, true);

            // this.CounterAttack();
        }

        public override void CastSpellB()
        {
            return;
            if ((double) this.player.GetSkillAmount(SkillType.Fight_Skill_B) < 1.0 && !GlobalEV.UNLOCK_ALL_SKILLS)
                return;
            if ((double) this.spellBCDCounter > 0.0 && !this.isSpinAttacking)
                this.RunCDAnimation(false);
            else if (!this.isSpinAttacking)
                this.BNoSpinAttack();
            else
                this.BSpinAttack();
        }

        public override void CastSpellA()
        {
            if ((double) this.player.GetSkillAmount(SkillType.Fight_Skill_A) < 1.0 && !GlobalEV.UNLOCK_ALL_SKILLS)
                return;
            if ((double) this.spellACDCounter > 0.0)
                this.RunCDAnimation(true);
            else
                this.AJumpAttack();
        }

        public override void CastSpecialR()
        {
        }

        public void StartSpellBCounter()
        {
            float amount = 17.5f;
            if (this.HasEquippedEquipment(EquipmentType.FTR_PullSpin))
                amount *= 1.2f;
            if (this.HasEquippedEquipment(EquipmentType.FTR_FastSpin))
                amount *= 1.4f;
            if (this.HasEquippedEquipment(EquipmentType.FTR_ChargeSpin))
                amount *= 1f;
            this.SetSpellBCounter(amount);
        }

        public override void HandleInput()
        {
            if (!this.player.IsLocal())
            {
                if (!(this.leftJoystickDirection != Vector2.Zero))
                    return;
                this.m_storedLeftStickDirection = this.leftJoystickDirection;
                return;
            }

            base.HandleInput();
            if (this.inAir || this.hasStatusEffect(StatusEffect.Stop) ||
                (this.hasStatusEffect(StatusEffect.Dizzy) || this.State == CharacterState.Stunned) ||
                (this.State == CharacterState.KnockedDown || this.State != CharacterState.Attacking ||
                 (this.m_currentAttack != this.m_spinAttack || !this.isSpinAttacking)))
                return;
            this.HandleMovement();
            this.UpdateFlipInput();
            if (this.Heading == Vector2.Zero)
            {
                this.HeadingX = -1f;
                if (this.Flip == SpriteEffects.None)
                    this.HeadingX = 1f;
            }

            this.CurrentSpeed = this.Speed * 0.95f;
            this.State = CharacterState.Attacking;
        }

        public void AddToSpinDuration(float amount)
        {
            this.m_spinAttack.AddToSpinDuration(amount);
        }

        public void StartChargeSpinCount()
        {
            this.m_startChargeSpinCount = true;
            this.m_chargeSpinCount = 0.0f;
        }

        public void StopChargeSpinCount()
        {
            this.m_startChargeSpinCount = false;
        }

        public override void Update(float elapsedSeconds)
        {
            if (this.hasStatusEffect(StatusEffect.Stop) || this.hasStatusEffect(StatusEffect.Dizzy))
                return;

            switch (this.State)
            {
                case CharacterState.KnockedDown:
                case CharacterState.Stunned:
                    // this.m_artilleryAttack.DestroySpeechBubble();
                    break;
            }

            if (this.m_startChargeSpinCount)
                this.m_chargeSpinCount += elapsedSeconds;
            if (this.m_counterCDCounter > 0.0)
                this.m_counterCDCounter -= elapsedSeconds;
            if (this.isCounterAttacking)
                this.m_currentActiveLS.Update(elapsedSeconds);
            if (this.isJumpAttacking)
                this.UpdateJumpAttack(elapsedSeconds);

            // this.m_artilleryAttack.Update(elapsedSeconds);

            if (!this.isCounterAttacking)
            {
                base.Update(elapsedSeconds);
                return;
            }

            if (!this.counterAttackComplete)
                this.UpdateCounterAttack(elapsedSeconds);
            this.ConstrainToCamera(elapsedSeconds);
            this.UpdateAbilityCDCounters(elapsedSeconds);
            this.CalculateEnemyEllipsisCollision();
            this.m_hasteUpdateCounter -= elapsedSeconds;
            if ((double) this.m_hasteUpdateCounter > 0.0)
                return;
            this.UpdateHasteShadows(elapsedSeconds);
            this.m_hasteUpdateCounter = 0.1f;
        }

        public override void UpdateFlipInput()
        {
            var flip = this.Flip;
            base.UpdateFlipInput();
            if (!this.enableMouseAiming || this.Flip == flip)
                return;
            GameController.soundManager.PlayEvent("event:/SFX/Characters/Fighter/Dunk/sfx_char_ftr_a_prep_switch",
                (IPositionalObj) this, false, false);
        }

        protected override void UpdateAnimations(float elapsedSeconds)
        {
            if (!this.isJumpAttacking)
            {
                base.UpdateAnimations(elapsedSeconds);
                return;
            }

            if (this.HasEquippedEquipment(EquipmentType.FTR_MiniLeap))
            {
                this.PlayAnimation("JumpFallingStart", "JumpFallingEnd", false, false);
                return;
            }

            if (this.fakeAccelerationY < 0.0)
            {
                this.PlayAnimation("JumpRisingStart", "JumpRisingEnd", false, false);
                return;
            }

            if (this.fakeAccelerationY <= 0.0)
                return;
            if (this.startingIndex == this.GetFrameFromLabel("JumpRisingStart"))
                GameController.soundManager.PlayEvent(
                    "event:/SFX/Characters/Fighter/Dunk/sfx_char_ftr_a_leap_down", (IPositionalObj) this, false,
                    false);
            this.PlayAnimation("JumpFallingStart", "JumpFallingEnd", false, false);
        }

        public void UpdateJumpAttack(float elapsedSeconds)
        {
            if (this.inAir)
                return;
            this.fakeAccelerationY = 0.0f;
            this.CurrentSpeed = 0.0f;
            this.jumpAnchorOffset = Vector2.Zero;
            this.isJumpAttacking = false;
            var arenaScreen = this.Game.ScreenManager.arenaScreen;

            if (this.HasEquippedEquipment(EquipmentType.FTR_MiniLeap))
            {
                this.SetSpellACounter(0.9f);
                if (arenaScreen?.currentGlobalZone != null && arenaScreen.currentGlobalZone.groundIsWater)
                {
                    this.RunWaterRippleEffect();
                    GameController.soundManager.PlayEvent(
                        "event:/SFX/Characters/Generic/Movement/sfx_char_gen_land_wet", (IPositionalObj) this, false,
                        false);
                    return;
                }

                GameController.soundManager.PlayEvent("event:/SFX/Characters/Generic/Movement/sfx_char_gen_land",
                    (IPositionalObj) this, false, false);
            }

            this.PlayAnimation("Spell1AttackStart", "Spell1AttackStart", false, false);
            float num1 = 1f + this.player.GetSkillAmount(SkillType.Fight_Leap_Radius);
            float num2 = 1f;
            float num3 = 1f;
            float num4 = 1f;
            if (this.HasEquippedEquipment(EquipmentType.FTR_AirLeap))
            {
                num1 *= 3f;
                num2 *= 1.5f;
                num3 *= 1f;
                num4 *= 0.65f;
            }

            var projData = new ProjectileData
            {
                Scale = new Vector2(PlayerEV.FIGHTER_JUMPATTACK_RADIUS.X * num1,
                    (float) ((double) PlayerEV.FIGHTER_JUMPATTACK_RADIUS.Y * (double) num1 / 2.65000009536743)),
                SpriteName = "Projectile_GiantBrute_ShockwaveLarge",
                Lifespan = 9999.9f,
                Damage = this.damage * num2,
                aggroRaise = this.aggroRaise,
                stunStr = this.stunStr,
                knockdownStr = this.knockdownStr
            };
            if (this.HasEquippedEquipment(EquipmentType.FTR_AirLeap))
            {
                projData.stunStr = 11;
                projData.knockdownStr = 9;
            }

            projData.destroyOnAnimationEnd = true;
            var projectileObj1 =
                this.Game.ProjectileManager.FireProjectile(projData, (CharacterObj) this, false, (GameObj) null);
            projectileObj1.Layer = 0.0f;
            projectileObj1.lockLayer = true;
            projectileObj1.Flip = SpriteEffects.None;
            projectileObj1.Position = new Vector2(this.X, this.AbsBounds.Bottom - 5f);
            projectileObj1.PlayAnimation(false, false);
            projectileObj1.knockdownDistanceMod = -0.25f * num3;
            projectileObj1.knockdownSpeedMod = 0.2f * num4;
            projectileObj1.CollisionID = 1; //Mod.Instance.WhenFriendlyChangeCollisionId(2, player.playerIndex);
            projectileObj1.AnimationSpeed = 0.05f;
            projectileObj1.canHitDowned = true;
            projectileObj1.applyTransparency = true;

            if (arenaScreen?.currentGlobalZone != null && arenaScreen.currentGlobalZone.groundIsWater)
                this.HandleWaterGround(projectileObj1, projData);
            else
                GameController.soundManager.PlayEvent("event:/SFX/Characters/Fighter/Dunk/sfx_char_ftr_a_land",
                    (IPositionalObj) this, false, false);

            float skillAmount = this.player.GetSkillAmount(SkillType.Fight_Leap_Shred);
            if (skillAmount > 0.0)
            {
                projectileObj1.givenStatusEffect = StatusEffect.ArmorShred;
                projectileObj1.givenStatusEffectDuration = skillAmount;
            }

            float callbackDelayMod = 1f;
            if (this.HasEquippedEquipment(EquipmentType.FTR_AirLeap))
                callbackDelayMod *= 2.25f;
            this.SetSpellACounter(4.5f * callbackDelayMod);
        }

        public void UpdateCounterAttack(float elapsedSeconds)
        {
            this.m_counterAttackDistanceCounter +=
                Math.Abs((this.Heading * this.CurrentSpeed * elapsedSeconds).Length());
            float num = 175f;
            if (this.HasEquippedEquipment(EquipmentType.FTR_RangeCounter))
                num *= 3f;
            if (this.HasEquippedEquipment(EquipmentType.FTR_RiskCounter))
                num *= 0.65f;
            if ((double) this.m_counterAttackDistanceCounter >= (double) num)
            {
                this.counterAttackComplete = true;
                this.CurrentSpeed = 0.0f;
            }

            // PlayerClassObj_Fighter playerClassObjFighter = this;
            // playerClassObjFighter.Position = playerClassObjFighter.Position + this.Heading * this.CurrentSpeed * elapsedSeconds;
        }

        public override void ActivateSpecialAbility()
        {
            if ((double) this.player.GetSkillAmount(SkillType.Passive_Fight) <= 0.0)
                return;
            for (int index = 0; index < this.Game.PlayerManager.activePlayerArray_count; ++index)
            {
                var activePlayer = this.Game.PlayerManager.activePlayerArray[index];
                if (activePlayer.currentPlayerClass.IsKilled) continue;
                activePlayer.currentPlayerClass.ApplyStatusEffect((CharacterObj) this, StatusEffect.PowerBoost, 6f,
                    true);
                activePlayer.currentPlayerClass.ApplyStatusEffect((CharacterObj) this, StatusEffect.TechBoost, 6f,
                    true);
                activePlayer.currentPlayerClass.ApplyStatusEffect((CharacterObj) this, StatusEffect.Haste, 6f,
                    true);
            }
        }

        public override void ResetStates(bool resetDeathState)
        {
            this.m_counterCDCounter = 0.0f;
            this.isJumpAttacking = false;
            this.isSpinAttacking = false;
            this.StopChargeSpinCount();
            base.ResetStates(resetDeathState);
        }

        public void ForceStopSpinEvent()
        {
            this.m_spinAttack.StopSpinSound();
        }

        public override GameObj Clone()
        {
            throw new NotImplementedException();
        }

        private void HandleWaterGround(ProjectileObj projectileObj1, ProjectileData projData)
        {
            projectileObj1.Opacity = 0.5f;
            projectileObj1.TextureColor = Color.SkyBlue;
            GameController.soundManager.PlayEvent("event:/SFX/Characters/Fighter/Dunk/sfx_char_ftr_a_land_wet",
                (IPositionalObj) this, false, false);
            var projectileObj2 =
                this.Game.ProjectileManager.FireProjectile(projData, (CharacterObj) this, false,
                    (GameObj) null);
            projectileObj2.ChangeSprite("Water_Ripple");
            projectileObj2.Layer = 0.0f;
            projectileObj2.lockLayer = true;
            projectileObj2.PlayAnimation(true, false);
            projectileObj2.Opacity = 0.5f;
            projectileObj2.TextureColor = Color.SkyBlue;
            projectileObj2.Scale = Vector2.One;
            projectileObj2.Scale = new Vector2(projectileObj1.AbsBounds.Width / projectileObj2.AbsBounds.Width);
            projectileObj2.ScaleY /= 2.65f;
            projectileObj2.Position = this.shadowPosition;
            projectileObj2.applyTransparency = true;
            projectileObj2.Collidable = false;
            Tween.To((object) projectileObj2, 2f, new Easing(Quad.EaseOut), (object) "ScaleX",
                (object) (float) ((double) projectileObj2.ScaleX * 2.0), (object) "ScaleY",
                (object) (float) ((double) projectileObj2.ScaleY * 2.0));
            Tween.To((object) projectileObj2, 1f, new Easing(Tween.EaseNone), (object) "delay", (object) 1,
                (object) "Opacity", (object) 0);
            Tween.AddEndHandlerToLastTween((object) projectileObj2, "StopAnimation");
        }
    }
}
