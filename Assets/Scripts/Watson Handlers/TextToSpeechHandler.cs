using UnityEngine;
using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using System.Collections;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Connection;
using Assets.Scripts;

public class TextToSpeechHandler : MonoBehaviour {

	#region PLEASE SET THESE VARIABLES IN THE INSPECTOR
	[Space(10)]
	[Tooltip("The service URL (optional). This defaults to \"https://stream.watsonplatform.net/text-to-speech/api\"")]
	[SerializeField]
	private string _serviceUrl;
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

	TextToSpeech _service;

	[Header("Watson Assistant")]
	// link to Assistant so that it will read out the conversation response each time
	public AssistantHandler WatsonAssistant;

	void Start()
	{
			LogSystem.InstallDefaultReactors();
			Runnable.Run(CreateService());

			WatsonAssistant.AssistantResponded += OnAssistantResponded;
	}

	private IEnumerator CreateService()
	{
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

			_service = new TextToSpeech(credentials);
	}

	void OnAssistantResponded(object source, OnAssistantResponseEventArgs e) {
		// Do not read out the dialog response for the 'action' and 'nav' intents
		if (e.Dialog != null) {
			Speak(e.Dialog);
		}
	}

	void Speak (string text) {
		// read the dialog response out-loud using Watson Text-to-Speech
		Debug.Log("SPEAK " + text);
		_service.Voice = VoiceType.en_US_Michael;
		_service.ToSpeech(HandleToSpeechCallback, OnFail, text, true);
	}

	void HandleToSpeechCallback(AudioClip clip, Dictionary<string, object> customData = null)
	{
			PlayClip(clip);
	}

	private void PlayClip(AudioClip clip)
	{
			if (Application.isPlaying && clip != null)
			{
					GameObject audioObject = new GameObject("AudioObject");
					AudioSource source = audioObject.AddComponent<AudioSource>();
					source.spatialBlend = 0.0f;
					source.loop = false;
					source.clip = clip;
					source.Play();

					Destroy(audioObject, clip.length);
			}
	}

	private void OnFail(RESTConnector.Error error, Dictionary<string, object> customData)
	{
			Log.Error("ExampleTextToSpeech.OnFail()", "Error received: {0}", error.ToString());
	}
}
