using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class TeleportCtrl : MonoBehaviour
{
    public AssistantHandler WatsonAssistant;
    private Vector3 startLocation;
  	private Rigidbody rb;

    void Start () {
      rb = GetComponent<Rigidbody> ();
      startLocation = this.gameObject.transform.position;

      WatsonAssistant.AssistantResponded += OnAssistantResponded;
    }

    public void OnAssistantResponded(object source, OnAssistantResponseEventArgs e) {
      // Toggle the debug panel is the #action--debug intent recognised
      string module = ModuleRecognised(e.Entities);
      if (e.Intent != "" && e.Intent == "nav--teleport" && module != "") {
        MovePlayer(module);
      } else if (e.Intent != "" && e.Intent == "nav--reset") {
        ResetPlayer();
      }
    }

    private string ModuleRecognised(List<object> entities) {
      foreach (object e in entities) {
        Debug.Log("[TeleportCtrl]");
        Debug.Log(e);
        Dictionary<string, object> eDict = (e as Dictionary<string, object>);
        // read e["entity"]
        object _entity = null;
        eDict.TryGetValue("entity", out _entity);
        string _entityType = _entity.ToString();
        if (_entityType == "ISS_Module") {
          return eDict["value"].ToString();
        }
      }
      return "";
    }

    private void MovePlayer (string module) {
      Debug.Log("[TransportCtrl] - Transport Player to: " + module);
      // get Module location - move player
      foreach(KeyValuePair<string, Vector3> m in LocationTracker.moduleLocations) {
        if (m.Key.Contains(module)) {
          StartCoroutine("FadeInOutLocation", LocationTracker.moduleLocations[module]);
        }
      }
    }

    private void ResetPlayer () {
      SteamVR_Fade.Start(Color.black, 1f);
      // SteamVR_Fade.Start(Color.clear, 1f);
      StartCoroutine("FadeInOutLocation", startLocation);
    }

    private IEnumerator FadeInOutLocation (Vector3 newLocation) {
      SteamVR_Fade.Start(Color.black, 0.5f);
      yield return new WaitForSeconds(0.5f);
      this.gameObject.transform.position = newLocation;
      StopPlayer();
      SteamVR_Fade.Start(Color.clear, 1f);
    }

    private void StopPlayer() {
      StopMotion();
      StopRotation();
    }

    private void StopMotion() {
  		rb.velocity = new Vector3 (0f, 0f, 0f);
  	}

  	private void StopRotation() {
  		rb.velocity = new Vector3 (0f, 0f, 0f);
  	}
}
