using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ExperimentsService : Singleton<ExperimentsService>
{
    // Use this for initialization
    void Start()
    {

        if (!string.IsNullOrEmpty(BackendConfigService.Instance.IP))
        {
            StartCoroutine(GetExperimentsList());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator GetExperimentsList()
    {
        // wait until authentication token available
        yield return new WaitUntil(() => !string.IsNullOrEmpty(AuthenticationService.Instance.token));

        string url = string.Format("http://{0}:{1}/proxy/storage/experiments", BackendConfigService.Instance.IP, BackendConfigService.Instance.ProxyPort);

        UnityWebRequest www = UnityWebRequest.Get(url);
        AuthenticationService.Instance.AddAuthHeader(ref www);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            //Debug.Log(www.downloadHandler.text);
        }
    }
}