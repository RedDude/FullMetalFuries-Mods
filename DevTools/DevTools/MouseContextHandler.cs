using System.Collections.Generic;
using System.Linq;
using Brawler2D;
using CDGEngine;
using ImGuiMod;
using Microsoft.Xna.Framework;

namespace QuickStart
{
    public class MouseContextHandler
    {
        public static List<DisplayObj> underMouseEntities = new List<DisplayObj>();

        public void CheckOpenContext(GameController game, IImGuiMenuApi imGuiMenuApi)
        {
            bool pressed = game.InputManager.MouseRightPressed();
            if (!pressed) return;

            var mouseWorldPosition = GetMouseWorldPosition(game);

            int globalEnemyArrayCount = GameController.g_game.ScreenManager.arenaScreen.currentAndGlobalEnemyArray_count;

            underMouseEntities.Clear();
            for (int i = 0; i < globalEnemyArrayCount; i++)
            {
                var entity = GameController.g_game.ScreenManager.arenaScreen.currentAndGlobalEnemyArray[i];
                if(CheckUnderMouse(mouseWorldPosition, entity) != null)
                    underMouseEntities.Add(entity);
            }

            var projectileManager = GameController.g_game.ProjectileManager;
            if (projectileManager != null)
            {
                var projectileList = projectileManager.activeObjList.ToList();
                for (int i = 0; i < projectileList.Count; i++)
                {
                    var entity =projectileList[i];
                    if(CheckUnderMouse(mouseWorldPosition, entity) != null)
                        underMouseEntities.Add(entity);
                }
            }

            for (int index = 0; index < game.PlayerManager.activePlayerArray_count; ++index)
            {
                var entity = game.PlayerManager.activePlayerArray[index];
                if(CheckUnderMouse(mouseWorldPosition, entity.currentPlayerClass) != null)
                    underMouseEntities.Add(entity.currentPlayerClass);
            }

            imGuiMenuApi.ShowContextMenu(mouseWorldPosition);
        }

        public List<DisplayObj> FindEntitiesUnderMouse(Vector2 position, List<DisplayObj> listToCheck, int count) //ArenaScreen arenaScreen)
        {
            var displayObjs = new List<DisplayObj>();
            for (int index = 0; index < count; ++index)
            {
                var entity = listToCheck[index];

                if(CheckUnderMouse(position, entity) != null)
                    displayObjs.Add(entity);
            }
            return displayObjs;
        }

        private static DisplayObj CheckUnderMouse(Vector2 position, DisplayObj entity)
        {
            var cdgRect =
                new CDGRect(entity.AbsX + entity.Bounds.X,
                    entity.AbsY + entity.Bounds.Y,
                    entity.Bounds.Width,
                    entity.Bounds.Height);

            return cdgRect.Contains(position) ? entity : null;
        }

        public static Vector2 GetMouseWorldPosition(GameController game)
        {
            float physicalScaleX = game.ScreenManager.virtualScreen.physicalScaleX;
            float physicalScaleY = game.ScreenManager.virtualScreen.physicalScaleY;
            var mouseMouse = new Vector2(
                game.InputManager.mouseX * 1f / physicalScaleX,
                game.InputManager.mouseY * 1f / physicalScaleY);
            var arenaScreen = game.ScreenManager.arenaScreen;
            var camera = arenaScreen.Camera;
            return Vector2.Transform(mouseMouse, Matrix.Invert(camera.TransformMatrix));
        }
    }
}
