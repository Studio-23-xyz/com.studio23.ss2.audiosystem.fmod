namespace Studio23.SS2.AudioSystem.fmod.Data
{
    public class FMODEventData
    {
        public string BankName { get; set; }
        public string EventName { get; set; }

        public FMODEventData(string bankName, string eventName)
        {
            BankName = bankName;
            EventName = eventName;
        }
    }
}

