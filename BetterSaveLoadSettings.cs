using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using MCM.Common;

namespace BetterSaveLoad
{
    public class BetterSaveLoadSettings : AttributeGlobalSettings<BetterSaveLoadSettings>
    {
        public override string Id => "BetterSaveLoad";

        public override string DisplayName => "Better Save/Load";

        public override string FolderName => "BetterSaveLoad";

        public override string FormatType => "json2";

        [SettingPropertyBool("Toggle Save Limits", Order = 0, RequireRestart = false, HintText = "Limit number of quick saves and battle auto saves. Enabled by default.", IsToggle = true)]
        [SettingPropertyGroup("Save Limits", GroupOrder = 0)]
        public bool ShouldLimitSaves { get; set; } = true;

        [SettingPropertyInteger("Quick Save Limit", 1, 100, "0", Order = 1, RequireRestart = false, HintText = "Maximum number of quick saves. Default is 3.")]
        [SettingPropertyGroup("Save Limits", GroupOrder = 0)]
        public int QuickSaveLimit { get; set; } = 3;

        [SettingPropertyInteger("Battle Auto Save Limit", 1, 100, "0", Order = 2, RequireRestart = false, HintText = "Maximum number of battle auto saves. Default is 3.")]
        [SettingPropertyGroup("Save Limits", GroupOrder = 0)]
        public int BattleAutoSaveLimit { get; set; } = 3;

        [SettingPropertyBool("Toggle Battle Auto Save", Order = 0, RequireRestart = false, HintText = "Auto save for battles. Enabled by default.", IsToggle = true)]
        [SettingPropertyGroup("Battle Auto Save", GroupOrder = 1)]
        public bool ShouldAutoSaveForBattle { get; set; } = true;

        [SettingPropertyDropdown("Trigger", Order = 1, RequireRestart = false, HintText = "When to trigger battle auto saves. Default is Pre-battle.")]
        [SettingPropertyGroup("Battle Auto Save", GroupOrder = 1)]
        public Dropdown<string> BattleAutoSaveTrigger { get; set; } = new Dropdown<string>(new string[] { "Pre-battle", "Post-battle", "Both" }, 0);

        [SettingPropertyInteger("Minimum Attacker Troop Count", 1, 1000, "0", Order = 2, RequireRestart = false, HintText = "Minimum number of attacking troops to trigger auto saving before battles. Default is 50.")]
        [SettingPropertyGroup("Battle Auto Save", GroupOrder = 1)]
        public int MinAttackerTroopCount { get; set; } = 50;

        [SettingPropertyInteger("Minimum Defender Troop Count", 1, 1000, "0", Order = 3, RequireRestart = false, HintText = "Minimum number of defending troops to trigger auto saving before battles. Default is 50.")]
        [SettingPropertyGroup("Battle Auto Save", GroupOrder = 1)]
        public int MinDefenderTroopCount { get; set; } = 50;
    }
}
