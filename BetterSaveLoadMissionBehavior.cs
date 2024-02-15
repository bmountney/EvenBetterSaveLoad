using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;

namespace BetterSaveLoad
{
    public class BetterSaveLoadMissionBehavior : MissionBehavior
    {
        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;

        public override void OnMissionTick(float dt)
        {
            if (Mission.InputManager.IsControlDown() && Mission.InputManager.IsKeyPressed(InputKey.L))
            {
                BetterSaveLoadManager.QuickLoadPreviousGame();
            }
        }
    }
}
