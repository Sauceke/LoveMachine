using BepInEx;
using BepInEx.Configuration;
using LoveMachine.Core.Common;
using LoveMachine.Core.NonPortable;
using System;
using System.Collections.Generic;

namespace LoveMachinePrototyper
{
    internal static class PrototyperConfig
    {
        public static ConfigEntry<string> PluginVersion { get; private set; }
        public static ConfigEntry<string> GameProcessName { get; private set; }

        public static ConfigEntry<string> PenisBaseName { get; private set; }
        public static ConfigEntry<string> AnimatorName { get; private set; }
        public static ConfigEntry<int> AnimationLayer { get; private set; }
        public static ConfigEntry<string> FemaleRootName { get; private set; }
        public static Dictionary<Bone, ConfigEntry<string>> FemaleBoneNames { get; private set; }
        public static ConfigEntry<string> HStartObjectName { get; private set; }
        public static event EventHandler ConfigChanged;

        public static void Initialize(BaseUnityPlugin plugin)
        {
            const string settingsTitle = "Prototyper Settings";
            PluginVersion = plugin.Config.Bind(
                section: settingsTitle,
                key: "Plugin Version",
                defaultValue: Globals.Version,
                new ConfigDescription(
                    "The LoveMachine version this config file was written in - do not edit",
                    tags: new ConfigurationManagerAttributes { ReadOnly = true }));
            GameProcessName = plugin.Config.Bind(
                section: settingsTitle,
                key: "Game Process Name",
                defaultValue: Paths.ProcessName,
                new ConfigDescription(
                    "The name of the game process this config file was written in - do not edit",
                    tags: new ConfigurationManagerAttributes { ReadOnly = true }));
            PenisBaseName = plugin.AddConfigEntry(
                section: settingsTitle,
                key: "Penis Base Name",
                defaultValue: "",
                description: "The name (or path) of any bone at the base of the male character's penis");
            AnimatorName = plugin.AddConfigEntry(
                section: settingsTitle,
                key: "Animator Name",
                defaultValue: "",
                description: "The name (or path) of the Animator controlling the H animations");
            AnimationLayer = plugin.AddConfigEntry(
                section: settingsTitle,
                key: "Animation Layer",
                defaultValue: 0,
                description: "The layer index of H animations, usually 0");
            FemaleRootName = plugin.AddConfigEntry(
                section: settingsTitle,
                key: "Female Root Name",
                defaultValue: "",
                description: "The name (or path) of the GameObject that contains the female character");
            FemaleBoneNames = new Dictionary<Bone, ConfigEntry<string>>();
            foreach (int i in Enum.GetValues(typeof(Bone)))
            {
                if ((Bone)i == Bone.Auto)
                {
                    continue;
                }
                string bone = Enum.GetName(typeof(Bone), i);
                FemaleBoneNames[(Bone)i] = plugin.AddConfigEntry(
                    section: settingsTitle,
                    key: $"Female Bone Name - {bone}",
                    defaultValue: "",
                    description: $"The name (or path) of the female character's {bone} bone");
            }
            HStartObjectName = plugin.AddConfigEntry(
                section: settingsTitle,
                key: "H Start Object Name",
                defaultValue: "",
                description: "The name (or path) of a GameObject that is only active when an H" +
                    "scene is playing. If empty, we default to the penis base.");
        }

        private static ConfigEntry<T> AddConfigEntry<T>(this BaseUnityPlugin plugin,
            string section, string key, T defaultValue, string description)
        {
            var entry = plugin.Config.Bind(section, key, defaultValue, description);
            entry.SettingChanged += OnSettingChanged;
            return entry;
        }

        private static void OnSettingChanged(object sender, EventArgs e)
        {
            ConfigChanged.Invoke(sender, e);
        }
    }
}
