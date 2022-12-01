using FrooxEngine;
using HarmonyLib;
using NeosModLoader;

namespace LocalVideoVolumeHeadless
{
    public class LocalVideoVolumeHeadless : NeosMod
    {
        public override string Name => "LocalVideoVolumeHeadless";
        public override string Author => "NeroWolf & Sox & LeCloutPanda";
        public override string Version => "1.0.0";
        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("net.nero.LocalVideoPlayerVolume");
            harmony.PatchAll();
        }
        [HarmonyPatch(typeof(VideoPlayer), "OnChanges")]
        private class LocalVideoPlayerVolumePatchHeadless
        {
            private static void Postfix(VideoPlayer __instance, SyncRef<AudioOutput> ____mainAudioOutput)
            {
                ValueUserOverride<float> valueUserOverride = __instance.Slot.AttachComponent<ValueUserOverride<float>>(true, null);
                valueUserOverride.CreateOverrideOnWrite.Value = true;
                valueUserOverride.Target.Target = ____mainAudioOutput.Target.Volume;
                valueUserOverride.Default.Value = 0f;
            }
        }
    }
}
    
