using UnityEngine;
using System.Collections.Generic;
using GzBridgeLib;
using SimpleJSON;

/// <summary>
/// Roboymanager has different tasks:
///
/// <b>- Run ROS:</b>
///     -# Connect to the simulation.
///     -# Add subscriber to the pose.
///     -# Add publisher for external force.
///     -# Add service response for world reset.
///
/// <b>- Receive and send ROS messages:</b>
///     -# receive pose msg to adjust roboy pose.
///     -# subscribe to external force event and send msg to simulation.
///     -# send service call for world reset.
///     -# FUTURE: receive motor msg and forward it to the according motors.
/// </summary>
public class GzBridgeManager : Singleton<GzBridgeManager>
{
    #region PUBLIC_MEMBER_VARIABLES

    /// <summary>
    /// The IP address of the VM or the machine where the simulation is running
    /// </summary>
    public string GZBRIDGE_URL = "192.168.0.17:8080/gzbridge";

    public GameObject GazeboScene = null;

    #endregion //PUBLIC_MEMBER_VARIABLES

    #region PRIVATE_MEMBER_VARIABLES

    /// <summary>
    /// ROSBridge websocket
    /// </summary>
    private GzBridgeWebSocketConnection m_GzBridge = null;

    /// <summary>
    /// Variable to check if the ROS connection is working!
    /// </summary>
    private bool m_Initialized = false;

    #endregion //PRIVATE_MEMBER_VARIABLES

    #region MONOBEHAVIOR_METHODS

    /// <summary>
    /// Initialize ROSBridge and roboy parts
    /// </summary>
    void Awake()
    {
        if (string.IsNullOrEmpty(GZBRIDGE_URL))
            return;

        if (GazeboScene == null || GazeboScene.GetComponent<GazeboSceneManager>() == null)
            return;

        m_GzBridge = new GzBridgeWebSocketConnection("ws://" + GZBRIDGE_URL);

        // DOES NOT WORK! m_Ros is never null if you call the Constructor, WAIT TILL SIMON IMPLEMENTS UDP BROADCAST WITH ROS CONFIGURATION, GET THE IP ADDRESS FROM THE BROADCAST
        if (m_GzBridge != null)
        {
            //m_GzBridge.AddSubscriber(typeof(GzResponseSubscriber));
            m_GzBridge.AddSubscriber(typeof(GzSceneTopicSubscriber));
            m_GzBridge.Connect();
            m_Initialized = true;

            Debug.Log("GzBridge successfully initialized!");
        }
        else
        {
            Debug.LogWarning("GzBridge could not be initialized!");
        }
    }

    /// <summary>
    /// Run ROSBridge
    /// </summary>
    void Update()
    {
        if (m_Initialized)
        {
            m_GzBridge.Render();
        }
    }

    /// <summary>
    /// Disconnect from the simulation when Unity is not running.
    /// </summary>
    void OnApplicationQuit()
    {
        if (!m_Initialized)
            m_GzBridge.Disconnect();
    }
    #endregion //MONOBEHAVIOR_METHODS

    #region PUBLIC_METHODS

    /// <summary>
    /// Main function to receive messages from ROSBridge. Adjusts the roboy pose and the motors values (future).
    /// </summary>
    /// <param name="msg">JSON msg containing roboy pose.</param>
    public void ReceiveMessage(GzSceneMsg msg)
    {
        //Debug.Log("Received GzSceneMsg");
        GazeboScene.GetComponent<GazeboSceneManager>().BuildScene(msg.MsgJSON);

        //Use additional data to adjust motor values

    }

    #endregion //PUBLIC_METHODS

    #region PRIVATE_METHODS

    #endregion //PRIVATE_METHODS
}
