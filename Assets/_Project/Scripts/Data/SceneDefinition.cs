using UnityEngine;
using AppManagement;

public abstract class SceneDefinition : ScriptableObject
{
    [HideInInspector] public string id;

    public string sceneName;
    public string displayNameKey;
    public abstract SceneType SceneType { get; }

    private void OnValidate()
    {
        // Generate a unique ID if it is empty
        if (string.IsNullOrEmpty(id))
        {
            id = System.Guid.NewGuid().ToString();
        }
    }
}
