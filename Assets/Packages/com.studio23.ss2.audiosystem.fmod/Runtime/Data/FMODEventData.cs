using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("com.studio23.ss2.audiosystem.fmod.playmode.tests")]
namespace Studio23.SS2.AudioSystem.fmod.Data
{
    public class FMODEventData
    {
        internal string BankName { get; set; }
        internal string EventName { get; set; }
        internal string EventGUID { get; set; }

        public FMODEventData(string bankName, string eventName, string eventGuid)
        {
            BankName = bankName;
            EventName = eventName;
            EventGUID = eventGuid;
        }
    }
}

