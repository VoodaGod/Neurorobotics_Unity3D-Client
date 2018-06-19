using System;
using UnityEngine;
using SimpleJSON;
using ROSBridgeLib;

public class GzEntityDeletePublisher : ROSBridgePublisher
{
    public new static string GetMessageTopic()
    {
        return "~/entity_delete";
    }

    public new static string GetMessageType()
    {
        return "gazebo.msgs.entity_delete";
    }

    public static string ToYAMLString(GzEntityDeleteMsg msg)
    {
        return msg.ToYAMLString();
    }
}
