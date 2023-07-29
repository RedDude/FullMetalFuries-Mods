using System;
using System.Collections;
using System.Collections.Generic;
using Brawler2D;
using CDGEngine;
using cs.Blit;
using FullModdedFuriesAPI.Events;
using FullModdedFuriesAPI.Framework.ModHelpers.CampHelper;
using FullModdedFuriesAPI.Framework.ModHelpers.DatabaseHelper;
using FullModdedFuriesAPI.Mods.MercenaryClass.Framework;
using FullModdedFuriesAPI.Mods.MercenaryClass.Source;
using FullModdedFuriesAPI.Mods.TitanClass.Source;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FullModdedFuriesAPI.Mods.MercenaryClass
{
    /// <summary>The main entry point for the mod.</summary>
    public class ModEntry : Mod
    {
        private ModConfig Config;
        private GameController game;

        public static IModHelper modHelper;
        public static IMonitor monitor;

        private SetupCharacter setupCharacter;

        private CharacterPatch _patch;
        private CharacterSelectionPatcher characterSelectionPatcher;
        private EnemyObjPatch enemyObjPatch;

        private SaveHandler saveHandler;
        // private InteractableObject interactableObject;
        private CustomCharacterData customData;
        private CampHandler campHandler;
        private InterfaceHandler interfaceHandler;
        private ShopkeeperCampHandler shopkeeperCampHandler;
        private SpeechBubble speechBubble;


        private float deliveryTime = 0f;
        private InteractableObject interactableObject;

        private ProjectileMileDrawPatcher projectileMileDrawPatcher;

        private EnemyObj moving = null;
        // private Effect grayScale;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            modHelper = this.Helper;
            this.Config = this.Helper.ReadConfig<ModConfig>();
            monitor = this.Monitor;

            this.game = GameController.g_game;
            // TrailerEV.displayDebug

            // var Assembly = typeof(GameController).Assembly;
            // var TrailerEV = Assembly.GetType("Brawler2D.TrailerEV");
            // var value = this.Helper.Reflection.GetField<bool>(TrailerEV, "displayDebug");
            // value.SetValue(true);

            //to FMODF
            this._patch = new CharacterPatch(this.game, helper);
            this._patch.Patch();

            this.characterSelectionPatcher = new CharacterSelectionPatcher(this.game, helper);
            this.characterSelectionPatcher.Patch();

            this.projectileMileDrawPatcher = new ProjectileMileDrawPatcher(this.game, helper);
            this.projectileMileDrawPatcher.Patch();

            this.enemyObjPatch = new EnemyObjPatch(this.game, helper);
            this.enemyObjPatch.Patch();

            this.ApplySkin();

            this.SetupCharacter(helper);
            this.SetupTitanCharacter(helper);

            // if (helper.Content.GetPatchHelper().TryParseManagedAssetKey(assetName, out string contentManagerID, out string relativePath))
            // {
            //     if (contentManagerID != this.Name)
            //         throw new SContentLoadException($"Can't load managed asset key '{assetName}' through content manager '{this.Name}' for a different mod.");
            //     assetName = relativePath;
            // }
            //
            // ParseCustomLevelDataHelper.ParseLevelData(this.game, (BrawlerGameScreen) this.game.ScreenManager.m_loadingScreen, helper.Content.);
        }

        private void SetupCharacter(IModHelper helper)
        {
            this.customData = new MercenarySetupCharacter(this.game, helper).Create();
            this.setupCharacter = new SetupCharacter(this.game, helper);
            this.setupCharacter.Setup(this.customData,
                playerObj => new PlayerClassObj_Mercenary(GameController.g_game, playerObj, this.Helper));

            var mercenaryDummySetup = new MercenaryDummySetup(this.game, helper);
            mercenaryDummySetup.Create(this.game);

            this.Helper.Events.GameLoop.UpdateTicking += (sender, args) =>
            {
                if (!Context.IsGameLaunched)
                    return;

                if (this.game.ScreenManager?.arenaScreen == null)
                    return;

                if (this.game.ScreenManager.arenaScreen.IsPaused)
                    return;

                if (this.game.PlayerManager.getMainPlayerOrHost == null)
                    return;

                if (!this.IsCamp()) return;
                if (this.shopkeeperCampHandler == null)
                {
                    var database = this.Helper.Database;
                    // classType unfortunately need to be get async
                    var classType = (ClassType) database.GetCustomClasses()["Mercenary"];
                    this.shopkeeperCampHandler = new ShopkeeperCampHandler(this.game, helper, classType);


                    this.game.ScreenManager.arenaScreen.StopMusic();
                    this.game.ScreenManager.arenaScreen.PlayMusic(SongType.World1_BaseCamp_Old);
                }

                this.shopkeeperCampHandler.SwapInCamp(); //better create a watcher for the swap


                if (this.game.InputManager.KeyJustPressed(Keys.J))
                {
                    this.Helper.Database.RegisterLevel("Map_CustomWorld_01", new CustomLevelData()
                    {
                        createClass = () => new Map_CustomWorld_01(this.game),
                        path = this.Helper.DirectoryPath + "\\assets\\Map_CustomWorld_01.xml",
                        type = ScreenType.Map
                    });

                    this.game.ScreenManager.LoadScreen(ScreenType.Map, this.Helper.DirectoryPath + "\\assets\\Map_CustomWorld_01.xml");
                }

                // this.SpeechBubbleTest();

                // var mouse = GetMouseWorldPosition(this.game);
                // bool pressed = this.game.InputManager.MouseLeftPressed();

                // if (pressed && this.moving == null)
                // {
                //     var e = this.FindClosestCharacters(mouse, 0.05f);
                //     e.Collidable = false;
                //     e.Position = GetMouseWorldPosition(this.game);
                //     this.moving = e;
                // }

                // if (this.moving != null)
                // {
                //     this.moving.Position = GetMouseWorldPosition(this.game);
                // }

                // if (pressed && this.moving != null)
                // {
                //     this.moving.Collidable = true;
                //     this.moving = null;
                // }


                // var findClosestCharacters = this.FindClosestCharacters(this.game.PlayerManager.getMainPlayerOrHost.currentPlayerObj.Position);
                // if(findClosestCharacters == null) return;


                // if (this.game.InputManager.KeyJustPressed(Keys.M))
                // {
                //     findClosestCharacters.Collidable = false;
                //     findClosestCharacters.Position = GetMouseWorldPosition(this.game);
                // }
                // if (this.game.InputManager.KeyJustReleased(Keys.K)){
                //     findClosestCharacters.Collidable = true;
                // }

                var reloadPercentBubble = new SpeechBubbleObj();

                if (this.game.InputManager.KeyJustPressed(Keys.K))
                {
                    this.RunDialog();
                }

            };
        }

        public static Vector2 GetMouseWorldPosition(GameController game)
        {
            float physicalScaleX = game.ScreenManager.virtualScreen.physicalScaleX;
            float physicalScaleY = game.ScreenManager.virtualScreen.physicalScaleY;
            var mouseMouse = new Vector2(
                game.InputManager.mouseX * 1f / physicalScaleX,
                game.InputManager.mouseY * 1f / physicalScaleY);
            var arenaScreen = GameController.g_game.ScreenManager.arenaScreen;
            var camera = arenaScreen.Camera;
            return Vector2.Transform(mouseMouse, Matrix.Invert(camera.TransformMatrix));
        }

        private void SpeechBubbleTest()
        {

            if (this.game.InputManager.KeyJustPressed(Keys.H))
            {
                // this.grayScale = this.game.nonDisposableContent.Load<Effect>(this.Helper.DirectoryPath + "\\assets\\GrayScale.fx");
                if (this.speechBubble == null)
                {
                    this.speechBubble = new SpeechBubble(this.game, 0f);
                    // var speechBubble =  new SpeechBubble(this.game, 0f);;//this.speechBubble;
                    var speechBubble = this.speechBubble;
                    speechBubble.ChangeSprite("PlayerEngineer_Pizza_Character");
                    speechBubble.triggerOnHit = true;
                    // var zoneRandomPosition = this.GetZoneRandomPosition();
                    var zoneRandomPosition = new Vector2(50, 200);
                    this.SpawnEnemy(speechBubble, zoneRandomPosition, EnemySpawnAnimType.TeleportWithPillar);
                    // Tween.RunFunction(0.1f, this, "SetScale", (DisplayObj) speechBubble, (Vector2) Vector2.One);
                    speechBubble.displayDuration = 1f;
                    speechBubble.SetContext(speechBubble);
                    speechBubble.textCallback = (b, obj) =>
                        b
                            ? obj != null ? "Drop it, " + this.GetCharacterName(obj.Name) + "!" : "Drop it!"
                            : "HERE IS YOUR PIZZA!!!";
                    speechBubble.completeCallback = (b, obj) =>
                    {
                        if (b) return;
                        var item = GameController.g_game.ItemDropManager.AddItemDropObj(ItemDropType.Medpack, speechBubble,
                            false);
                        item.Scale = new Vector2(3);
                    };
                }

                this.speechBubble.Scale = new Vector2(2);
                var speechBubble2 = new SpeechBubble(this.game);
                this.SpawnEnemy(speechBubble2, this.game.PlayerManager.getMainPlayerOrHost.currentPlayerClass.Position);
                // speechBubble2.displayDuration = 10f;
                speechBubble2.Say(this.game.PlayerManager.getMainPlayerOrHost.currentPlayerClass,
                    "Man, I would die for some pizza...");
            }

            if (this.speechBubble != null)
            {
                this.deliveryTime++;

                if (this.deliveryTime == 100f)
                {
                    this.speechBubble.Scale = new Vector2(2);
                    var speechBubble2 = new SpeechBubble(this.game);
                    this.SpawnEnemy(speechBubble2, this.game.PlayerManager.getMainPlayerOrHost.currentPlayerClass.Position);
                    // speechBubble2.displayDuration = 10f;
                    speechBubble2.Say(this.game.PlayerManager.getMainPlayerOrHost.currentPlayerClass, "What the hell?!");
                }

                if (this.deliveryTime == 350f)
                {
                    this.speechBubble.Scale = new Vector2(2);
                    var speechBubble2 = new SpeechBubble(this.game);
                    this.SpawnEnemy(speechBubble2, this.speechBubble.Position);
                    // speechBubble2.displayDuration = 10f;
                    speechBubble2.Say(this.speechBubble, "HELLO! I'M HERE!");
                }
            }
        }

        public EnemyObj FindClosestCharacters(Vector2 position, float tolerance = 1f) //ArenaScreen arenaScreen)
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
                    Math.Abs(CDGMath.DistanceBetweenPts(position, currentAndGlobalEnemy.AbsPosition));
                if (!((double) num2 < (double) num1) && num2 > tolerance) continue;
                num1 = num2;
                enemyObj = currentAndGlobalEnemy;
            }
            return enemyObj;
        }

        private string GetCharacterName(string name)
        {
            if(name == "Player_Hammer")
                return "Alex";
            if(name == "Player_Medic")
                return "Erin";
            if(name == "Player_Tank")
                return "Tris";
            if(name == "Player_Sniper")
                return "Meg";
            if(name == "Player_Mercenary")
                return "Lisa";
            if(name == "Player_Titan")
                return "Styx";

            return "Wierdo";
        }

        private void SetupTitanCharacter(IModHelper helper)
        {
            this.customData = new TitanSetupCharacter(this.game, helper).Create();
            this.setupCharacter = new SetupCharacter(this.game, helper);
            this.setupCharacter.Setup(this.customData,
                playerObj => new PlayerClassObj_Titan(GameController.g_game, playerObj, this.Helper));
            // classType unfortunately need to be get async
            // this._classType = classType;
        }

        // AddInteractiveObject();

        private bool IsCamp()
        {
            return this.game.ScreenManager.arenaScreen.stageName.ToLower().Contains("camp");
        }

        private void AddInteractiveObject()
        {

            this.Helper.Events.GameLoop.OneSecondUpdateTicked  += (sender, args) =>
            {
                if (!Context.IsGameLaunched)
                    return;

                if (this.game.ScreenManager == null)
                    return;

                // this.UpdatePlayerMapIcon(this._customClassType, $"Player{this.customData.Name}_Character");

                if (this.game.ScreenManager.arenaScreen == null)
                    return;

                if (this.game.ScreenManager.arenaScreen.IsPaused)
                    return;

                if (this.game.PlayerManager.getMainPlayerOrHost == null)
                    return;

                if (!this.IsCamp())
                    return;

                try
                {
                    if (this.interactableObject == null)
                    {
                        this.interactableObject = new InteractableObject(this.game, this.game.ScreenManager.arenaScreen,
                            this.Helper, ModEntry.monitor);
                    }

                    this.interactableObject.ContextObj.methodObject1 = this;
                    this.interactableObject.ContextObj.method1Name = "RunDialog";
                    this.interactableObject.UpdateCollisionCheck();
                }
                catch (Exception e)
                {
                }

            };
        }

        public void RunDialog()
        {
            var dialogueTest01 = new Dialogue_Test_01();
            dialogueTest01.RunDialogue(this.game, this.game.PlayerManager.hostPlayer.controllerIndex, null);
        }
        // public IEnumerable<object> LoadAsset()
        // {
        //     this.Helper.Content.LoadSpritesheet("Art//Misc//sprites", ContentSource.ModFolder);
        //     yield return (object) null;
        // }

        public IEnumerable<object> LoadSpritesheet(string className)
        {
            var enumerator =
                this.Helper.Content.LoadSpritesheet($"Assets/"+className, ContentSource.ModFolder).GetEnumerator();
            // this.Helper.Content.LoadSpritesheet("Art//Players//PlayerMercenarySpritesheet", ContentSource.ModFolder).GetEnumerator();

            while (enumerator.MoveNext())
            {
                object current = enumerator.Current;
                yield return null;
            }
        }


        // public void ApplySkin(string spritesheet, string skinName, SpriteObj originalSpriteObj, ClassType classType)
        // {
        //     this.LoadSpritesheet(spritesheet);
        //     //todo: Generic class and replace string with lookup dictonary
        //     this.Helper.Events.Specialized.SpriteChanged += (sender, args) =>
        //     {
        //         string spriteName = args.spriteName;
        //         var spriteObj = args.SpriteObj;
        //         // if (spriteName.StartsWith("PlayerMedic_Medkit"))
        //         // {
        //             // spriteObj.ChangeSprite("Pizza_Medkit");
        //             // return;
        //         // }
        //         if (spriteName.Contains("ngineer_"))
        //         {
        //             spriteObj.ChangeSprite(spriteName.Replace("ngineer_", "ngineer_Pizza_"));
        //             return;
        //         }
        //         this.OnSpriteNotFound(args.spriteName, args.SpriteObj, classType);
        //     };
        // }

        public static void SetScale(DisplayObj sprite, Vector2 scale) //string spriteName, SpriteObj spriteObj, ClassType classType)
        {
            sprite.Scale = scale;
        }

        public void ApplySkin() //string spriteName, SpriteObj spriteObj, ClassType classType)
        {
            var enumerator = this.LoadSpritesheet("playerEngineerSpritesheet_pizza").GetEnumerator();
            while (enumerator.MoveNext())
            {
                ;
            }

            //todo: Generic class and replace string with lookup dictonary
            this.Helper.Events.Specialized.SpriteChanged += (sender, args) =>
            {

                // if(this.game.ScreenManager.arenaScreen == null)
                //     return;

                var spriteObj = args.SpriteObj;
                string spriteName = args.spriteName;

                if (spriteName.StartsWith("PlayerMedic_Medkit"))
                {
                    spriteObj.ChangeSprite("Pizza_Medkit");
                    return;
                }

                if (spriteObj is PlayerClassObj)
                {
                    return;
                }
                if (spriteName.Contains("PlayerEngineer_Sprite") && spriteObj.Tag != "")
                {
                    // this.Monitor.Log(spriteObj.Bounds.ToString());
                    spriteObj.ChangeSprite(spriteName.Replace("ngineer_", "ngineer_Pizza_"));
                }


                // if (!this.IsCamp()) return;

                if(this.game.PlayerManager == null || this.game.PlayerManager.activePlayerArray == null)
                    return;

                foreach (var playerObj in this.game.PlayerManager.activePlayerArray)
                {
                    if(playerObj == null)
                        return;

                    var p = (DisplayObj) playerObj.currentPlayerClass;


                    if(spriteObj.Bounds == p.Bounds)
                        return;

                    if (p.Equals(spriteObj))
                    {
                        // }
                        // if (spriteObj is PlayerClassObj playerCObj)
                        this.Monitor.Log(playerObj.playerIndex.ToString());

                        if (playerObj.playerIndex == 0 || playerObj.playerIndex == 1)
                        {

                            if (spriteName.Contains("ngineer_") && !spriteName.Contains("ngineer_Pizza_"))
                            {
                                spriteObj.ChangeSprite(spriteName.Replace("ngineer_", "ngineer_Pizza_"));
                                return;
                            }
                        }
                    }
                }

                // string spriteName = args.spriteName;
                // var spriteObj = args.SpriteObj;
                // if (spriteName.StartsWith("PlayerMedic_Medkit"))
                // {
                //     spriteObj.ChangeSprite("Pizza_Medkit");
                //     spriteObj.Scale = Vector2.One / 2;
                //     return;
                // }
                // if (spriteName.Contains("ngineer_") && !spriteName.Contains("ngineer_Pizza_"))
                // {
                // spriteObj.ChangeSprite(spriteName.Replace("ngineer_", "ngineer_Pizza_"));
                // return;
                // }


                //
                // if (spriteName.Contains("ngineer_") && !spriteName.Contains("ngineer_Pizza_"))
                // {
                //     for (int index = 0; index < this.game.PlayerManager.activePlayerArray_count; ++index)
                //     {
                //         var activePlayer = this.game.PlayerManager.activePlayerArray[index];
                //         if (index == 0 && activePlayer.currentPlayerClass.spriteName.Contains("ngineer_"))
                //             spriteObj.ChangeSprite(spriteName.Replace("ngineer_", "ngineer_Pizza_"));
                //         // activePlayer.currentPlayerClass.chil
                //         // (((BrawlerContainerObj)activePlayer.currentPlayerClass).ChangeSprite() as ) == (spriteObj as BrawlerContainerObj)
                //
                //     }
                //
                //     return;
                // }
            };
        }

        // temp silly fix to avoid collision with mercenary
        // [HarmonyPatch(typeof(Camp_Base), "UpdateCollisionCheck")]
        // [HarmonyPostfix]
        // static void UpdateCollisionCheck(Camp_Base __instance)
        // {
        //     var playerManager = __instance.Game.PlayerManager;
        //     var playerArrayCount = playerManager.activePlayerArray_count;
        //     for (var index1 = 0; index1 < playerArrayCount; ++index1)
        //     {
        //         var activePlayer = playerManager.activePlayerArray[index1];
        //         if (activePlayer.currentPlayerClass == null ||
        //             activePlayer.currentPlayerClass.classType != ClassType.Mercenary) continue;
        //         activePlayer.displayInputIcon = false;
        //         activePlayer.contextObj = null;
        //     }
        // }

        private EventHandler<UpdateTickedEventArgs> CheckStuff()
        {
            return (sender, args) =>
            {
                var game = GameController.g_game;
                if (!game.gameLoaded)
                    return;

                if (!BlitNet.Lobby.IsMaster || (!game.PlayerManager.allPlayersDown ||
                                                game.PlayerManager.numActivePlayers <= 0))
                    return;
                // this.Helper.Reflection.GetField<bool>(GameController.g_game.ScreenManager.arenaScreen,
                // "awaitingRunMissionFailed").SetValue(true);

                var logicSet = this.Helper.Reflection.GetField<LogicSet>(
                    game.ScreenManager.arenaScreen,
                    "m_missionFailedLS").GetValue();
                if (!logicSet.IsActive) return;

                logicSet.ForceStop();

                var enemyManager = game.EnemyManager;
                if (enemyManager == null) return;
                if (!enemyManager.areEnemiesAlive) return;

                var enemyArray = enemyManager.enemyArray;
                for (int i = 0; i < enemyManager.enemyArray_count; i++)
                {
                    var enemy = enemyArray[i];
                    if (enemy.IsKilled || !enemy.Active)
                        continue;

                    enemy.StopAllActions();
                }


                // this.Helper.Reflection.GetMethod(GameController.g_game.ScreenManager.arenaScreen, "CancelGameOverAnimation").Invoke();
                // Tween.RunFunction(0.75f, (object) this, "RunMissionFailed");
                // this.awaitingRunMissionFailed = true;
            };
        }

        public Vector2 GetZoneRandomPosition()
        {
            ArenaZone currentZone = (this.game.ScreenManager.CurrentScreen as ArenaScreen).currentZone;
            CDGRect cdgRect = currentZone.zoneRect.ToCDGRect();
            NavMeshObj navMesh = currentZone.navMesh;
            if (navMesh != null)
                cdgRect = navMesh.AbsBounds;
            return new Vector2(RNG.get(205).RandomFloat(cdgRect.Left, cdgRect.Right), RNG.get(206).RandomFloat(cdgRect.Top, cdgRect.Bottom));
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

        public EnemyObj SpawnEnemy(EnemyObj newEnemyObj, Vector2 position, EnemySpawnAnimType enemySpawnAnimType = EnemySpawnAnimType.None)
        {
            var enemyManager = GameController.g_game.EnemyManager;
            int enemyObjIndex = this.PreloadEnemy(newEnemyObj, position);
            var enemyObj = enemyManager.enemyArray[enemyObjIndex];
            enemyObj.spawnAnimType = enemySpawnAnimType;
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
