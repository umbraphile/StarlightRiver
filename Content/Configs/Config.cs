﻿using Terraria.ModLoader.Config;

using StarlightRiver.Core;
using StarlightRiver.Helpers;
using System.ComponentModel;

namespace StarlightRiver.Configs
{
	public enum TitleScreenStyle
    {
        Starlight = 0,
        Vitric = 1,
        Overgrow = 2,
        CorruptJungle = 3,
        CrimsonJungle = 4,
        HallowJungle = 5,
        None = 6
    }

    public enum CustomSounds
    {
        All = 0,
        Specific = 1,
        None = 2

    }

    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Label("Menu Theme")]
        [Tooltip("Changes or disables the menu theme")]
        public TitleScreenStyle Style;

        [Label("Lighting Buffer Poll Rate")]
        [Tooltip("Changes how often the lighting buffer polls for data. Higher values increase performance but make lighting update slower on some objects. Lower values result in smoother moving light but may hurt performance.")]
        [Range(1, 30)]
        public int LightingPollRate = 5;

        [Label("Scrolling Lighting Buffer Building")]
        [Tooltip("Causes the lighting buffer to be built over its poll rate instead of all at once. May help normalize lag spikes but cause strange lighting artifacts.")]
        public bool ScrollingLightingPoll = false;

        [Label("Extra Particles")]
        [Tooltip("Enables/Disables special particles. Disable this if you have performance issues.")]
        public bool ParticlesActive = true;

        [Label("Lighting buffer update delay")]
        [Tooltip("The delay between updating the lighting buffer")]
        [Range(2, 20)]
        [DrawTicks]
        [Slider]
        [DefaultValue(5f)]
        public int LightingUpdateDelay = 5;

        [Label("High quality lit textures")]
        [Tooltip("Enables/Disables fancy lighting on large textures. Disable this if you have performance issues.")]
        public bool HighQualityLighting = true;

        [Label("Custom Inventory Sounds")]
        [Tooltip("If custom inventory sounds should play for all items or a select few, or none at all.")]
        public CustomSounds InvSounds = CustomSounds.All;
    }
}