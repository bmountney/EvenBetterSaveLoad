using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine.Screens;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;
using SandBox;

namespace BetterSaveLoad
{
    [HarmonyPatch(typeof(MBSaveLoad))]
    public static class BetterSaveLoadHelper
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
        // Set the quick save index to the highest number in the list of quick save file names.
        // Set the battle auto save index to the highest number in the list of battle auto save file names.
        // Display the file name of the loaded game in a debug message.
        public static void InitializeSaveIndexes()
        {
            List<int> list = new List<int>();
            List<int> list2 = new List<int>();
            foreach (string text in MBSaveLoad.GetSaveFileNames())
            {
                if (text.Contains(QuickSaveNamePrefix))
                {
                    string[] array = text.Split(new char[] { '_' });
                    if (array.Length == 3 && int.TryParse(array[array.Length - 1], out int num) && num > 0)
                    {
                        list.Add(num);
                    }
                }
                if (text.Contains(BattleAutoSaveNamePrefix))
                {
                    string[] array = text.Split(new char[] { '_' });
                    if (array.Length == 4 && int.TryParse(array[array.Length - 1], out int num) && num > 0)
                    {
                        list2.Add(num);
                    }
                }
            }
            if (list.Count > 0)
            {
                QuickSaveIndex = list.Max();
            }
            if (list2.Count > 0)
            {
                BattleAutoSaveIndex = list2.Max();
            }
            if (ActiveSaveSlotName != null)
            {
                HasLoadedAutoSave = ActiveSaveSlotName.Contains(BattleAutoSaveNamePrefix);
                InformationManager.DisplayMessage(new InformationMessage("Game loaded: \"" + ActiveSaveSlotName + "\"."));
            }
        }
        // Get the latest quick save, manual save or auto save.
        public static void QuickLoadPreviousGame()
        {
            SaveGameFileInfo[] saveFiles = MBSaveLoad.GetSaveFiles(null);
            if (saveFiles.IsEmpty<SaveGameFileInfo>())
            {
                InformationManager.DisplayMessage(new InformationMessage("No save files to load!"));
                return;
            }
            SaveHelper.TryLoadSave(saveFiles.MaxBy((SaveGameFileInfo s) => s.MetaData.GetCreationTime()), new Action<LoadResult>(StartGame), null);
        }
        private static void StartGame(LoadResult loadResult)
        {
            ScreenManager.PopScreen();
            GameStateManager.Current.CleanStates(0);
            GameStateManager.Current = Module.CurrentModule.GlobalGameStateManager;
            MBGameManager.StartNewGame(new SandBoxGameManager(loadResult));
        }
        // Do not execute if a battle auto save was loaded to prevent auto saving again.
        // Execute only if the numbers of attackers and defenders are greater than or equal to the minimum numbers in the settings.
        // Increment the battle auto save index. Reset the battle auto save index if it is greater than the maximum number in the settings.
        // Display the file name of the saved game in a debug message.
        public static void AutoSaveBeforeBattle(MapEvent playerMapEvent)
        {
            if (!HasLoadedAutoSave)
            {
                if (Settings.ShouldAutoSaveBeforeBattle && playerMapEvent.AttackerSide.TroopCount >= Settings.MinAttackerTroopCount && playerMapEvent.DefenderSide.TroopCount >= Settings.MinDefenderTroopCount)
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
            else
            {
                HasLoadedAutoSave = false;
            }
        }
        private static int QuickSaveIndex = 0;
        private static int BattleAutoSaveIndex = 0;
        private static string QuickSaveNamePrefix = "save_quick_";
        private static string BattleAutoSaveNamePrefix = "save_auto_battle_";
        private static string ActiveSaveSlotName = null;
        private static bool HasLoadedAutoSave = false;
        private static BetterSaveLoadSettings Settings = BetterSaveLoadSettings.Instance;
    }
}
