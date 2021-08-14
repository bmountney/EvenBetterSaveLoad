using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;
using SandBox.View.Map;

namespace BetterSaveLoad
{
    // This mod adds functionality for quick loading and incremental quick saving, as well as auto saving before battles.
    public class BetterSaveLoadSubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad() => new Harmony("mod.bannerlord.bettersaveload").PatchAll();
        // Quick save or quick load when the respective keys are pressed. Auto save when the player enters a battle.
        protected override void OnApplicationTick(float dt)
        {
            if (ScreenManager.TopScreen is MapScreen)
            {
                if (Input.IsKeyDown(InputKey.LeftControl) || Input.IsKeyDown(InputKey.RightControl))
                {
                    if (Input.IsKeyPressed(InputKey.S))
                    {
                        Campaign.Current.SaveHandler.QuickSaveCurrentGame();
                    }
                    else if (Input.IsKeyPressed(InputKey.L))
                    {
                        BetterSaveLoadHelper.QuickLoadPreviousGame();
                    }
                }
                if (Input.IsKeyPressed(InputKey.F9))
                {
                    BetterSaveLoadHelper.QuickLoadPreviousGame();
                }
                if (MapEvent.PlayerMapEvent != null)
                {
                    if (!_isAutoSaving)
                    {
                        BetterSaveLoadHelper.AutoSaveBeforeBattle(MapEvent.PlayerMapEvent);
                        _isAutoSaving = true;
                    }
                }
                else
                {
                    _isAutoSaving = false;
                }
            }
        }
        public override void OnGameLoaded(Game game, object initializerObject) => BetterSaveLoadHelper.InitializeSaveIndexes();
        public override void OnNewGameCreated(Game game, object initializerObject) => BetterSaveLoadHelper.InitializeSaveIndexes();
        private bool _isAutoSaving;
    }
}
