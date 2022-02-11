using Bannerlord.ButterLib.HotKeys;
using HarmonyLib;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.MountAndBlade;

namespace BetterSaveLoad
{
    // This mod adds functionality for quick loading and incremental quick saving, as well as auto saving before and after battles.
    public class BetterSaveLoadSubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad() => new Harmony("mod.bannerlord.bettersaveload").PatchAll();
        // Quick save or quick load when the respective keys are pressed.
        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            if (!_isHotKeyManagerCreated)
            {
                HotKeyManager hotKeyManager = HotKeyManager.Create("BetterSaveLoad");
                BetterSaveLoadHotKeys.LCtrl lCtrl = hotKeyManager.Add<BetterSaveLoadHotKeys.LCtrl>();
                BetterSaveLoadHotKeys.RCtrl rCtrl = hotKeyManager.Add<BetterSaveLoadHotKeys.RCtrl>();
                BetterSaveLoadHotKeys.S s = hotKeyManager.Add<BetterSaveLoadHotKeys.S>();
                BetterSaveLoadHotKeys.L l = hotKeyManager.Add<BetterSaveLoadHotKeys.L>();
                BetterSaveLoadHotKeys.F9 f9 = hotKeyManager.Add<BetterSaveLoadHotKeys.F9>();
                s.Predicate = () => ScreenManager.TopScreen is MapScreen;
                l.Predicate = () => ScreenManager.TopScreen is MapScreen;
                f9.Predicate = () => ScreenManager.TopScreen is MapScreen;
                bool isCtrlDown = false;
                lCtrl.OnPressedEvent += () => isCtrlDown = true;
                lCtrl.OnReleasedEvent += () => isCtrlDown = false;
                rCtrl.OnPressedEvent += () => isCtrlDown = true;
                rCtrl.OnReleasedEvent += () => isCtrlDown = false;
                s.OnPressedEvent += () =>
                {
                    if (isCtrlDown)
                    {
                        Campaign.Current.SaveHandler.QuickSaveCurrentGame();
                    }
                };
                l.OnPressedEvent += () =>
                {
                    if (isCtrlDown)
                    {
                        BetterSaveLoadManager.QuickLoadPreviousGame();
                    }
                };
                f9.OnPressedEvent += () => BetterSaveLoadManager.QuickLoadPreviousGame();
                hotKeyManager.Build();
                _isHotKeyManagerCreated = true;
            }
        }
        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            if (game.GameType is Campaign)
            {
                CampaignGameStarter campaignStarter = (CampaignGameStarter)gameStarter;
                campaignStarter.AddBehavior(new BetterSaveLoadBehavior());
            }
        }
        private bool _isHotKeyManagerCreated;
    }
}
