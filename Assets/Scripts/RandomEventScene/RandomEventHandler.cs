using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RandomEvent;
using TMPro;
using System.Reflection;
using System.Linq;
using Cysharp.Threading.Tasks;

public class RandomEventHandler : MonoBehaviour, IResetable, ILoadable {
    [SerializeField] private Image _illust;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private SelectionButton[] _selectionButtons;
    private EventsDataParser _parser;
    private EventsData[] _eventsDataArray;
    private Dictionary<string, IRandomEvent> _eventEffectDictionary;
    private Dictionary<string, Sprite> _eventSpriteDictionary;
    private System.Action _roomExitCallback;
    public static bool IsLoadCompleted {
        get;
        private set;
    }

    private async UniTaskVoid Awake() {
        var assetLoader = AssetLoader.Instance;
        _parser = new EventsDataParser();

        _eventEffectDictionary = new Dictionary<string, IRandomEvent>();
        
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

    public void SetRoomExitCallback(System.Action roomExitCallback) {
        _roomExitCallback = roomExitCallback;
    }

    public void Initialize() {
        int randomIndex = Random.Range(0, _eventsDataArray.Length);
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
        if (effects != null) {
            foreach (EventEffects effect in effects) {
                if (_eventEffectDictionary.TryGetValue(effect.eventName, out IRandomEvent instance)) {
                    instance.Execute(effect.variables);
                }
            }
        }
        _roomExitCallback.Invoke();
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
