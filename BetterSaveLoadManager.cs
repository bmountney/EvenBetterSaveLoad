using HarmonyLib;
using SandBox;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;

namespace BetterSaveLoad
{
    [HarmonyPatch(typeof(MBSaveLoad))]
    public static class BetterSaveLoadManager
    {
        private static readonly BetterSaveLoadSettings Settings = BetterSaveLoadSettings.Instance;
        private static readonly string QuickSaveNamePrefix = "save_quick_";
        private static readonly string BattleAutoSaveNamePrefix = "save_auto_battle_";
        private static int QuickSaveIndex = 0;
        private static int BattleAutoSaveIndex = 0;
        private static string ActiveSaveSlotName = null;

        public static string PlayerClanAndMainHeroName => Clan.PlayerClan.Name.ToString().ToLower() + "_" + Hero.MainHero.Name.ToString().ToLower() + "_";

        public static bool CanLoad => SaveFileWithName != null && !SaveFileWithName.IsCorrupted;

        public static SaveGameFileInfo SaveFileWithName { get; set; }

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
            ___ActiveSaveSlotName = QuickSaveNamePrefix + PlayerClanAndMainHeroName + QuickSaveIndex;
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
            string quickSaveNameWithoutIndex = QuickSaveNamePrefix + PlayerClanAndMainHeroName;
            string battleAutoSaveNameWithoutIndex = BattleAutoSaveNamePrefix + PlayerClanAndMainHeroName;
            List<SaveGameFileInfo> saveFiles = new List<SaveGameFileInfo>(MBSaveLoad.GetSaveFiles());
            quickSaveName = saveFiles.Find(saveFile => saveFile.Name.StartsWith(quickSaveNameWithoutIndex))?.Name;
            battleAutoSaveName = saveFiles.Find(saveFile => saveFile.Name.StartsWith(battleAutoSaveNameWithoutIndex))?.Name;
            QuickSaveIndex = 0;
            BattleAutoSaveIndex = 0;
            if (!string.IsNullOrEmpty(quickSaveName) && int.TryParse(quickSaveName.Substring((quickSaveNameWithoutIndex).Length), out int quickSaveIndex) && quickSaveIndex > 0 && quickSaveIndex <= Settings.QuickSaveLimit)
            {
                QuickSaveIndex = quickSaveIndex;
            }
            if (!string.IsNullOrEmpty(battleAutoSaveName) && int.TryParse(battleAutoSaveName.Substring((battleAutoSaveNameWithoutIndex).Length), out int battleAutoSaveIndex) && battleAutoSaveIndex > 0 && battleAutoSaveIndex <= Settings.BattleAutoSaveLimit)
            {
                BattleAutoSaveIndex = battleAutoSaveIndex;
            }
            if (ActiveSaveSlotName != null)
            {
                InformationManager.DisplayMessage(new InformationMessage("Game loaded: \"" + ActiveSaveSlotName + "\"."));
            }
        }

        // Execute only if the numbers of attackers and defenders are greater than or equal to the minimum numbers in the settings.
        // Increment the battle auto save index. Reset the battle auto save index if it is greater than the maximum number in the settings.
        // Display the file name of the saved game in a debug message.
        public static void AutoSaveForBattle(MapEvent mapEvent)
        {
            bool shouldAutoSaveBeforeBattle = !mapEvent.IsFinalized && (Settings.BattleAutoSaveTrigger.SelectedIndex == 0 || Settings.BattleAutoSaveTrigger.SelectedIndex == 2) && mapEvent.AttackerSide.TroopCount >= Settings.MinAttackerTroopCount && mapEvent.DefenderSide.TroopCount >= Settings.MinDefenderTroopCount;
            bool shouldAutoSaveAfterBattle = mapEvent.IsFinalized && (Settings.BattleAutoSaveTrigger.SelectedIndex == 1 || Settings.BattleAutoSaveTrigger.SelectedIndex == 2);
            if (Settings.ShouldAutoSaveForBattle && (shouldAutoSaveBeforeBattle || shouldAutoSaveAfterBattle))
            {
                BattleAutoSaveIndex++;
                if (Settings.ShouldLimitSaves && BattleAutoSaveIndex > Settings.BattleAutoSaveLimit)
                {
                    BattleAutoSaveIndex = 1;
                }
                ActiveSaveSlotName = BattleAutoSaveNamePrefix + PlayerClanAndMainHeroName + BattleAutoSaveIndex;
                Campaign.Current.SaveHandler.SaveAs(ActiveSaveSlotName);
                InformationManager.DisplayMessage(new InformationMessage("Game saved: \"" + ActiveSaveSlotName + "\"."));
            }
        }

        // Get the latest quick save, manual save or auto save.
        public static void QuickLoadPreviousGame()
        {
            SaveFileWithName = MBSaveLoad.GetSaveFileWithName(BannerlordConfig.LatestSaveGameName);
            if (CanLoad)
            {
                MBGameManager.EndGame();
                return;
            }
            InformationManager.DisplayMessage(new InformationMessage("No save files to load!"));
        }

        public static void StartGame(LoadResult loadResult)
        {
            SaveFileWithName = null;
            MBGameManager.StartNewGame(new SandBoxGameManager(loadResult));
        }
    }
}
