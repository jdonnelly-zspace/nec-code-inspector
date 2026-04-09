using UnityEngine;
using System.Collections.Generic;
using AppManagement;

[CreateAssetMenu(fileName = "LocationCatalog", menuName = "LocationCatalog")]
public class LocationCatalog : ScriptableObject
{
    public List<LocationDefinition> locations;

    public LocationDefinition GetLocationBySceneName(string sceneName)
    {
        return locations.Find(location => location.sceneName == sceneName);
    }
}
