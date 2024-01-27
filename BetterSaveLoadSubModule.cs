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
using static BetterSaveLoad.BetterSaveLoadHotKeys;

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
                LCtrl lCtrl = hotKeyManager.Add<LCtrl>();
                RCtrl rCtrl = hotKeyManager.Add<RCtrl>();
                S s = hotKeyManager.Add<S>();
                L l = hotKeyManager.Add<L>();
                F9 f9 = hotKeyManager.Add<F9>();

                s.Predicate = () => Campaign.Current != null && ScreenManager.TopScreen is MapScreen;
                l.Predicate = () => Campaign.Current != null && (ScreenManager.TopScreen is MapScreen || ScreenManager.TopScreen is MissionScreen);
                f9.Predicate = () => Campaign.Current != null && ScreenManager.TopScreen is MapScreen;

                lCtrl.OnPressedEvent += () =>
                {
                    s.IsEnabled = true;
                    l.IsEnabled = true;
                };
                lCtrl.OnReleasedEvent += () =>
                {
                    s.IsEnabled = false;
                    l.IsEnabled = false;
                };
                rCtrl.OnPressedEvent += () =>
                {
                    s.IsEnabled = true;
                    l.IsEnabled = true;
                };
                rCtrl.OnReleasedEvent += () =>
                {
                    s.IsEnabled = false;
                    l.IsEnabled = false;
                };
                s.OnPressedEvent += () => Campaign.Current.SaveHandler.QuickSaveCurrentGame();
                l.OnPressedEvent += () => BetterSaveLoadManager.QuickLoadPreviousGame();
                f9.OnPressedEvent += () => BetterSaveLoadManager.QuickLoadPreviousGame();

                hotKeyManager.Build();

                _isHotKeyManagerCreated = true;
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (game.GameType is Campaign)
            {
                ((CampaignGameStarter)gameStarterObject).AddBehavior(new BetterSaveLoadCampaignBehavior());
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
