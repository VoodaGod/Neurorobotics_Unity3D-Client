﻿using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class MeshUpdater : MonoBehaviour {

    /// <summary>
    /// State enum to track the current state of the mesh updater
    /// </summary>
    public enum State
    {
        None = 0,
        Initialized = 1,
        BlenderPathSet = 2,
        Scanned = 3
    }

    public string Github_Repository = @"https://github.com/Roboy/roboy_models/";

    /// <summary>
    /// Path to blender.exe. Is set via the user via a file selection through the file explorer.
    /// </summary>
    [HideInInspector]
    public string PathToBlender {
        get { return m_PathToBlender; }
        set {
                m_PathToBlender =  value;
                m_CurrentState = (State) Mathf.Max((int)State.BlenderPathSet, (int)m_CurrentState);
        }
    }
    /// <summary>
    /// Public property of the URL Dic for the editor script
    /// </summary>
    public Dictionary<string, string> URLDictionary { get { return m_URLDictionary; } }

    /// <summary>
    /// Dictionary to store the users choice whether he wants to update the model or not
    /// </summary>
    public Dictionary<string, bool> ModelChoiceDictionary = new Dictionary<string, bool>();

    /// <summary>
    /// Public property for the editor script
    /// </summary>
    public State CurrentState { get { return m_CurrentState; } }

    /// <summary>
    /// Current state of the meshupdater
    /// </summary>
    private State m_CurrentState = State.None;

    /// <summary>
    /// Private variable for the blender path to encapsulate the get and set in a property instead of a function.
    /// </summary>
    private string m_PathToBlender;

    /// <summary>
    /// This should be the path to the "MeshDownloader". It is located in the ExternalTools directory.
    /// </summary>
    private string m_PathToDownloadScript;

    /// <summary>
    /// This should be the path to the "MeshScanner". It is located in the ExternalTools directory.
    /// </summary>
    private string m_PathToScanScript;

    /// <summary>
    /// Cached variable of the projects assets directory.
    /// </summary>
    private string m_ProjectFolder;

    /// <summary>
    /// Stores all model "Titles + URLs"
    /// </summary>
    private Dictionary<string, string> m_URLDictionary = new Dictionary<string, string>();

    // Use this for initialization
    void Awake () {
        Initialize();
    }

    /// <summary>
    /// Initializes the paths of the python scripts.
    /// </summary>
    public void Initialize()
    {
        m_ProjectFolder = Application.dataPath;
        m_PathToDownloadScript = m_ProjectFolder + @"/ExternalTools/ModelDownloader.py";
        m_PathToScanScript = m_ProjectFolder + @"/ExternalTools/ModelScanner.py";

        showWarnings();
    }

    /// <summary>
    /// Calls the python scan script through a commandline.
    /// </summary>
    public void Scan()
    {
        string[] scanArguments = { "python", m_PathToScanScript, Github_Repository };
        CommandlineUtility.RunCommandLine(scanArguments);
        // to do whether scan file exists and is right
        // check whether file exists
        string pathToScanFile = m_ProjectFolder + @"/tempModelURLs.txt";
        if (!File.Exists(pathToScanFile))
        {
            Debug.LogWarning("Scan file not found! Check whether it exists or if python script is working!");
            return;
        }
        // get file content of format title:url
        string[] scanContent = File.ReadAllLines(pathToScanFile);
        Dictionary<string, string> tempURLDic = new Dictionary<string, string>();
        foreach (var line in scanContent)
        {
            // split at ":"
            string[] titleURL = line.Split(';');
            // check if there is exactly one ";" meaning only two elements
            if (titleURL.Length != 2)
            {
                Debug.Log("In line:\n" + line + "\nthe format does not match title;URL");
                continue;
            }
            // ignore link if it is not in the github repo
            if (!titleURL[1].Contains(Github_Repository))
            {
                Debug.Log("Link does not have the github repository!");
                continue;
            }
            tempURLDic.Add(titleURL[0], titleURL[1]);
        }
        // clear all old links and add the new links
        m_URLDictionary.Clear();
        m_URLDictionary = tempURLDic;
        foreach (var urlDicEntry in m_URLDictionary)
        {
            ModelChoiceDictionary.Add(urlDicEntry.Key, false);
        }
        m_CurrentState = State.Scanned;
    }

    public void UpdateModels()
    {
        //var processInfo = new ProcessStartInfo("cmd.exe", "/C" + "start \"\" \"" + m_PathToBlender + "\" -P \"" + m_PathToDownloadScript + "\" \"" + pathToMeshes + "\" \"" + m_pathToProjectModels + "\" \"\"");
        UnityEngine.Debug.Log("Run not implemented yet!");
        // get a list of all entries which the user wants to update
        List<KeyValuePair<string, bool>> tempURLList = ModelChoiceDictionary.Where(entry => entry.Value == true).ToList();
        foreach (var urlEntry in tempURLList)
        {
            Debug.Log(m_URLDictionary[urlEntry.Key]);
        }
    }

    /// <summary>
    /// Shows warnings for each python script.
    /// </summary>
    private void showWarnings()
    {
        if (File.Exists(m_PathToDownloadScript))
        {
            Debug.Log("Download script found!");
        }
        else
        {
            Debug.LogWarning("Download script not found!");
        }

        if (File.Exists(m_PathToScanScript))
        {
            Debug.Log("Scan script found!");
        }
        else
        {
            Debug.LogWarning("Scan script not found!");
        }
    }
}