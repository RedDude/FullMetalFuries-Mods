using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Brawler2D;
using CDGEngine;
using cs.Blit;
using FullModdedFuriesAPI.Mods.MercenaryClass.Source.ControllableEnemies.ControllableEnemyData;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FullModdedFuriesAPI.Mods.MercenaryClass.Source.ControllableEnemies
{
    public class EnemyControllableManager
    {
        private FieldInfo wanderLogicField;
        private FieldInfo wanderParamsField;
        private FieldInfo meleeLogicField;
        private FieldInfo meleeParamsField;
        private FieldInfo projectileLogicField;
        private FieldInfo projectileParamsField;

        static public Dictionary<EnemyObj, LogicSetState> logicBlockStates = new Dictionary<EnemyObj, LogicSetState>();

        static public Dictionary<Type, IEnemyControllable> ControllableData = new();

        static public List<EnemyObj> _activeControlledEnemies = new();

        static public List<ProjectileObj> changeColorFeedback = new();
        private static bool baseInstalled;


        public EnemyControllableManager()
        {
            var type = typeof(EnemyObj);
            this.wanderLogicField = AccessTools.Field(type, "m_wanderRangeLB");
            this.wanderParamsField = AccessTools.Field(type, "m_wanderLBParams");

            this.meleeLogicField = AccessTools.Field(type, "m_meleeRangeLB");
            this.meleeParamsField = AccessTools.Field(type, "m_meleeLBParams");

            this.projectileLogicField = AccessTools.Field(type, "m_projectileRangeLB");
            this.projectileParamsField = AccessTools.Field(type, "m_projectileLBParams");

            if(!baseInstalled)
                InstallBaseEnemyControllableData();

            // this.currentLBField = AccessTools.Field(type, "m_currentLB");

        }

        public class LogicSetState
        {
            public LogicBlock wanderRangeLogic;
            public int[] wanderRangeLogicParams;

            public LogicBlock meleeRangeLogic;
            public int[] meleeRangeLogicParams;

            public LogicBlock projectileRangeLogic;
            public int[] projectileRangeLogicParams;
            public Vector2 hudOffset;
            public LogicBlock controllableLogic;
            public PlayerObj player;
            public int storedCollisionID;
        }

        public void Update()
        {
            for (int index = _activeControlledEnemies.Count - 1; index >= 0; index--)
            {
                var activeControlledEnemy = _activeControlledEnemies[index];
                if (activeControlledEnemy.IsDisposed || activeControlledEnemy.IsKilled || activeControlledEnemy.jumpDistance > 0)
                {
                    _activeControlledEnemies.RemoveAt(index);
                    logicBlockStates.Remove(activeControlledEnemy);
                    continue;
                }

                var logicBlockState = logicBlockStates[activeControlledEnemy];
                if (activeControlledEnemy.m_currentLB == logicBlockState.meleeRangeLogic ||
                    activeControlledEnemy.m_currentLB == logicBlockState.projectileRangeLogic ||
                    activeControlledEnemy.m_currentLB == logicBlockState.wanderRangeLogic)
                {
                    activeControlledEnemy.ExecuteLB(logicBlockState.controllableLogic, new int[1] {100});
                }


            }
        }

        public void MakeEnemyControllable(EnemyObj enemyObj, PlayerObj player)
        {
            if (_activeControlledEnemies.Contains(enemyObj))
                return;

            _activeControlledEnemies.Add(enemyObj);

            if (logicBlockStates.ContainsKey(enemyObj))
            {
                var logicSetStates = logicBlockStates[enemyObj];
                // logicSetState.meleeRangeLogic?.AddLogicSet(controlEnemyLogicSet);
                // logicSetState.projectileRangeLogic?.AddLogicSet(controlEnemyLogicSet);

                // StopAndMakePlayable(enemyObj, logicSetStates);
                this.StopAndReplace(enemyObj, logicSetStates);

                return;
            }

            // if (!this._logicBlockStates.ContainsKey(enemyObj))
            {

            }

            var enemyControllableLogicSet =
                new EnemyControllableLogicSet(enemyObj, player, _activeControlledEnemies, 0f, false);

            var logicBlock = new LogicBlock(RNG.seed);
            // var idleLogic = new LogicSet("IDLE");
            // LogicSet.Begin(idleLogic);
            // // LogicSet.Add(new ChangePropertyAction(enemyObj, "State", (object) CharacterState.Idle));
            // // LogicSet.Add(new ChangePropertyAction(enemyObj, "CurrentSpeed", (object) 0));
            // // LogicSet.Add(new PlayAnimationAction(enemyObj, "StandStart", "StandEnd", true));
            // LogicSet.Add(new ChangeColourAction(enemyObj, Color.Yellow, 20f, -1));
            // LogicSet.End();

            var controlEnemyLogicSet = new LogicSet("Control_Enemy");
            LogicSet.Begin(controlEnemyLogicSet);
            // LogicSet.Add(new ChangeColourAction(enemyObj, Color.Yellow, 20f, -1), SequenceType.Parallel);
            // LogicSet.Add(new ChangePropertyAction((object) enemyObj, "State", (object) CharacterState.Walking),
            // SequenceType.Serial);
            // LogicSet.Add(new PlayAnimationAction((DisplayObj) enemyObj, "RunStart", "RunEnd", true, false, false),
            // SequenceType.Serial);
            // LogicSet.Add((LogicAction) new ChangePropertyAction((object) this, "CurrentSpeed", (object) spawnEnemy.Speed), SequenceType.Serial);

            // LogicSet.Add(new ChangeColourAction(enemyObj, Color.Yellow, 20f, -1),
            // SequenceType.Serial);


            LogicSet.Add(enemyControllableLogicSet, SequenceType.Serial);
            // LogicSet.Add(new RunLogicSetAction(idleLogic), SequenceType.Serial);
            LogicSet.End();
            logicBlock.AddLogicSet(controlEnemyLogicSet);

            // var empty = new LogicBlock(RNG.seed);
            // empty.AddLogicSet(idleLogic);

            logicBlockStates.Add(enemyObj, new LogicSetState()
            {
                wanderRangeLogic = ((LogicBlock) this.wanderLogicField.GetValue(enemyObj)),
                wanderRangeLogicParams = (int[]) this.wanderParamsField.GetValue(enemyObj),
                meleeRangeLogic = (LogicBlock) this.meleeLogicField.GetValue(enemyObj),
                meleeRangeLogicParams = (int[]) this.meleeParamsField.GetValue(enemyObj),
                projectileRangeLogic = (LogicBlock) this.projectileLogicField.GetValue(enemyObj),
                projectileRangeLogicParams = (int[]) this.projectileParamsField.GetValue(enemyObj),
                hudOffset = enemyObj.hudOffset,
                controllableLogic = logicBlock,
                player = player,
                storedCollisionID = enemyObj.CollisionID
            });

            // enemyObj.rigidBody = true;
            enemyObj.CollisionID = 55;
            var logicSetState = logicBlockStates[enemyObj];
            // logicSetState.wanderRangeLogic?.ActiveLS?.AddActionAt(enemyControllableLogicSet, 0);
            // logicSetState.meleeRangeLogic?.ActiveLS?.AddActionAt(enemyControllableLogicSet, 0);
            // logicSetState.projectileRangeLogic?.ActiveLS?.AddActionAt(enemyControllableLogicSet, 0);
            // // logicSetState.meleeRangeLogic?.AddLogicSet(controlEnemyLogicSet);
            // // logicSetState.projectileRangeLogic?.AddLogicSet(controlEnemyLogicSet);
            //
            // logicSetState.wanderRangeLogic?.ActiveLS?.Reset();
            // logicSetState.meleeRangeLogic?.ActiveLS?.Reset();
            // logicSetState.projectileRangeLogic?.ActiveLS?.Reset();
            //
            // enemyObj.hudOffset = new Vector2(enemyObj.hudOffset.X, -enemyObj.AbsBounds.Height);

            // StopAndMakePlayable(enemyObj, logicSetState);
            this.StopAndReplace(enemyObj, logicSetState);

            // this.wanderLogicField.SetValue(enemyObj, null);
            // this.meleeLogicField.SetValue(enemyObj, null);
            // this.projectileLogicField.SetValue(enemyObj, null);

            // enemyObj.m_currentLB = logicBlock;

        }

        private void StopAndReplace(EnemyObj enemyObj, LogicSetState logicSetStates)
        {
            // enemyObj.m_currentLB?.ActiveLS.Reset();

            enemyObj.StopAllActions();

            if (enemyObj.State == CharacterState.Walking || enemyObj.State == CharacterState.Idle ||
                enemyObj.State == CharacterState.Attacking)
                if (enemyObj.GetFrameFromLabel("StunStart") != -1)
                {
                    enemyObj.StopAnimation();
                    enemyObj.PlayAnimation("StunStart", "StunEnd", false, false);
                }

            enemyObj.ExecuteLB(logicSetStates.controllableLogic, new int[1] {100});

            // enemyObj.m_currentLB.Stop();
            // enemyObj.m_currentLB = null;
            // enemyObj.State = CharacterState.Idle;

            // logicSetState.meleeRangeLogic?.ActiveLS?.Reset();
            // logicSetState.projectileRangeLogic?.ActiveLS?.Reset();



            // this.wanderLogicField.SetValue(enemyObj, logicSetStates.controllableLogic);
            // this.wanderParamsField.SetValue(enemyObj, new int[1] {100});
            // this.meleeLogicField.SetValue(enemyObj, logicSetStates.controllableLogic);
            // this.meleeParamsField.SetValue(enemyObj, new int[1] {100});
            // this.projectileLogicField.SetValue(enemyObj, logicSetStates.controllableLogic);
            // this.projectileParamsField.SetValue(enemyObj, new int[1] {100});
        }

        private void StopAndRevert(EnemyObj enemyObj, LogicSetState logicSetStates)
        {
            if (enemyObj.m_currentLB == logicSetStates.controllableLogic)
            {
                enemyObj.m_currentLB.Stop();
            }
            enemyObj.displayArrowIndicator = true;
            enemyObj.CurrentSpeed = 0.0f;
            enemyObj.State = CharacterState.Idle;

            enemyObj.currentTarget = logicSetStates.player.currentPlayerClass;
            enemyObj.m_currentLB = null;

            enemyObj.CollisionID = logicSetStates.storedCollisionID;
            logicSetStates.player.currentPlayerClass.shadowWidth = 1f;
            // this.wanderLogicField.SetValue(enemyObj, logicSetStates.wanderRangeLogic);
            // this.wanderParamsField.SetValue(enemyObj, logicSetStates.wanderRangeLogicParams);
            // this.meleeLogicField.SetValue(enemyObj, logicSetStates.meleeRangeLogic);
            // this.meleeParamsField.SetValue(enemyObj, logicSetStates.meleeRangeLogicParams);
            // this.projectileLogicField.SetValue(enemyObj, logicSetStates.projectileRangeLogic);
            // this.projectileParamsField.SetValue(enemyObj, logicSetStates.wanderRangeLogicParams);
        }

        // private static void StopAndMakePlayable(EnemyObj enemyObj, LogicSetState logicSetStates)
        // {
        //     logicSetStates.wanderRangeLogic?.ActiveLS?.Stop();
        //     // logicSetStates.wanderRangeLogic?.ActiveLS?.Reset();
        //     // logicSetStates.wanderRangeLogic?.ActiveLS?.ForceExecute();
        //
        //     logicSetStates.meleeRangeLogic?.ActiveLS?.Stop();
        //     // logicSetStates.meleeRangeLogic?.ActiveLS?.Reset();
        //     // logicSetStates.meleeRangeLogic?.ActiveLS?.ForceExecute();
        //
        //     logicSetStates.projectileRangeLogic?.ActiveLS?.Stop();
        //     // logicSetStates.projectileRangeLogic?.ActiveLS?.Reset();
        //     // logicSetStates.projectileRangeLogic?.ActiveLS?.ForceExecute();
        //
        //     // enemyObj.m_currentLB = logicSetStates.controllableLogic;
        //     // enemyObj.m_currentLB.Execute((Dictionary<string, float>) null, 50, 50);
        // }

        public void UnmakeEnemyControllable(EnemyObj enemyObj)
        {
            if (!logicBlockStates.ContainsKey(enemyObj))
                return;

            var logicSetState = logicBlockStates[enemyObj];
            // var activeLs = logicSetState.wanderRangeLogic?.ActiveLS;
            // if (activeLs != null)
            // {
            //     activeLs.acto
            // }
            // activeLs?.AddAction(new EnemyControllableLogicSet(enemyObj, 0f, false, this._logicBlockStates));
            // logicSetState.meleeRangeLogic?.ActiveLS?.AddAction(new EnemyControllableLogicSet(enemyObj, 0f, false));
            // logicSetState.projectileRangeLogic?.ActiveLS?.AddAction(new EnemyControllableLogicSet(enemyObj, 0f, false));

            // var w = (LogicBlock) this.wanderLogicField.GetValue(enemyObj);
            // var m = (LogicBlock) this.meleeLogicField.GetValue(enemyObj);
            // var p = (LogicBlock) this.projectileLogicField.GetValue(enemyObj);
            //
            // enemyObj.m_currentLB = null;
            //
            // var logicSetState = this._logicBlockStates[enemyObj];
            // int num = 0;
            // for (int index = 0; index < logicSetState.wanderRangeLogicParams.Length; ++index)
            //     num += logicSetState.wanderRangeLogicParams[index];
            // if (num == 100)
            //     enemyObj.SetWanderRangeLogic(logicSetState.wanderRangeLogic, logicSetState.wanderRangeLogicParams);
            //
            // for (int index = 0; index < logicSetState.meleeRangeLogicParams.Length; ++index)
            //     num += logicSetState.meleeRangeLogicParams[index];
            // if (num == 100)
            //     enemyObj.SetMeleeRangeLogic(logicSetState.MeleeRangeLogic, logicSetState.meleeRangeLogicParams);
            //
            // for (int index = 0; index < logicSetState.projectileRangeLogicParams.Length; ++index)
            //     num += logicSetState.projectileRangeLogicParams[index];
            // if (num == 100)
            //     enemyObj.SetProjectileRangeLogic(logicSetState.projectileRangeLogic, logicSetState.projectileRangeLogicParams);
            //
            // w.Dispose();
            // m.Dispose();
            // p.Dispose();

            // var logicSetState = this._logicBlockStates[enemyObj];
            // var e = logicSetState.wanderRangeLogic?.LogicSetList.Find(l => l.Name == "Control_Enemy");
            // logicSetState.wanderRangeLogic?.LogicSetList.Remove(e);
            //
            // e = logicSetState.meleeRangeLogic?.LogicSetList.Find(l => l.Name == "Control_Enemy");
            // logicSetState.meleeRangeLogic?.LogicSetList.Remove(e);
            //
            // e = logicSetState.projectileRangeLogic?.LogicSetList.Find(l => l.Name == "Control_Enemy");
            // logicSetState.projectileRangeLogic?.LogicSetList.Remove(e);

            this.StopAndRevert(enemyObj, logicSetState);
            enemyObj.hudOffset = logicSetState.hudOffset;
            _activeControlledEnemies.Remove(enemyObj);
            enemyObj.CollisionID = 1;
            // logicSetState.wanderRangeLogic?.ActiveLS?.Reset();
            // logicSetState.wanderRangeLogic?.ActiveLS?.ForceExecute();
            // logicSetState.meleeRangeLogic?.ActiveLS?.Reset();
            // logicSetState.meleeRangeLogic?.ActiveLS?.ForceExecute();
            // logicSetState.projectileRangeLogic?.ActiveLS?.Reset();
            // logicSetState.projectileRangeLogic?.ActiveLS?.ForceExecute();

            // this._logicBlockStates.Remove(enemyObj);
        }

        // public void DrawProjectileFeedback(Camera2D camera, float elapsedSeconds)
        // {
        //     for (int index = changeColorFeedback.Count - 1; index >= 0; index--)
        //     {
        //         var projectileObj = changeColorFeedback[index];
        //         if (projectileObj == null || projectileObj.IsDisposed)
        //             changeColorFeedback.Remove(projectileObj);
        //
        //         switch (projectileObj.projectileType)
        //         {
        //             case ProjectileType.MineRadius_TriggerPlayer:
        //             case ProjectileType.MineRadius_TriggerEnemy:
        //             case ProjectileType.MineRadius_TriggerAll:
        //                 Vector2 scale =
        //                     new Vector2(
        //                         projectileObj.collisionEllipse.radiusX * 2f /
        //                         (float) GameController.GenericCircle.Width,
        //                         projectileObj.collisionEllipse.radiusY * 2f /
        //                         (float) GameController.GenericCircle.Height);
        //                 var color = PlayerClassObj_Mercenary.MERCENARY_COLOUR;
        //                 if ((double) scale.X < 0.5)
        //                 {
        //                     scale = new Vector2(
        //                         projectileObj.collisionEllipse.radiusX * 2f /
        //                         (float) GameController.GenericCircleSmall.Width,
        //                         projectileObj.collisionEllipse.radiusY * 2f /
        //                         (float) GameController.GenericCircleSmall.Height);
        //                     camera.Draw(GameController.GenericCircleSmall,
        //                         new Vector2(projectileObj.collisionEllipse.x, projectileObj.collisionEllipse.y),
        //                         new Rectangle?(), color, 0.0f, new Vector2(50f, 50f), scale, SpriteEffects.None,
        //                         1f);
        //                     break;
        //                 }
        //
        //                 camera.Draw(GameController.GenericCircle,
        //                     new Vector2(projectileObj.collisionEllipse.x, projectileObj.collisionEllipse.y),
        //                     new Rectangle?(), color, 0.0f, new Vector2(250f, 250f), scale, SpriteEffects.None, 1f);
        //                 break;
        //         }
        //     }
        // }

        public static void DrawProjectileFeedback(ProjectileObj projectileObj, Camera2D camera, float elapsedSeconds)
        {
            bool has = changeColorFeedback.Contains(projectileObj);
            if (!has)
                return;

            if (projectileObj == null || projectileObj.IsDisposed)
                changeColorFeedback.Remove(projectileObj);

            switch (projectileObj.projectileType)
            {
                case ProjectileType.MineRadius_TriggerPlayer:
                case ProjectileType.MineRadius_TriggerEnemy:
                case ProjectileType.MineRadius_TriggerAll:
                    Vector2 scale =
                        new Vector2(
                            projectileObj.collisionEllipse.radiusX * 2f /
                            (float) GameController.GenericCircle.Width,
                            projectileObj.collisionEllipse.radiusY * 2f /
                            (float) GameController.GenericCircle.Height);
                    var color = PlayerClassObj_Mercenary.MERCENARY_COLOUR;
                    if ((double) scale.X < 0.5)
                    {
                        scale = new Vector2(
                            projectileObj.collisionEllipse.radiusX * 2f /
                            (float) GameController.GenericCircleSmall.Width,
                            projectileObj.collisionEllipse.radiusY * 2f /
                            (float) GameController.GenericCircleSmall.Height);
                        camera.Draw(GameController.GenericCircleSmall,
                            new Vector2(projectileObj.collisionEllipse.x, projectileObj.collisionEllipse.y),
                            new Rectangle?(), color, 0.0f, new Vector2(50f, 50f), scale, SpriteEffects.None,
                            1f);
                        break;
                    }

                    camera.Draw(GameController.GenericCircle,
                        new Vector2(projectileObj.collisionEllipse.x, projectileObj.collisionEllipse.y),
                        new Rectangle?(), color, 0.0f, new Vector2(250f, 250f), scale, SpriteEffects.None, 1f);
                    break;
            }
        }

        public static void RunEnemyAttack(EnemyObj enemyObj, ref LogicSet toRunLogic, ref LogicBlock toRunBlock)
        {
            enemyObj.StopAllActions();

            // if (toRunBlock == null)
            // {
            // toRunBlock = new LogicBlock(RNG.seed);
            // //     toRunBlock.AddLogicSet(toRunLogic);
            // }

            toRunBlock = new LogicBlock(RNG.seed);
            toRunBlock.AddLogicSet(toRunLogic);
            enemyObj.ExecuteLB(toRunBlock, new int[1] {100});

            // enemyObj.m_currentLB = toRunBlock;
            // enemyObj.m_currentLB.Execute(null, 100);
        }

        private static void InstallBaseEnemyControllableData()
        {
            // if (!ControllableData.ContainsKey(typeof(Enemy_Artillerist_Basic)))
            {
                ControllableData.Add(typeof(Enemy_Artillerist_Basic), new Artillerist_ControllableData());
                ControllableData.Add(typeof(Enemy_Artillerist_Advanced), new Artillerist_ControllableData());
                ControllableData.Add(typeof(Enemy_Artillerist_Expert), new Artillerist_ControllableData());
            }

            // if (!ControllableData.ContainsKey(typeof(Enemy_BlackMask_Basic)))
            {
                ControllableData.Add(typeof(Enemy_BlackMask_Basic), new BlackMask_ControllableData());
                ControllableData.Add(typeof(Enemy_BlackMask_Advanced), new BlackMask_ControllableData());
                ControllableData.Add(typeof(Enemy_BlackMask_Expert), new BlackMask_ControllableData());
            }

            // if (!ControllableData.ContainsKey(typeof(Enemy_Buffer_Basic)))
            {
                ControllableData.Add(typeof(Enemy_Buffer_Basic), new Buffer_ControllableData());
                ControllableData.Add(typeof(Enemy_Buffer_Advanced), new Buffer_ControllableData());
                ControllableData.Add(typeof(Enemy_Buffer_Expert), new Buffer_ControllableData());
            }

            // if (!ControllableData.ContainsKey(typeof(Enemy_GiantBrute_Basic)))
            {
                ControllableData.Add(typeof(Enemy_GiantBrute_Basic), new GiantBrute_ControllableData());
                ControllableData.Add(typeof(Enemy_GiantBrute_Advanced), new GiantBrute_ControllableData());
                ControllableData.Add(typeof(Enemy_GiantBrute_Expert), new GiantBrute_ControllableData());
            }

            // if (!ControllableData.ContainsKey(typeof(Enemy_Grenadier_Basic)))
            {
                ControllableData.Add(typeof(Enemy_Grenadier_Basic), new Grenadier_ControllableData());
                ControllableData.Add(typeof(Enemy_Grenadier_Advanced), new Grenadier_ControllableData());
                ControllableData.Add(typeof(Enemy_Grenadier_Expert), new Grenadier_ControllableData());
            }

            // if (!ControllableData.ContainsKey(typeof(Enemy_Hexxer_Basic)))
            {
                ControllableData.Add(typeof(Enemy_Hexxer_Basic), new Hexxer_ControllableData());
                ControllableData.Add(typeof(Enemy_Hexxer_Advanced), new Hexxer_ControllableData());
                ControllableData.Add(typeof(Enemy_Hexxer_Expert), new Hexxer_ControllableData());
            }

            // if (!ControllableData.ContainsKey(typeof(Enemy_Hoplite_Basic)))
            {
                ControllableData.Add(typeof(Enemy_Hoplite_Basic), new Hoplite_ControllableData());
                ControllableData.Add(typeof(Enemy_Hoplite_Advanced), new Hoplite_ControllableData());
                ControllableData.Add(typeof(Enemy_Hoplite_Expert), new Hoplite_ControllableData());
            }

            // if (!ControllableData.ContainsKey(typeof(Enemy_Leech_Basic)))
            {
                ControllableData.Add(typeof(Enemy_Leech_Basic), new Leech_ControllableData());
                ControllableData.Add(typeof(Enemy_Leech_Advanced), new Leech_ControllableData());
                ControllableData.Add(typeof(Enemy_Leech_Expert), new Leech_ControllableData());
            }

            // if (!ControllableData.ContainsKey(typeof(Enemy_Lycaon_Basic)))
            {
                ControllableData.Add(typeof(Enemy_Lycaon_Basic), new Lycaon_ControllableData());
                ControllableData.Add(typeof(Enemy_Lycaon_Advanced), new Lycaon_ControllableData());
                ControllableData.Add(typeof(Enemy_Lycaon_Expert), new Lycaon_ControllableData());
            }

            // if (!ControllableData.ContainsKey(typeof(Enemy_MGTurret_Basic)))
            {
                ControllableData.Add(typeof(Enemy_MGTurret_Basic), new MGTurret_ControllableData());
                ControllableData.Add(typeof(Enemy_MGTurret_Advanced), new MGTurret_ControllableData());
                ControllableData.Add(typeof(Enemy_MGTurret_Expert), new MGTurret_ControllableData());
            }

            // if (!ControllableData.ContainsKey(typeof(Enemy_Minitaur_Basic)))
            {
                ControllableData.Add(typeof(Enemy_Minitaur_Basic), new Minitaur_ControllableData());
                ControllableData.Add(typeof(Enemy_Minitaur_Advanced), new Minitaur_ControllableData());
                ControllableData.Add(typeof(Enemy_Minitaur_Expert), new Minitaur_ControllableData());
            }

            // if (!ControllableData.ContainsKey(typeof(Enemy_ShieldKnight_Basic)))
            {
                ControllableData.Add(typeof(Enemy_ShieldKnight_Basic), new ShieldKnight_ControllableData());
                ControllableData.Add(typeof(Enemy_ShieldKnight_Advanced), new ShieldKnight_ControllableData());
                ControllableData.Add(typeof(Enemy_ShieldKnight_Expert), new ShieldKnight_ControllableData());
            }

            // if (!ControllableData.ContainsKey(typeof(Enemy_Sniper_Basic)))
            {
                ControllableData.Add(typeof(Enemy_Sniper_Basic), new Sniper_ControllableData());
                ControllableData.Add(typeof(Enemy_Sniper_Advanced), new Sniper_ControllableData());
                ControllableData.Add(typeof(Enemy_Sniper_Expert), new Sniper_ControllableData());
            }

            // if (!ControllableData.ContainsKey(typeof(Enemy_Sparker_Basic)))
            {
                ControllableData.Add(typeof(Enemy_Sparker_Basic), new Sparker_ControllableData());
                ControllableData.Add(typeof(Enemy_Sparker_Advanced), new Sparker_ControllableData());
                ControllableData.Add(typeof(Enemy_Sparker_Expert), new Sparker_ControllableData());
            }

            // if (!ControllableData.ContainsKey(typeof(Enemy_Spinner_Basic)))
            {
                ControllableData.Add(typeof(Enemy_Spinner_Basic), new Spinner_ControllableData());
                ControllableData.Add(typeof(Enemy_Spinner_Advanced), new Spinner_ControllableData());
                ControllableData.Add(typeof(Enemy_Spinner_Expert), new Spinner_ControllableData());
            }

            // if (!ControllableData.ContainsKey(typeof(Enemy_TimeBomb_Basic)))
            {
                ControllableData.Add(typeof(Enemy_TimeBomb_Basic), new TimeBomb_ControllableData());
                ControllableData.Add(typeof(Enemy_TimeBomb_Advanced), new TimeBomb_ControllableData());
                ControllableData.Add(typeof(Enemy_TimeBomb_Expert), new TimeBomb_ControllableData());
            }

            // if (!ControllableData.ContainsKey(typeof(Enemy_WallCannon_Basic)))
            {
                ControllableData.Add(typeof(Enemy_WallCannon_Basic), new WallCannon_ControllableData());
                ControllableData.Add(typeof(Enemy_WallCannon_Advanced), new WallCannon_ControllableData());
                ControllableData.Add(typeof(Enemy_WallCannon_Expert), new WallCannon_ControllableData());
            }

            baseInstalled = true;
        }
    }
}
