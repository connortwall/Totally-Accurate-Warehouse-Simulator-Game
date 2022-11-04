using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class SaveLoadManager
{
    private SavedDataScriptableObject _savedData;
    
    private const string _scriptableObjectsFolderPath = "Assets/ScriptableObjects/";
    private const string _scriptableObjectDefaultName = "SavedData";
    private const string _scritableObjectExtension = ".asset";

    /* 
    * #TODO: SAVE/LOAD
    * fix any potential compilation errors
    */

    // #region SINGLETON

    // private static readonly Lazy<SaveLoadManager> _instance =
    //     new Lazy<SaveLoadManager>(() => new SaveLoadManager());

    // public static SaveLoadManager Instance { get { return _instance.Value; } }

    // #endregion

    
    // #region PUBLIC INTERFACES

    // public void Awake()
    // {
    //     string dataPath = _scriptableObjectsFolderPath + _scriptableObjectDefaultName + _scritableObjectExtension;
    //     System.Object asset;
        
    //     if (TryLoadAssetAtPath(dataPath, out asset))
    //     {
    //         _savedData = asset as SavedDataScriptableObject;
    //     }
    //     else
    //     {
    //         _savedData = ScriptableObject.CreateInstance<SavedDataScriptableObject>();
    //         AssetDatabase.CreateAsset(_savedData, dataPath);
    //     }
    // }

    // public void Save(SavedData data)
    // {
    //     _savedData.Data = data;
    // }

    // public SavedData Load()
    // {
    //     return _savedData.Data;
    // }

    // #endregion
    

    // #region PRIVATE HELPERS

    // private SaveLoadManager()
    // {
        
    // }
    
    // private bool TryLoadAssetAtPath(string path, out System.Object asset)
    // {
    //     asset = AssetDatabase.LoadAssetAtPath(path, typeof(SavedDataScriptableObject)) as SavedDataScriptableObject;
    //     return asset switch
    //     {
    //         null => false,
    //         _ => true
    //     };
    // }

    // #endregion
}
