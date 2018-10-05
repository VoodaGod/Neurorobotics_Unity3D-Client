using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserAvatarVisuals : MonoBehaviour {

    public float opacity = 0.5f;

	// Use this for initialization
	void Start () {
        Renderer[] renderers = this.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                Color color = material.color;
                color.a = opacity;
                material.SetColor("_Color", color);
                this.SetRenderModeTransparent(material);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void SetRenderModeTransparent (Material material)
    {
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
    }
}
