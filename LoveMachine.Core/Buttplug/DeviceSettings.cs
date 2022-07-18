using System;
using System.Linq;
using System.Text.RegularExpressions;
using BepInEx.Bootstrap;
using UnityEngine;

namespace LoveMachine.Core
{
    public class DeviceSettings
    {
        public string DeviceName { get; set; }
        public int GirlIndex { get; set; } = 0;
        public Bone Bone { get; set; } = Bone.Auto;
        public int LatencyMs { get; set; } = 0;
        public int UpdateFrequencyHz { get; set; } = 10;
        public StrokerSettings StrokerSettings { get; set; } = null;

        public void Draw()
        {
            var defaults = new DeviceSettings();
            var game = Chainloader.ManagerObject.GetComponent<GameDescriptor>();
            GUILayout.BeginHorizontal();
            {
                if (game.MaxHeroineCount > 1)
                {
                    GUILayout.Label("Group Role");
                    string[] ordinals = { "First", "Second", "Third", "Fourth", "Fifth", "Sixth" };
                    string[] girlMappingOptions = Enumerable.Range(0, game.MaxHeroineCount)
                        .Select(index => $"{ordinals[index]} Girl")
                        .Concat(new string[] { "Off" })
                        .ToArray();
                    GirlIndex = GUILayout.SelectionGrid(
                        selected: GirlIndex,
                        girlMappingOptions,
                        xCount: 5);
                }
            }
            GUILayout.EndHorizontal();
            GUIUtil.SmallSpace();
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Body Part");
                string[] boneNames = Enum.GetNames(typeof(Bone))
                    .Select(camelCase => Regex.Replace(camelCase, ".([A-Z])", " $1"))
                    .ToArray();
                Bone = (Bone)GUILayout.SelectionGrid(
                    selected: (int)Bone,
                    boneNames,
                    xCount: 5);
            }
            GUILayout.EndHorizontal();
            GUIUtil.SmallSpace();
            LatencyMs = GUIUtil.IntSlider(
                label: "Latency (milliseconds)",
                tooltip: "",
                value: LatencyMs,
                defaultValue: defaults.LatencyMs,
                min: -500,
                max: 500);
            GUIUtil.SmallSpace();
            UpdateFrequencyHz = GUIUtil.IntSlider(
                label: "Update Frequency (per second)",
                tooltip: "",
                value: UpdateFrequencyHz,
                defaultValue: defaults.UpdateFrequencyHz,
                min: 1,
                max: 30);
            GUIUtil.SmallSpace();
            StrokerSettings?.Draw();
        }
    }
}
