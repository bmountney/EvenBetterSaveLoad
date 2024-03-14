using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using MCM.Common;
using TaleWorlds.Localization;

namespace BetterSaveLoad
{
    public class BetterSaveLoadSettings : AttributeGlobalSettings<BetterSaveLoadSettings>
    {
        public override string Id => "EvenBetterSaveLoad";

        public override string DisplayName => "Even Better Save/Load";

        public override string FolderName => "EvenBetterSaveLoad";

        public override string FormatType => "json2";

        [SettingPropertyBool("{=BSLopt001}Incremental File Index", Order = 0, RequireRestart = false, HintText = "{=BSLopt001Hint}When enabled an incremental index is used for save files, otherwise a time stamp in the format YYYYMMDD-HHMMSS is used. Enabled by default.", IsToggle = true)]
        [SettingPropertyGroup("{=BSLoptg001}Incremental File Index (turn off to use time stamp)", GroupOrder = 0)]
        public bool ShouldUseIndex { get; set; } = true;

        [SettingPropertyInteger("{=BSLopt002}Index Zero Padding", 0, 4, Order = 1, RequireRestart = false, HintText = "{=BSLopt002Hint}Pad index with zeros. Default is 2.")]
        [SettingPropertyGroup("{=BSLoptg001}Incremental File Index (turn off to use time stamp)", GroupOrder = 0)]
        public int IndexZeroPadding { get; set; } = 2;

        [SettingPropertyBool("{=BSLopt003}Toggle Save Limits", Order = 2, RequireRestart = false, HintText = "{=BSLopt003Hint}Limit number of quick saves and battle auto saves. Enabled by default.", IsToggle = true)]
        [SettingPropertyGroup("{=BSLoptg001}Incremental File Index (turn off to use time stamp)/Save Limits", GroupOrder = 0)]
        public bool ShouldLimitSaves { get; set; } = true;

        [SettingPropertyInteger("{=BSLopt004}Quick Save Limit", 1, 999, "0", Order = 3, RequireRestart = false, HintText = "{=BSLopt004Hint}Maximum number of quick saves. Default is 3.")]
        [SettingPropertyGroup("{=BSLoptg001}Incremental File Index (turn off to use time stamp)/Save Limits", GroupOrder = 0)]
        public int QuickSaveLimit { get; set; } = 3;

        [SettingPropertyInteger("{=BSLopt005}Auto Save Limit", 1, 999, "0", Order = 4, RequireRestart = false, HintText = "{=BSLopt005Hint}Maximum number of auto saves. Default is 3.")]
        [SettingPropertyGroup("{=BSLoptg001}Incremental File Index (turn off to use time stamp)/Save Limits", GroupOrder = 0)]
        public int AutoSaveLimit { get; set; } = 3;

        [SettingPropertyInteger("{=BSLopt006}Pre-Battle Auto Save Limit", 1, 999, "0", Order = 5, RequireRestart = false, HintText = "{=BSLopt006Hint}Maximum number of pre-battle auto saves. Default is 3.")]
        [SettingPropertyGroup("{=BSLoptg001}Incremental File Index (turn off to use time stamp)/Save Limits", GroupOrder = 0)]
        public int PreBattleSaveLimit { get; set; } = 3;

        [SettingPropertyInteger("{=BSLopt007}Post-Battle Auto Save Limit", 1, 999, "0", Order = 6, RequireRestart = false, HintText = "{=BSLopt007Hint}Maximum number of post-battle auto saves. Default is 3.")]
        [SettingPropertyGroup("{=BSLoptg001}Incremental File Index (turn off to use time stamp)/Save Limits", GroupOrder = 0)]
        public int PostBattleSaveLimit { get; set; } = 3;

        [SettingPropertyBool("{=BSLopt008}Toggle Battle Auto Save", Order = 0, RequireRestart = false, HintText = "{=BSLopt008Hint}Auto save for battles. Enabled by default.", IsToggle = true)]
        [SettingPropertyGroup("{=BSLoptg002}Battle Auto Save", GroupOrder = 1)]
        public bool ShouldAutoSaveForBattle { get; set; } = true;

        [SettingPropertyDropdown("{=BSLopt009}Trigger", Order = 1, RequireRestart = false, HintText = "{=BSLopt009Hint}When to trigger battle auto saves. Default is Pre-battle.")]
        [SettingPropertyGroup("{=BSLoptg002}Battle Auto Save", GroupOrder = 1)]
        public Dropdown<TextObject> BattleAutoSaveTrigger { get; set; } = new Dropdown<TextObject>(new TextObject[] { new TextObject("{=BSLoptv001}Pre-battle"), new TextObject("{=BSLoptv002}Post-battle"), new TextObject("{=BSLoptv003}Both") }, 0);

        [SettingPropertyInteger("{=BSLopt010}Minimum Attacker Troop Count", 1, 1000, "0", Order = 2, RequireRestart = false, HintText = "{=BSLopt010Hint}Minimum number of attacking troops to trigger auto saving before and after battles. Default is 50.")]
        [SettingPropertyGroup("{=BSLoptg002}Battle Auto Save", GroupOrder = 1)]
        public int MinAttackerTroopCount { get; set; } = 50;

        [SettingPropertyInteger("{=BSLopt011}Minimum Defender Troop Count", 1, 1000, "0", Order = 3, RequireRestart = false, HintText = "{=BSLopt011Hint}Minimum number of defending troops to trigger auto saving before and after battles. Default is 50.")]
        [SettingPropertyGroup("{=BSLoptg002}Battle Auto Save", GroupOrder = 1)]
        public int MinDefenderTroopCount { get; set; } = 50;

        [SettingPropertyText("{=BSLopt012}Save File Name Prefix", Order = 0, RequireRestart = false, HintText = "{=BSLopt012Hint}Prefix on all save file names.")]
        [SettingPropertyGroup("{=BSLoptg003}File Name Format", GroupOrder = 2)]
        public string SaveFileNamePrefix { get; set; } = "Save ";

        [SettingPropertyText("{=BSLopt013}Quick Save File Name Suffix", Order = 1, RequireRestart = false, HintText = "{=BSLopt013Hint}Suffix on quick save file name.")]
        [SettingPropertyGroup("{=BSLoptg003}File Name Format", GroupOrder = 2)]
        public string QuickSaveFileNameSuffix { get; set; } = " Quick";

        [SettingPropertyText("{=BSLopt014}Auto Save File Name Suffix", Order = 2, RequireRestart = false, HintText = "{=BSLopt014Hint}Suffix on auto save file name.")]
        [SettingPropertyGroup("{=BSLoptg003}File Name Format", GroupOrder = 2)]
        public string AutoSaveFileNameSuffix { get; set; } = " Auto";

        [SettingPropertyText("{=BSLopt015}Before Battle Save File Name Suffix", Order = 3, RequireRestart = false, HintText = "{=BSLopt015Hint}Suffix on battle auto save file name.")]
        [SettingPropertyGroup("{=BSLoptg003}File Name Format", GroupOrder = 2)]
        public string PreBattleFileNameSuffix { get; set; } = " Before";

        [SettingPropertyText("{=BSLopt016}After Battle Save File Name Suffix", Order = 4, RequireRestart = false, HintText = "{=BSLopt016Hint}Suffix on battle auto save file name.")]
        [SettingPropertyGroup("{=BSLoptg003}File Name Format", GroupOrder = 2)]
        public string PostBattleFileNameSuffix { get; set; } = " After";
    }
}
