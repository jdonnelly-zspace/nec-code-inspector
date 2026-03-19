using UnityEngine;
using AppManagement;

[CreateAssetMenu(fileName = "LocationDefinition", menuName = "LocationDefinition")]
public class LocationDefinition : SceneDefinition
{
    public override SceneType SceneType => SceneType.Location;
}
