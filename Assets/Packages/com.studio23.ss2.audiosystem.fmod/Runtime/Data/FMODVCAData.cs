using FMOD.Studio;
using FMODUnity;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("com.studio23.ss2.audiosystem.fmod.playmode.tests")]
namespace Studio23.SS2.AudioSystem.fmod.Data
{
    public class FMODVCAData
    {
        internal VCA VCA;
        internal string VCAName;
        internal float DefaultVolume;
        internal float CurrentVolume;
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
