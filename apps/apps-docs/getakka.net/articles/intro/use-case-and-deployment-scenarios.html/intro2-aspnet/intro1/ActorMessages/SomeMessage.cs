using System;

namespace intro1.ActorMessages
{
    public class SomeMessage
    {
        public string GuidFormatted { get; set; }
        public string DateString { get; set; }
        public SomeMessage(DateTime date, string guid)
        {
            DateString = date.ToString("G");
            GuidFormatted = guid.Replace("-", "");
        }


    }
}