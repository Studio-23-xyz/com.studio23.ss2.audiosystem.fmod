namespace Studio23.SS2.AudioSystem.Data
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

