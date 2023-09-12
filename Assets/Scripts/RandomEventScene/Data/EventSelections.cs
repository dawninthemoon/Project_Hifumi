using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RandomEvent {
    public struct EventEffects {
        public string eventName;
        public string[] variables;
        public EventEffects(string name, string[] var) {
            eventName = name;
            variables = var;
        }
    }

    public class EventSelections {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public EventEffects[] Effects { get; set; }
    }
}