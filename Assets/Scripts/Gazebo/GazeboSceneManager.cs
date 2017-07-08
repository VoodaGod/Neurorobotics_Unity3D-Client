using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEditor;

public class GazeboSceneManager : MonoBehaviour {

    public enum GZ_LIGHT_TYPE
    {
        POINT = 1,
        SPOT = 2,
        DIRECTIONAL = 3,
        UNKNOWN = 4
    }

    public string ModelBasePath = "Assets/Models/NRPModelDatabase";
    public Material CollisionMaterial = null;

    private string scene_name_ = null;

	// Use this for initialization
	void Start () {
        Camera.main.transform.position = new Vector3(6, 3, 6);
        Camera.main.transform.LookAt(new Vector3());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool BuildScene(JSONNode sceneMsg)
    {
        // clear scene before building new one
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in transform)
            children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));

        // name
        if (sceneMsg["name"] != null)
            scene_name_ = sceneMsg["name"];

        // ambient color
        JSONClass ambient = sceneMsg["ambient"].AsObject;
        if (ambient != null)
        {
            Color color_ambient = new Color(ambient["r"].AsFloat, ambient["g"].AsFloat, ambient["b"].AsFloat, ambient["a"].AsFloat);
            RenderSettings.ambientLight = color_ambient;
        }

        // background color
        JSONClass background = sceneMsg["background"].AsObject;
        if (ambient != null)
        {
            Color color_background = new Color(background["r"].AsFloat, background["g"].AsFloat, background["b"].AsFloat, background["a"].AsFloat);
            Camera.main.backgroundColor = color_background;
        }

        // lights
        GameObject lights_parent = new GameObject("lights");
        lights_parent.transform.parent = this.gameObject.transform;

        JSONArray lights = sceneMsg["light"].AsArray;
        foreach(JSONNode light in lights)
        {
            this.CreateLightFromJSON(light, lights_parent.transform);
        }

        // models
        GameObject models_parent = new GameObject("models");
        models_parent.transform.parent = this.gameObject.transform;

        JSONArray models = sceneMsg["model"].AsArray;
        foreach (JSONNode model in models)
        {
            this.CreateModelFromJSON(model, models_parent.transform);
        }

        // joints
        GameObject joints_parent = new GameObject("joints");
        joints_parent.transform.parent = this.gameObject.transform;

        JSONArray joints = sceneMsg["joint"].AsArray;
        foreach (JSONNode joint in joints)
        {
            this.CreateJointFromJSON(joint, joints_parent.transform);
        }

        return true;
    }

    #region CREATE_SCENE_ELEMENTS

    private void CreateLightFromJSON(JSONNode json_light, Transform parent_transform)
    {
        string light_name = json_light["name"];
        GameObject light_gameobject = new GameObject(light_name);
        light_gameobject.transform.parent = parent_transform;
        Light light_component = light_gameobject.AddComponent<Light>();

        // pose
        JSONNode json_pose = json_light["pose"];
        if (json_pose != null)
        {
            this.SetPoseFromJSON(json_pose, light_gameobject);
        }

        // color
        JSONNode json_diffuse = json_pose["diffuse"];
        if (json_diffuse != null)
        {
            Color color_diffuse = new Color(json_diffuse["r"].AsFloat, json_diffuse["g"].AsFloat, json_diffuse["b"].AsFloat, json_diffuse["a"].AsFloat);
            light_component.color = color_diffuse;
        }
        //TODO: what to do with specular part of gazebo light?
        
        // light type
        JSONNode light_type = json_light["type"];
        if (light_type != null)
        {
            if (light_type.AsInt == (int)GZ_LIGHT_TYPE.POINT)
            {
                light_component.type = LightType.Point;
                light_component.range = json_light["range"].AsFloat;
            }
            else if (light_type.AsInt == (int)GZ_LIGHT_TYPE.SPOT)
            {
                light_component.type = LightType.Spot;
                light_component.range = json_light["range"].AsFloat;
                light_component.spotAngle = Mathf.Rad2Deg * json_light["spot_outer_angle"].AsFloat;

                JSONNode json_direction = json_light["direction"];
                Vector3 direction = new Vector3(json_direction["x"].AsFloat, json_direction["y"].AsFloat, json_direction["z"].AsFloat);
                light_gameobject.transform.LookAt(light_gameobject.transform.position + Gz2UnityVec3(direction));
            }
            else if (light_type.AsInt == (int)GZ_LIGHT_TYPE.DIRECTIONAL)
            {
                light_component.type = LightType.Directional;

                JSONNode json_direction = json_light["direction"];
                Vector3 direction = new Vector3(json_direction["x"].AsFloat, json_direction["y"].AsFloat, json_direction["z"].AsFloat);
                light_gameobject.transform.LookAt(light_gameobject.transform.position + Gz2UnityVec3(direction));
            }
        }

        // intensity (from gazebo attenuation factors)
        // equation taken from https://docs.blender.org/manual/en/dev/render/blender_render/lighting/lights/light_attenuation.html
        if (json_light["attenuation_linear"] != null && json_light["attenuation_quadratic"] != null)
        {
            float E = 1.0f;
            float D = 1.0f;
            float r = 1.0f;
            float L = json_light["attenuation_linear"].AsFloat;
            float Q = json_light["attenuation_quadratic"].AsFloat;

            float intensity = E * (D / (D + L * r)) * (Mathf.Pow(D, 2.0f) / (Mathf.Pow(D, 2.0f) + Q * Mathf.Pow(r, 2.0f)));
            light_component.intensity = intensity;
        }

        // shadows
        bool cast_shadows = json_light["cast_shadows"].AsBool;
        if (cast_shadows)
        {
            light_component.shadows = LightShadows.Soft;
        }
        else
        {
            light_component.shadows = LightShadows.None;
        }
    }

    private void CreateModelFromJSON(JSONNode json_model, Transform parent_transform)
    {
        string model_name = json_model["name"];
        GameObject model_gameobject = new GameObject(model_name);
        model_gameobject.transform.SetParent(parent_transform, false);

        // pose
        JSONNode json_pose = json_model["pose"];
        if (json_pose != null)
        {
            this.SetPoseFromJSON(json_pose, model_gameobject);
        }

        // scale
        JSONNode json_model_scale = json_model["scale"];
        model_gameobject.transform.localScale = new Vector3(json_model_scale["x"].AsFloat, json_model_scale["z"].AsFloat, json_model_scale["y"].AsFloat);

        // links
        GameObject links_parent = new GameObject("links");
        links_parent.transform.SetParent(model_gameobject.transform, false);

        JSONArray links = json_model["link"].AsArray;
        foreach (JSONNode link in links)
        {
            this.CreateLinkFromJSON(link, links_parent.transform, json_model_scale);
        }
    }

    private void CreateLinkFromJSON(JSONNode json_link, Transform parent_transform, JSONNode json_model_scale)
    {
        string link_name = json_link["name"];
        GameObject link_gameobject = new GameObject(link_name);
        link_gameobject.transform.SetParent(parent_transform, false);

        // pose
        JSONNode json_pose = json_link["pose"];
        if (json_pose != null)
        {
            this.SetPoseFromJSON(json_pose, link_gameobject);
        }

        // visuals
        JSONArray visuals = json_link["visual"].AsArray;
        if (visuals.Count > 0)
        {
            GameObject visuals_parent = new GameObject("visuals");
            visuals_parent.transform.SetParent(link_gameobject.transform, false);
            foreach (JSONNode visual in visuals)
            {
                this.CreateVisualFromJSON(visual, visuals_parent.transform, json_model_scale);
            }
        }

        // collisions
        JSONArray collisions = json_link["collision"].AsArray;
        if (collisions.Count > 0)
        {
            GameObject collisions_parent = new GameObject("collisions");
            collisions_parent.transform.SetParent(link_gameobject.transform, false);
            foreach (JSONNode collision in collisions)
            {
                this.CreateCollisionFromJSON(collision, collisions_parent.transform, json_model_scale);
            }
        }

        // sensors
        JSONArray sensors = json_link["sensor"].AsArray;
        if (sensors.Count > 0)
        {
            GameObject sensors_parent = new GameObject("sensors");
            sensors_parent.transform.SetParent(link_gameobject.transform, false);
            foreach (JSONNode sensor in sensors)
            {
                this.CreateSensorFromJSON(sensors, sensors_parent.transform);
            }
        }
    }

    private GameObject CreateVisualFromJSON(JSONNode json_visual, Transform parent_transform, JSONNode json_model_scale)
    {
        GameObject visual_gameobject = null;
        JSONNode json_geometry = json_visual["geometry"];
        if (json_geometry != null)
        {
            string visual_name = json_visual["name"];
            visual_gameobject = new GameObject(visual_name);
            visual_gameobject.transform.SetParent(parent_transform, false);

            // pose
            JSONNode json_pose = json_visual["pose"];
            if (json_pose != null)
            {
                this.SetPoseFromJSON(json_pose, visual_gameobject);
            }

            // geometry
            JSONNode json_material = json_visual["material"];
            this.CreateGeometryFromJSON(json_geometry, json_material, visual_gameobject.transform, json_model_scale);

            // cast shadows
            bool json_cast_shadows = json_visual["cast_shadows"].AsBool;
            if (json_cast_shadows)
            {
                
            }

            // receive shadows

        }
        return visual_gameobject;
    }

    private GameObject CreateGeometryFromJSON(JSONNode json_geometry, JSONNode json_material, Transform parent_transform, JSONNode json_model_scale)
    {
        GameObject geometry_gameobject = null;
        if (json_geometry["box"] != null)
        {
            geometry_gameobject = GameObject.CreatePrimitive(PrimitiveType.Cube);

            JSONNode json_size = json_geometry["box"].AsObject["size"];
            geometry_gameobject.transform.localScale = new Vector3(json_size["x"].AsFloat / json_model_scale["x"].AsFloat, 
                json_size["z"].AsFloat / json_model_scale["z"].AsFloat, 
                json_size["y"].AsFloat / json_model_scale["y"].AsFloat);
        }
        else if (json_geometry["cylinder"] != null)
        {
            geometry_gameobject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            float radius = json_geometry["cylinder"].AsObject["radius"].AsFloat;
            float length = json_geometry["cylinder"].AsObject["length"].AsFloat;

            // rescale (unity standard sizes differ from gazebo)
            geometry_gameobject.transform.localScale = new Vector3(radius * 2, length / 2, radius * 2);
        }
        else if (json_geometry["sphere"] != null)
        {
            geometry_gameobject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        }
        else if (json_geometry["plane"] != null)
        {
            geometry_gameobject = GameObject.CreatePrimitive(PrimitiveType.Plane);

            JSONNode json_normal = json_geometry["plane"].AsObject["normal"];
            geometry_gameobject.transform.up = new Vector3(json_normal["x"].AsFloat, json_normal["z"].AsFloat, json_normal["y"].AsFloat);
            
            JSONNode json_size = json_geometry["plane"].AsObject["size"];
            geometry_gameobject.transform.localScale = new Vector3(json_size["x"].AsFloat, 1.0f, json_size["y"].AsFloat);
        }
        else if (json_geometry["mesh"] != null)
        {
            this.LoadMeshFromJSON(json_geometry["mesh"], parent_transform, json_model_scale);
        }

        if (geometry_gameobject != null)
        {
            geometry_gameobject.transform.SetParent(parent_transform, false);
            // don't need collisions
            Destroy(geometry_gameobject.GetComponent<Collider>());
        }

        return geometry_gameobject;
    }

    private void LoadMeshFromJSON(JSONNode json_mesh, Transform parent_transform, JSONNode json_model_scale)
    {
        string json_mesh_uri = json_mesh["filename"];
        //Debug.Log("json mesh uri: " + json_mesh_uri);

        string mesh_uri_type = json_mesh_uri.Substring(0, json_mesh_uri.IndexOf("://"));
        //Debug.Log("json mesh uri type: " + mesh_uri_type);

        // need "file" or "model" to load
        if (mesh_uri_type != "file" && mesh_uri_type != "model")
            return;

        string mesh_subpath = json_mesh_uri.Substring(json_mesh_uri.IndexOf("://") + 3);
        string mesh_uri = this.ModelBasePath + "/" + mesh_subpath;
        //Debug.Log("mesh uri: " + mesh_uri);


        GameObject mesh_prefab = null;
        // import Mesh
        mesh_prefab = (GameObject)AssetDatabase.LoadAssetAtPath(mesh_uri, typeof(Object));
        //StartCoroutine(importModelCoroutine(path, (result) => { meshPrefab = result; }));
        if (mesh_prefab == null)
            Debug.Log("Could not import model! (" + mesh_uri + ")");

        if (mesh_prefab != null)
        {
            GameObject mesh_gameobject = Instantiate(mesh_prefab);
            mesh_gameobject.transform.SetParent(parent_transform, false);
        }
    }

    private void CreateCollisionFromJSON(JSONNode json_collision, Transform parent_transform, JSONNode json_model_scale)
    {
        // visuals
        GameObject visuals_parent = new GameObject("visuals");
        visuals_parent.transform.SetParent(parent_transform, false);

        JSONArray visuals = json_collision["visual"].AsArray;
        foreach (JSONNode visual in visuals)
        {
            GameObject visual_gameobject = this.CreateVisualFromJSON(visual, visuals_parent.transform, json_model_scale);
            if (visual_gameobject != null && visual_gameobject.GetComponentInChildren<Renderer>() != null)
            {
                visual_gameobject.GetComponentInChildren<Renderer>().material = this.CollisionMaterial;
                // deactivate collision visuals by default
                visual_gameobject.GetComponentInChildren<Renderer>().enabled = false;
            }
        }
    }

    private void CreateSensorFromJSON(JSONNode json_sensor, Transform parent_transform)
    {

    }

    private void CreateJointFromJSON(JSONNode json_joint, Transform parent_transform)
    {

    }

    #endregion //CREATE_SCENE_ELEMENTS

    #region HELPER_FUNCTIONS

    private void SetPoseFromJSON(JSONNode json_pose, GameObject gameobject)
    {
        // position
        JSONNode json_position = json_pose["position"];
        Vector3 position = new Vector3(json_position["x"].AsFloat, json_position["y"].AsFloat, json_position["z"].AsFloat);
        gameobject.transform.localPosition = Gz2UnityVec3(position);
        // rotation
        JSONNode json_rotation = json_pose["orientation"];
        Quaternion rotation = new Quaternion(json_rotation["x"].AsFloat, json_rotation["y"].AsFloat, json_rotation["z"].AsFloat, json_rotation["w"].AsFloat);
        gameobject.transform.localRotation = Gz2UnityQuaternion(rotation);
    }

    #region Convert function from gazebo to unity and vice versa.
    /// <summary>
    /// Converts a quaternion in gazebo coordinate frame to unity coordinate frame.
    /// </summary>
    /// <param name="gazeboRot">Quaternion in gazebo coordinate frame.</param>
    /// <returns>Quaternion in unity coordinate frame.</returns>
    private Quaternion Gz2UnityQuaternion(Quaternion gazeboRot)
    {
        Quaternion rotX = Quaternion.AngleAxis(180f, Vector3.right);
        Quaternion rotZ = Quaternion.AngleAxis(180f, Vector3.forward);

        Quaternion tempRot = new Quaternion(-gazeboRot.x, -gazeboRot.z, -gazeboRot.y, gazeboRot.w);

        Quaternion finalRot = tempRot * rotZ * rotX;

        return finalRot;
    }

    /// <summary>
    /// Converts a vector in gazebo coordinate frame to unity coordinate frame.
    /// </summary>
    /// <param name="gazeboPos">Vector in gazebo coordinate frame.</param>
    /// <returns>Vector in unity coordinate frame.</returns>
    private Vector3 Gz2UnityVec3(Vector3 gazeboPos)
    {
        return new Vector3(-gazeboPos.x, gazeboPos.z, -gazeboPos.y);
    }

    /// <summary>
    /// Converts a vector in unity coordinate frame to gazebo coordinate frame.
    /// </summary>
    /// <param name="unityPos">Vector in unity coordinate frame.</param>
    /// <returns>Vector in gazebo coordinate frame.</returns>
    private Vector3 Unity2GzVec3(Vector3 unityPos)
    {
        return new Vector3(unityPos.x, unityPos.z, unityPos.y);
    }

    /// <summary>
    /// Converts a quaternion in unity coordinate frame to gazebo coordinate frame.
    /// </summary>
    /// <param name="unityRot">Quaternion in unity coordinate frame.</param>
    /// <returns>Quaternion in gazebo coordinate frame.</returns>
    private Quaternion Unity2GzQuaternion(Quaternion unityRot)
    {
        Quaternion rotX = Quaternion.AngleAxis(180f, Vector3.right);
        Quaternion rotZ = Quaternion.AngleAxis(180f, Vector3.forward);

        Quaternion tempRot = unityRot * rotX * rotZ;

        Quaternion finalRot = new Quaternion(-tempRot.x, -tempRot.z, -tempRot.y, tempRot.w);

        return finalRot;
    }
    #endregion

    #endregion //HELPER_FUNCTIONS
}
