using ROSBridgeLib.geometry_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserAvatarService : Singleton<UserAvatarService>
{

    private string avatar_name = null;

    private GameObject user_avatar = null;

    // Use this for initialization
    void Start()
    {
        this.SpawnAvatarRayman();  // test
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnApplicationQuit()
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
        Vector3 spawn_pos = GazeboSceneManager.Unity2GzVec3(new Vector3(-6.5f, 0f, 2f));
        Quaternion spawn_rot = new Quaternion();

        GzFactoryMsg msg = new GzFactoryMsg(this.avatar_name, avatar_model_name, new PointMsg(spawn_pos.x, spawn_pos.y, spawn_pos.z), new QuaternionMsg(spawn_rot.x, spawn_rot.y, spawn_rot.z, spawn_rot.w));

        GzBridgeService.Instance.gzbridge.Publish(GzFactoryPublisher.GetMessageTopic(), msg);
    }

    private IEnumerator TestAvatarRaymanMovement()
    {


        yield return null;
    }
}
