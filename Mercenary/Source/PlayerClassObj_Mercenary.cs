using System;
using System.Collections.Generic;
using Brawler2D;
using CDGEngine;
using FullModdedFuriesAPI.Framework.ModHelpers.DatabaseHelper;
using FullModdedFuriesAPI.Mods.MercenaryClass.Source.ControllableEnemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteSystem2;
using Tweener;
using Tweener.Ease;

namespace FullModdedFuriesAPI.Mods.MercenaryClass.Source
{
    public class PlayerClassObj_Mercenary : PlayerClassObj
    {
        public static ClassType MercenaryClassType = (ClassType) 0;

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
        private MercenaryDummyLeap m_dummyAttack;
        private FighterPullLeapAttack m_pullJumpAttack;
        private bool m_isJumpAttacking;
        private List<BaseAttack> m_heavyAttackList;
        private List<BaseAttack> m_flameAttackList;
        private bool m_hitEnemyWithLastAttack;
        private float m_chargeSpinCount;
        private bool m_startChargeSpinCount;

        public static Color
            MERCENARY_COLOUR = new Color(199, 93, 153); //new Color(147, 76, 116);//new Color(168, 67, 71); //;


        private IModHelper helper;
        private readonly SkillType Tec_Mercenary_Index;
        private readonly SkillType Atk_Mercenary_Index;
        private readonly SkillType Hp_Mercenary_Index;


        private MercenaryArtilleryAttack m_artilleryAttack;
        private bool m_artilleryAttacking;
        private IDatabaseHelper database;
        private SkillType Mercenary_Skill_Y;
        private SkillType Mercenary_Skill_B;
        private SkillType Mercenary_Skill_X;
        private SkillType Mercenary_Skill_A;
        private bool m_isSitting;
        private bool m_isSit;

        private bool m_isGettingUp;
        private float m_isGettingUpCooldown;

        private EnemyControllableManager _enemyControllableManager = new();
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
            this.m_currentAttack = (BaseAttack) this.m_dummyAttack;
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
            // this.targetSpriteDistance = 0;
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
            base.IntelligenceFlatMods + (int) this.player.GetSkillAmount(this.Tec_Mercenary_Index);

        public override float StrengthFlatMods =>
            base.StrengthFlatMods + (int) this.player.GetSkillAmount(this.Atk_Mercenary_Index);

        public override float MaxHealthFlatMods => base.MaxHealthFlatMods + this.player.GetSkillAmount(this.Hp_Mercenary_Index);

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

        public bool isSitting
        {
            get => this.m_isSitting;
            set => this.m_isSitting = value;
        }

        public bool isSit
        {
            get => this.m_isSit;
            set => this.m_isSit = value;
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

        public bool isArtilleryAttacking
        {
            get => this.m_artilleryAttacking;
            set
            {
                if (value != this.m_artilleryAttacking)
                    this.useCachedTec = false;
                this.m_artilleryAttacking = value;
            }
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
                // // if (this.isSpinAttacking)
                // // {
                // //     num *= 0.95f;
                // //     if (this.HasEquippedEquipment(EquipmentType.FTR_FastSpin))
                // //         num *= 2f;
                // //     if (this.HasEquippedEquipment(EquipmentType.FTR_PullSpin))
                // //         num *= 0.425f;
                // //     if (this.HasEquippedEquipment(EquipmentType.FTR_ChargeSpin))
                // //         num *= 0.65f;
                // // }
                // else if (this.isCounterAttacking)
                //     num *= 4.25f;

                return base.Speed * num;
            }
            set => base.Speed = value;
        }

        public PlayerClassObj_Mercenary(GameController game, PlayerObj player, IModHelper helper)
            : base(game, player, "PlayerMercenary_Character")
        {
            this.helper = helper;

            this.Name = "Player_Mercenary";
            this.m_objType = 5;
            this.m_classType = (ClassType) this.helper.Database.GetCustomClasses()["Mercenary"];
            MercenaryClassType = this.m_classType;
            this.m_heavyAttackList = new List<BaseAttack>();
            this.m_flameAttackList = new List<BaseAttack>();


            this.Tec_Mercenary_Index = (SkillType) this.helper.Database.GetCustomSkillIndex("TEC_Mercenary");
            this.Atk_Mercenary_Index = (SkillType) this.helper.Database.GetCustomSkillIndex("ATK_Mercenary");
            this.Hp_Mercenary_Index = (SkillType) this.helper.Database.GetCustomSkillIndex("HP_Mercenary");
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
            // this.m_instrument1Name =
            //     "event:/Music/interactive instruments/mus_interactiveinstrument_guitar/mus_interactiveinstrument_guitar_a";
            // this.m_instrument2Name =
            //     "event:/Music/interactive instruments/mus_interactiveinstrument_guitar/mus_interactiveinstrument_guitar_b";
            // this.m_instrument3Name =
            //     "event:/Music/interactive instruments/mus_interactiveinstrument_guitar/mus_interactiveinstrument_guitar_csharp";
            // this.m_instrument4Name =
            //     "event:/Music/interactive instruments/mus_interactiveinstrument_guitar/mus_interactiveinstrument_guitar_d";
            // this.m_instrument5Name =
            //     "event:/Music/interactive instruments/mus_interactiveinstrument_guitar/mus_interactiveinstrument_guitar_e";
            // this.m_instrument6Name =
            //     "event:/Music/interactive instruments/mus_interactiveinstrument_guitar/mus_interactiveinstrument_guitar_fsharp";
            // this.m_instrument7Name =
            //     "event:/Music/interactive instruments/mus_interactiveinstrument_guitar/mus_interactiveinstrument_guitar_fsharphigh";
            // this.m_instrument8Name =
            //     "event:/Music/interactive instruments/mus_interactiveinstrument_guitar/mus_interactiveinstrument_guitar_gsharp";

            string instrumentCowbellBank = ModEntry.modHelper.DirectoryPath + "\\assets\\cowbell";
            string instrumentCowbell = "event:mus_interactiveinstrument_cowbell/";

            this.m_instrument1Name =
                instrumentCowbell + "mus_interactiveinstrument_cowbell_a";
            this.m_instrument2Name =
                instrumentCowbell + "mus_interactiveinstrument_cowbell_b";
            this.m_instrument3Name =
                instrumentCowbell + "mus_interactiveinstrument_cowbell_csharp";
            this.m_instrument4Name =
                instrumentCowbell + "mus_interactiveinstrument_cowbell_d";
            this.m_instrument5Name =
                instrumentCowbell + "mus_interactiveinstrument_cowbell_e";
            this.m_instrument6Name =
                instrumentCowbell + "mus_interactiveinstrument_cowbell_fsharp";
            this.m_instrument7Name =
                instrumentCowbell + "mus_interactiveinstrument_cowbell_fsharphigh";
            this.m_instrument8Name =
                instrumentCowbell + "mus_interactiveinstrument_cowbell_gsharp";
            //
            // GameController.soundManager.LoadBank(instrumentCowbellBank, false);
            // yield return (object) null;
            // GameController.soundManager.LoadBank(instrumentCowbellBank+".strings", false);
            // yield return (object) null;


            foreach (var obj in base.LoadContent())
                yield return null;
        }

        public override void Initialize()
        {
            this.classColour = MERCENARY_COLOUR;
            base.Initialize();
            this.shadowYOffset = 32f;
            this.targetSpriteEllipseRatio = PlayerEV.FIGHTER_LEAP_ELLIPSE_RATIO;
            this.database = this.helper.Database;
            this.Mercenary_Skill_Y = (SkillType) this.database.GetCustomSkillIndex("Mercenary_Skill_Y");
            this.Mercenary_Skill_X = (SkillType) this.database.GetCustomSkillIndex("Mercenary_Skill_X");
            this.Mercenary_Skill_A = (SkillType) this.database.GetCustomSkillIndex("Mercenary_Skill_A");
            this.Mercenary_Skill_B = (SkillType) this.database.GetCustomSkillIndex("Mercenary_Skill_B");
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
            // this.m_spinAttack = new FighterSpinAttack((PlayerClassObj) this, "Hammer_Spin", PlayerAction.SpellB);
            // this.m_spinAttack.InitializeLogic();
            this.m_counterAttack = new FighterCounterAttack((PlayerClassObj) this, "Hammer_Counter");
            this.m_counterAttack.InitializeLogic();
            this.m_counterDefend = new FighterCounterDefend((PlayerClassObj) this, "Hammer_Counter");
            this.m_counterDefend.InitializeLogic();
            this.m_riskCounterAttack = new FighterRiskCounter((PlayerClassObj) this, "Hammer_Counter");
            this.m_riskCounterAttack.InitializeLogic();
            this.m_jumpAttack = new FighterJumpAttack((PlayerClassObj) this, "Hammer_Jump", PlayerAction.SpellA);
            this.m_jumpAttack.InitializeLogic();

            this.m_dummyAttack = new MercenaryDummyLeap((PlayerClassObj) this, "Hammer_Jump", PlayerAction.SpellA);
            this.m_dummyAttack.InitializeLogic();

            this.m_artilleryAttack = new MercenaryArtilleryAttack(this,
                "Hammer_Jump", // "Mercenary_Artillery",
                PlayerAction.SpellY);
            this.m_artilleryAttack.InitializeLogic();

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
            // this.jumpDistance = 1;
            // this.fakeWeighted = true;

            // if (this.isSitting)
            // {
            //     this.isSitting = false;
            //     this.isSit = false;
            //     return;
            // }

            if (this.m_isGettingUp)
            {
                return;
            }

            if (this.isSit)
            {
                this.GetUpFromTarget();
                return;
            }

            this.target = this.FindClosestCharacters();//this.getTarget(TargetType.RandomEnemyObj);
            if(this.target == null)
                return;

            if (!this.isSit && !this.isSitting && !this.m_isGettingUp)
            {
                this.isSitting = true;
                this.lastTime = 0;
            }


            // Tween.To(this, 0.25f, new Easing(Back.EaseOutLarge), (object) "Po", (object) num, (object) "ScaleY", (object) num);
            // this.isSit = !this.isSit;

            // if (this.game.InputManager.KeyJustPressed(Keys.C))
            // {
                // var playerManager = this.game.PlayerManager;
                // int playerArrayCount = playerManager.activePlayerArray_count;
                // var database = this.Helper.Database;
                // var classType = (ClassType) database.GetCustomClasses()["Mercenary"];
                // for (int index = 0; index < playerArrayCount; ++index)
                // {
                    // var currentPlayerClass = playerManager.activePlayerArray[index].currentPlayerClass;

                    // if (currentPlayerClass.classType == classType)
                        // ((PlayerClassObj_Mercenary) currentPlayerClass).isSit =
                            // !((PlayerClassObj_Mercenary) currentPlayerClass).isSit;

                // }

            // }

            return;
            // if ((double) this.player.GetSkillAmount(SkillType.Fight_Skill_Y) < 1.0 && !GlobalEV.UNLOCK_ALL_SKILLS)
                if ((double) this.player.GetSkillAmount(this.Mercenary_Skill_Y) < 1.0 && !GlobalEV.UNLOCK_ALL_SKILLS)
                return;

            if ((double) this.spellBCDCounter > 0.0 && !this.isArtilleryAttacking)
                this.RunCDAnimation(true);

            this.target = this.getTarget(TargetType.RandomEnemyObj);
            this.m_artilleryAttack.Execute();
            this.currentAttack = this.m_artilleryAttack;
            this.isArtilleryAttacking = true;

            using (new NetScope<Forced>())
                this.m_currentAttack.Execute();
            this.player.SetCurrentEquipSlotMasteryPoints(EquipmentSlotType.ButtonY, 1, true);

            // this.CounterAttack();
        }

        private void GetUpFromTarget()
        {
            this.isSit = false;
            this.isSitting = false;
            this.m_isGettingUp = true;
            this.DoJump(this.storedLeftStickDirection);
            this._enemyControllableManager.UnmakeEnemyControllable((EnemyObj) this.target);
        }

        public EnemyObj FindClosestCharacters() //ArenaScreen arenaScreen)
        {
            var arenaScreen = GameController.g_game.ScreenManager.arenaScreen;
            float num1 = (float) int.MaxValue;
            var enemyObj = (EnemyObj) null;
            int globalEnemyArrayCount = GameController.g_game.ScreenManager.arenaScreen.currentAndGlobalEnemyArray_count;
            for (int index = 0; index < globalEnemyArrayCount; ++index)
            {
                var currentAndGlobalEnemy = arenaScreen.currentAndGlobalEnemyArray[index];
                // if (currentAndGlobalEnemy != enemy && currentAndGlobalEnemy.Active && (!currentAndGlobalEnemy.IsKilled && !currentAndGlobalEnemy.isNotEnemy))
                if (!currentAndGlobalEnemy.Active ||
                    (currentAndGlobalEnemy.IsKilled || currentAndGlobalEnemy.isNotEnemy)) continue;

                float num2 =
                    Math.Abs(CDGMath.DistanceBetweenPts(this.AbsPosition, currentAndGlobalEnemy.AbsPosition));
                if (!((double) num2 < (double) num1)) continue;
                num1 = num2;
                enemyObj = currentAndGlobalEnemy;
            }
            return enemyObj;
        }

        public override void CastSpellB()
        {
            if(this.isSitting || this.isSit)
                return;
            // if ((double) this.player.GetSkillAmount(this.Mercenary_Skill_B) < 1.0 && !GlobalEV.UNLOCK_ALL_SKILLS)
                // return;
            if ((double) this.spellBCDCounter > 0.0)
            {
                this.RunCDAnimation(false);
                return;
            }

            // else if (!this.isSpinAttacking)
            //     this.BNoSpinAttack();
            // else
            //     this.BSpinAttack();

            // Net.Master.Cast((byte) this.player.playerIndex, PlayerActions.BSpinAttack);
            this.StopAllActions();

            this.target = this.getTarget(TargetType.ClosestOpponent);
            this.m_artilleryAttack.Execute();
            this.currentAttack = this.m_artilleryAttack;
            this.isArtilleryAttacking = true;
            this.StartSpellBCounter();

            using (new NetScope<Forced>())
                this.m_currentAttack.Execute();
            this.player.SetCurrentEquipSlotMasteryPoints(EquipmentSlotType.ButtonB, 1, true);

        }

        public override void CastSpellA()
        {
            if(this.isSitting || this.isSit)
                return;
            if ((double) this.player.GetSkillAmount(SkillType.Fight_Skill_A) < 1.0 && !GlobalEV.UNLOCK_ALL_SKILLS)
                return;
            if ((double) this.spellACDCounter > 0.0)
                this.RunCDAnimation(true);
            else
                this.AJumpAttack();
        }

        public override void CastSpecialR()
        {
            if (this.isSit)
            {
                if (this.target is EnemyObj e)
                {
                    e.CurrentHealth = -1;
                    e.Knockdown(this);
                }
            }
            else
            {
                this.Position = Vector2.Zero;
            }

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
            this.SetSpellBCounter(2f);
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

            if(this.m_currentAttack == this.m_artilleryAttack && this.isArtilleryAttacking)
            {
                return;
            }

            if (this.inAir || this.hasStatusEffect(StatusEffect.Stop) ||
                (this.hasStatusEffect(StatusEffect.Dizzy) || this.State == CharacterState.Stunned) ||
                this.State == CharacterState.KnockedDown)
            {
                return;
            }

            if(this.isSitting)
                return;

            base.HandleInput();
            // this.HandleMovement();
            // this.UpdateFlipInput();
            // if (this.Heading == Vector2.Zero)
            // {
            //     this.HeadingX = -1f;
            //     if (this.Flip == SpriteEffects.None)
            //         this.HeadingX = 1f;
            // }
            //
            // this.CurrentSpeed = this.Speed * 0.95f;
            // this.State = CharacterState.Attacking;
            //
            // base.HandleInput();
        }

        protected override void HandleMovement()
        {
            if (this.isSit || this.isSitting || this.m_isGettingUp)
                return;

            base.HandleMovement();
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
            this._enemyControllableManager.Update();
            if (this.target is EnemyObj enemyObj)
            {
                ModEntry.modHelper.Reflection
                    .GetField<float>(enemyObj, "m_cooldownDebugCounter").SetValue(0);
                if (enemyObj.IsKilled && this.isSit)
                    this.GetUpFromTarget();
            }

            if (this.hasStatusEffect(StatusEffect.Stop) || this.hasStatusEffect(StatusEffect.Dizzy))
                return;

            var e = this.FindClosestCharacters();
            if (e != null)
                this.UpdateMiscSprites(e.Position);
            // this.UpdateMiscSprites(Vector2.One);

            switch (this.State)
            {
                case CharacterState.KnockedDown:
                case CharacterState.Stunned:
                    this.m_artilleryAttack?.DestroySpeechBubble();
                    break;
            }

            if (this.isArtilleryAttacking)
                this.m_artilleryAttack?.Update(elapsedSeconds);

            if (this.m_startChargeSpinCount)
                this.m_chargeSpinCount += elapsedSeconds;

            if (this.m_isGettingUpCooldown > 0.0)
                this.m_isGettingUpCooldown -= elapsedSeconds;

            if (this.m_counterCDCounter > 0.0)
                this.m_counterCDCounter -= elapsedSeconds;
            if (this.isCounterAttacking)
                this.m_currentActiveLS.Update(elapsedSeconds);
            if (this.isJumpAttacking)
                this.UpdateJumpAttack(elapsedSeconds);

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

        private float lastTime = 0f;
        private Vector2 startJumpPosition = Vector2.Zero;
        private float shadowYOffsetOriginal = -1;

        protected override void UpdateAnimations(float elapsedSeconds)
        {
            if (this.m_isGettingUp)
            {
                if (this.startingIndex != this.GetFrameFromLabel("BackFlipStart"))
                {
                    this.lastTime = 0;
                    this.AnimationSpeed = 1.5f;
                    this.PlayAnimation("BackFlipStart", "BackFlipEnd", false, false);
                    // this.PlayAnimationThenFunction(this, "EndGettingUp");
                    return;
                }

                this.lastTime += 0.1f;
                if (this.lastTime >= 6f)
                {
                    this.lastTime = 0;
                    this.m_isGettingUp = false;
                    this.PlayAnimation("StandStart", "StandEnd", false, false);
                }
                // if (this.fakeAccelerationY < 0.0)
                // {
                //     this.m_isGettingUp = false;
                //     return;
                // }
            }

            if (!this.isSitting)
            {
                if(this.shadowYOffsetOriginal == -1)
                    this.shadowYOffsetOriginal = this.shadowYOffset;
                this.shadowYOffset =  this.shadowYOffsetOriginal;
                this.AnimationSpeed = 0.1f;
            }

            if (this.isSitting)
            {
                if (this.target == null)
                {
                    this.isSitting = false;
                    this.isSit = false;
                    // this.Collidable = false;
                }

                this.Collidable = false;
                this.collisionEllipseWidth = 0;
                if (this.target is EnemyObj enemyObj)
                {
                    if (!enemyObj.Active || (enemyObj.IsKilled || enemyObj.isNotEnemy))
                    {
                        this.isSitting = false;
                        this.isSit = false;
                    }
                }

                var shadowDistance = CDGMath.VectorBetweenPts(this.Position, this.target.shadowSprite.Position);

                // if (this.startingIndex != this.GetFrameFromLabel("SitTellBegin"))
                if (this.lastTime == 0.00f)
                {
                    this.lastTime = 0.01f;
                    this.startJumpPosition = this.Position;
                    this.AnimationSpeed = 0.06f;
                    // if (this.startingIndex != this.GetFrameFromLabel("SitTellBegin"))
                    // this.PlayAnimation("SitTellBegin", "SitTellEnd", false, false);
                    this.PlayAnimation("BackFlipToSitStart", "BackFlipToSitEnd", false, false);

                    GameController.soundManager.PlayEvent("event:/SFX/Characters/Fighter/Dunk/sfx_char_ftr_a_leap_up", (IPositionalObj) this, false, false);
                    this.JumpEffect(this.shadowPosition, this.Layer - 0.0001f);
                    // this.JumpAttack();
                }

                if (this.CurrentFrame < this.startingIndex + 2)
                {
                    return;
                }

                if (this.JumpOnEnemy(this.startJumpPosition, this.target.Position, shadowDistance))
                {
                    if (this.target is EnemyObj enemyObj2)
                    {
                        if (enemyObj2.State == CharacterState.KnockedDown)
                        {
                            enemyObj2.PlayAnimation("KODownStart", "KODownEnd", false, true);
                            enemyObj2.PerformGoldDropCheck();
                        }


                    }
                };
                // lastPoint = bezierPt;

                // if (this.Position != this.target.Position)
                // if (Math.Abs(this.Position.X - this.target.X) > 0.5f && Math.Abs(this.Position.Y - this.target.AbsBounds.Top - 1f)> 0.5f)
                {

                    // float distance = CDGMath.DistanceBetweenPts(this.target.Position, this.Position);
                    // this.fakeWeighted = true;
                    // // this.jumpDistance = distance;
                    // float num2 = CDGMath.DistanceBetweenPts(Vector2.Zero, new Vector2((float) (225.0 * Math.Cos(num1)), (float) (225.0 * (1.0 / (double) PlayerEV.FIGHTER_MINI_ELLIPSE_RATIO) * Math.Sin(num1))));
                    // float x = MathHelper.Lerp(this.Position.X, this.target.X, 0.1f);
                    // // float y = MathHelper.Lerp(this.Position.Y, this.target.Y, 0.05f);
                    // float y = MathHelper.Lerp(this.Position.Y, this.target.AbsBounds.Top - 4f, 0.1f);
                    //
                    // this.Position = new Vector2(x, y);

                    // float y = this.Y - this.m_arrowHeight;
                    // if ((double) this.m_targetDistance.Y > 0.0)
                    //     y = this.m_source.Y - this.m_arrowHeight;
                    // Vector2 position = this.m_source.Position;
                    // int num1 = (int) ((double) this.m_stripList.Count / 2.0);
                    // Vector2 vector2_1 = new Vector2(this.X - this.m_targetDistance.X / 2f, y);
                    // Vector2 vector2_2 = new Vector2(position.X, vector2_1.Y);
                    // this.startJumpPosition = this.Position;
                    // var targetDistance = CDGMath.VectorBetweenPts(this.startJumpPosition, this.target.Position);
                    // float y = this.target.Y - this.target.Bounds.Height;
                    // if ((double) targetDistance.Y > 0.0)
                    //     y = this.startJumpPosition.Y - this.target.Bounds.Height;
                    //
                    // // var vector2_1 = new Vector2(this.target.Position.X - targetDistance.X / 2f, y);
                    // var vector2_1 = new Vector2(this.target.Position.X, y);
                    // var vector2_2 = new Vector2(this.startJumpPosition.X, vector2_1.Y);
                    // var pts = new List<Vector2>();
                    // pts.Clear();
                    // pts.Add(this.startJumpPosition);
                    // pts.Add(vector2_2);
                    // pts.Add(vector2_1);
                    // // float num2 = 0.5f / (float) num1;
                    // // for (int index = 0; index < num1; ++index)
                    // // {
                    //     Vector2 bezierPt = BezierHelper.GetBezierPt(pts, this.lastTime);
                    //
                    //     this.Position = new Vector2(bezierPt.X, bezierPt.Y);
                    //     this.lastTime += 0.01f;
                }
                // else
                // {
                //
                //
                //     this.isSitting = false;
                //     this.isSit = true;
                // }
            }
            else
            {
            }

            if (this.isSit)
            {
                if (this.startingIndex != this.GetFrameFromLabel("SitBegin"))
                {
                    this.PlayAnimation("SitBegin", "SitEnd", false, false);

                    this.collisionEllipseWidth = 0;
                    this.Collidable = false;
                    this.hasShadow = false;
                    this._enemyControllableManager.MakeEnemyControllable((EnemyObj) this.target, this.player);

                    // BrawlerSpriteObj layeredSprite = this.Game.SpriteManager.GetLayeredSprite("PlayerMercenary_Character");
                    // layeredSprite.Position = new Vector2(this.target.X, this.target.AbsBounds.Top + 5f);
                    // // layeredSprite.AnimationSpeed = 0.05f;
                    // layeredSprite.Scale = new Vector2(2f);
                    // layeredSprite.PlayAnimation("SitBegin", "SitEnd", false, false);
                    // layeredSprite.lockLayer = true;
                    // layeredSprite.Layer = this.target.Layer;
                }

                float x = MathHelper.Lerp(this.Position.X, this.target.X, 0.5f);
                // float y = MathHelper.Lerp(this.Position.Y, this.target.Y, 0.05f);
                float y = MathHelper.Lerp(this.Position.Y, this.target.AbsBounds.Top + 5f, 0.5f);

                if (this.target.Flip != this.Flip)
                    this.Flip = this.target.Flip;

                this.Position = new Vector2(x, y);

                return;
            }
            else
            {
                this.Collidable = true;
                this.hasShadow = true;
            }


            if (!this.isJumpAttacking)
            {
                base.UpdateAnimations(elapsedSeconds);
                return;
            }

            // if (this.isSitting)
            // {
            //     this.PlayAnimation("BackFlipFullStart", "BackFlipFullEnd", false, false);
            //     return;
            // }

            // if (this.HasEquippedEquipment(EquipmentType.FTR_MiniLeap))
            // {
            //     this.PlayAnimation("JumpFallingStart", "JumpFallingEnd", false, false);
            //     return;
            // }

            if (this.startingIndex != this.GetFrameFromLabel("BackFlipFullStart") && this.fakeAccelerationY < 0.0)
            {
                this.PlayAnimation("BackFlipFullStart", "BackFlipFullEnd", false, false);
                return;
            }

            if (this.fakeAccelerationY <= 0.0)
                return;
            if (this.startingIndex == this.GetFrameFromLabel("BackFlipFullStart"))
                GameController.soundManager.PlayEvent(
                    "event:/SFX/Characters/Fighter/Dunk/sfx_char_ftr_a_leap_down", (IPositionalObj) this, false,
                    false);
            // this.PlayAnimation("BackFlipTellStart", "BackFlipTellEnd", false, false);
        }

        private bool JumpOnEnemy(Vector2 originJumpPosition, Vector2 destiny, Vector2 shadowDistance)
        {
            var targetDistance = CDGMath.VectorBetweenPts(originJumpPosition, destiny);
            float y = this.target.Y - this.target.Bounds.Height;

            // var vector2_1 = new Vector2(this.target.Position.X - targetDistance.X / 2f, y);
            var vector2_1 = new Vector2(this.target.Position.X, y);
            var targetBevVec = new Vector2(originJumpPosition.X, vector2_1.Y);
            if (targetDistance.Y > 0)
                targetBevVec = new Vector2(this.target.X,
                    MathHelper.Min(this.target.Y + this.target.Bounds.Height + 10,
                        originJumpPosition.Y - this.target.Bounds.Height));

            // if (shadowDistance.Y > 0)
            {
                this.shadowYOffset = MathHelper.Lerp(shadowDistance.Y, this.shadowYOffset, this.lastTime);
                this.shadowWidth = MathHelper.Lerp(this.shadowWidth, this.target.shadowWidth, this.lastTime);
            }

            var pts = new List<Vector2>();
            pts.Clear();
            pts.Add(originJumpPosition);
            pts.Add(targetBevVec);
            pts.Add(vector2_1);
            // float num = 0.1f;
            int num1 = (int) 20;

            if (this.lastTime > 1)
            {
                this.isSitting = false;
                this.isSit = true;

                return true;
            }

            var bezierPt = BezierHelper.GetBezierPt(pts, this.lastTime);
            this.Position = new Vector2(bezierPt.X, bezierPt.Y);

            this.lastTime += 0.05f;
            return false;
        }

        private bool DoGettingUpJump(Vector2 jumpPosition, Vector2 destiny, Vector2 shadowDistance)
        {
            var targetDistance = CDGMath.VectorBetweenPts(jumpPosition, destiny);
            float y = this.target.Y - this.target.Bounds.Height;

            // var vector2_1 = new Vector2(this.target.Position.X - targetDistance.X / 2f, y);
            var vector2_1 = new Vector2(this.target.Position.X, y);
            var targetBevVec = new Vector2(jumpPosition.X, vector2_1.Y);
            if (targetDistance.Y > 0)
                targetBevVec = new Vector2(this.target.X,
                    MathHelper.Min(this.target.Y + this.target.Bounds.Height + 10,
                        jumpPosition.Y - this.target.Bounds.Height));

            // if (shadowDistance.Y > 0)
            {
                this.shadowYOffset = MathHelper.Lerp(this.shadowYOffset, shadowDistance.Y, this.lastTime);
                this.shadowWidth = MathHelper.Lerp(this.shadowYOffset, this.target.shadowWidth, this.lastTime);
            }

            var pts = new List<Vector2>();
            pts.Clear();
            pts.Add(jumpPosition);
            pts.Add(targetBevVec);
            pts.Add(vector2_1);
            // float num = 0.1f;
            int num1 = (int) 20;

            if (this.lastTime > 1)
            {
                this.m_isGettingUp = false;
                this.isSitting = false;
                this.isSit = false;
                return true;
            }

            var bezierPt = BezierHelper.GetBezierPt(pts, this.lastTime);
            this.Position = new Vector2(bezierPt.X, bezierPt.Y);

            this.lastTime += 0.05f;
            return false;
        }
        // public override void CalculateEnemyEllipsisCollision()
        // {
        //     if (this.isSitting || this.isSit)
        //     {
        //         return;
        //     }
        //     base.CalculateEnemyEllipsisCollision();
        // }

        public static float LEAP_ELLIPSE_RATIO = 1.2525f;

        public void DoJump(Vector2 position)
        {
            if (position == Vector2.Zero)
            {
                position = new Vector2(1f, 0.0f);
                if (this.Flip == SpriteEffects.FlipHorizontally)
                    position = new Vector2(-1f, 0.0f);
            }

            var normalized = position;
            normalized.Normalize();
            var jumpDirection = normalized;
            if (jumpDirection == Vector2.Zero)
                jumpDirection.X = this.Flip != SpriteEffects.None ? -1f : 1f;
            float angle = CDGMath.VectorToAngle(normalized);
            double num1 = Math.Atan((double) byte.MaxValue * Math.Tan((double) angle * Math.PI / 180.0) /
                                    ((double) byte.MaxValue * (1.0 / (double) LEAP_ELLIPSE_RATIO)));
            if ((double) angle > 90.0)
                num1 += Math.PI;
            else if ((double) angle < -90.0)
                num1 -= Math.PI;
            float num2 = CDGMath.DistanceBetweenPts(Vector2.Zero,
                new Vector2((float) ((double) byte.MaxValue * Math.Cos(num1)),
                    (float) ((double) byte.MaxValue * (1.0 / (double) LEAP_ELLIPSE_RATIO) * Math.Sin(num1))));
            float num3 = num2 / (float) byte.MaxValue;
            this.HeadingX = normalized.X;
            this.HeadingY = normalized.Y;
            float num4 = 1f;
            this.CurrentSpeed = 175f * num3 * num4;
            this.jumpDistance = num2 / 3;
            GameController.soundManager.PlayEvent("event:/SFX/Characters/Fighter/Dunk/sfx_char_ftr_a_leap_up",
                (IPositionalObj) this, false, false);
            this.CalculateFakeAccelerationY();
            this.UpdateFlipInput();
            this.JumpEffect(this.shadowPosition, this.Layer - 0.0001f);
        }

        public void JumpEffect(Vector2 pos, float layer)
        {
            GameController.soundManager.PlayEvent("event:/SFX/Characters/Fighter/Dunk/sfx_char_ftr_a_leap_up",
                (IPositionalObj) this, false, false);
            BrawlerSpriteObj layeredSprite = this.Game.SpriteManager.GetLayeredSprite("Jump_Attack");
            layeredSprite.Position = pos;
            layeredSprite.AnimationSpeed = 0.05f;
            layeredSprite.Scale = new Vector2(1.5f);
            layeredSprite.PlayAnimation(false, false);
            layeredSprite.lockLayer = true;
            layeredSprite.Layer = layer;
            // if (!this.source.HasEquippedEquipment(EquipmentType.FTR_AirLeap))
            // return;
            // layeredSprite.Scale = new Vector2(3f);
        }

        public override void CollisionResponse(GameObj otherObj, Vector2 mtd, Hitbox thisBox, Hitbox otherBox)
        {
            if (this.isSitting)
            {
                return;
            }

            if (this.isSit)
            {
                this.target.CollisionResponse(otherObj, mtd, thisBox, otherBox);
                return;
            }

            base.CollisionResponse(otherObj, mtd, thisBox, otherBox);
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

            // if (this.HasEquippedEquipment(EquipmentType.FTR_MiniLeap))
            // {
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
            // }

            // this.PlayAnimation("Spell1AttackStart", "Spell1AttackStart", false, false);
            // float num1 = 1f + this.player.GetSkillAmount(SkillType.Fight_Leap_Radius);
            // float num2 = 1f;
            // float num3 = 1f;
            // float num4 = 1f;
            // if (this.HasEquippedEquipment(EquipmentType.FTR_AirLeap))
            // {
            //     num1 *= 3f;
            //     num2 *= 1.5f;
            //     num3 *= 1f;
            //     num4 *= 0.65f;
            // }

            // var projData = new ProjectileData
            // {
            //     Scale = new Vector2(PlayerEV.FIGHTER_JUMPATTACK_RADIUS.X * num1,
            //         (float) ((double) PlayerEV.FIGHTER_JUMPATTACK_RADIUS.Y * (double) num1 / 2.65000009536743)),
            //     SpriteName = "Projectile_GiantBrute_ShockwaveLarge",
            //     Lifespan = 9999.9f,
            //     Damage = this.damage * num2,
            //     aggroRaise = this.aggroRaise,
            //     stunStr = this.stunStr,
            //     knockdownStr = this.knockdownStr
            // };
            // if (this.HasEquippedEquipment(EquipmentType.FTR_AirLeap))
            // {
            //     projData.stunStr = 11;
            //     projData.knockdownStr = 9;
            // }

            // projData.destroyOnAnimationEnd = true;
            // var projectileObj1 =
            //     this.Game.ProjectileManager.FireProjectile(projData, (CharacterObj) this, false, (GameObj) null);
            // projectileObj1.Layer = 0.0f;
            // projectileObj1.lockLayer = true;
            // projectileObj1.Flip = SpriteEffects.None;
            // projectileObj1.Position = new Vector2(this.X, this.AbsBounds.Bottom - 5f);
            // projectileObj1.PlayAnimation(false, false);
            // projectileObj1.knockdownDistanceMod = -0.25f * num3;
            // projectileObj1.knockdownSpeedMod = 0.2f * num4;
            // projectileObj1.CollisionID = 1; //Mod.Instance.WhenFriendlyChangeCollisionId(2, player.playerIndex);
            // projectileObj1.AnimationSpeed = 0.05f;
            // projectileObj1.canHitDowned = true;
            // projectileObj1.applyTransparency = true;

            // if (arenaScreen?.currentGlobalZone != null && arenaScreen.currentGlobalZone.groundIsWater)
            //     this.HandleWaterGround(projectileObj1, projData);
            // else
            //     GameController.soundManager.PlayEvent("event:/SFX/Characters/Fighter/Dunk/sfx_char_ftr_a_land",
            //         (IPositionalObj) this, false, false);

            // float skillAmount = this.player.GetSkillAmount(SkillType.Fight_Leap_Shred);
            // if (skillAmount > 0.0)
            // {
            //     projectileObj1.givenStatusEffect = StatusEffect.ArmorShred;
            //     projectileObj1.givenStatusEffectDuration = skillAmount;
            // }
            //
            // float callbackDelayMod = 1f;
            // if (this.HasEquippedEquipment(EquipmentType.FTR_AirLeap))
            //     callbackDelayMod *= 2.25f;
            // this.SetSpellACounter(4.5f * callbackDelayMod);
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
            if (this.m_counterAttackDistanceCounter >= num)
            {
                this.counterAttackComplete = true;
                this.CurrentSpeed = 0.0f;
            }

            // PlayerClassObj_Fighter playerClassObjFighter = this;
            // playerClassObjFighter.Position = playerClassObjFighter.Position + this.Heading * this.CurrentSpeed * elapsedSeconds;
        }

        public override void ActivateSpecialAbility()
        {
            if (this.player.GetSkillAmount(SkillType.Passive_Fight) <= 0.0)
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
            this.isArtilleryAttacking = false;
            this.StopChargeSpinCount();
            base.ResetStates(resetDeathState);
        }

        private void UpdateMiscSprites(Vector2 target)
        {
            // this.targetSpriteDistance = 50;
            // Vector2 pt = this.storedLeftStickDirection;
            // if (pt == Vector2.Zero)
            //     pt.X = this.Flip != SpriteEffects.None ? -1f : 1f;
            // float angle = CDGMath.VectorToAngle(pt);
            // double num = Math.Atan((double) this.targetSpriteDistance * Math.Tan((double) angle * Math.PI / 180.0) / ((double) this.targetSpriteDistance * (1.0 / (double) this.targetSpriteEllipseRatio)));
            // if ((double) angle > 90.0)
            //     num += Math.PI;
            // else if ((double) angle < -90.0)
            //     num -= Math.PI;
            // Vector2 pt2 = new Vector2((float) this.targetSpriteDistance * (float) Math.Cos(num), (float) ((double) this.targetSpriteDistance * (1.0 / (double) this.targetSpriteEllipseRatio) * Math.Sin(num)));
            // CDGMath.DistanceBetweenPts(Vector2.Zero, pt2);
            // Vector2 vector2 = this.shadowPosition + pt2;
            // this.m_arrowSprite.Position = vector2;
            // this.m_arrowSprite.indicatorSprite.Position = vector2;
            //
            // if (this.Game.ScreenManager.CurrentScreenType != ScreenType.Arena)
            //     return;
            // this.m_arrowSprite.Layer = (this.Game.ScreenManager.CurrentScreen as ArenaScreen).getLayerValue(this.m_arrowSprite.Y);
            // this.m_arrowSprite.Visible = true;

            //
            // // if (this.targetSpriteDistance <= 0)
            //     // return;
            var pt = this.leftJoystickDirection;

            float targetDistance = CDGMath.DistanceBetweenPts(this.Position, target);
            // float targetDistance = 50;
            // if (pt == Vector2.Zero)
                // pt.X = this.Flip != SpriteEffects.None ? -1f : 1f;
            // if (this is PlayerClassObj_Engineer classObjEngineer)
            //     pt = classObjEngineer.storedLeftStickDirection;
            // if (this is PlayerClassObj_Sniper playerClassObjSniper)
            //     pt = playerClassObjSniper.storedLeftStickDirection;
            // if (this is PlayerClassObj_Fighter playerClassObjFighter)
            //     pt = playerClassObjFighter.storedLeftStickDirection;
            pt = this.storedLeftStickDirection;
            // pt.X = this.Flip != SpriteEffects.None ? -1f : 1f;
            // target.X *= this.Flip != SpriteEffects.None ? -1f : 1f;
            float angle = CDGMath.VectorToAngle(target);
            double num1 = Math.Atan((double) targetDistance * Math.Tan((double) angle * Math.PI / 180.0) / ((double) targetDistance * (1.0 / (double) this.targetSpriteEllipseRatio)));
            if ((double) angle > 90.0)
                num1 += Math.PI;
            else if ((double) angle < -90.0)
                num1 -= Math.PI;
            Vector2 pt2 = new Vector2((float) targetDistance * (float) Math.Cos(num1), (float) ((double) targetDistance * (1.0 / (double) this.targetSpriteEllipseRatio) * Math.Sin(num1)));
            double num2 = (double) CDGMath.DistanceBetweenPts(Vector2.Zero, pt2);
            Vector2 vector2 = this.shadowPosition + pt2;
            this.m_arrowSprite.Position = target;
            this.m_arrowSprite.indicatorSprite.Position = target;
            if (this.Game.ScreenManager.CurrentScreenType != ScreenType.Arena)
                return;
            this.m_arrowSprite.Layer = (this.Game.ScreenManager.CurrentScreen as ArenaScreen).getLayerValue(this.m_arrowSprite.Y);
            // this.m_arrowSprite.Visible = true;
        }

        public override void Draw(Camera2D camera, float elapsedSeconds)
        {
            base.Draw(camera, elapsedSeconds);

            if (this.target is EnemyObj enemyObj)
            {
                // this.DisplayEnemyCooldowns(camera, elapsedSeconds, enemyObj);
            }
            // this.DisplayEnemyHitbox(camera, elapsedSeconds, null);
            // this._enemyControllableManager.DrawProjectileFeedback(camera, elapsedSeconds);

            return;
            var e = this.FindClosestCharacters();



            this.target = e;
            if(this.startJumpPosition == Vector2.Zero)
                this.startJumpPosition = this.Position;
            // this.startJumpPosition = this.Position;
            var targetDistance = CDGMath.VectorBetweenPts(this.startJumpPosition, this.target.Position);
            float y = this.target.Y - this.target.Bounds.Height;

            // var vector2_1 = new Vector2(this.target.Position.X - targetDistance.X / 2f, y);
            var vector2_1 = new Vector2(this.target.Position.X, y);
            var targetBevVec = new Vector2(this.startJumpPosition.X, vector2_1.Y);
            if (targetDistance.Y > 0)
            {
                float jumpOffsetY = this.startJumpPosition.Y - this.target.Bounds.Height;
                targetBevVec = new Vector2(this.target.X, jumpOffsetY);
            }
            var pts = new List<Vector2>();
            pts.Clear();
            pts.Add(this.startJumpPosition);
            pts.Add(targetBevVec);
            pts.Add(vector2_1);
            // float num = 0.1f;
            int num1 = (int) 20;
            float num2 = 0.5f / (float) num1;

            var lastPoint = this.Position;
            var cdgRect0 = new CDGRect(this.startJumpPosition.X, this.startJumpPosition.Y, 5f, 5f);
            var cdgRect1 = new CDGRect(vector2_1.X, vector2_1.Y, 5f, 5f);
            var targetBev = new CDGRect(targetBevVec.X, targetBevVec.Y, 5f, 5f);
            camera.Draw(GameController.GenericTexture, cdgRect0.ToRectangle(), new Rectangle?(), Color.Green, 1f, Vector2.Zero, SpriteEffects.None, 1f);
            camera.Draw(GameController.GenericTexture, cdgRect1.ToRectangle(), new Rectangle?(), Color.Green, 1f, Vector2.Zero, SpriteEffects.None, 1f);
            camera.Draw(GameController.GenericTexture, targetBev.ToRectangle(), new Rectangle?(), Color.Yellow, 1f, Vector2.Zero, SpriteEffects.None, 1f);

            for (int index = 0; index < num1; ++index)
            {
                var bezierPt = BezierHelper.GetBezierPt(pts, index / (float) num1);
                // float width = CDGMath.DistanceBetweenPts(lastPoint, bezierPt);
                // float degrees = CDGMath.AngleBetweenPts(lastPoint, bezierPt);
                var cdgRect = new CDGRect(bezierPt.X, bezierPt.Y, 5f, 5f);
                lastPoint = bezierPt;
                camera.Draw(GameController.GenericTexture, cdgRect.ToRectangle(), new Rectangle?(), Color.Red, 1f, Vector2.Zero, SpriteEffects.None, 1f);
            }

            // for (int index = 0; index < num; ++index)
            // {
            //     var bezierPt = BezierHelper.GetBezierPt(pts, num);
            //
            //     this.Position = new Vector2(bezierPt.X, bezierPt.Y);
            //     this.lastTime += 0.01f;
            // }
        }

        private void DisplayEnemyCooldowns(Camera2D camera, float elapsedSeconds, EnemyObj enemyObj)
        {
            // enemyObj.DrawCooldownTimers(camera, elapsedSeconds);
            var m_debugCDText = ModEntry.modHelper.Reflection
                .GetField<BrawlerTextObj>(enemyObj, "m_debugCDText").GetValue();
            float m_cooldownDebugCounter = ModEntry.modHelper.Reflection
                .GetField<float>(enemyObj, "m_cooldownDebugCounter").GetValue();
            m_debugCDText.Text = BrawlerHelper.BlitSingleFormatHelper(m_cooldownDebugCounter, "0.##");
            m_debugCDText.Position =
                (enemyObj.shadowPosition + enemyObj.inAirOffset) * camera.Zoom;
            m_debugCDText.Y += 7f;
            m_debugCDText.Draw(camera, elapsedSeconds);
        }

        private Dictionary<EnemyObj, string> colInfo = new Dictionary<EnemyObj, string>();
        private Dictionary<EnemyObj, TextObj> colInfoText= new Dictionary<EnemyObj, TextObj>();
        private Dictionary<EnemyObj, TextObj> colInfoText2= new Dictionary<EnemyObj, TextObj>();

        private void DisplayEnemyHitbox(Camera2D camera, float elapsedSeconds, EnemyObj enemyObj)
        {
            this.Game.PhysicsManager.CollisionIDsCollide = true;
            ArenaScreen arenaScreen = this.Game.ScreenManager.arenaScreen;
            int globalEnemyArrayCount = arenaScreen.currentAndGlobalEnemyArray_count;
            for (int index1 = 0; index1 < globalEnemyArrayCount; ++index1)
            {
                EnemyObj currentAndGlobalEnemy = arenaScreen.currentAndGlobalEnemyArray[index1];
                enemyObj = currentAndGlobalEnemy;

                string info = "";
                string info2 = "";
                Hitbox[] hitboxesArray1 = enemyObj.HitboxesArray;
                int hitboxesCount1 = enemyObj.HitboxesCount;
                // camera.Begin();
                for (int index2 = 0; index2 < hitboxesCount1; ++index2)
                {
                    Hitbox hitbox1 = hitboxesArray1[index2];
                    bool flag = false;
                    if (hitbox1.Type == HitboxType.Body)
                    {
                        CDGRect cdgRect = new CDGRect(hitbox1.X, hitbox1.Y, hitbox1.Width, hitbox1.Height);
                        // cdgRect.Y -= enemyObj.AnchorY * enemyObj.ScaleY;

                        camera.Draw(GameController.GenericTexture, cdgRect.ToRectangle(), Color.Blue);
                        // enemyObj.DrawCooldownTimers(camera, elapsedSeconds);

                        info += cdgRect.ToRectangle().ToString() + "\n";

                        //
                        // Hitbox[] hitboxesArray2 = this.HitboxesArray;
                        // int hitboxesCount2 = this.HitboxesCount;
                        // for (int index3 = 0; index3 < hitboxesCount2; ++index3)
                        // {
                        //     Hitbox hitbox2 = hitboxesArray2[index3];
                        //     if (hitbox2.Type == HitboxType.Weapon)
                        //     {
                        //         Rectangle rectangle1 = cdgRect.ToRectangle();
                        //         Rectangle rect = hitbox2.ToRect();
                        //         camera.Draw(GameController.GenericTexture, rect, Color.Red);
                        //     }
                        // }
                    }

                    if (hitbox1.Type == HitboxType.Weapon)
                    {
                        CDGRect cdgRect = new CDGRect(hitbox1.X, hitbox1.Y, hitbox1.Width, hitbox1.Height);
                        // cdgRect.Y -= enemyObj.AnchorY * enemyObj.ScaleY;

                        camera.Draw(GameController.GenericTexture, cdgRect.ToRectangle(), Color.Red);
                        info += "w: " + cdgRect.ToRectangle().ToString() + "\n";


                        for (int index6 = 0; index6 < globalEnemyArrayCount; ++index6)
                        {
                            EnemyObj other = arenaScreen.currentAndGlobalEnemyArray[index6];
                            EnemyObj collObj = other;
                            if (collObj.Collidable && collObj.Active &&
                                ((!enemyObj.rigidBody || !collObj.rigidBody) && collObj != enemyObj))
                            {
                                // collObj.m_useCachedAbsPosition = false;
                                // collObj.HitboxesCalculated = false;
                                // collObj.UpdateHitboxes();
                                Hitbox[] hitboxesArray = collObj.HitboxesArray;
                                int hitboxesCount = collObj.HitboxesCount;
                                for (int index3 = 0; index3 < hitboxesCount; ++index3)
                                {
                                    Hitbox hitbox = hitboxesArray[index3];
                                        if (hitbox1.Type == HitboxType.Weapon && hitbox.Type == HitboxType.Body &&
                                            collObj.weaponCollidable || hitbox1.Type == HitboxType.Body &&
                                            hitbox.Type == HitboxType.Weapon)
                                        {
                                            CDGRect hb1Rect = new CDGRect(hitbox1.X, hitbox1.Y, hitbox1.Width, hitbox1.Height);
                                            CDGRect hbOther = new CDGRect(hitbox.X, hitbox.Y, hitbox.Width, hitbox.Height);

                                            Vector2 mtd = CDGMath.RotatedRectIntersectsMTD(hb1Rect,
                                                hitbox1.Rotation, Vector2.Zero, hbOther, hitbox.Rotation,
                                                Vector2.Zero);
                                            if (mtd != Vector2.Zero)
                                            {
                                                camera.Draw(GameController.GenericTexture, cdgRect.ToRectangle(),
                                                    collObj.CollisionID == enemyObj.CollisionID ? Color.Coral : Color.Yellow);
                                            }
                                        }
                                }
                            }
                        }
                    }

                    if (hitbox1.Type == HitboxType.Bounds)
                    {
                        CDGRect cdgRect = new CDGRect(hitbox1.X, hitbox1.Y, hitbox1.Width, hitbox1.Height);
                        cdgRect.Y -= enemyObj.AnchorY * enemyObj.ScaleY;

                        camera.Draw(GameController.GenericTexture, cdgRect.ToRectangle(), Color.Green);
                    }

                }

                if (!this.colInfo.ContainsKey(currentAndGlobalEnemy))
                {
                    this.colInfo.Add(currentAndGlobalEnemy, "");
                }

                this.colInfo[currentAndGlobalEnemy] = info;

                if (!this.colInfoText.ContainsKey(currentAndGlobalEnemy))
                {
                    var text = new BrawlerTextObj("Banger35");
                    text.HeightAnchorAlign = HeightAlign.Centre;
                    text.FontSize = 8f;
                    text.forceDraw = true;
                    this.colInfoText.Add(currentAndGlobalEnemy, text);
                }

                var textObj = this.colInfoText[currentAndGlobalEnemy];
                textObj.Text = info;
                textObj.Position =
                    (enemyObj.shadowPosition + enemyObj.inAirOffset) * camera.Zoom;
                textObj.Y += 7f;
                textObj.Draw(camera, elapsedSeconds);
            }




            // camera.End();
        }

        // public override void DrawAimingCone(Camera2D camera, float elapsedSeconds)
        // {
        //     if( this.target == null)
        //         return;
        //
        //     this.startJumpPosition = this.Position;
        //     var targetDistance = CDGMath.VectorBetweenPts(this.startJumpPosition, this.target.Position);
        //     float y = this.target.Y - this.target.Bounds.Height;
        //     if (targetDistance.Y > 0.0)
        //         y = this.startJumpPosition.Y - this.target.Bounds.Height;
        //
        //     // var vector2_1 = new Vector2(this.target.Position.X - targetDistance.X / 2f, y);
        //     var vector2_1 = new Vector2(this.target.Position.X, y);
        //     var vector2_2 = new Vector2(this.startJumpPosition.X, vector2_1.Y);
        //     var pts = new List<Vector2>();
        //     pts.Clear();
        //     pts.Add(this.startJumpPosition);
        //     pts.Add(vector2_2);
        //     pts.Add(vector2_1);
        //     // float num = 0.1f;
        //     int num1 = (int) 10;
        //     float num2 = 0.5f / (float) num1;
        //
        //     var lastPoint = this.Position;
        //     for (int index = 0; index < num1; ++index)
        //     {
        //         var bezierPt = BezierHelper.GetBezierPt(pts, index / (float) num1);
        //         // float width = CDGMath.DistanceBetweenPts(lastPoint, bezierPt);
        //         // float degrees = CDGMath.AngleBetweenPts(lastPoint, bezierPt);
        //         var cdgRect = new CDGRect(bezierPt.X, bezierPt.Y, 5f, 5f);
        //         lastPoint = bezierPt;
        //         camera.Draw(GameController.GenericTexture, cdgRect.ToRectangle(), new Rectangle?(), Color.Red, 1f, Vector2.Zero, SpriteEffects.None, 1f);
        //     }
        // //    RenderTarget2D currentRenderTarget = camera.currentRenderTarget;
        // //   camera.GraphicsDevice.SetRenderTarget(PlayerClassObj_Engineer.m_attackCircleRT);
        // //   camera.GraphicsDevice.Clear(Color.Transparent);
        // //   camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, camera.TransformMatrix);
        // //   this.m_attackRadiusFilledSprite.Draw(camera, elapsedSeconds);
        // //   camera.End();
        // //   camera.GraphicsDevice.SetRenderTarget(PlayerClassObj_Engineer.m_attackTriangleRT);
        // //   camera.GraphicsDevice.Clear(Color.Transparent);
        // //   camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, camera.TransformMatrix);
        // //   this.m_attackTriangleSprite.Draw(camera, elapsedSeconds);
        // //   camera.End();
        // //   this.Game.shaderManager.Begin(ShaderType.InvertMask);
        // //   camera.GraphicsDevice.Textures[1] = (Texture) PlayerClassObj_Engineer.m_attackCircleRT;
        // //   camera.GraphicsDevice.SetRenderTarget(this.Game.shaderManager.spareRT);
        // //   camera.GraphicsDevice.Clear(Color.Transparent);
        // //   camera.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
        // //   camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState) null, (RasterizerState) null, this.Game.shaderManager.GetShader(ShaderType.InvertMask));
        // //   camera.Draw((Texture2D) PlayerClassObj_Engineer.m_attackTriangleRT, Vector2.Zero, Color.White * 0.5f);
        // //   camera.End();
        // //   this.Game.shaderManager.End();
        // //   camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, (DepthStencilState) null, (RasterizerState) null, (Effect) null, camera.TransformMatrix);
        // //   this.m_attackRadiusOutlineSprite.Draw(camera, elapsedSeconds);
        // //   camera.End();
        // //   this.Game.shaderManager.Begin(ShaderType.InvertMask);
        // //   camera.GraphicsDevice.SetRenderTarget(currentRenderTarget);
        // //   camera.GraphicsDevice.Textures[1] = (Texture) this.Game.ScreenManager.arenaScreen.transparencyRT;
        // //   camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState) null, (RasterizerState) null, this.Game.shaderManager.GetShader(ShaderType.InvertMask));
        // //   camera.Draw((Texture2D) this.Game.shaderManager.spareRT, Vector2.Zero, Color.White);
        // //   camera.End();
        // //   this.Game.shaderManager.End();
        // }

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
