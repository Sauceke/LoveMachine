using LoveMachine.Core.Buttplug;
using LoveMachine.Core.Buttplug.Settings;
using LoveMachine.Core.Common;
using LoveMachine.Core.Game;
using LoveMachine.Core.NonPortable;
using LoveMachine.Core.UI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LoveMachine.Core.UI.Settings
{
    class FeatureSettingsUI : DeviceSettingsUI
    {
        private static readonly string[] ordinals = { "First", "Second", "Third" };

        private GameAdapter game;

        public override void Draw(Device device)
        {
            var settings = device.Settings;
            var defaults = new DeviceSettings();
            settings.UseSeparateFeatureSettings = GUIUtil.Toggle(
                label: "Separate Tracking Settings",
                tooltip: "Use separate tracking settings for each feature.",
                value: settings.UseSeparateFeatureSettings,
                defaultValue: defaults.UseSeparateFeatureSettings);
            if (settings.UseSeparateFeatureSettings)
            {
                device.AllFeatures.SelectMany(f => f)
                    .Select(feature => new DeviceFeature(device, feature))
                    .ToList()
                    .ForEach(feature => DrawFeature(feature));
            }
            else
            {
                DrawFeatureSettings(settings.GlobalFeatureSettings,
                    "Tracking Settings: All Features");
            }
        }

        public override void Draw(DeviceSettings settings)
        { }

        private void DrawFeature(DeviceFeature feature)
        {
            string title = $"Tracking Settings: {feature.Feature.ActuatorType} (#{feature.FeatureIndex})";
            DrawFeatureSettings(feature.Settings, title);
        }

        private void DrawFeatureSettings(FeatureSettings settings, string title)
        {
            GUIUtil.Title(title);
            string[] girlChoices = Enumerable.Range(0, game.MaxHeroineCount)
                .Select(index => $"{GetOrdinal(index)} Girl")
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
            var defaults = new FeatureSettings();
            settings.Enabled = GUIUtil.Toggle(
                label: "Enabled",
                tooltip: "Turns this feature on/off.",
                value: settings.Enabled,
                defaultValue: defaults.Enabled);
            if (game.MaxHeroineCount > 1)
            {
                settings.GirlIndex = GUIUtil.MultiChoice(
                    label: "Group Role",
                    tooltip: "The feature will be synced to this girl.",
                    choices: girlChoices,
                    value: settings.GirlIndex);
            }
            settings.Bone = bones[GUIUtil.MultiChoice(
                label: "Body Part",
                tooltip: "The feature will be synced to this body part.",
                choices: boneNames,
                value: bones.IndexOf(settings.Bone))];
            settings.PhaseShift = GUIUtil.FloatSlider(
                label: "Phase Shift",
                tooltip: "How much this feature should lag behind the animation.",
                value: settings.PhaseShift,
                defaultValue: defaults.PhaseShift,
                min: 0f,
                max: 1f);
            settings.Axis = (Axis)GUIUtil.MultiChoice(
                label: "Axis",
                tooltip: "The axis for this feature to track.",
                choices: Enum.GetNames(typeof(Axis)),
                value: ((int)settings.Axis));
            settings.MovementType = (MovementType)GUIUtil.MultiChoice(
                label: "Movement Type",
                tooltip: "The type of movement for this feature to track.",
                choices: Enum.GetNames(typeof(MovementType)),
                value: ((int)settings.MovementType));
        }

        private void Start() => game = GetComponent<GameAdapter>();

        private static string GetOrdinal(int index) =>
            index < ordinals.Length ? ordinals[index] : $"{index + 1}th";
    }
}
