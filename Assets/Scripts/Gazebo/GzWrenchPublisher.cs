using System;
using UnityEngine;
using SimpleJSON;
using ROSBridgeLib;

public class GzWrenchPublisher : ROSBridgePublisher
{
    private string topic;

    public GzWrenchPublisher(string topic)
    {
        this.topic = topic;
    }

    public new string GetMessageTopic()
    {
        return this.topic;
    }

    public new static string GetMessageType()
    {
        return "gazebo.msgs.Wrench";
    }

    public static string ToYAMLString(GzWrenchMsg msg)
    {
        return msg.ToYAMLString();
    }
}
