using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;
// using Assets.Scripts2;

public class STTTranscript : MonoBehaviour {

	public SpeechToTextHandler WatsonSTT;
	public AssistantHandler WatsonAssistant;

	public int TranscriptLength = 3;
	public Text UITranscript;
	private Canvas canvas;

	public VRInputCtrl LeftHand;
	public VRInputCtrl RightHand;

	private bool showTranscript = true;

	private List<TranscriptLine> _transcript;

	private TranscriptLine line = new TranscriptLine();

	private class TranscriptLine {
		public string text = "";
		public string intent = "";
		public List<object> entities = new List<object>();
		public string dialog = "";
	}

	// Use this for initialization
	void Start () {
		canvas = this.GetComponent<Canvas>();

		_transcript = new List<TranscriptLine>();
		_transcript.Add(line);

		WatsonSTT.SpeechRecognized += OnSpeechRecognized;
		WatsonSTT.EndOfLineRecognized += OnEndOfLineRecognized;
		WatsonAssistant.AssistantResponded += OnAssistantResponsed;
		WatsonAssistant.AssistantFailed += OnAssistantFailed;
	}

	// Use this for initialization
	void Update () {
		if (LeftHand.GetPadDown()) {
			ToggleTranscriptActive();
		}
	}

	public void OnSpeechRecognized(object source, OnRecognizeEventArgs e) {
		Transcribe(e.Transcript);
	}

	public void OnEndOfLineRecognized(object source, OnRecognizeEventArgs e) {
		FinishLine();
	}

	public void OnAssistantResponsed(object source, OnAssistantResponseEventArgs e) {
		SetResponse(e.Input, e.Intent, e.Entities, e.Dialog);
		// Toggle the debug panel is the #action--debug intent recognised
		if (e.Intent != "" && e.Intent == "action--debug") {
			ToggleTranscriptActive();
		}
	}

	private void ToggleTranscriptActive() {
		showTranscript = !showTranscript;
		canvas.enabled = showTranscript;
	}

	public void OnAssistantFailed(object source, OnAssistantErrorEventArgs e) {
		SetResponse(e.Input, "Error", new List<object>(), "");
	}

	private void OnRecognize (System.Object input) {
		Debug.Log("Listened to event...");
		Debug.Log(input);
	}

	public void Transcribe (System.Object stt) {
		string text = (string) stt;
		if (text != "") {
			line.text = text;
			line.intent = "Listening...";
			UpdateUITranscript();
		}
	}

	public void FinishLine () {
		// update state of finished line
		line.intent = "Getting Intent...";
		// add new line for future listening
		line = new TranscriptLine();
		_transcript.Add(line);
		UpdateUITranscript();
	}

	public void SetResponse (string text, string intent, List<object> entities, string dialog) {
		foreach(TranscriptLine sentence in _transcript) {
			if (sentence.text.Equals(text)) {
				sentence.intent = (intent == "") ? "<no intent>" : "#" + intent;
				if (entities.Count > 0) {
					sentence.entities = entities;
				}
				sentence.dialog = dialog;
			}
		}
		UpdateUITranscript();
	}

	private void UpdateUITranscript () {
		UITranscript.text = "";
		int start = _transcript.Count > (TranscriptLength - 1) ? _transcript.Count - TranscriptLength : 0;
		for (int l = start; l < _transcript.Count; l++) {
			UITranscript.text = UITranscript.text + FormatText(_transcript[l]);
		}
	}

	private string FormatText(TranscriptLine sentence) {
		string text = "<color=white>" + sentence.text + "</color>";
		text = text + " <color=#40ddec>" + sentence.intent + "</color>";
		if (sentence.entities.Count > 0) {
			foreach (object e in sentence.entities) {
				// convert entity object to a Dictionary
				Dictionary<string, object> eDict = (e as Dictionary<string, object>);
        // read e["entity"]
        object _entity = null;
        eDict.TryGetValue("entity", out _entity);
        string _entityType = _entity.ToString();
        // read e["value"]
        object _value = null;
        eDict.TryGetValue("value", out _value);
        string _entityValue = _value.ToString();
				text = text + " <color=#FFB9FF>@" + _entityType + ":" + _entityValue + "</color>";
			}
		}
		text = text + "\n";
		return text + "<color=#59E396>" + sentence.dialog + "</color>\n\n";
	}
}
