using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ExperimentsService : Singleton<ExperimentsService>
{
    private BackendConfigService backend = null;

    // Use this for initialization
    void Start () {
        this.backend = BackendConfigService.Instance;

        if (!string.IsNullOrEmpty(backend.IP))
        {
            StartCoroutine(GetExperiments());
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator GetExperiments()
    {
        // wait until authentication token received
        yield return new WaitUntil(() => !string.IsNullOrEmpty(AuthenticationService.Instance.token));

        string experiments_url = "http://" + this.backend.IP + ":" + this.backend.ProxyPort.ToString() + "/proxy/storage/experiments";

        UnityWebRequest www = UnityWebRequest.Get(experiments_url);
        www.SetRequestHeader("Authorization", "Bearer " + AuthenticationService.Instance.token);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
        }
    }
}
