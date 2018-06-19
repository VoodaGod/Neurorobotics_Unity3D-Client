using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib;
using ROSBridgeLib.geometry_msgs;
using SimpleJSON;

public class GzEntityDeleteMsg : ROSBridgeMsg
{
    #region PRIVATE_MEMBER_VARIABLES

    private string _name;


    #endregion //PRIVATE_MEMBER_VARIABLES


    #region PUBLIC_METHODS

    public GzEntityDeleteMsg(JSONNode msg)
    {
        _name = msg["name"];
    }

    public GzEntityDeleteMsg(string name)
    {
        _name = name;
    }

    public static string GetMessageType()
    {
        return "gazebo.msgs.entity_delete";
    }

    public string GetName()
    {
        return _name;
    }

    public override string ToString()
    {
        return "entity_delete [name=" + _name + "]";
    }

    public override string ToYAMLString()
    {
        return "{\"name\": \"" + _name + "\"}";
    }

    #endregion //PUBLIC_METHODS
}
