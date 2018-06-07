using ROSBridgeLib.geometry_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserAvatarService : Singleton<UserAvatarService>
{

    private string avatar_name = null;

    // Use this for initialization
    void Start()
    {
        this.SpawnAvatarRayman();  // test
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnAvatarRayman()
    {
        StartCoroutine(SpawnAvatar("user_avatar_rayman"));
    }

    private IEnumerator SpawnAvatar(string avatar_model_name)
    {
        // wait until authentication token available
        yield return new WaitUntil(() => !string.IsNullOrEmpty(AuthenticationService.Instance.token));

        this.avatar_name = "user_avatar_" + AuthenticationService.Instance.token;
        Vector3 spawn_pos = new Vector3();
        Quaternion spawn_rot = new Quaternion();

        GzFactoryMsg msg = new GzFactoryMsg(this.avatar_name, avatar_model_name, new PointMsg(spawn_pos.x, spawn_pos.y, spawn_pos.z), new QuaternionMsg(spawn_rot.x, spawn_rot.y, spawn_rot.z, spawn_rot.w));

        GzBridgeService.Instance.gzbridge.Publish(GzFactoryPublisher.GetMessageTopic(), msg);
    }
}
