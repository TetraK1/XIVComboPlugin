namespace XIVComboExpandedPlugin.Combos
{
    internal static class RPR
    {
        public const byte JobID = byte.MaxValue;

        public const uint
            // Single Target
            Slice = uint.MaxValue,
            WaxingSlice = uint.MaxValue,
            // AoE
            InfernalSlice = uint.MaxValue,
            SpinningScythe = uint.MaxValue,
            NightmareScythe = uint.MaxValue;

        public static class Buffs
        {
            public const ushort
                Placeholder = 0;
        }

        public static class Debuffs
        {
            public const ushort
                Placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                Slice = 1,
                WaxingSlice = 5,
                InfernalSlice = 30,
                SpinningScythe = 25,
                NightmareScythe = 45;
        }
    }

    internal class ReaperSliceCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.ReaperSliceCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == RPR.Slice)
            {
                if (comboTime > 0)
                {
                    if (lastComboMove == RPR.Slice && level >= RPR.Levels.WaxingSlice)
                        return RPR.WaxingSlice;

                    if (lastComboMove == RPR.WaxingSlice && level >= RPR.Levels.InfernalSlice)
                        return RPR.InfernalSlice;
                }

                return RPR.Slice;
            }

            return actionID;
        }
    }

    internal class ReaperScytheCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.ReaperScytheCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == RPR.SpinningScythe)
            {
                if (comboTime > 0)
                {
                    if (lastComboMove == RPR.SpinningScythe && level >= RPR.Levels.NightmareScythe)
                        return RPR.NightmareScythe;
                }

                return RPR.Slice;
            }

            return actionID;
        }
    }

}