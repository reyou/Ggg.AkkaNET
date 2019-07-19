using System;

namespace intro1.WorkItems
{
    public class SomeRequest
    {
        public DateTime Date { get; set; }
        public string Guid { get; set; }
        public SomeRequest(DateTime date, string guid)
        {
            Date = date;
            Guid = guid;
        }
    }
}