using CDGEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Brawler2D;
using HarmonyLib;

namespace FMODF
{
    public class ParseCustomLevelDataHelper
    {
        public static IEnumerable<object> ParseLevelData(
            GameController game,
            BrawlerGameScreen gameScreen,
            string filePath,
            ContentManager contentManager = null)
        {

            var ICloudObjType = typeof(ParseCustomLevelDataHelper).Assembly.GetType("Brawler2D.ICloudObj");
            var Trigger_SpriteObjType = typeof(ParseCustomLevelDataHelper).Assembly.GetType("Brawler2D.Trigger_SpriteObj");
            var spriteLibrary = typeof(ParseCustomLevelDataHelper).Assembly.GetType("Brawler2D.SpriteLibrary");

            var ParseTag = AccessTools.Method(Trigger_SpriteObjType, "ParseTag");
            var LoadSpritesheetCoro = AccessTools.Method(spriteLibrary, "LoadSpritesheetCoro");

            // var ParseTag = AccessTools.Method(spriteLibrary, "ParseTag");
            // var CampBaseType = this.Assembly.GetType("Brawler2D.Camp_Base");

            MapScreen mapScreen = gameScreen as MapScreen;
            ArenaScreen arenaScreen = gameScreen as ArenaScreen;
            CutsceneScreen cutsceneScreen = gameScreen as CutsceneScreen;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;
            XmlReader reader = null;
            List<CustomCodeObj> m_CustomCodeObjList = new List<CustomCodeObj>();
            if (contentManager == null)
            {
                reader = XmlReader.Create(filePath, settings);
            }
            else
            {
                string str = AppDomain.CurrentDomain.BaseDirectory + "\\" + contentManager.RootDirectory +
                             "\\Levels\\Compiled\\" + filePath + ".bef";
                string inputUri = AppDomain.CurrentDomain.BaseDirectory + "\\" + contentManager.RootDirectory +
                                  "\\Levels\\" + filePath + ".xml";
                if (File.Exists(str))
                {
                    reader = XmlReader.Create(str, settings);
                }
                else
                {
                    Console.WriteLine("WARNING: Could not find BEF file path: " + str +
                                      ". You may need to compile the XML file first in the editor. Using the XML version...");
                    reader = XmlReader.Create(inputUri, settings);
                }
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "Spritesheet":
                            reader.MoveToAttribute("Name");
                            string sheetName = reader.Value;
                            // var ss = (Enumerator) LoadSpritesheetCoro.Invoke(spriteLibrary, new[]
                            // {
                            //     game.disposableContent,
                            //     "art\\" + sheetName
                            // });
                            // foreach (object obj in SpriteLibrary.LoadSpritesheetCoro(game.disposableContent,
                            //     "art\\" + sheetName))
                            //     yield return null;
                            // if (SpriteLibrary.justLoadedSpritesheetOk)
                            // {
                            //     Console.WriteLine("Loading: " + sheetName);
                            //     game.ScreenManager.AddLoadedSpritesheetPath("art\\" + sheetName);
                            //     break;
                            // }

                            break;
                        case "Layer":
                            reader.MoveToAttribute("Name");
                            string layerName = reader.Value;
                            reader.MoveToAttribute("ParallaxX");
                            float parallaxX = float.Parse(reader.Value, NumberStyles.Any, game.cultureInfo);
                            reader.MoveToAttribute("ParallaxY");
                            float parallaxY = float.Parse(reader.Value, NumberStyles.Any, game.cultureInfo);
                            reader.MoveToAttribute("ScrollSpeedX");
                            float scrollSpeedX = float.Parse(reader.Value, NumberStyles.Any, game.cultureInfo);
                            reader.MoveToAttribute("ScrollSpeedY");
                            float scrollSpeedY = float.Parse(reader.Value, NumberStyles.Any, game.cultureInfo);
                            Color layerOverlay = Color.White;
                            if (reader.MoveToAttribute("LayerOverlay"))
                            {
                                List<string> list = reader.Value.Split(',').ToList<string>();
                                layerOverlay.R = byte.Parse(list[0], NumberStyles.Any, game.cultureInfo);
                                layerOverlay.G = byte.Parse(list[1], NumberStyles.Any, game.cultureInfo);
                                layerOverlay.B = byte.Parse(list[2], NumberStyles.Any, game.cultureInfo);
                                if (list.Count > 3)
                                    layerOverlay.A = byte.Parse(list[3], NumberStyles.Any, game.cultureInfo);
                            }

                            if (layerOverlay.A == 0)
                                layerOverlay = Color.White;
                            bool autoLayer = false;
                            if (reader.MoveToAttribute("SortLayer"))
                                autoLayer = bool.Parse(reader.Value);
                            bool addToGameLayer = false;
                            if (reader.MoveToAttribute("AddToGameLayer"))
                                addToGameLayer = bool.Parse(reader.Value);
                            bool applyGaussianBlur = false;
                            if (reader.MoveToAttribute("ApplyGaussian"))
                                applyGaussianBlur = bool.Parse(reader.Value);
                            if (LevelParser.parsingFlag == LevelParser.ParsingFlag.None)
                                gameScreen.AddLayer(layerName, parallaxX, parallaxY, scrollSpeedX, scrollSpeedY,
                                    autoLayer, layerOverlay, applyGaussianBlur, addToGameLayer, true, -1);
                            else
                                gameScreen.AddLayer(layerName, parallaxX, parallaxY, scrollSpeedX, scrollSpeedY,
                                    autoLayer, layerOverlay, applyGaussianBlur, addToGameLayer, false, -1);
                            if (layerName == "Game Layer" && gameScreen.gameLayerIndex == 0)
                            {
                                gameScreen.gameLayerIndex = gameScreen.layerCount - 1;
                                break;
                            }

                            break;
                        case "Room":
                            if (LevelParser.parsingFlag != LevelParser.ParsingFlag.SpritesAndCollHullsOnly)
                            {
                                reader.MoveToAttribute("X");
                                float zoneX = float.Parse(reader.Value, NumberStyles.Any, game.cultureInfo);
                                reader.MoveToAttribute("Y");
                                float zoneY = float.Parse(reader.Value, NumberStyles.Any, game.cultureInfo);
                                reader.MoveToAttribute("Width");
                                float zoneWidth = float.Parse(reader.Value, NumberStyles.Any, game.cultureInfo);
                                reader.MoveToAttribute("Height");
                                float zoneHeight = float.Parse(reader.Value, NumberStyles.Any, game.cultureInfo);
                                reader.MoveToAttribute("InnerZonePos");
                                AxisLock axisLock = (AxisLock) Enum.Parse(typeof(AxisLock), reader.Value);
                                reader.MoveToAttribute("InnerZonePercent");
                                float innerZonePercent = float.Parse(reader.Value, NumberStyles.Any, game.cultureInfo);
                                reader.MoveToAttribute("Tag");
                                string zoneTag = reader.Value;
                                reader.MoveToAttribute("IsArenaZone");
                                if (mapScreen != null)
                                {
                                    if (bool.Parse(reader.Value))
                                    {
                                        MapZone mapZone = mapScreen.AddMapZone(zoneX, zoneY, zoneWidth, zoneHeight,
                                            zoneTag);
                                        reader.MoveToAttribute("Name");
                                        mapZone.stageFakeName = reader.Value;
                                        if (mapZone.stageFakeName == "New Room")
                                            mapZone.stageFakeName = "";
                                        reader.MoveToAttribute("Code");
                                        if (reader.Value != "")
                                        {
                                            m_CustomCodeObjList.Add(new CustomCodeObj(reader.Value, new object[1]
                                            {
                                                mapZone
                                            }));
                                            break;
                                        }

                                        break;
                                    }

                                    ArenaZone arenaZone =
                                        mapScreen.AddArenaZone(zoneX, zoneY, zoneWidth, zoneHeight, zoneTag);
                                    reader.MoveToAttribute("Code");
                                    if (reader.Value != "")
                                    {
                                        m_CustomCodeObjList.Add(new CustomCodeObj(reader.Value, new object[1]
                                        {
                                            arenaZone
                                        }));
                                        break;
                                    }

                                    break;
                                }

                                if (arenaScreen != null)
                                {
                                    bool hookToNavMesh = false;
                                    if (bool.Parse(reader.Value))
                                    {
                                        reader.MoveToAttribute("Name");
                                        string name = reader.Value;
                                        if (reader.MoveToAttribute("HookNavMesh"))
                                            hookToNavMesh = bool.Parse(reader.Value);
                                        float maxCameraZoomOut = 1f;
                                        if (reader.MoveToAttribute("MaxCameraZoomOut"))
                                            maxCameraZoomOut = float.Parse(reader.Value, NumberStyles.Any,
                                                game.cultureInfo);
                                        ArenaZone arenaZone = arenaScreen.AddArenaZone(zoneX, zoneY, zoneWidth,
                                            zoneHeight, innerZonePercent, axisLock, name, hookToNavMesh,
                                            maxCameraZoomOut, zoneTag);
                                        reader.MoveToAttribute("Code");
                                        m_CustomCodeObjList.Add(new CustomCodeObj(reader.Value, new object[1]
                                        {
                                            arenaZone
                                        }));
                                        break;
                                    }

                                    if (reader.MoveToAttribute("HookNavMesh"))
                                        hookToNavMesh = bool.Parse(reader.Value);
                                    float maxCameraZoomOut1 = 1f;
                                    if (reader.MoveToAttribute("MaxCameraZoomOut"))
                                        maxCameraZoomOut1 = float.Parse(reader.Value, NumberStyles.Any,
                                            game.cultureInfo);
                                    ArenaZone arenaZone1 = arenaScreen.AddGlobalZone(zoneX, zoneY, zoneWidth,
                                        zoneHeight, innerZonePercent, axisLock, hookToNavMesh, maxCameraZoomOut1,
                                        zoneTag);
                                    reader.MoveToAttribute("Code");
                                    m_CustomCodeObjList.Add(new CustomCodeObj(reader.Value, new object[1]
                                    {
                                        arenaZone1
                                    }));
                                    break;
                                }

                                if (cutsceneScreen != null)
                                {
                                    ArenaZone arenaZone = cutsceneScreen.AddZone(zoneX, zoneY, zoneWidth, zoneHeight,
                                        innerZonePercent, axisLock);
                                    reader.MoveToAttribute("Code");
                                    if (reader.Value != "")
                                    {
                                        m_CustomCodeObjList.Add(new CustomCodeObj(reader.Value, new object[1]
                                        {
                                            arenaZone
                                        }));
                                        break;
                                    }

                                    break;
                                }

                                break;
                            }

                            break;
                        case "Trigger":
                            if (LevelParser.parsingFlag != LevelParser.ParsingFlag.SpritesAndCollHullsOnly)
                            {
                                reader.MoveToAttribute("Width");
                                float triggerWidth = float.Parse(reader.Value, NumberStyles.Any, game.cultureInfo);
                                reader.MoveToAttribute("Height");
                                float triggerHeight = float.Parse(reader.Value, NumberStyles.Any, game.cultureInfo);
                                reader.MoveToAttribute("TriggerType");
                                TriggerType triggerType = (TriggerType) Enum.Parse(typeof(TriggerType), reader.Value);
                                if (mapScreen == null && arenaScreen != null)
                                {
                                    reader.MoveToAttribute("IsArenaTrigger");
                                    bool flag = bool.Parse(reader.Value);
                                    TriggerObj triggerObj =
                                        new TriggerObj(game, (int) triggerWidth, (int) triggerHeight);
                                    triggerObj.triggerType = triggerType;
                                    triggerObj.isArenaTrigger = flag;
                                    LevelParser.LoadGenericData(reader, triggerObj, game);
                                    arenaScreen.AddObjToArenaZone(triggerObj);
                                    break;
                                }

                                break;
                            }

                            break;
                        case "CollHull":
                            if (LevelParser.parsingFlag != LevelParser.ParsingFlag.AllButSpritesAndCollHulls)
                            {
                                reader.MoveToAttribute("Width");
                                float collHullWidth = float.Parse(reader.Value, NumberStyles.Any, game.cultureInfo);
                                reader.MoveToAttribute("Height");
                                float collHullHeight = float.Parse(reader.Value, NumberStyles.Any, game.cultureInfo);
                                TransitionType transitionType = TransitionType.None;
                                if (reader.MoveToAttribute("TransitionType"))
                                    transitionType = (TransitionType) Enum.Parse(typeof(TransitionType), reader.Value);
                                bool isNavMesh = false;
                                if (reader.MoveToAttribute("IsNavMesh"))
                                    isNavMesh = bool.Parse(reader.Value);
                                bool jumpable = false;
                                if (reader.MoveToAttribute("Jumpable"))
                                    jumpable = bool.Parse(reader.Value);
                                bool canShootThrough = false;
                                if (reader.MoveToAttribute("CanShoot"))
                                    canShootThrough = bool.Parse(reader.Value);
                                bool playerCanShootThrough = false;
                                if (reader.MoveToAttribute("PlayerCanShoot"))
                                    playerCanShootThrough = bool.Parse(reader.Value);
                                bool onlyPlayersCollide = false;
                                if (reader.MoveToAttribute("PlayersCollide"))
                                    onlyPlayersCollide = bool.Parse(reader.Value);
                                bool onlyEnemiesCollide = false;
                                if (reader.MoveToAttribute("EnemiesCollide"))
                                    onlyEnemiesCollide = bool.Parse(reader.Value);
                                bool hidesEngineerUI = false;
                                if (reader.MoveToAttribute("HidesEngineerUI"))
                                    hidesEngineerUI = bool.Parse(reader.Value);
                                reader.MoveToAttribute("Code");
                                string code = reader.Value;
                                if (arenaScreen != null)
                                {
                                    if (transitionType != TransitionType.None)
                                    {
                                        var transitionZone = new TransitionZone(arenaScreen, (int) collHullWidth,
                                            (int) collHullHeight);
                                        transitionZone.transitionType = transitionType;
                                        transitionZone.isJumpTransition = jumpable;
                                        LevelParser.LoadGenericData(reader, transitionZone, game);
                                        arenaScreen.AddObjToArenaZone(transitionZone);
                                        if (code != "")
                                        {
                                            m_CustomCodeObjList.Add(new CustomCodeObj(code, new object[1]
                                            {
                                                transitionZone
                                            }));
                                            break;
                                        }

                                        break;
                                    }

                                    if (code != "" && !isNavMesh)
                                    {
                                        reader.MoveToAttribute("Name");
                                        string str = reader.Value;
                                        reader.MoveToAttribute("X");
                                        float num1 = float.Parse(reader.Value, NumberStyles.Any, game.cultureInfo);
                                        reader.MoveToAttribute("Y");
                                        float num2 = float.Parse(reader.Value, NumberStyles.Any, game.cultureInfo);
                                        reader.MoveToAttribute("ScaleX");
                                        float num3 = float.Parse(reader.Value, NumberStyles.Any, game.cultureInfo);
                                        reader.MoveToAttribute("ScaleY");
                                        float num4 = float.Parse(reader.Value, NumberStyles.Any, game.cultureInfo);
                                        reader.MoveToAttribute("Layer");
                                        int layerIndex = int.Parse(reader.Value, NumberStyles.Any, game.cultureInfo);
                                        int layerListCount = gameScreen.getLayerListCount(layerIndex);
                                        m_CustomCodeObjList.Add(new CustomCodeObj(code, new object[9]
                                        {
                                            str,
                                            num1,
                                            num2,
                                            collHullWidth,
                                            collHullHeight,
                                            num3,
                                            num4,
                                            layerIndex,
                                            layerListCount
                                        }));
                                        break;
                                    }

                                    if (!isNavMesh)
                                    {
                                        BrawlerCollHullObj collHull =
                                            new BrawlerCollHullObj((int) collHullWidth, (int) collHullHeight);
                                        collHull.jumpable = jumpable;
                                        collHull.canShootThrough = canShootThrough;
                                        collHull.playerCanShootThrough = playerCanShootThrough;
                                        collHull.onlyPlayersCollide = onlyPlayersCollide;
                                        collHull.onlyEnemiesCollide = onlyEnemiesCollide;
                                        collHull.hidesEngineerUI = hidesEngineerUI;
                                        LevelParser.LoadGenericData(reader, collHull, game);
                                        gameScreen.AddCollHull(collHull);
                                        break;
                                    }

                                    BrawlerCollHullObj brawlerCollHullObj =
                                        new BrawlerCollHullObj((int) collHullWidth, (int) collHullHeight);
                                    LevelParser.LoadGenericData(reader, brawlerCollHullObj, game);
                                    arenaScreen.AddObjToArenaZone(brawlerCollHullObj);
                                    break;
                                }

                                if (mapScreen != null)
                                {
                                    BrawlerCollHullObj collHull =
                                        new BrawlerCollHullObj((int) collHullWidth, (int) collHullHeight);
                                    LevelParser.LoadGenericData(reader, collHull, game);
                                    mapScreen.AddCollHull(collHull);
                                    break;
                                }

                                break;
                            }

                            break;
                        case "Sprite":
                            if (LevelParser.parsingFlag != LevelParser.ParsingFlag.AllButSpritesAndCollHulls)
                            {
                                reader.MoveToAttribute("SpriteName");
                                string spriteName = reader.Value;
                                reader.MoveToAttribute("Layer");
                                int layer = int.Parse(reader.Value, NumberStyles.Any, game.cultureInfo);
                                if (!(LevelParser.ParseAnimatableObj(spriteName, false, gameScreen) is BrawlerSpriteObj
                                    sprite))
                                    sprite = new BrawlerSpriteObj(spriteName);
                                if (reader.MoveToAttribute("Jumpable"))
                                    sprite.jumpable = bool.Parse(reader.Value);
                                if (reader.MoveToAttribute("AddPhysics"))
                                    sprite.addPhysics = bool.Parse(reader.Value);
                                if (reader.MoveToAttribute("ForceAdd"))
                                    sprite.forceAddToStage = bool.Parse(reader.Value);
                                if (spriteName.Contains("_Pixel"))
                                    sprite.isPixelSprite = true;
                                LevelParser.LoadGenericData(reader, sprite, game);
                                gameScreen.AddGameObj(sprite, layer);
                                if (sprite.GetType() == ICloudObjType && arenaScreen != null)
                                {
                                    arenaScreen.AddObjToArenaZone(sprite);
                                    break;
                                }

                                break;
                            }

                            break;
                        case "Container":
                            if (LevelParser.parsingFlag != LevelParser.ParsingFlag.AllButSpritesAndCollHulls)
                            {
                                reader.MoveToAttribute("SpriteName");
                                string containerName = reader.Value;
                                reader.MoveToAttribute("Layer");
                                int containerLayer = int.Parse(reader.Value, NumberStyles.Any, game.cultureInfo);
                                if (containerName.Contains("Group_"))
                                {
                                    BrawlerContainerObj brawlerContainerObjx = new BrawlerContainerObj(containerName);
                                    if (reader.MoveToAttribute("Jumpable"))
                                        brawlerContainerObjx.jumpable = bool.Parse(reader.Value);
                                    if (reader.MoveToAttribute("AddPhysics"))
                                        brawlerContainerObjx.addPhysics = bool.Parse(reader.Value);
                                    if (reader.MoveToAttribute("ForceAdd"))
                                        brawlerContainerObjx.forceAddToStage = bool.Parse(reader.Value);
                                    LevelParser.LoadGenericData(reader, brawlerContainerObjx, game);
                                    for (int index = 0; index < brawlerContainerObjx.NumChildren; ++index)
                                    {
                                        SpriteObj childAt = brawlerContainerObjx.GetChildAt(index) as SpriteObj;
                                        BrawlerSpriteObj animatableObj =
                                            LevelParser.ParseAnimatableObj(childAt.spriteName, false, gameScreen) as
                                                BrawlerSpriteObj;
                                        animatableObj.Position = childAt.AbsPosition;
                                        animatableObj.Flip = brawlerContainerObjx.Flip;
                                        animatableObj.Scale = brawlerContainerObjx.Scale;
                                        animatableObj.Tag = brawlerContainerObjx.Tag;
                                        if (animatableObj is RollingCloud_SpriteObj rollingCloudSpriteObj)
                                            rollingCloudSpriteObj.internalOpacity = brawlerContainerObjx.Opacity;
                                        else
                                            animatableObj.Opacity = brawlerContainerObjx.Opacity;
                                        if (animatableObj is ISwayObj swayObj)
                                            swayObj.internalRotation = brawlerContainerObjx.Rotation;
                                        else
                                            animatableObj.Rotation = brawlerContainerObjx.Rotation;
                                        if (animatableObj.GetType() == Trigger_SpriteObjType)
                                            ParseTag.Invoke(animatableObj, new[] {game});

                                        animatableObj.jumpable = brawlerContainerObjx.jumpable;
                                        animatableObj.addPhysics = brawlerContainerObjx.addPhysics;
                                        animatableObj.forceAddToStage = brawlerContainerObjx.forceAddToStage;
                                        gameScreen.AddGameObj(animatableObj, containerLayer);
                                        if (containerName.Contains("_Pixel"))
                                            animatableObj.isPixelSprite = true;
                                    }

                                    brawlerContainerObjx.Dispose();
                                    break;
                                }

                                if (!(LevelParser.ParseAnimatableObj(containerName, true, gameScreen) is
                                    BrawlerContainerObj brawlerContainerObj))
                                    brawlerContainerObj = new BrawlerContainerObj(containerName);
                                if (reader.MoveToAttribute("Jumpable"))
                                    brawlerContainerObj.jumpable = bool.Parse(reader.Value);
                                if (reader.MoveToAttribute("AddPhysics"))
                                    brawlerContainerObj.addPhysics = bool.Parse(reader.Value);
                                if (reader.MoveToAttribute("ForceAdd"))
                                    brawlerContainerObj.forceAddToStage = bool.Parse(reader.Value);
                                if (containerName.Contains("_Pixel"))
                                    brawlerContainerObj.isPixelSprite = true;
                                LevelParser.LoadGenericData(reader, brawlerContainerObj, game);
                                gameScreen.AddGameObj(brawlerContainerObj, containerLayer);
                                break;
                            }

                            break;
                        case "Marker":
                            if (LevelParser.parsingFlag != LevelParser.ParsingFlag.SpritesAndCollHullsOnly &&
                                arenaScreen != null)
                            {
                                MarkerObj marker = new MarkerObj();
                                LevelParser.LoadGenericData(reader, marker, game);
                                reader.MoveToAttribute("ID");
                                marker.Name = reader.Value;
                                arenaScreen.AddObjToArenaZone(marker);
                                break;
                            }

                            break;
                        case "PlayerStart":
                            if (LevelParser.parsingFlag != LevelParser.ParsingFlag.SpritesAndCollHullsOnly)
                            {
                                Vector2 playerStartPos = new Vector2();
                                reader.MoveToAttribute("X");
                                playerStartPos.X = float.Parse(reader.Value, NumberStyles.Any, game.cultureInfo);
                                reader.MoveToAttribute("Y");
                                playerStartPos.Y = float.Parse(reader.Value, NumberStyles.Any, game.cultureInfo);
                                bool debugOnly = false;
                                if (reader.MoveToAttribute("DebugOnly"))
                                    debugOnly = bool.Parse(reader.Value);
                                if (arenaScreen != null)
                                {
                                    if (!debugOnly && !GameController.runningFromEditor ||
                                        arenaScreen.playerStartingPos == Vector2.Zero ||
                                        debugOnly && GameController.runningFromEditor)
                                    {
                                        arenaScreen.playerStartingPos = playerStartPos;
                                        break;
                                    }

                                    break;
                                }

                                if (cutsceneScreen != null)
                                {
                                    if (!debugOnly && !GameController.runningFromEditor ||
                                        cutsceneScreen.cameraStartPos == Vector2.Zero ||
                                        debugOnly && GameController.runningFromEditor)
                                    {
                                        cutsceneScreen.cameraStartPos = playerStartPos;
                                        break;
                                    }

                                    break;
                                }

                                if (mapScreen != null && (!debugOnly && !GameController.runningFromEditor ||
                                                          mapScreen.startingPos == Vector2.Zero ||
                                                          debugOnly && GameController.runningFromEditor))
                                {
                                    mapScreen.startingPos = playerStartPos;
                                    break;
                                }

                                break;
                            }

                            break;
                        case "Enemy":
                            if (LevelParser.parsingFlag != LevelParser.ParsingFlag.SpritesAndCollHullsOnly)
                            {
                                reader.MoveToAttribute("Type");
                                string enemyTypeString = reader.Value;
                                string spritesheetString = enemyTypeString + "_Spritesheet";
                                string fullPathString = "art\\enemies\\" + spritesheetString;
                                // if (!SpriteLibrary.IsSpritesheetLoaded(spritesheetString))
                                // {
                                //     foreach (object obj in SpriteLibrary.LoadSpritesheetCoro(game.disposableContent,
                                //         fullPathString))
                                //         yield return null;
                                //     if (SpriteLibrary.justLoadedSpritesheetOk)
                                //         game.ScreenManager.AddLoadedSpritesheetPath(fullPathString);
                                // }

                                EnemyType enemyType = (EnemyType) Enum.Parse(typeof(EnemyType), enemyTypeString);
                                EnemyObj enemyObj =
                                    Activator.CreateInstance(Type.GetType("Brawler2D." + enemyType),
                                        (object) game) as EnemyObj;
                                string enemyName = enemyObj.Name;
                                Vector2 enemyScale = enemyObj.Scale;
                                LevelParser.LoadGenericData(reader, enemyObj, game);
                                enemyObj.Scale = enemyScale;
                                enemyObj.Name = enemyName;
                                if (arenaScreen != null)
                                {
                                    arenaScreen.AddObjToArenaZone(enemyObj);
                                    break;
                                }

                                if (cutsceneScreen != null)
                                {
                                    reader.MoveToAttribute("Layer");
                                    cutsceneScreen.AddGameObj(enemyObj,
                                        int.Parse(reader.Value, NumberStyles.Any, game.cultureInfo));
                                    break;
                                }

                                break;
                            }

                            break;
                    }

                    yield return null;
                }
            }

            reader.Close();
            if (LevelParser.parsingFlag != LevelParser.ParsingFlag.SpritesAndCollHullsOnly)
            {
                if (arenaScreen != null)
                {
                    game.EnemyManager.ClearRPEnemyIndexList();
                    yield return null;
                    foreach (object sortZoneObject in arenaScreen.SortZoneObjects())
                        yield return null;
                    foreach (object loadCustomCodeObj in LoadCustomCodeDataObjsHelper.LoadCustomCodeObjs(
                        m_CustomCodeObjList, game, (BrawlerScreen) arenaScreen))
                        yield return null;
                }
                else if (cutsceneScreen != null)
                {
                    foreach (object loadCustomCodeObj in LoadCustomCodeDataObjsHelper.LoadCustomCodeObjs(
                        m_CustomCodeObjList, game, (BrawlerScreen) cutsceneScreen))
                        yield return null;
                }
                else if (mapScreen != null)
                {
                    foreach (object loadCustomCodeObj in LoadCustomCodeDataObjsHelper.LoadCustomCodeObjs(
                        m_CustomCodeObjList, game, (BrawlerScreen) mapScreen))
                        yield return null;
                }
            }
        }
    }
}
