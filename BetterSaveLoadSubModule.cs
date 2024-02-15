using HarmonyLib;
using SandBox;
using SandBox.View.Map;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.SaveSystem.Load;
using TaleWorlds.ScreenSystem;

namespace BetterSaveLoad
{
    // This mod adds functionality for quick loading and incremental quick saving, as well as auto saving before and after battles.
    public class BetterSaveLoadSubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad() => new Harmony("mod.bannerlord.bettersaveload").PatchAll();

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (game.GameType is Campaign)
            {
                ((CampaignGameStarter)gameStarterObject).AddBehavior(new BetterSaveLoadCampaignBehavior());
            }
        }

        protected override void OnApplicationTick(float dt)
        {
            if (Campaign.Current != null)
            {
                IInputContext input = MapScreen.Instance?.Input ?? Mission.Current?.InputManager;

                if (input != null)
                {
                    bool isMapScreen = ScreenManager.TopScreen is MapScreen, isMissionScreen = ScreenManager.TopScreen is MissionScreen;

                    if (input.IsControlDown())
                    {
                        if (input.IsKeyPressed(InputKey.S) && isMapScreen)
                        {
                            Campaign.Current.SaveHandler.QuickSaveCurrentGame();
                        }

                        if (input.IsKeyPressed(InputKey.L) && (isMapScreen || isMissionScreen))
                        {
                            BetterSaveLoadManager.QuickLoadPreviousGame();
                        }
                    }

                    if (input.IsKeyPressed(InputKey.F9) && isMapScreen)
                    {
                        BetterSaveLoadManager.QuickLoadPreviousGame();
                    }
                }
            }
        }

        public override void OnInitialState()
        {
            if (BetterSaveLoadManager.CanLoad)
            {
                SandBoxSaveHelper.TryLoadSave(BetterSaveLoadManager.SaveFile, new Action<LoadResult>(BetterSaveLoadManager.StartGame), null);
            }
        }
    }
}
