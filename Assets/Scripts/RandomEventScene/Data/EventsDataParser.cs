using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;
using System.Text.RegularExpressions;

namespace RandomEvent {
    public class EventsDataParser {
        private static readonly string IDColumn = "ID";
        private static readonly string EventNameColumn = "EventName";
        private static readonly string SpriteNameColumn = "SpriteName";
        private static readonly string DescriptionColumn = "Description";

        private static readonly string SelectionNameColumn = "Name";
        private static readonly string SelectionTextColumn = "Text";
        private static readonly string SelectionEffectsColumn = "Effects";

        private static readonly char commaChar = ',';
        private static readonly string blankStr = "";
        private static readonly string RegexStr = "\\([^)]*\\)";
        private Regex _effectVariableRegex;

        public EventsDataParser() {
            _effectVariableRegex = new Regex(RegexStr);
        }

        public EventsData[] ParseData(TextAsset eventJsonText, TextAsset selectionJsonText) {
            JSONObject eventJsonObject = new JSONObject(eventJsonText.ToString());
            JSONObject selectionJsonObject = new JSONObject(selectionJsonText.ToString());

            int numOfEvents = eventJsonObject.list.Count;
            EventsData[] eventsArray = new EventsData[numOfEvents];

            for (int i = 0; i < numOfEvents; ++i) {
                JSONObject jsonObj = eventJsonObject.list[i];
                
                EventsData data = new EventsData();
                data.ID = jsonObj.GetField(IDColumn).stringValue;
                data.EventName = jsonObj.GetField(EventNameColumn).stringValue;
                data.SpriteName = jsonObj.GetField(SpriteNameColumn).stringValue;
                data.Description = jsonObj.GetField(DescriptionColumn).stringValue;
                
                data.Selection1 = ParseSelectionData(selectionJsonObject, jsonObj.GetField("Selection1ID").stringValue);
                data.Selection2 = ParseSelectionData(selectionJsonObject, jsonObj.GetField("Selection2ID").stringValue);
                data.Selection3 = ParseSelectionData(selectionJsonObject, jsonObj.GetField("Selection3ID").stringValue);
            
                eventsArray[i] = data;
            }
            return eventsArray;
        }

        private EventSelections ParseSelectionData(JSONObject selectionJsonObj, string selectionID) {
            selectionJsonObj = selectionJsonObj.GetField(selectionID);
            if (selectionJsonObj == null) {
                return null;
            }

            EventSelections selection = new EventSelections();
            selection.ID = selectionID;
            selection.Name = selectionJsonObj.GetField(SelectionNameColumn).stringValue;
            selection.Text = selectionJsonObj.GetField(SelectionTextColumn).stringValue;

            string effects = selectionJsonObj.GetField(SelectionEffectsColumn).stringValue;
            if (effects != null) {
                string[] splitedEffects = effects.Split(',');
                EventEffects[] effectsArray = new EventEffects[splitedEffects.Length];

                for (int i = 0; i < splitedEffects.Length; ++i) {
                    string variableStr = _effectVariableRegex.Match(splitedEffects[i]).ToString();
                    string eventName = _effectVariableRegex.Replace(splitedEffects[i], blankStr);
                    variableStr.Trim();
                    effectsArray[i] = new EventEffects(eventName, variableStr.Split(commaChar));
                }
                selection.Effects = effectsArray;
            }
            return selection;
        }
    }
}