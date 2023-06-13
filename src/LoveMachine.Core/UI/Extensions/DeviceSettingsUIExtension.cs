using System;
using System.Linq;
using System.Text.RegularExpressions;
using LoveMachine.Core.Common;
using LoveMachine.Core.Game;
using LoveMachine.Core.PlatformSpecific;
using LoveMachine.Core.Settings;
using LoveMachine.Core.UI.Util;

namespace LoveMachine.Core.UI.Extensions
{
    internal static class DeviceSettingsUIExtension
    {
        public static void Draw(this DeviceSettings settings)
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
                settings.GirlIndex = GUIUtil.MultiChoice(
                    label: "Group Role",
                    tooltip: "The device will be synced to this girl.",
                    choices: girlChoices,
                    value: settings.GirlIndex);
            }
            settings.Bone = bones[GUIUtil.MultiChoice(
                label: "Body Part",
                tooltip: "The device will be synced to this body part.",
                choices: boneNames,
                value: bones.IndexOf(settings.Bone))];
            settings.LatencyMs = GUIUtil.IntSlider(
                label: "Latency (ms)",
                tooltip: "The difference in latency between this device and your display.\n" +
                         "Negative if this device has lower latency than your display.",
                value: settings.LatencyMs,
                defaultValue: defaults.LatencyMs,
                min: -500,
                max: 500);
            settings.UpdatesHz = GUIUtil.IntSlider(
                label: "Updates Per Second",
                tooltip: "Maximum number of commands this device can handle per second.",
                value: settings.UpdatesHz,
                defaultValue: defaults.UpdatesHz,
                min: 1,
                max: 30);
            settings.StrokerSettings?.Draw();
            settings.VibratorSettings?.Draw();
            settings.ConstrictSettings?.Draw();
        }
    }
}