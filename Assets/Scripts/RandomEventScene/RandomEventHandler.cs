using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using RandomEvent;
using TMPro;
using System.Reflection;
using System.Linq;
using Cysharp.Threading.Tasks;

public class RandomEventHandler : MonoBehaviour, IResetable, ILoadable {
    [SerializeField, Tooltip("For Test")] private string _targetEventID;
    [SerializeField] private Image _illust;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private SelectionButton[] _selectionButtons;
    private EventsDataParser _parser;
    private EventsData[] _eventsDataArray;
    private Dictionary<string, IRandomEvent> _eventEffectDictionary;
    private Dictionary<string, Sprite> _eventSpriteDictionary;
    private UnityEvent _roomExitEvent;
    public static bool IsLoadCompleted {
        get;
        private set;
    }

    private async UniTaskVoid Awake() {
        var assetLoader = AssetLoader.Instance;
        _parser = new EventsDataParser();

        _eventEffectDictionary = new Dictionary<string, IRandomEvent>();
        _roomExitEvent = new UnityEvent();
        _roomExitEvent.AddListener(ResetEvents);
        
        string eventsFileName = "EventsData";
        string selectionsFileName = "EventSelections";
        TextAsset eventJsonText = await assetLoader.LoadAssetAsync<TextAsset>(eventsFileName);
        TextAsset selectionJsonText = await assetLoader.LoadAssetAsync<TextAsset>(selectionsFileName);
            
        _eventsDataArray = _parser.ParseData(eventJsonText, selectionJsonText);

        InitializeRandomEventEffects();
        
        string eventSpriteKey = "EventSprites";
        assetLoader.LoadAssetsAsync<Sprite>(
            eventSpriteKey,
            (handle) => {
                _eventSpriteDictionary
                    = handle.Result.ToDictionary(x => x.name);
                IsLoadCompleted = true;
            }
        );
    }

    private void Start() {
        foreach (SelectionButton button in _selectionButtons) {
            button.AddListener(OnSelectionButtonClicked);
        }
    }

    public void SetRoomExitCallback(UnityAction roomExitCallback) {
        _roomExitEvent.AddListener(roomExitCallback);
    }

    public void Initialize() {
        int randomIndex = Random.Range(0, _eventsDataArray.Length);

        // Only For Test
        if (_targetEventID != null) {
            for (int i = 0; i < _eventsDataArray.Length; ++i) {
                EventsData e = _eventsDataArray[i];
                if (e.ID.Equals(_targetEventID))
                    randomIndex = i;
            }
        }

        EventsData randomEvent = _eventsDataArray[randomIndex];

        if (randomEvent.SpriteName != null)
            _illust.sprite = _eventSpriteDictionary[randomEvent.SpriteName];
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
        bool repeatEvent = false;
        if (effects != null) {
            foreach (EventEffects effect in effects) {
                ExecuteEvent(effect.eventName, effect.variables, ref repeatEvent);
            }
        }
        
        if (!repeatEvent) {
            _roomExitEvent.Invoke();
        }
    }

    private void ExecuteEvent(string eventName, string[] variables, ref bool repeatEvent) {
        if (_eventEffectDictionary.TryGetValue(eventName, out IRandomEvent instance)) {
            IEnumerator routine = instance.Execute(variables);
            while (routine.MoveNext()) {
                var nestRoutine = routine?.Current as YieldInstruction;
                if (nestRoutine is RepeatEvent) {
                    repeatEvent = true;
                }
            }
        }
    }

    private void ResetEvents() {
        foreach (SelectionButton selection in _selectionButtons) {
            EventSelections selectionData = selection.SelectionData;
            if (selectionData == null) continue;
            if (selectionData.Effects != null) {
                foreach (EventEffects effect in selectionData.Effects) {
                    string eventName = effect.eventName;
                    if (_eventEffectDictionary.TryGetValue(eventName, out IRandomEvent instance)) {
                        IResetable resetableEvent = instance as IResetable;
                        resetableEvent?.Reset();
                    }
                }
            }
        }
    }

    private void InitializeRandomEventEffects() {
        string namespaceName = "RandomEvent";
        string interfaceName = "IRandomEvent";
        var eventTypes = Assembly.GetExecutingAssembly().GetTypes();
        _eventEffectDictionary 
            = eventTypes
                .Where(type => (type.Namespace == namespaceName) && !type.IsInterface)
                .Where(type => (type.GetInterface(interfaceName) != null))
                .ToDictionary(type => type.Name, type => System.Activator.CreateInstance(type) as IRandomEvent);
    }
}
