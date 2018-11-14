using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRDebugLogger : MonoBehaviour {

	public Text VRUILogs;
	public int LoggerLength = 3;

	[Header("Display Log Types")]
	public bool IncludeLogs = true;
	public bool IncludeAsserts = true;
	public bool IncludeWarnings = true;
	public bool IncludeErrors = true;
	public bool IncludeExceptions = true;

	[Header("VR Controllers")]
	[Tooltip("By default, RightHand.GetPadDown() will toggle the debug screen")]
	public VRInputCtrl LeftHand;
	public VRInputCtrl RightHand;

	private Canvas _canvas;
	private bool _showTranscript;
	private List<LoggerLine> _transcript = new List<LoggerLine>();

	private class LoggerLine {
		public string type = "";
		public string log = "";
		public LoggerLine (LogType _type, string _log) {
			type = _type.ToString();
			log = _log;
		}
	}

	void Start () {
		_canvas = this.GetComponent<Canvas>();
	}

	// Use this for initialization
	void Update () {
		if (RightHand.GetPadDown()) {
			ToggleTranscriptActive();
		}
	}

	void OnEnable () {
		Application.logMessageReceived += HandleLog;
	}

	void OnDisable () {
		Application.logMessageReceived -= HandleLog;
	}

	// Update is called once per frame
	void HandleLog (string log, string strackTrace, LogType type) {
		LoggerLine line = new LoggerLine(type, log);
		_transcript.Add(line);
		UpdateUITranscript();
	}

	private string FormatLog (LoggerLine line) {
		string color = "#59E396";
		switch(line.type) {
			case "Log":
				color = "#BFBFBF";
				break;
			case "Assert":
				color = "#FF8940";
				break;
			case "Warning":
				color = "#FFD770";
				break;
			case "Error":
				color = "#FF405F";
				break;
			case "Exception":
				color = "#FF8940";
				break;
		}
		return "<color=" + color + ">[" + line.type + "] : " + line.log +  "</color>\n\n";
	}

	private bool ShowLog (LoggerLine line) {
		switch(line.type) {
			case "Log":
				return IncludeLogs;
			case "Assert":
				return IncludeAsserts;
			case "Warning":
				return IncludeWarnings;
			case "Error":
				return IncludeErrors;
			case "Exception":
				return IncludeExceptions;
			default:
				return false;
		}
	}

	private void UpdateUITranscript () {
		VRUILogs.text = "";
		int logsAdded = 0;
		for (int l = _transcript.Count - 1; l >= 0; l--) {
			if (logsAdded < LoggerLength) {
				LoggerLine line = _transcript[l];
				if (ShowLog(line)) {
					VRUILogs.text = VRUILogs.text + FormatLog(line);
					logsAdded++;
				}
			} else {
				break;
			}
		}
	}

	private void ToggleTranscriptActive() {
		_showTranscript = !_showTranscript;
		_canvas.enabled = _showTranscript;
	}
}
