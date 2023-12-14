using System;
using FMOD.Studio;
using FMODUnity;

namespace Studio23.SS2.AudioSystem.Data
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
            CurrentVolume = DefaultVolume;
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void SetVolume(float volume)
        {
            VCA.setVolume(volume);
        }
    }
}
