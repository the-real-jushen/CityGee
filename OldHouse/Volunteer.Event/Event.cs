using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.Volunteer.VolunteerEvent
{
    public class Event
    {
        public Event(string type, object subject, object sender)
        {
            Id = System.Guid.NewGuid();
            Type = type;
            Subject = subject;
            Sender = sender;
            hasHandled = false;
            Time = DateTime.Now;
        }
        public Guid Id { get; set; }
        public DateTime Time { get; set; }
        public string Type { get; set; }
        public object Subject { get; set; }
        public object Sender { get; set; }
        public bool hasHandled { get; set; }
    }
}
