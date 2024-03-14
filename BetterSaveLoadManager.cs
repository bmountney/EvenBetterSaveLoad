using HarmonyLib;
using SandBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;
using System.Text.RegularExpressions;

namespace BetterSaveLoad
{
    [HarmonyPatch(typeof(MBSaveLoad))]
    public static class BetterSaveLoadManager
    {
        private static readonly BetterSaveLoadSettings Settings = BetterSaveLoadSettings.Instance;
        //private static readonly string QuickSaveNamePrefix = "", BattleAutoSaveNamePrefix = "";

        private static int QuickSaveIndex = 0, AutoSaveIndex = 0, PreBattleSaveIndex = 0, PostBattleSaveIndex = 0;
        private static string ActiveSaveSlotName = null;

        public static SaveGameFileInfo SaveFile { get; set; }

        public static bool CanLoad => SaveFile != null && !SaveFile.IsCorrupted;

        //public static string PlayerClanAndMainHeroName => Clan.PlayerClan.Name.ToString().ToLower() + "_" + Hero.MainHero.Name.ToString().ToLower() + "_";
        public static string PlayerClanAndMainHeroName => Clan.PlayerClan.Name.ToString() + " " + Hero.MainHero.Name.ToString();

        // Get the name of the currently loaded save file.
        [HarmonyPostfix]
        [HarmonyPatch("LoadSaveGameData")]
        public static void Postfix1(string ___ActiveSaveSlotName) => ActiveSaveSlotName = ___ActiveSaveSlotName;

        [HarmonyPostfix]
        [HarmonyPatch("OnNewGame")]
        public static void Postfix2(string ___ActiveSaveSlotName) => ActiveSaveSlotName = ___ActiveSaveSlotName;

        [HarmonyPatch("QuickSaveCurrentGame")]
        public static void Prefix(ref string ___ActiveSaveSlotName)
        {
            string saveFileIndex;
            if (Settings.ShouldUseIndex)
            {
                if (++QuickSaveIndex > Settings.QuickSaveLimit && Settings.ShouldLimitSaves)
                {
                    // Reset the quick save index if it is greater than the maximum number in the settings.
                    QuickSaveIndex = 1;
                }
                saveFileIndex = GetQuickSaveFileIndex();
            }
            else
            {
                saveFileIndex = GetSaveFileTimeStamp();
            }
            ActiveSaveSlotName = ___ActiveSaveSlotName = GetSaveFileBaseName() + saveFileIndex + Settings.QuickSaveFileNameSuffix;

            // Display the file name of the saved game in a debug message.
            InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=BSLmsg001}Game saved: \"{SAVENAME}\".").SetTextVariable("SAVENAME", ActiveSaveSlotName).ToString()));
        }

        public static void InitializeSaveIndexes()
        {
            string quickSaveName = string.Empty, autoSaveName = string.Empty, preBattleSaveName = string.Empty, postBattleSaveName = string.Empty,
                saveBaseName = GetSaveFileBaseName(),
                quickSaveSearchName = WildCardToRegular(GetSaveFileBaseName() + "*" + Settings.QuickSaveFileNameSuffix),
                autoSaveSearchName = WildCardToRegular(GetSaveFileBaseName() + "*" + Settings.AutoSaveFileNameSuffix),
                preBattleSaveSearchName = WildCardToRegular(GetSaveFileBaseName() + "*" + Settings.PreBattleFileNameSuffix),
                postBasttleSaveSearchName = WildCardToRegular(GetSaveFileBaseName() + "*" + Settings.PostBattleFileNameSuffix),
                quickSaveNameIndex, autoSaveNameIndex, preBattleSaveNameIndex, postBattleSaveNameIndex;

            quickSaveName = MBSaveLoad.GetSaveFiles().FirstOrDefault(saveFile => Regex.IsMatch(saveFile.Name, quickSaveSearchName))?.Name;
            autoSaveName = MBSaveLoad.GetSaveFiles().FirstOrDefault(saveFile => Regex.IsMatch(saveFile.Name, autoSaveSearchName))?.Name;
            preBattleSaveName = MBSaveLoad.GetSaveFiles().FirstOrDefault(saveFile => Regex.IsMatch(saveFile.Name, preBattleSaveSearchName))?.Name;
            postBattleSaveName = MBSaveLoad.GetSaveFiles().FirstOrDefault(saveFile => Regex.IsMatch(saveFile.Name, postBasttleSaveSearchName))?.Name;

            QuickSaveIndex = 0;
            AutoSaveIndex = 0;
            PreBattleSaveIndex = 0;
            PostBattleSaveIndex = 0;

            if (Settings.ShouldUseIndex)
            {
                if (!string.IsNullOrEmpty(quickSaveName))
                {
                    // Set the quick save index to the number in the latest quick save file name.
                    quickSaveNameIndex = quickSaveName.Substring(saveBaseName.Length, quickSaveName.Length - saveBaseName.Length - Settings.QuickSaveFileNameSuffix.Length).TrimStart('0');
                    if (int.TryParse(quickSaveNameIndex, out int quickSaveIndex) && quickSaveIndex > 0 && quickSaveIndex <= Settings.QuickSaveLimit)
                        QuickSaveIndex = quickSaveIndex;
                }
                if (!string.IsNullOrEmpty(autoSaveName))
                {
                    // Set the auto save index to the number in the latest quick save file name.
                    autoSaveNameIndex = autoSaveName.Substring(saveBaseName.Length, autoSaveName.Length - saveBaseName.Length - Settings.AutoSaveFileNameSuffix.Length).TrimStart('0');
                    if (int.TryParse(autoSaveNameIndex, out int autoSaveIndex) && autoSaveIndex > 0 && autoSaveIndex <= Settings.AutoSaveLimit)
                        AutoSaveIndex = autoSaveIndex;
                }
                if (!string.IsNullOrEmpty(preBattleSaveName))
                {
                    // Set the pre save index to the number in the latest quick save file name.
                    preBattleSaveNameIndex = preBattleSaveName.Substring(saveBaseName.Length, preBattleSaveName.Length - saveBaseName.Length - Settings.PreBattleFileNameSuffix.Length).TrimStart('0');
                    if (int.TryParse(preBattleSaveNameIndex, out int preBattleSaveIndex) && preBattleSaveIndex > 0 && preBattleSaveIndex <= Settings.PreBattleSaveLimit)
                        PreBattleSaveIndex = preBattleSaveIndex;
                }
                if (!string.IsNullOrEmpty(postBattleSaveName))
                {
                    // Set the post save index to the number in the latest quick save file name.
                    postBattleSaveNameIndex = postBattleSaveName.Substring(saveBaseName.Length, postBattleSaveName.Length - saveBaseName.Length - Settings.PostBattleFileNameSuffix.Length).TrimStart('0');
                    if (int.TryParse(postBattleSaveNameIndex, out int postBattleSaveIndex) && postBattleSaveIndex > 0 && postBattleSaveIndex <= Settings.PostBattleSaveLimit)
                        PostBattleSaveIndex = postBattleSaveIndex;
                }
            }

            if (ActiveSaveSlotName != null)
            {
                // Display the file name of the loaded game in a debug message.
                InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=BSLmsg002}Game loaded: \"{SAVENAME}\".").SetTextVariable("SAVENAME", ActiveSaveSlotName).ToString()));
            }
        }

        public static void AutoSaveForBattle(MapEvent mapEvent)
        {
            bool shouldAutoSaveBeforeBattle = !mapEvent.IsFinalized && (Settings.BattleAutoSaveTrigger.SelectedIndex == 0 || Settings.BattleAutoSaveTrigger.SelectedIndex == 2) && mapEvent.AttackerSide.TroopCount >= Settings.MinAttackerTroopCount && mapEvent.DefenderSide.TroopCount >= Settings.MinDefenderTroopCount;
            bool shouldAutoSaveAfterBattle = mapEvent.IsFinalized && (Settings.BattleAutoSaveTrigger.SelectedIndex == 1 || Settings.BattleAutoSaveTrigger.SelectedIndex == 2) && mapEvent.AttackerSide.TroopCount + mapEvent.AttackerSide.Casualties >= Settings.MinAttackerTroopCount && mapEvent.DefenderSide.TroopCount + mapEvent.DefenderSide.Casualties >= Settings.MinDefenderTroopCount;

            // Execute only if the numbers of attackers and defenders are greater than or equal to the minimum numbers in the settings.
            if (Settings.ShouldAutoSaveForBattle && (shouldAutoSaveBeforeBattle || shouldAutoSaveAfterBattle))
            {
                string saveFileIndex = null, saveFileSuffix = null;
                if (Settings.ShouldUseIndex)
                {
                    if (shouldAutoSaveBeforeBattle)
                    {
                        if (++PreBattleSaveIndex > Settings.PreBattleSaveLimit && Settings.ShouldLimitSaves)
                        {
                            // Reset the before battle auto save index if it is greater than the maximum number in the settings.
                            PreBattleSaveIndex = 1;
                        }
                        saveFileIndex = GetPreBattleSaveFileIndex();
                        saveFileSuffix = Settings.PreBattleFileNameSuffix;
                    }
                    else if (shouldAutoSaveAfterBattle)
                    {
                        if (++PostBattleSaveIndex > Settings.PostBattleSaveLimit && Settings.ShouldLimitSaves)
                        {
                            // Reset the after battle auto save index if it is greater than the maximum number in the settings.
                            PostBattleSaveIndex = 1;
                        }
                        saveFileIndex = GetPostBattleSaveFileIndex();
                        saveFileSuffix = Settings.PostBattleFileNameSuffix;
                    }
                }
                else
                {
                    saveFileIndex = GetSaveFileTimeStamp();
                    saveFileSuffix = (shouldAutoSaveBeforeBattle ? Settings.PreBattleFileNameSuffix : Settings.PostBattleFileNameSuffix);
                }
                ActiveSaveSlotName = GetSaveFileBaseName() + saveFileIndex + saveFileSuffix;

                Campaign.Current.SaveHandler.SaveAs(ActiveSaveSlotName);
                // Display the file name of the saved game in a debug message.
                InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=BSLmsg001}Game saved: \"{SAVENAME}\".").SetTextVariable("SAVENAME", ActiveSaveSlotName).ToString()));
            }
        }

        public static void QuickLoadPreviousGame()
        {
            // Get the latest quick save, manual save or auto save.
            SaveFile = MBSaveLoad.GetSaveFileWithName(BannerlordConfig.LatestSaveGameName);

            if (CanLoad)
            {
                MBGameManager.EndGame();

                return;
            }

            InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=BSLmsg003}No save files to load!").ToString()));
        }

        public static void StartGame(LoadResult loadResult)
        {
            SaveFile = null;

            MBGameManager.StartNewGame(new SandBoxGameManager(loadResult));
        }

        [HarmonyPatch("GetAutoSaveName")]
        public static string Postfix(string ___result)
        {
            string saveFileIndex;
            if (Settings.ShouldUseIndex)
            {
                if (++AutoSaveIndex > Settings.QuickSaveLimit && Settings.ShouldLimitSaves)
                {
                    // Reset the save index if it is greater than the maximum number in the settings.
                    AutoSaveIndex = 1;
                }
                saveFileIndex = GetAutoSaveFileIndex();
            }
            else
            {
                saveFileIndex = GetSaveFileTimeStamp();
            }
            return GetSaveFileBaseName() + saveFileIndex + Settings.AutoSaveFileNameSuffix;
        }

        public static string GetSaveFileBaseName()
        {
            return Settings.SaveFileNamePrefix + PlayerClanAndMainHeroName + " ";
        }

        public static string GetQuickSaveFileIndex()
        {
            return QuickSaveIndex.ToString("N0").PadLeft(Settings.IndexZeroPadding + 1, '0');
        }

        public static string GetAutoSaveFileIndex()
        {
            return AutoSaveIndex.ToString("N0").PadLeft(Settings.IndexZeroPadding + 1, '0');
        }

        public static string GetPreBattleSaveFileIndex()
        {
            return PreBattleSaveIndex.ToString("N0").PadLeft(Settings.IndexZeroPadding + 1, '0');
        }

        public static string GetPostBattleSaveFileIndex()
        {
            return PostBattleSaveIndex.ToString("N0").PadLeft(Settings.IndexZeroPadding + 1, '0');
        }

        public static string GetSaveFileTimeStamp()
        {
            string timeStamp;
            DateTime ts = DateTime.Now;
            timeStamp = $"{ts.Year:0000}" + $"{ts.Month:00}" + $"{ts.Day:00}" + $"-{ts.Hour:00}" + $"{ts.Minute:00}" + $"{ts.Second:00}"; // + $"-{ts.Millisecond:000}";

            return $"{timeStamp}";
        }

        private static String WildCardToRegular(String value)
        {
            return "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";
        }
    }
}
