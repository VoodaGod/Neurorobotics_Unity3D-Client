using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackendConfigService : Singleton<BackendConfigService>
{
    public string IP = "xxx.xxx.xxx.xxx";
    public int GzBridgePort = 8080;
    public int ROSBridgePort = 9090;
    public int ProxyPort = 9000;

    private ROSBridge ROSBridgeManager;

	// Use this for initialization
	void Start ()
    {
        if (!string.IsNullOrEmpty(this.IP))
        {
            this.ConnectToROSBridge();
        }
    }
	
	// Update is called once per frame
	void Update () {

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
