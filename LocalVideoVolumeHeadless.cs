using FrooxEngine;
using HarmonyLib;
using NeosModLoader;

namespace LocalVideoVolumeHeadless
{
    public class LocalVideoVolumeHeadless : NeosMod
    {
        public override string Name => "LocalVideoVolumeHeadless";
        public override string Author => "NeroWolf & LeCloutPanda";
        public override string Version => "2.0.0";

        public override void OnEngineInit()
        {
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
            if (arg2.GetType() == typeof(AudioOutput))
            {
                VideoPlayer videoPlayer = arg1.GetComponent<VideoPlayer>();
                if (videoPlayer == null) return;

                arg1.RunInUpdates(3, () =>
                {
                    AudioOutput audioOutput = (AudioOutput)arg2;

                    ValueUserOverride<float> userOverride = arg1.GetComponent<ValueUserOverride<float>>();

                    if (userOverride == null)
                    {
                        ValueUserOverride<float> valueUserOverride = arg1.AttachComponent<ValueUserOverride<float>>(true, null);
                        valueUserOverride.CreateOverrideOnWrite.Value = true;
                        valueUserOverride.Target.Target = audioOutput.Volume;
                        valueUserOverride.Default.Value = 0f;
                    }
                    else if (userOverride != null)
                    {
                        userOverride.Default.Value = 0;
                        audioOutput.Volume.Value = 0f;
                    }

                    Slot volume = arg1.FindChild(ch => ch.Name.Equals("Volume"), 1);
                    TextRenderer text = volume.AddSlot("Local Text").AttachComponent<TextRenderer>();
                    text.Text.Value = "Local";
                    text.Slot.Scale_Field.Value = new BaseX.float3(0.5f, 0.5f, 0.5f);
                    text.Slot.Position_Field.Value = new BaseX.float3(0f, 0f, -0.02f);
                });
            }
        }
    }
}





