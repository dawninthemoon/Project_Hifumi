using System.Collections;

namespace RandomEvent {
    public interface IRandomEvent {
        IEnumerator Execute(string[] variables);
    }
}