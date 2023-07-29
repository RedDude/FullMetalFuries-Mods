using CDGEngine;
using FMOD_.Studio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Brawler2D
{
  internal class Map_CustomWorld_01 : MapScreen
  {
    private int m_groundLayerIndex;
    // private RenderTarget2D m_waterWaveRT;
    // public bool m_isPlayingSecret;
    // private SpriteObj m_menoetiusFloat;
    // private SpriteObj m_menoetiusArena;
    // private float m_storedArenaY;
    // private float m_storedFloatY;
    private float m_elapsedTime;

    public Map_CustomWorld_01(GameController game)
      : base(game)
    {
      this.m_enemySpawnLimit = new float[4]
      {
          4.25f,
          4.5f,
          4.75f,
          5f
      };
      this.m_enemyStartLevel = 2;
      this.m_defaultSongType = SongType.World1_Map;
      this.stageName = "Map_CustomWorld_01";
      // this.m_secretStages.Add("Dialogue_W01_PromNote_02");
      // this.m_secretStages.Add("Dialogue_W01_PromNote_01");
      this.m_chestList = new EquipmentType[0]
      {
      };
      this.m_trophiesList = new SkillType[0]
      {
      };
      this.m_monolithList = new SkillType[0]
      {
      };
    }

    public override IEnumerable<object> LoadContent()
    {
      GraphicsDevice graphicsDevice = this.Camera.GraphicsDevice;
      // this.m_waterWaveRT = new RenderTarget2D(graphicsDevice, 960, 540, false, graphicsDevice.PresentationParameters.BackBufferFormat, graphicsDevice.PresentationParameters.DepthStencilFormat, graphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.PreserveContents);
      foreach (object obj in base.LoadContent())
        yield return (object) null;
    }

    public override void HandleInput()
    {
      // if (this.m_isPlayingSecret)
        // return;
      base.HandleInput();
    }

    public void SecretAnimationComplete()
    {
      // this.m_isPlayingSecret = false;
    }

    public override void Initialize()
    {
      base.Initialize();
      for (int index = 0; index < this.gameLayerIndex; ++index)
      {
        if (this.m_layerArray[index].layerID == 2)
        {
          this.m_groundLayerIndex = index;
          break;
        }
      }
      /*LayerList layer = this.m_layerArray[this.m_groundLayerIndex];
      for (int index = 0; index < layer.count; ++index)
      {
        if (layer.values[index] is Cloud_SpriteObj cloudSpriteObj)
          cloudSpriteObj.Speed = 10f;
      }
      this.m_mapIconsList[this.m_mapZones.IndexOf(this.getZoneByStageName("SecretButton03"))].Active = false;
      this.m_menoetiusFloat = this.GetObjByName("Menoetius Float") as SpriteObj;
      this.m_menoetiusArena = this.GetObjByName("Menoetius Arena") as SpriteObj;
      this.m_storedFloatY = this.m_menoetiusFloat.Y;
      this.m_storedArenaY = this.m_menoetiusArena.Y;*/
    }

    public override void Update(float elapsedSeconds)
    {
      this.m_elapsedTime += elapsedSeconds;
      float num = (float) Math.Sin((double) this.m_elapsedTime) * 3.5f;
      // this.m_menoetiusFloat.Y = this.m_storedFloatY + num;
      // this.m_menoetiusArena.Y = this.m_storedArenaY + num;
      base.Update(elapsedSeconds);
    }

    public override void MoveToZone(MapZone zone, string direction, bool animate)
    {
      base.MoveToZone(zone, direction, animate);
      if (BlitNet.Lobby.IsMaster && zone.stageName == "Level_W01_Interlude_02")
      {
        if (this.Game.PlayerManager.hostPlayer.GetStageState("Level_W01_Interlude_02") == StageState.Beaten)
          return;
        Secrets.W01I02();
      }
      else
      {
        if (!BlitNet.Lobby.IsMaster || !(zone.stageName == "SecretButton03") || this.Game.PlayerManager.hostPlayer.GetStageState("SecretButton03") == StageState.Beaten)
          return;
        Secrets.W01S02();
      }
    }

    public override void LoadLevel(MapZone zoneSelected, PlayerObj player)
    {
      if (zoneSelected.teleportToTag != "" && zoneSelected.teleportToTag != null)
      {
        this.MoveToZone(this.getZoneByTag(zoneSelected.teleportToTag), "Teleport", true);
      }
      else
      {
        if (zoneSelected.isLinker)
          return;
        string str = zoneSelected.stageName;
        PlayerObj hostPlayer = this.Game.PlayerManager.hostPlayer;
        if (zoneSelected.stageToForceLoad != null && zoneSelected.stageToForceLoad != "")
          str = zoneSelected.stageToForceLoad;
        if (str == "Level_W01_02" && hostPlayer != null && !hostPlayer.act1Seen)
        {
          GameController.soundManager.PlayEvent("event:/SFX/Front End/Level Select/sfx_fe_lvlselect_startlvl", (IPositionalObj) null, false, false);
          if ((HandleBase_Studio) BrawlerScreen.musicEvent != (HandleBase_Studio) null)
          {
            int num = (int) BrawlerScreen.musicEvent.setParameterValue("leave_map", 1f);
          }
          this.Game.ScreenManager.stageFakeName = zoneSelected.stageFakeName;
          hostPlayer.loadLevelFromAct = true;
          this.Game.ScreenManager.LoadScreen("Cutscene_Act1", (int) hostPlayer.controllerIndex);
        }
        else
          base.LoadLevel(zoneSelected, player);
      }
    }

    protected override void DrawStaticObjs(
      int startLayerIndex,
      int endLayerIndex,
      float elapsedSeconds,
      LayerList[] layers = null)
    {
      if (startLayerIndex == this.m_groundLayerIndex)
      {
        RenderTarget2D currentRenderTarget = this.Camera.currentRenderTarget;
        // this.Camera.GraphicsDevice.SetRenderTarget(this.m_waterWaveRT);
        base.DrawStaticObjs(this.m_groundLayerIndex, this.m_groundLayerIndex, elapsedSeconds, (LayerList[]) null);
        this.Camera.GraphicsDevice.SetRenderTarget(currentRenderTarget);
        // this.Game.shaderManager.globalShader.Parameters["waveOffset"].SetValue(new Vector3(0.0f, 0.0f, 1f));
        // this.Game.shaderManager.SetHeatWaveProperties(20f, 3f, 0.005f, false);
        // this.Game.shaderManager.ToggleGlobalShader(ShaderType.HeatWave, true);
        this.Camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, (DepthStencilState) null, (RasterizerState) null, this.Game.shaderManager.globalShader);
        // this.Camera.Draw((Texture2D) this.m_waterWaveRT, Vector2.Zero, Color.White);
        this.Camera.End();
        // this.Game.shaderManager.ToggleGlobalShader(ShaderType.HeatWave, false);
        base.DrawStaticObjs(this.m_groundLayerIndex + 1, endLayerIndex, elapsedSeconds, (LayerList[]) null);
      }
      else
        base.DrawStaticObjs(startLayerIndex, endLayerIndex, elapsedSeconds, (LayerList[]) null);
    }

    public override void Dispose()
    {
      if (this.IsDisposed)
        return;
      // this.m_waterWaveRT.Dispose();
      // this.m_waterWaveRT = (RenderTarget2D) null;
      // this.m_menoetiusArena = (SpriteObj) null;
      // this.m_menoetiusFloat = (SpriteObj) null;
      base.Dispose();
    }
  }
}
