using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SimulationControlService : Singleton<SimulationControlService> {

    public const string STATE_STARTED = "started";
    public const string STATE_PAUSED = "paused";
    public const string STATE_STOPPED = "stopped";

    public int running_simulation_ID
    {
        get { return this.running_simulation["simulationID"].AsInt; }
    }

    private JSONNode simulations = null;
    private JSONNode running_simulation = null;

    // Use this for initialization
    void Start () {
        StartCoroutine(RequestSimulationGET());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetState(string state)
    {
        StartCoroutine(RequestStatePUT(state));
    }

    private IEnumerator RequestStatePUT(string state)
    {
        string state_json = string.Format("{{\"state\": \"{0}\"}}", state);
        var state_data = System.Text.Encoding.UTF8.GetBytes(state_json);
        
        string url = string.Format("http://{0}:{1}/simulation/{2}/state", BackendConfigService.Instance.IP, BackendConfigService.Instance.GzBridgePort, this.running_simulation_ID);

        UnityWebRequest uwr = UnityWebRequest.Put(url, state_data);
        AuthenticationService.Instance.AddAuthHeader(ref uwr);
        uwr.SetRequestHeader("Content-Type", "application/json");
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError || uwr.isHttpError)
        {
            Debug.Log(uwr.error);
        }
        else
        {
            //Debug.Log(uwr.downloadHandler.text);
        }
    }
    
    private IEnumerator RequestSimulationGET()
    {
        // wait until authentication token available
        yield return new WaitUntil(() => !string.IsNullOrEmpty(AuthenticationService.Instance.token));

        string url = string.Format("http://{0}:{1}/simulation", BackendConfigService.Instance.IP, BackendConfigService.Instance.GzBridgePort);

        UnityWebRequest uwr = UnityWebRequest.Get(url);
        AuthenticationService.Instance.AddAuthHeader(ref uwr);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError || uwr.isHttpError)
        {
            Debug.Log(uwr.error);
        }
        else
        {
            //Debug.Log(uwr.downloadHandler.text);
            this.simulations = JSONNode.Parse(uwr.downloadHandler.text);

            for (int i = this.simulations.AsArray.Count-1; i >= 0; i = i -1)
            {
                if (this.simulations.AsArray[i]["state"] != "stopped")
                {
                    this.running_simulation = this.simulations.AsArray[i];
                    break;
                }
                else
                {
                    this.running_simulation = null;
                }
            }
        }
    }
}
