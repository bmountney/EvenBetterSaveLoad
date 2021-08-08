using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TaleWorlds.Core;

namespace BetterSaveLoad
{
    [HarmonyPatch(typeof(MBSaveLoad))]
    public static class BetterSaveLoadPatch
    {
        // Get the name of the currently loaded save file.
        [HarmonyPatch("LoadSaveGameData")]
        public static void Postfix(string ___ActiveSaveSlotName) => ActiveSaveSlotName = ___ActiveSaveSlotName;
        // Increment the quick save index. Replace the save file name with a custom one with the quick save index.
        // Display the file name of the saved game in a debug message.
        [HarmonyPatch("QuickSaveCurrentGame")]
        public static void Prefix(ref string ___ActiveSaveSlotName)
        {
            QuickSaveIndex++;
            ___ActiveSaveSlotName = QuickSaveNamePrefix + QuickSaveIndex;
            InformationManager.DisplayMessage(new InformationMessage("Game saved: \"" + ___ActiveSaveSlotName + "\"."));
        }
        // Set the quick save index to the highest number in the list of quick save file names.
        // Display the file name of the loaded game in a debug message.
        public static void InitializeQuickSaveIndex()
        {
            List<int> list = new List<int>();
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
            }
            if (list.Count > 0)
            {
                QuickSaveIndex = list.Max();
            }
            InformationManager.DisplayMessage(new InformationMessage("Game loaded: \"" + ActiveSaveSlotName + "\"."));
        }
        private static int QuickSaveIndex = 0;
        private static string QuickSaveNamePrefix = "save_quick_";
        private static string ActiveSaveSlotName = null;
    }
}
