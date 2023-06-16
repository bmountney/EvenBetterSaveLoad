using Bannerlord.ButterLib.HotKeys;
using HarmonyLib;
using SandBox;
using SandBox.View.Map;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.SaveSystem.Load;
using TaleWorlds.ScreenSystem;

namespace BetterSaveLoad
{
    // This mod adds functionality for quick loading and incremental quick saving, as well as auto saving before and after battles.
    public class BetterSaveLoadSubModule : MBSubModuleBase
    {
        private bool _isHotKeyManagerCreated;

        protected override void OnSubModuleLoad() => new Harmony("mod.bannerlord.bettersaveload").PatchAll();

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
                bool isCtrlDown = false;

                s.Predicate = () => ScreenManager.TopScreen is MapScreen;
                l.Predicate = () => ScreenManager.TopScreen is MapScreen || ScreenManager.TopScreen is MissionScreen;
                f9.Predicate = () => ScreenManager.TopScreen is MapScreen;
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

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (game.GameType is Campaign)
            {
                ((CampaignGameStarter)gameStarterObject).AddBehavior(new BetterSaveLoadBehavior());
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
