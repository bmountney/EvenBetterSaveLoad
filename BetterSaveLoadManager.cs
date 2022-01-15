using HarmonyLib;
using SandBox;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;

namespace BetterSaveLoad
{
    [HarmonyPatch(typeof(MBSaveLoad))]
    public static class BetterSaveLoadManager
    {
        // Get the name of the currently loaded save file.
        [HarmonyPostfix]
        [HarmonyPatch("LoadSaveGameData")]
        public static void Postfix1(string ___ActiveSaveSlotName) => ActiveSaveSlotName = ___ActiveSaveSlotName;
        [HarmonyPostfix]
        [HarmonyPatch("OnNewGame")]
        public static void Postfix2(string ___ActiveSaveSlotName) => ActiveSaveSlotName = ___ActiveSaveSlotName;
        // Increment the quick save index. Reset the quick save index if it is greater than the maximum number in the settings.
        // Replace the save file name with a custom one with the quick save index.
        // Display the file name of the saved game in a debug message.
        [HarmonyPatch("QuickSaveCurrentGame")]
        public static void Prefix(ref string ___ActiveSaveSlotName)
        {
            QuickSaveIndex++;
            if (Settings.ShouldLimitSaves && QuickSaveIndex > Settings.QuickSaveLimit)
            {
                QuickSaveIndex = 1;
            }
            ___ActiveSaveSlotName = QuickSaveNamePrefix + QuickSaveIndex;
            ActiveSaveSlotName = ___ActiveSaveSlotName;
            InformationManager.DisplayMessage(new InformationMessage("Game saved: \"" + ActiveSaveSlotName + "\"."));
        }
        // Set the quick save index to the number in the latest quick save file name.
        // Set the battle auto save index to the number in the latest battle auto save file name.
        // Display the file name of the loaded game in a debug message.
        public static void InitializeSaveIndexes()
        {
            string quickSaveName = string.Empty;
            string battleAutoSaveName = string.Empty;
            List<SaveGameFileInfo> saveFiles = new List<SaveGameFileInfo>(MBSaveLoad.GetSaveFiles());
            saveFiles.Reverse();
            foreach (SaveGameFileInfo saveFile in saveFiles)
            {
                if (saveFile.Name.StartsWith(QuickSaveNamePrefix))
                {
                    quickSaveName = saveFile.Name;
                }
                if (saveFile.Name.StartsWith(BattleAutoSaveNamePrefix))
                {
                    battleAutoSaveName = saveFile.Name;
                }
            }
            if (!string.IsNullOrEmpty(quickSaveName) && int.TryParse(quickSaveName.Substring(QuickSaveNamePrefix.Length), out int num) && num > 0 && num <= Settings.QuickSaveLimit)
            {
                QuickSaveIndex = num;
            }
            if (!string.IsNullOrEmpty(battleAutoSaveName) && int.TryParse(battleAutoSaveName.Substring(BattleAutoSaveNamePrefix.Length), out int num2) && num2 > 0 && num2 <= Settings.BattleAutoSaveLimit)
            {
                BattleAutoSaveIndex = num2;
            }
            if (ActiveSaveSlotName != null)
            {
                InformationManager.DisplayMessage(new InformationMessage("Game loaded: \"" + ActiveSaveSlotName + "\"."));
            }
        }
        // Get the latest quick save, manual save or auto save.
        public static void QuickLoadPreviousGame()
        {
            SaveGameFileInfo saveFileWithName = MBSaveLoad.GetSaveFileWithName(BannerlordConfig.LatestSaveGameName);
            if (saveFileWithName != null && !saveFileWithName.IsCorrupted)
            {
                SaveHelper.TryLoadSave(saveFileWithName, new Action<LoadResult>(StartGame), null);
                return;
            }
            InformationManager.DisplayMessage(new InformationMessage("No save files to load!"));
        }
        private static void StartGame(LoadResult loadResult)
        {
            ScreenManager.PopScreen();
            GameStateManager.Current.CleanStates(0);
            GameStateManager.Current = Module.CurrentModule.GlobalGameStateManager;
            MBGameManager.StartNewGame(new SandBoxGameManager(loadResult));
        }
        // Execute only if the numbers of attackers and defenders are greater than or equal to the minimum numbers in the settings.
        // Increment the battle auto save index. Reset the battle auto save index if it is greater than the maximum number in the settings.
        // Display the file name of the saved game in a debug message.
        public static void AutoSaveBeforeBattle(MapEvent mapEvent)
        {
            if (Settings.ShouldAutoSaveBeforeBattle && mapEvent.AttackerSide.TroopCount >= Settings.MinAttackerTroopCount && mapEvent.DefenderSide.TroopCount >= Settings.MinDefenderTroopCount)
            {
                BattleAutoSaveIndex++;
                if (Settings.ShouldLimitSaves && BattleAutoSaveIndex > Settings.BattleAutoSaveLimit)
                {
                    BattleAutoSaveIndex = 1;
                }
                ActiveSaveSlotName = BattleAutoSaveNamePrefix + BattleAutoSaveIndex;
                Campaign.Current.SaveHandler.SaveAs(ActiveSaveSlotName);
                InformationManager.DisplayMessage(new InformationMessage("Game saved: \"" + ActiveSaveSlotName + "\"."));
            }
        }
        private static int QuickSaveIndex = 0;
        private static int BattleAutoSaveIndex = 0;
        private static string ActiveSaveSlotName = null;
        private static readonly string QuickSaveNamePrefix = "save_quick_";
        private static readonly string BattleAutoSaveNamePrefix = "save_auto_battle_";
        private static readonly BetterSaveLoadSettings Settings = BetterSaveLoadSettings.Instance;
    }
}
