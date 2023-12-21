using FMOD.Studio;
using FMODUnity;

namespace Studio23.SS2.AudioSystem.fmod.Data
{
    public class FMODVCAData
    {
        public VCA VCA;
        public string VCAName;
        public float DefaultVolume;
        public float CurrentVolume;

        public FMODVCAData(string vcaName, float defaultVolume)
        {
            VCA = RuntimeManager.GetVCA(vcaName);
            VCAName = vcaName;
            DefaultVolume = defaultVolume;
            CurrentVolume = defaultVolume;
            SetVolume(defaultVolume);
        }

        /// <summary>
        /// Sets the volume for a VCA.
        /// </summary>
        /// <param name="volume"></param>
        public void SetVolume(float volume)
        {
            VCA.setVolume(volume);
            CurrentVolume = volume;
        }
    }
}
