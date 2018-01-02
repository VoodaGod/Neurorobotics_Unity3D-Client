using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NRPBackendManager : MonoBehaviour {

    public string NRPBackendIP = "192.168.0.153";
    public int GzBridgePort = 8080;
    public int ROSBridgePort = 9090;
    public int BackendProxyPort = 8000;
    public GameObject GazeboScene = null;

    private GzBridgeManager GzBridgeManager;
    private ROSBridge ROSBridgeManager;
    private string authToken = null;

	// Use this for initialization
	void Start ()
    {
        // initialize GzBridge component
        if (this.gameObject.GetComponent<GzBridgeManager>() == null)
        {
            this.gameObject.AddComponent<GzBridgeManager>();
        }
        this.GzBridgeManager = this.gameObject.GetComponent<GzBridgeManager>();

        // initialize ROSBridge component
        if (this.gameObject.GetComponent<ROSBridge>() == null)
        {
            this.gameObject.AddComponent<ROSBridge>();
        }
        this.ROSBridgeManager = this.gameObject.GetComponent<ROSBridge>();

        if (!string.IsNullOrEmpty(this.NRPBackendIP))
        {
            // get token

            //TODO: find out how to retrieve token in a clean way
            GzBridgeManager.URL = NRPBackendIP + ":" + GzBridgePort.ToString() + "/gzbridge?token=" + authToken;
            GzBridgeManager.GazeboScene = this.GazeboScene;
            GzBridgeManager.ConnectToGzBridge();

            ROSBridgeManager.ROSCoreIP = NRPBackendIP;
            ROSBridgeManager.Port = ROSBridgePort;
        }

        Debug.Log(GetAuthTocken());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator GetAuthTocken()
    {
        string urlAuthenticate = NRPBackendIP + ":" + BackendProxyPort.ToString() + "/authentication/authenticate";
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("user=nrpuser&password=password"));

        UnityWebRequest request = UnityWebRequest.Post(urlAuthenticate, formData);
        yield return request.Send();

        if (request.isError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
    }
}
