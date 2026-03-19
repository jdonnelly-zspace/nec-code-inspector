using System;
using System.Collections;
using System.Collections.Generic;
using AppManagement;
using UnityEngine;
using UnityEngine.Localization;
using zSpace.Core.Utility;

public class ObjectHoverTextManager : ZSingleton<ObjectHoverTextManager>
{
    [SerializeField] private GameObject hoverTextPrefab;
    // private ObjectHoverText hoverTextInstance;
    private Dictionary<string, ObjectHoverText> hoverTextInstances = new Dictionary<string, ObjectHoverText>();

    protected override void Awake()
    {
        base.Awake();
        if (ObjectHoverTextManager.Instance == this)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public ObjectHoverText ShowText(LocalizedString stringReference, Vector3 offset, string key = null, float scale = .1f)
    {
        return ShowText(null, stringReference, offset, key, scale);
    }
    public ObjectHoverText ShowText(Transform parent, LocalizedString stringReference, string key = null, float scale = .1f)
    {
        return ShowText(parent, stringReference, Vector3.zero, key, scale);
    }
    public ObjectHoverText ShowText(Transform parent, LocalizedString stringReference, Vector3 offset, string key = null, float scale = .1f)
    {
        if (hoverTextPrefab == null)
        {
            AppLogger.LogError("Hover text prefab is not assigned.");
            return null;
        }

        // Clear any existing hover text before creating a new one
        if (!string.IsNullOrEmpty(key))
            ClearText(key);


        var hoverTextGameObject = Instantiate(hoverTextPrefab);
        var hoverTextInstance = hoverTextGameObject.GetComponent<ObjectHoverText>();

        if (hoverTextInstance == null)
        {
            AppLogger.LogError("ObjectHoverText component not found on the hover text prefab.");
            return null;
        }

        if (string.IsNullOrEmpty(key))
            key = hoverTextInstance.gameObject.GetInstanceID().ToString();

        // Store the instance in the dictionary if a key is provided
        hoverTextInstances[key] = hoverTextInstance;

        if (parent != null)
            hoverTextInstance.transform.SetParent(parent, true);
        hoverTextInstance.SetOffset(offset);
        hoverTextInstance.SetText(stringReference);

        return hoverTextInstance;
    }

    public void ClearText(string key)
    {
        if (hoverTextInstances.ContainsKey(key))
        {
            ClearText(hoverTextInstances[key]);
        }
    }

    public void ClearText(ObjectHoverText instance)
    {
        if (instance != null)
        {
            if (instance.gameObject != null) Destroy(instance.gameObject);
            foreach (var key in hoverTextInstances.Keys)
            {
                if (hoverTextInstances[key] == instance)
                {
                    hoverTextInstances.Remove(key);
                    break;
                }
            }
        }
    }

    public void ClearAllTexts()
    {
        foreach (var instance in hoverTextInstances.Values)
        {
            if (instance != null)
            {
                Destroy(instance.gameObject);
            }
        }
        hoverTextInstances.Clear();
    }
}
