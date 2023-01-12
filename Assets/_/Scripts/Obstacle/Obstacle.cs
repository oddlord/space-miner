using System;
using UnityEngine;
using Zenject;

namespace SpaceMiner
{
    public interface Obstacle
    {
        public class Factory : PlaceholderFactory<UnityEngine.Object, Obstacle> { }

        public int PointsWorth { get; set; }

        public Action<Obstacle> OnDestroyed { get; set; }

        // This ensures that any implementation is a MonoBehaviour
        public GameObject GetObject();
    }
}
