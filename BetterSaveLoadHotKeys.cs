using Bannerlord.ButterLib.HotKeys;
using TaleWorlds.InputSystem;

namespace BetterSaveLoad
{
    public class BetterSaveLoadHotKeys
    {
        public class LCtrl : HotKeyBase
        {
            protected override InputKey DefaultKey { get; }

            public LCtrl() : base(nameof(LCtrl)) => DefaultKey = InputKey.LeftControl;
        }

        public class RCtrl : HotKeyBase
        {
            protected override InputKey DefaultKey { get; }

            public RCtrl() : base(nameof(RCtrl)) => DefaultKey = InputKey.RightControl;
        }

        public class S : HotKeyBase
        {
            protected override InputKey DefaultKey { get; }

            public S() : base(nameof(S)) => DefaultKey = InputKey.S;
        }

        public class L : HotKeyBase
        {
            protected override InputKey DefaultKey { get; }

            public L() : base(nameof(L)) => DefaultKey = InputKey.L;
        }

        public class F9 : HotKeyBase
        {
            protected override InputKey DefaultKey { get; }

            public F9() : base(nameof(F9)) => DefaultKey = InputKey.F9;
        }
    }
}
