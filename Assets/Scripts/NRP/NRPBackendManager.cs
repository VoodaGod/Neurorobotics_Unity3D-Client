using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NRPBackendManager : MonoBehaviour {

    public string NRPBackendIP = "192.168.0.109";
    public int GzBridgePort = 8080;
    public int ROSBridgePort = 9090;
    public int BackendProxyPort = 9000;
    public string auth_username = "nrpuser";
    public string auth_password = "password";
    public GameObject GazeboScene = null;

    private GzBridgeManager GzBridgeManager;
    private ROSBridge ROSBridgeManager;
    public string auth_token = null;

	// Use this for initialization
	void Start ()
    {
        if (!string.IsNullOrEmpty(this.NRPBackendIP))
        {
            StartCoroutine(Authenticate());
            StartCoroutine(GetExperiments());
            this.ConnectToGazeboBridge();
            this.ConnectToROSBridge();
        }
    }
	
	// Update is called once per frame
	void Update () {

    }

    IEnumerator Authenticate()
    {
        string auth_url = "http://" + this.NRPBackendIP + ":" + this.BackendProxyPort.ToString() + "/proxy/authentication/authenticate";
        string auth_json = "{\"user\": \"" + this.auth_username + "\", \"password\": \"" + this.auth_password + "\"}";

        WWW www;
        Dictionary<string, string> post_header = new Dictionary<string, string>();
        post_header.Add("Content-Type", "application/json");
        var post_data = System.Text.Encoding.UTF8.GetBytes(auth_json);
        www = new WWW(auth_url, post_data, post_header);
        yield return www;

        if (www.error != null)
        {
            Debug.LogWarning("There was an error sending request: " + www.error);
        }
        else
        {
            Debug.Log("auth token: " + www.text);
        }
    }

    IEnumerator GetExperiments()
    {
        // wait until authentication token received
        yield return new WaitUntil(() => auth_token != null);
        Debug.Log("GetExperiments(): auth token = " + this.auth_token);

        string experiments_url = "http://" + this.NRPBackendIP + ":" + this.BackendProxyPort.ToString() + "/proxy/storage/experiments";

        UnityWebRequest www = UnityWebRequest.Get(experiments_url);
        www.SetRequestHeader("Authorization", "Bearer " + this.auth_token);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log("got experiments !!!!");
            Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }

        /*WWW www;
        Dictionary<string, string> post_header = new Dictionary<string, string>();
        post_header.Add("Authorization", "Bearer " + this.auth_token);
        //var post_data = System.Text.Encoding.UTF8.GetBytes(auth_json);
        www = new WWW(experiments_url, new byte[0], post_header);
        yield return www;

        if (www.error != null)
        {
            Debug.LogWarning("There was an error sending request: " + www.error);
        }
        else
        {
            Debug.Log("experiments list: " + www.text);
        }*/
    }

    private void ConnectToGazeboBridge()
    {
        // initialize GzBridge component
        if (this.gameObject.GetComponent<GzBridgeManager>() == null)
        {
            this.gameObject.AddComponent<GzBridgeManager>();
        }
        this.GzBridgeManager = this.gameObject.GetComponent<GzBridgeManager>();
        
        GzBridgeManager.URL = NRPBackendIP + ":" + GzBridgePort.ToString() + "/gzbridge";
        GzBridgeManager.GazeboScene = this.GazeboScene;
        GzBridgeManager.ConnectToGzBridge();
    }

    private void ConnectToROSBridge()
    {
        // initialize ROSBridge component
        if (this.gameObject.GetComponent<ROSBridge>() == null)
        {
            this.gameObject.AddComponent<ROSBridge>();
        }
        this.ROSBridgeManager = this.gameObject.GetComponent<ROSBridge>();

        ROSBridgeManager.ROSCoreIP = NRPBackendIP;
        ROSBridgeManager.Port = ROSBridgePort;
    }
}
