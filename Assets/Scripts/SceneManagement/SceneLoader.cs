using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.SceneManagement;


/// <summary>
/// Spawns a list of scenes.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    // Static instance of the object.
    public static SceneLoader Instance = null;
    
    [Header("Add scenes to list. They will not all be loaded at the same time.")]
    [SerializeField] private List<Utilities.SceneField> scenes;
    [SerializeField] private bool mergeScenesAtStart = false;
    public bool IsSceneLoaded { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        
        Services.InitializeServices();
        
        StartCoroutine(LoadScenes());
    }

    private IEnumerator LoadScenes()
    {
        Scene startScene = SceneManager.GetActiveScene();
        
        foreach (Utilities.SceneField sceneField in scenes)
        {
            Debug.Log($"Loading scene {sceneField.SceneName}...");
            
            // Searches currently loaded scenes for this scene, if it is valid, then it is already loaded.
            if (SceneManager.GetSceneByName(sceneField.SceneName).IsValid())
            {
                Debug.LogError($"Cannot load scene {sceneField.SceneName}, it is already loaded.");
                continue;
            }

            AsyncOperation loadingLevel = SceneManager.LoadSceneAsync(sceneField.SceneName, LoadSceneMode.Additive);

            while (loadingLevel.progress < .9f)
            {
                float prog = Mathf.Clamp01(loadingLevel.progress / .9f);
                // set text on loading bar here if needed.
                yield return null;
            }
            
            if (mergeScenesAtStart)
            {
                while (!loadingLevel.isDone)
                {
                    yield return null;
                }

                Scene asSceneObject = SceneManager.GetSceneByName(sceneField.SceneName);

                SceneManager.MergeScenes(asSceneObject, startScene);
            }
        }
        
        Debug.Log($"All scenes loaded :)");
        IsSceneLoaded = true;
        
        Services.EventManager.Fire(new ScenesLoaded());
        
        // Can put event here to notify all objects that the scene is loaded.
    }
}

public interface ICallOnScenesLoaded : IEventSystemHandler
{
    // functions that can be called via the messaging system
    void OnScenesLoaded();
}
 
 namespace Utilities
 {
     [System.Serializable]
     public class SceneField
     {
         [SerializeField] private Object sceneAsset;
         [SerializeField] private string sceneName = "";
 
         public string SceneName
         {
             get { return sceneName; }
         }
 
         // makes it work with the existing Unity methods (LoadLevel/LoadScene)
         public static implicit operator string(SceneField sceneField)
         {
             return sceneField.SceneName;
         }
     }
 
 #if UNITY_EDITOR
     [CustomPropertyDrawer(typeof(SceneField))]
     public class SceneFieldPropertyDrawer : PropertyDrawer
     {
         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
         {
             EditorGUI.BeginProperty(position, GUIContent.none, property);
             var sceneAsset = property.FindPropertyRelative("sceneAsset");
             var sceneName = property.FindPropertyRelative("sceneName");
             position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
             if (sceneAsset != null)
             {
                 EditorGUI.BeginChangeCheck();
                 var value = EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);
                 if (EditorGUI.EndChangeCheck())
                 {
                     sceneAsset.objectReferenceValue = value;
                     if (sceneAsset.objectReferenceValue != null)
                     {
                         var scenePath = AssetDatabase.GetAssetPath(sceneAsset.objectReferenceValue);
                         var assetsIndex = scenePath.IndexOf("Assets", StringComparison.Ordinal) + 7;
                         var extensionIndex = scenePath.LastIndexOf(".unity", StringComparison.Ordinal);
                         scenePath = scenePath.Substring(assetsIndex, extensionIndex - assetsIndex);
                         sceneName.stringValue = scenePath;
                     }
                 }
             }
             EditorGUI.EndProperty();
         }
     }
 #endif
 }