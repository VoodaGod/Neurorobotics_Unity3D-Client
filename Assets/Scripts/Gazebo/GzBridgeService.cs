using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GzBridgeLib;
using ROSBridgeLib;

public class GzBridgeService : Singleton<GzBridgeService> {

    #region PUBLIC_MEMBER_VARIABLES
    
    public GameObject GazeboScene = null;

    #endregion //PUBLIC_MEMBER_VARIABLES

    public GzBridgeWebSocketConnection gzbridge
    {
        get { return this.m_GzBridge; }
    }

    #region PRIVATE_MEMBER_VARIABLES

    private GzBridgeWebSocketConnection m_GzBridge = null;
    private bool m_Initialized = false;

    #endregion //PRIVATE_MEMBER_VARIABLES

    #region MONOBEHAVIOR_METHODS

    // Use this for initialization
    void Start () {
        string url = string.Format("{0}:{1}/gzbridge", BackendConfigService.Instance.IP, BackendConfigService.Instance.GzBridgePort);
        this.ConnectToGzBridge(url);
    }
	
	// Update is called once per frame
	void Update () {
        if (m_Initialized)
        {
            m_GzBridge.Render();
        }
    }

    void OnApplicationQuit()
    {
        if (m_Initialized)
            m_GzBridge.Disconnect();
    }

    #endregion //MONOBEHAVIOR_METHODS

    #region PUBLIC_METHODS

    /// <summary>
    /// Main function to receive scene messages from GzBridge.
    /// </summary>
    /// <param name="msg">JSON msg containing scene message.</param>
    public void ReceiveMessage(GzSceneMsg msg)
    {
        GazeboScene.GetComponent<GazeboSceneManager>().OnSceneMsg(msg.MsgJSON);
    }

    /// <summary>
    /// Main function to receive pose info messages from GzBridge.
    /// </summary>
    /// <param name="msg">JSON msg containing pose info message.</param>
    public void ReceiveMessage(GzPoseInfoMsg msg)
    {
        GazeboScene.GetComponent<GazeboSceneManager>().OnPoseInfoMsg(msg.MsgJSON);
    }

    /// <summary>
    /// Main function to receive model info messages from GzBridge.
    /// </summary>
    /// <param name="msg">JSON msg containing model info message.</param>
    public void ReceiveMessage(GzModelInfoMsg msg)
    {
        GazeboScene.GetComponent<GazeboSceneManager>().OnModelInfoMsg(msg.MsgJSON);
    }

    /// <summary>
    /// Main function to receive material messages from GzBridge.
    /// </summary>
    /// <param name="msg">JSON msg containing material message.</param>
    public void ReceiveMessage(GzMaterialMsg msg)
    {
        GazeboScene.GetComponent<GazeboSceneManager>().OnMaterialMsg(msg.MsgJSON);
    }

    public void ConnectToGzBridge(string url)
    {
        if (string.IsNullOrEmpty(url))
            return;

        if (GazeboScene == null || GazeboScene.GetComponent<GazeboSceneManager>() == null)
            return;

        m_GzBridge = new GzBridgeWebSocketConnection("ws://" + url);

        // DOES NOT WORK! m_Ros is never null if you call the Constructor, WAIT TILL SIMON IMPLEMENTS UDP BROADCAST WITH ROS CONFIGURATION, GET THE IP ADDRESS FROM THE BROADCAST
        if (m_GzBridge != null)
        {
            //m_GzBridge.AddSubscriber(typeof(GzResponseSubscriber));
            m_GzBridge.AddSubscriber(typeof(GzSceneMsgSubscriber));
            m_GzBridge.AddSubscriber(typeof(GzPoseInfoSubscriber));
            m_GzBridge.AddSubscriber(typeof(GzModelInfoSubscriber));
            m_GzBridge.AddSubscriber(typeof(GzMaterialSubscriber));
            m_GzBridge.Connect();
            m_Initialized = true;

            Debug.Log("GzBridge successfully initialized!");
        }
        else
        {
            Debug.LogWarning("GzBridge could not be initialized!");
        }
    }

    #endregion //PUBLIC_METHODS

    #region PRIVATE_METHODS

    /*IEnumerator Connect()
    {
        yield return new WaitUntil(() => !string.IsNullOrEmpty(BackendConfigService.Instance.IP));

        string url = string.Format("{0}:{1}/gzbridge", BackendConfigService.Instance.IP, BackendConfigService.Instance.GzBridgePort);
        this.ConnectToGzBridge(url);
    }*/

    #endregion //PRIVATE_METHODS
}
