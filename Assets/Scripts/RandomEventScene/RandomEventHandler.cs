using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RandomEvent;
using TMPro;
using System.Reflection;
using System.Linq;

public class RandomEventHandler : MonoBehaviour, IResetable {
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private SelectionButton[] _selectionButtons;
    private EventsDataParser _parser;
    private EventsData[] _eventsDataArray;
    private Dictionary<string, IRandomEvent> _eventEffectDictionary;

    private void Awake() {
        _parser = new EventsDataParser();
        _eventsDataArray = _parser.ParseData();

        InitializeRandomEventEffects();

        foreach (SelectionButton button in _selectionButtons) {
            button.AddListener(OnSelectionButtonClicked);
        }
    }

    public void Initialize() {
        int randomIndex = Random.Range(0, _eventsDataArray.Length);
        EventsData randomEvent = _eventsDataArray[randomIndex];
        
        _descriptionText.text = randomEvent.Description;
        _selectionButtons[0].Initialize(randomEvent.Selection1);
        _selectionButtons[1].Initialize(randomEvent.Selection2);
        _selectionButtons[2].Initialize(randomEvent.Selection3);
    }

    public void Reset() {
        foreach (SelectionButton button in _selectionButtons) {
            button.Reset();
        }
    }

    private void OnSelectionButtonClicked(EventEffects[] effects) {
        if (effects == null)
            return;
            
        foreach (EventEffects effect in effects) {
            if (_eventEffectDictionary.TryGetValue(effect.eventName, out IRandomEvent instance)) {
                instance.Execute(effect.variables);
            }
        }
    }

    private void InitializeRandomEventEffects() {
        string namespaceName = "RandomEvent";
        var eventTypes = Assembly.GetExecutingAssembly().GetTypes();
        _eventEffectDictionary 
            = eventTypes
                .Where(type => (type.Namespace == namespaceName))
                .Select(type => type)
                .ToDictionary(type => type.Name, type => System.Activator.CreateInstance(type) as IRandomEvent);
    }
}
