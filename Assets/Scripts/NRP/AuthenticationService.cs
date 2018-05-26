using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthenticationService : Singleton<AuthenticationService> {

    public string auth_username = "nrpuser";
    public string auth_password = "password";

    private string auth_token = null;
    private BackendConfigService backend = null;

    public string token
    {
        get { return auth_token; }
    }

    // Use this for initialization
    void Start()
    {
        backend = BackendConfigService.Instance;
        if (!string.IsNullOrEmpty(backend.IP))
        {
            StartCoroutine(Authenticate());
        }
    }
	
	// Update is called once per frame
	void Update () {

    }

    IEnumerator Authenticate()
    {
        Debug.Log("AuthenticationService - authenticating ...");
        string auth_url = string.Format("http://{0}:{1}/proxy/authentication/authenticate", backend.IP, backend.ProxyPort);
        string auth_json = string.Format("{{\"user\": \"{0}\", \"password\": \"{1}\"}}", this.auth_username, this.auth_password);

        WWW www;
        Dictionary<string, string> post_header = new Dictionary<string, string>();
        post_header.Add("Content-Type", "application/json");
        var post_data = System.Text.Encoding.UTF8.GetBytes(auth_json);
        www = new WWW(auth_url, post_data, post_header);
        yield return www;

        if (www.error != null)
        {
            Debug.LogWarning("There was an error sending request: " + www.error);
        }
        else
        {
            Debug.Log("AuthenticationService - auth token: " + www.text);
            this.auth_token = www.text;
        }
    }
}
