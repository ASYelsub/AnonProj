using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract MonoBehaviour base which calls a function when all scenes are loaded.
/// Simply make your object inherit from CalledOnScenesLoadedBase,
/// and your implementation of the OnScenesLoaded function will be called when all scenes are loaded.
/// </summary>

public abstract class CalledOnScenesLoadedBase : MonoBehaviour
{
    private void Awake()
    {
        Services.EventManager.Register<ScenesLoaded>(OnScenesLoaded);
    }

    private void OnDestroy()
    {
        Services.EventManager.Unregister<ScenesLoaded>(OnScenesLoaded);
    }

    protected abstract void OnScenesLoaded(NEvent e);
    
}
