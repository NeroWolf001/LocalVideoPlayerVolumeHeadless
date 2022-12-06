using FrooxEngine;
using HarmonyLib;
using NeosModLoader;

namespace LocalVideoVolumeHeadless
{
    public class LocalVideoVolumeHeadless : NeosMod
    {
        public override string Name => "LocalVideoVolumeHeadless";
        public override string Author => "NeroWolf & LeCloutPanda";
        public override string Version => "2.0.1";

        public static ModConfiguration config;

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool> ENABLED = new ModConfigurationKey<bool>("Set user video player volume to 0 in hosted sessions.", "", () => true);
        public override void OnEngineInit()
        {
            config = base.GetConfiguration();
            config.Save(true);

            Harmony harmony = new Harmony($"dev.{Author}.{Name}");
            harmony.PatchAll();

            Engine.Current.OnReady += Current_OnReady;
        }

        private void Current_OnReady()
        {
            Engine.Current.WorldManager.WorldAdded += WorldAdded;
            Engine.Current.WorldManager.WorldRemoved += WorldRemoved;
        }

        private void WorldAdded(World obj) => obj.ComponentAdded += OnComponentAdded;
        private void WorldRemoved(World obj) => obj.ComponentAdded -= OnComponentAdded;

        private void OnComponentAdded(Slot arg1, Component arg2)
        {
            if (!config.GetValue(ENABLED)) return;

            if (!arg1.LocalUser.IsHost) return;

            if (arg2.GetType() == typeof(AudioOutput))
            {
                VideoPlayer videoPlayer = arg1.GetComponent<VideoPlayer>();
                if (videoPlayer == null) return;

                AudioOutput audioOutput = (AudioOutput)arg2;
                ValueUserOverride<float> userOverride = audioOutput.Volume.OverrideForUser<float>(arg1.World.HostUser, 0);
                userOverride.CreateOverrideOnWrite.Value = true;
                userOverride.Default.Value = 0;

                arg1.RunInUpdates(3, () =>
                {

                    Slot volume = arg1.FindChild(ch => ch.Name.Equals("Volume"), 1);
                    if (volume.FindChild(ch => ch.Name.Equals("Local Text"), 1) != null) return;

                    TextRenderer text = volume.AddSlot("Local Text").AttachComponent<TextRenderer>();
                    text.Text.Value = "Local";
                    text.Slot.Scale_Field.Value = new BaseX.float3(0.5f, 0.5f, 0.5f);
                    text.Slot.Position_Field.Value = new BaseX.float3(0f, 0f, -0.02f);

                });
            }
        }
    }
}





