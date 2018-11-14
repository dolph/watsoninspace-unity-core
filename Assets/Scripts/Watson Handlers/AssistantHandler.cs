using System;
using System.Collections;
using System.Collections.Generic;
using FullSerializer;
using UnityEngine;
using IBM.Watson.DeveloperCloud.Connection;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Services.Assistant.v1;
using IBM.Watson.DeveloperCloud.Utilities;

namespace Assets.Scripts {

	public class OnAssistantResponseEventArgs : EventArgs {
		public string Input { get; set; }
		public string Intent { get; set; }
		public List<object> Entities { get; set; }
		public string Dialog { get; set; }
	}

	public class OnAssistantErrorEventArgs : EventArgs {
		public string Input { get; set; }
		public string Error { get; set; }
	}

	public class AssistantHandler : MonoBehaviour {

		#region PLEASE SET THESE VARIABLES IN THE INSPECTOR
		[Space(10)]
		[Tooltip("The service URL (optional). This defaults to \"https://gateway.watsonplatform.net/assistant/api\"")]
		[SerializeField]
		private string _serviceUrl;
		[Tooltip("The workspaceId to run the example.")]
		[SerializeField]
		private string _workspaceId;
		[Tooltip("The version date with which you would like to use the service in the form YYYY-MM-DD. Attained from \"https://console.bluemix.net/docs/services/conversation/release-notes.html#release-notes\".")]
		[SerializeField]
		private string _versionDate;
		[Header("CF Authentication")]
		[Tooltip("The authentication username.")]
		[SerializeField]
		private string _username;
		[Tooltip("The authentication password.")]
		[SerializeField]
		private string _password;
		[Header("IAM Authentication")]
		[Tooltip("The IAM apikey.")]
		[SerializeField]
		private string _iamApikey;
		[Tooltip("The IAM url used to authenticate the apikey (optional). This defaults to \"https://iam.bluemix.net/identity/token\".")]
		[SerializeField]
		private string _iamUrl;
		#endregion

		private Assistant _service;

		private fsSerializer _serializer = new fsSerializer();
		private Dictionary<string, object> _context = null;

		// Use this for initialization
		void Start () {
			LogSystem.InstallDefaultReactors();
			Runnable.Run(CreateService());
		}

		/*
		Connect to Watson Assistant given the provided credentials
		*/
		private IEnumerator CreateService() {
			//  Create credential and instantiate service
			Credentials credentials = null;
			if (!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password))
			{
				//  Authenticate using username and password
				credentials = new Credentials(_username, _password, _serviceUrl);
			}
			else if (!string.IsNullOrEmpty(_iamApikey))
			{
				//  Authenticate using iamApikey
				TokenOptions tokenOptions = new TokenOptions()
				{
					IamApiKey = _iamApikey,
					IamUrl = _iamUrl
				};

				credentials = new Credentials(tokenOptions, _serviceUrl);

				//  Wait for tokendata
				while (!credentials.HasIamTokenData())
				yield return null;
			}
			else
			{
				throw new WatsonException("Please provide either username and password or IAM apikey to authenticate the service.");
			}

			_service = new Assistant(credentials);
			_service.VersionDate = _versionDate;

			// Runnable.Run(Examples());
		}

		// define the function (subcriber) structure
		public delegate void AssistantResponseHandler(object source, OnAssistantResponseEventArgs args);
		// define the event that was trigger the function and subscribers
		public event AssistantResponseHandler AssistantResponded;

		// define the function (subcriber) structure
		public delegate void AssistantErrorHandler(object source, OnAssistantErrorEventArgs args);
		// define the event that was trigger the function and subscribers
		public event AssistantErrorHandler AssistantFailed;

		private string lastInput = "";

		/*
		Send message to Watson Assistant
		*/
		public void Ask (string text) {
			Debug.Log("asking...");
			Debug.Log(text);

			lastInput = text;
			if (_context == null) {
				_context = new Dictionary<string, object>();
			}
			if (_context.ContainsKey("location")) {
				_context["location"] = LocationTracker.currentLocation;
			} else {
				_context.Add("location", LocationTracker.currentLocation);
			}
			if (_context.ContainsKey("lookingAt")) {
				_context["lookingAt"] = LookingAtTracker.activeObject;
			}	 else {
				_context.Add("lookingAt", LookingAtTracker.activeObject);
			}

			// create the message request to be sent to Watson
			MessageRequest messageRequest = new MessageRequest() {
				Input = new Dictionary<string, object>() {{"text", text}},
				Context = _context
			};
			// send the message to Watson Assistant
			_service.Message(OnMessage, OnFail, _workspaceId, messageRequest);
		}

		/*
		Handler for a successful response from Watson Assistant
		*/
		private void OnMessage(object response, Dictionary<string, object> customData)
		{
			Log.Debug("ExampleAssistant.OnMessage()", "Response: {0}", customData["json"].ToString());

			//  Convert resp to fsdata
			fsData fsdata = null;
			fsResult r = _serializer.TrySerialize(response.GetType(), response, out fsdata);
			if (!r.Succeeded)
			throw new WatsonException(r.FormattedMessages);

			//  Convert fsdata to MessageResponse
			MessageResponse messageResponse = new MessageResponse();
			object obj = messageResponse;
			r = _serializer.TryDeserialize(fsdata, obj.GetType(), ref obj);
			if (!r.Succeeded)
			throw new WatsonException(r.FormattedMessages);

			// Convert response to Dictionart
			Dictionary<string, object> resp = (response as Dictionary<string, object>);

			//  Set context for next round of messaging
			object _tempContext = null;
			resp.TryGetValue("context", out _tempContext);

			if (_tempContext != null)
				_context = _tempContext as Dictionary<string, object>;
			else
				Log.Debug("ExampleAssistant.OnMessage()", "Failed to get context");

			string intent = GetIntentFromResponse(response);
			List<object> entities = GetEntitiesFromResponse(response);
			string dialog = "";
			// not all interactions require text answer from Watson
			dialog = GetDialogFromResponse(response);

			OnAssistantResponseEventArgs args = new OnAssistantResponseEventArgs();
			args.Input = lastInput;
			args.Intent = intent;
			args.Entities = entities;
			args.Dialog = dialog;

			AssistantResponded(this, args);
		}

		private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
		{
			Log.Debug("ExampleAssistant.OnFail()", "Response: {0}", customData["json"].ToString());
			Log.Error("AssistantHandler.OnFail()", "Error received: {0}", error.ToString());
			OnAssistantErrorEventArgs args = new OnAssistantErrorEventArgs();
			args.Input = lastInput;
			args.Error = error.ToString();

			AssistantFailed(this, args);
		}

		/*
		Parse the Watson Assistant response and return the classified intent
		*/
		private string GetIntentFromResponse (object response) {
			Dictionary<string, object> resp = (response as Dictionary<string, object>);

			try {
				//  Get intent
				object _intentsObj = null;
				// convert response to a dictionary and try to get response['intents']
				resp.TryGetValue("intents", out _intentsObj);

				// get the first index of the response['intents'] List
				object _intentObj = (_intentsObj as List<object>)[0];
				object _intent = null;
				// get response['intents'][0]['intent']
				(_intentObj as Dictionary<string, object>).TryGetValue("intent", out _intent);
				return _intent.ToString();
			}
			catch (Exception) {
				return "";
			}
		}

		/*
		Parse the Watson Assistant response and return the recognised Entities
		*/
		private List<object> GetEntitiesFromResponse (object response) {
			Dictionary<string, object> resp = (response as Dictionary<string, object>);
			List<object> _entities = new List<object>();
			try {
				//  Get intent
				object _entitiesObj = null;
				// convert response to a dictionary and try to get response['entities']
				resp.TryGetValue("entities", out _entitiesObj);
				List<object> _entitiesList = (_entitiesObj as List<object>);
				return _entitiesList;
			}
			catch (Exception) {
				return _entities;
			}
		}

		/*
		{
			"intents":[{
				"intent":"welcome",
				"confidence":1
			}],
			"entities":[],
			"input":{
				"text":"hello"
			},
			"output":{
				"generic":[{
					"response_type":"text",
					"text":"Hi and welcome to the International Space Station."
				}],
				"text":["Hi and welcome to the International Space Station."],
				"nodes_visited":["Welcome"],
				"log_messages":[]
			},
			"context":{
				"conversation_id":"df10509f-23b3-49cc-a319-c414849fba98",
				"system":{"dialog_stack":[{"dialog_node":"root"}],
				"dialog_turn_counter":1,
				"dialog_request_counter":1,
				"_node_output_map":{"Welcome":{"0":[0]}},
				"branch_exited":true,
				"branch_exited_reason":"completed"
			}
		}
		*/

		/*
		Parse the Watson Assistant response and return the classified intent
		*/
		private string GetDialogFromResponse (object response) {
			Dictionary<string, object> resp = (response as Dictionary<string, object>);

			try {
				//  Get intent
				object _outputObj = null;
				// convert response to a dictionary and try to get response['intents']
				resp.TryGetValue("output", out _outputObj);

				object _output = null;
				// get response['intents'][0]['intent']
				(_outputObj as Dictionary<string, object>).TryGetValue("text", out _output);
				return (_output as List<object>)[0].ToString();
			}
			catch (Exception) {
				return "";
			}
		}

		private string GetLocallyDefinedResponse(string intent) {
			switch(intent) {
				case "in-game--where-user":
					return "You are currently in the '" + LocationTracker.currentLocation + "' module";
				case "in-game--looking-at":
					return "That is the '" + LookingAtTracker.activeObject + "'";
				default:
					return "[Assistant Handler] - Unable to respond.";
			}
		}
	}
}
