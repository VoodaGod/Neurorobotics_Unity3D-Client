using UnityEngine;
using System.Collections.Generic;
using ROSBridgeLib;

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
public class ROSBridgeManager : Singleton<ROSBridgeManager>
{


    #region PUBLIC_MEMBER_VARIABLES

    /// <summary>
    /// The IP address of the VM or the machine where the simulation is running
    /// </summary>
    public string NRP_BACKEND_IP = "";
    public int PORT = 0;

    #endregion //PUBLIC_MEMBER_VARIABLES

    #region PRIVATE_MEMBER_VARIABLES

    /// <summary>
    /// ROSBridge websocket
    /// </summary>
    private ROSBridgeWebSocketConnection m_Ros = null;

    /// <summary>
    /// Variable to check if the ROS connection is working!
    /// </summary>
    private bool m_ROSInitialized = false;

    #endregion //PRIVATE_MEMBER_VARIABLES

    #region MONOBEHAVIOR_METHODS

    /// <summary>
    /// Initialize ROSBridge and roboy parts
    /// </summary>
    void Awake()
    {
        if (string.IsNullOrEmpty(NRP_BACKEND_IP))
            return;

        m_Ros = new ROSBridgeWebSocketConnection("ws://" + NRP_BACKEND_IP, PORT);

        // DOES NOT WORK! m_Ros is never null if you call the Constructor, WAIT TILL SIMON IMPLEMENTS UDP BROADCAST WITH ROS CONFIGURATION, GET THE IP ADDRESS FROM THE BROADCAST
        if (m_Ros != null)
        {
            m_Ros.Connect();
            m_ROSInitialized = true;

            Debug.Log("ROS successfully initialized!");
        }
        else
        {
            Debug.LogWarning("ROS could not be initialized!");
        }
    }

    /// <summary>
    /// Run ROSBridge
    /// </summary>
    void Update()
    {

    }

    /// <summary>
    /// Disconnect from the simulation when Unity is not running.
    /// </summary>
    void OnApplicationQuit()
    {
        if (!m_ROSInitialized)
            m_Ros.Disconnect();
    }
    #endregion //MONOBEHAVIOR_METHODS

    #region PUBLIC_METHODS
    
    #endregion //PUBLIC_METHODS

    #region PRIVATE_METHODS

    #endregion //PRIVATE_METHODS
}
