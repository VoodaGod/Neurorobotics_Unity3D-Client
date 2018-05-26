using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackendConfigService : Singleton<BackendConfigService>
{
    public string IP = "xxx.xxx.xxx.xxx";
    public int GzBridgePort = 8080;
    public int ROSBridgePort = 9090;
    public int ProxyPort = 9000;
    public GameObject GazeboScene = null;

    private GzBridgeManager GzBridgeManager;
    private ROSBridge ROSBridgeManager;

	// Use this for initialization
	void Start ()
    {
        if (!string.IsNullOrEmpty(this.IP))
        {
            this.ConnectToGazeboBridge();
            this.ConnectToROSBridge();
        }
    }
	
	// Update is called once per frame
	void Update () {

    }

    private void ConnectToGazeboBridge()
    {
        // initialize GzBridge component
        if (this.gameObject.GetComponent<GzBridgeManager>() == null)
        {
            this.gameObject.AddComponent<GzBridgeManager>();
        }
        this.GzBridgeManager = this.gameObject.GetComponent<GzBridgeManager>();
        
        GzBridgeManager.URL = this.IP + ":" + GzBridgePort.ToString() + "/gzbridge";
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

        ROSBridgeManager.ROSCoreIP = this.IP;
        ROSBridgeManager.Port = ROSBridgePort;
    }
}
