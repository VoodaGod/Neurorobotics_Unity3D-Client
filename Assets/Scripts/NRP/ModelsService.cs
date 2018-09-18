using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class ModelsLibrary
{
    public ModelCategory[] categories;
}

[System.Serializable]
public class ModelCategory
{
    public string title;
    public string thumbnail;
    public Model[] models;
}

[System.Serializable]
public class Model
{
    public string modelPath;
    public string modelTitle;
    public string thumbnail;
}

public class ModelsService : Singleton<ModelsService>
{
    private static string nrp_models_library_subpath = "/libraries/model_library.json";

    public string nrp_models_subdirectory = "Models/NRP";

    private ModelsLibrary models_library_ = null;
    private Dictionary<string, string> dict_model_subpaths_ = new Dictionary<string, string>();

    void Start () {
        this.ReadModelsLibraryJSON();
        this.InitDictionaryModelSubpaths();
    }

    public string GetModelSubpath(string model_name)
    {
        return this.dict_model_subpaths_[model_name];
    }

    #region HELPER_FUNCTIONS

    private void ReadModelsLibraryJSON()
    {
        string model_lib_file_path = Application.dataPath + "/" + this.nrp_models_subdirectory + nrp_models_library_subpath;
        string dataAsJson = File.ReadAllText(model_lib_file_path);
        dataAsJson = "{\"categories\":" + dataAsJson + "}";
        models_library_ = JsonUtility.FromJson<ModelsLibrary>(dataAsJson);
    }

    private void InitDictionaryModelSubpaths()
    {
        foreach(ModelCategory category in this.models_library_.categories)
        {
            foreach(Model model in category.models)
            {
                this.dict_model_subpaths_.Add(model.modelTitle, model.modelPath);
            }
        }
    }

    #endregion //HELPER_FUNCTIONS
}
