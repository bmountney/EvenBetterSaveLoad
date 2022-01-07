using HarmonyLib;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;

namespace BetterSaveLoad
{
    // This mod adds functionality for quick loading and incremental quick saving, as well as auto saving before battles.
    public class BetterSaveLoadSubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad() => new Harmony("mod.bannerlord.bettersaveload").PatchAll();
        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            if (game.GameType is Campaign)
            {
                CampaignGameStarter campaignStarter = (CampaignGameStarter)gameStarter;
                campaignStarter.AddBehavior(new BetterSaveLoadBehavior());
            }
        }
        // Quick save or quick load when the respective keys are pressed.
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
                        BetterSaveLoadManager.QuickLoadPreviousGame();
                    }
                }
                if (Input.IsKeyPressed(InputKey.F9))
                {
                    BetterSaveLoadManager.QuickLoadPreviousGame();
                }
            }
        }
    }
}
