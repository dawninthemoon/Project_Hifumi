using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RandomEvent {
    public class Repeat : IRandomEvent, IResetable {
        private RepeatEvent _eventRepeatInstruction;
        private int _repeatedTime;

        public Repeat() {
            _eventRepeatInstruction = new RepeatEvent();
            _repeatedTime = 0;
        }

        public IEnumerator Execute(string[] variables) {
            int targetRepeatTime = int.Parse(variables[0]);
            if (++_repeatedTime < targetRepeatTime) {
                yield return _eventRepeatInstruction;
            }
        }

        public void Reset() {
            _repeatedTime = 0;
        }
    }
}
