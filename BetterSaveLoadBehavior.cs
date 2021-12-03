using System;
using TaleWorlds.CampaignSystem;

namespace BetterSaveLoad
{
    public class BetterSaveLoadBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(OnNewGameCreated));
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(OnGameLoaded));
            CampaignEvents.MapEventStarted.AddNonSerializedListener(this, new Action<MapEvent, PartyBase, PartyBase>(OnMapEventStarted));
        }
        public override void SyncData(IDataStore dataStore) { }
        public void OnNewGameCreated(CampaignGameStarter campaignGameStarter) => BetterSaveLoadManager.InitializeSaveIndexes();
        public void OnGameLoaded(CampaignGameStarter campaignGameStarter) => BetterSaveLoadManager.InitializeSaveIndexes();
        // Auto save when the player enters a battle.
        public void OnMapEventStarted(MapEvent mapEvent, PartyBase attackerParty, PartyBase defenderParty)
        {
            if (attackerParty == PartyBase.MainParty && defenderParty.MapFaction.IsAtWarWith(PartyBase.MainParty.MapFaction))
            {
                BetterSaveLoadManager.AutoSaveBeforeBattle(mapEvent);
            }
        }
    }
}
