using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RandomEvent {
    public class EventsData {
        public string ID { get; set; }
        public string EventName { get; set; }
        public string SpriteName { get; set; }
        public string Description { get; set; }
        public EventSelections Selection1 { get; set; }
        public EventSelections Selection2 { get; set; }
        public EventSelections Selection3 { get; set; }
    }
}