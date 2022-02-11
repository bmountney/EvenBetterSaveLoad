using Bannerlord.ButterLib.HotKeys;
using TaleWorlds.InputSystem;

namespace BetterSaveLoad
{
    public class BetterSaveLoadHotKeys
    {
        public class LCtrl : HotKeyBase
        {
            public LCtrl() : base(nameof(LCtrl)) => DefaultKey = InputKey.LeftControl;
            protected override InputKey DefaultKey { get; }
        }
        public class RCtrl : HotKeyBase
        {
            public RCtrl() : base(nameof(RCtrl)) => DefaultKey = InputKey.RightControl;
            protected override InputKey DefaultKey { get; }
        }
        public class S : HotKeyBase
        {
            public S() : base(nameof(S)) => DefaultKey = InputKey.S;
            protected override InputKey DefaultKey { get; }
        }
        public class L : HotKeyBase
        {
            public L() : base(nameof(L)) => DefaultKey = InputKey.L;
            protected override InputKey DefaultKey { get; }
        }
        public class F9 : HotKeyBase
        {
            public F9() : base(nameof(F9)) => DefaultKey = InputKey.F9;
            protected override InputKey DefaultKey { get; }
        }
    }
}
