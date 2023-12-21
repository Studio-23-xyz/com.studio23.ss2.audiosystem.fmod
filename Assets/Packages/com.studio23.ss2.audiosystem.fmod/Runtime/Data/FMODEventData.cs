using FMOD;

namespace Studio23.SS2.AudioSystem.fmod.Data
{
    public class FMODEventData
    {
        public string BankName { get; set; }
        public string EventName { get; set; }
        public string EventGUID { get; set; }

        public FMODEventData(string bankName, string eventName, string eventGuid)
        {
            BankName = bankName;
            EventName = eventName;
            EventGUID = eventGuid;
        }
    }
}

