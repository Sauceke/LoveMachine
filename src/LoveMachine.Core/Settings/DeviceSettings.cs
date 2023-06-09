using System;
using System.Linq;
using System.Text.RegularExpressions;
using LoveMachine.Core.Game;
using LoveMachine.Core.PlatformSpecific;
using LoveMachine.Core.Util;

namespace LoveMachine.Core.Settings
{
    public class DeviceSettings
    {
        public string DeviceName { get; set; }
        public int GirlIndex { get; set; } = 0;
        public Bone Bone { get; set; } = Bone.Auto;
        public int LatencyMs { get; set; } = 0;
        public int UpdatesHz { get; set; } = 10;
        public StrokerSettings StrokerSettings { get; set; } = new StrokerSettings();
        public VibratorSettings VibratorSettings { get; set; } = new VibratorSettings();
        public ConstrictSettings ConstrictSettings { get; set; } = new ConstrictSettings();

        internal void Draw()
        {
            var defaults = new DeviceSettings();
            var game = Globals.ManagerObject.GetComponent<GameAdapter>();
            string[] ordinals = { "First", "Second", "Third", "Fourth", "Fifth", "Sixth" };
            string[] girlChoices = Enumerable.Range(0, game.MaxHeroineCount)
                        .Select(index => $"{ordinals[index]} Girl")
                        .Concat(new[] { "Off" })
                        .ToArray();
            var bones = new[] { Bone.Auto }
                .Concat(game.FemaleBoneNames.Keys)
                .OrderBy(bone => bone)
                .ToList();
            string[] boneNames = bones
                .Select(bone => Enum.GetName(typeof(Bone), bone))
                .Select(name => Regex.Replace(name, "(.)([A-Z])", "$1 $2"))
                .ToArray();
            if (game.MaxHeroineCount > 1)
            {
                GirlIndex = GUIUtil.MultiChoice(
                    label: "Group Role",
                    tooltip: "The device will be synced to this girl.",
                    choices: girlChoices,
                    value: GirlIndex);
            }
            Bone = bones[GUIUtil.MultiChoice(
                label: "Body Part",
                tooltip: "The device will be synced to this body part.",
                choices: boneNames,
                value: bones.IndexOf(Bone))];
            LatencyMs = GUIUtil.IntSlider(
                label: "Latency (ms)",
                tooltip: "The difference in latency between this device and your display.\n" +
                    "Negative if this device has lower latency than your display.",
                value: LatencyMs,
                defaultValue: defaults.LatencyMs,
                min: -500,
                max: 500);
            UpdatesHz = GUIUtil.IntSlider(
                label: "Updates Per Second",
                tooltip: "Maximum number of commands this device can handle per second.",
                value: UpdatesHz,
                defaultValue: defaults.UpdatesHz,
                min: 1,
                max: 30);
            StrokerSettings?.Draw();
            VibratorSettings?.Draw();
            ConstrictSettings?.Draw();
        }
    }
}