using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserverSubject {
    void Attach(IObserver observer);
    void Detach(IObserver observer);
    void Notify();
}
