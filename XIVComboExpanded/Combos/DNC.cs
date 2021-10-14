using Dalamud.Game.ClientState.JobGauge.Types;

namespace XIVComboExpandedPlugin.Combos
{
    internal static class DNC
    {
        public const byte JobID = 38;

        public const uint
            // Single Target
            Cascade = 15989,
            Fountain = 15990,
            ReverseCascade = 15991,
            Fountainfall = 15992,
            // AoE
            Windmill = 15993,
            Bladeshower = 15994,
            RisingWindmill = 15995,
            Bloodshower = 15996,
            // Dancing
            StandardStep = 15997,
            TechnicalStep = 15998,
            // Fans
            FanDance1 = 16007,
            FanDance2 = 16008,
            FanDance3 = 16009,
            FanDance4 = uint.MaxValue,
            // Other
            SaberDance = 16005,
            EnAvant = 16010,
            Flourish = 16013,
            Improvisation = 16014;

        public static class Buffs
        {
            public const ushort
                FlourishingSymmetry = ushort.MaxValue,
                FlourishingFlow = ushort.MaxValue,
                StandardStep = 1818,
                TechnicalStep = 1819,
                ThreefoldFanDance = ushort.MaxValue,
                FourfoldFanDance = ushort.MaxValue;
        }

        public static class Debuffs
        {
            public const ushort
                Placeholder = 0;
        }

        public static class Levels
        {
            public const byte
                Fountain = 2,
                Bladeshower = 25,
                FanDance4 = byte.MaxValue;
        }
    }

    internal class DancerDanceComboCompatibility : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.DancerDanceComboCompatibility;

        protected override uint[] ActionIDs => Service.Configuration.DancerDanceCompatActionIDs;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            var gauge = GetJobGauge<DNCGauge>();
            if (gauge.IsDancing)
            {
                var actionIDs = Service.Configuration.DancerDanceCompatActionIDs;

                if (actionID == actionIDs[0] || (actionIDs[0] == 0 && actionID == DNC.Cascade))
                    return OriginalHook(DNC.Cascade);

                if (actionID == actionIDs[1] || (actionIDs[1] == 0 && actionID == DNC.Flourish))
                    return OriginalHook(DNC.Fountain);

                if (actionID == actionIDs[2] || (actionIDs[2] == 0 && actionID == DNC.FanDance1))
                    return OriginalHook(DNC.ReverseCascade);

                if (actionID == actionIDs[3] || (actionIDs[3] == 0 && actionID == DNC.FanDance2))
                    return OriginalHook(DNC.Fountainfall);
            }

            return actionID;
        }
    }

    internal class DancerFanDanceCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.DancerFanDanceCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DNC.FanDance1 || actionID == DNC.FanDance2)
            {
                if (HasEffect(DNC.Buffs.FourfoldFanDance))
                    return DNC.FanDance4;

                if (HasEffect(DNC.Buffs.ThreefoldFanDance))
                    return DNC.FanDance3;

                return actionID;
            }

            return actionID;
        }
    }

    internal class DancerDanceStepCombo : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.DancerDanceStepCombo;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DNC.StandardStep)
            {
                var gauge = GetJobGauge<DNCGauge>();
                if (gauge.IsDancing && HasEffect(DNC.Buffs.StandardStep))
                {
                    if (gauge.CompletedSteps < 2)
                        return gauge.NextStep;

                    return OriginalHook(DNC.StandardStep);
                }
            }

            if (actionID == DNC.TechnicalStep)
            {
                var gauge = GetJobGauge<DNCGauge>();
                if (gauge.IsDancing && HasEffect(DNC.Buffs.TechnicalStep))
                {
                    if (gauge.CompletedSteps < 4)
                        return gauge.NextStep;

                    return OriginalHook(DNC.TechnicalStep);
                }
            }

            return actionID;
        }
    }

    internal class DancerFlourishFeature : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.DancerFlourishFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DNC.Flourish)
            {
                if (HasEffect(DNC.Buffs.FlourishingFlow))
                    return DNC.Fountainfall;

                if (HasEffect(DNC.Buffs.FourfoldFanDance))
                    return DNC.FanDance4;

                if (HasEffect(DNC.Buffs.FlourishingSymmetry))
                    return DNC.ReverseCascade;

                if (HasEffect(DNC.Buffs.ThreefoldFanDance))
                    return DNC.FanDance3;

                return DNC.Flourish;
            }

            return actionID;
        }
    }

    internal class DancerSingleTargetMultibutton : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.DancerSingleTargetMultibutton;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DNC.Cascade)
            {
                // From Fountain
                if (HasEffect(DNC.Buffs.FlourishingFlow))
                    return DNC.Fountainfall;

                // From Cascade
                if (HasEffect(DNC.Buffs.FlourishingSymmetry))
                    return DNC.ReverseCascade;

                // Cascade Combo
                if (lastComboMove == DNC.Cascade && level >= DNC.Levels.Fountain)
                    return DNC.Fountain;

                return DNC.Cascade;
            }

            return actionID;
        }
    }

    internal class DancerAoeMultibutton : CustomCombo
    {
        protected override CustomComboPreset Preset => CustomComboPreset.DancerAoeMultibutton;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == DNC.Windmill)
            {
                // From Bladeshower
                if (HasEffect(DNC.Buffs.FlourishingFlow))
                    return DNC.Bloodshower;

                // From Windmill
                if (HasEffect(DNC.Buffs.FlourishingSymmetry))
                    return DNC.RisingWindmill;

                // Windmill Combo
                if (lastComboMove == DNC.Windmill && level >= DNC.Levels.Bladeshower)
                    return DNC.Bladeshower;

                return DNC.Windmill;
            }

            return actionID;
        }
    }
}
