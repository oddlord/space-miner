using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace SpaceMiner
{
    [CreateAssetMenu(menuName = ScriptableObjects.MENU_PREFIX + "WaveObstacleCollection")]
    public class WaveObstacleCollection : SerializedScriptableObject
    {
        [OdinSerialize] public List<Obstacle> Prefabs;
    }
}
