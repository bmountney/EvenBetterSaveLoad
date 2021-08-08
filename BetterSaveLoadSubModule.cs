using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;
using SandBox;
using SandBox.View.Map;

namespace BetterSaveLoad
{
    // This mod adds functionality for quick loading and incremental quick saving.
    public class BetterSaveLoadSubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad() => new Harmony("mod.bannerlord.bettersaveload").PatchAll();
        public override void OnGameLoaded(Game game, object initializerObject) => BetterSaveLoadPatch.InitializeQuickSaveIndex();
        // Quick save or quick load when the respective keys are pressed.
        protected override void OnApplicationTick(float dt)
        {
            if (ScreenManager.TopScreen is MapScreen)
            {
                if (Input.IsKeyDown(InputKey.LeftControl) || Input.IsKeyDown(InputKey.RightControl))
                {
                    if (Input.IsKeyDown(InputKey.S) && !_isSaving)
                    {
                        Campaign.Current.SaveHandler.QuickSaveCurrentGame();
                        _isSaving = true;
                    }
                    else if (Input.IsKeyDown(InputKey.L) && !_isLoading)
                    {
                        QuickLoadPreviousGame();
                        _isLoading = true;
                    }
                }
                else
                {
                    _isSaving = false;
                    _isLoading = false;
                }
                if (Input.IsKeyPressed(InputKey.F9))
                {
                    QuickLoadPreviousGame();
                }
            }
        }
        // Get the latest quick save, manual save or auto save.
        public void QuickLoadPreviousGame()
        {
            SaveGameFileInfo[] saveFiles = MBSaveLoad.GetSaveFiles(null);
            if (Extensions.IsEmpty<SaveGameFileInfo>(saveFiles))
            {
                InformationManager.DisplayMessage(new InformationMessage("No save files to load!"));
            }
            SaveHelper.TryLoadSave(Extensions.MaxBy<SaveGameFileInfo, DateTime>(saveFiles, (SaveGameFileInfo s) => TaleWorlds.Core.MetaDataExtensions.GetCreationTime(s.MetaData)), new Action<LoadResult>(StartGame), null);
        }
        private void StartGame(LoadResult loadResult)
        {
            ScreenManager.PopScreen();
            GameStateManager.Current.CleanStates(0);
            GameStateManager.Current = Module.CurrentModule.GlobalGameStateManager;
            MBGameManager.StartNewGame(new SandBoxGameManager(loadResult));
        }
        private bool _isSaving;
        private bool _isLoading;
    }
}
