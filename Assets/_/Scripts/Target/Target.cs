using System;
using UnityEngine;
using Zenject;

namespace SpaceMiner
{
    public class Target : MonoBehaviour
    {
        public class Factory : PlaceholderFactory<UnityEngine.Object, Target> { }

        public bool Active = true;

        public int PointsWorth;
        public Hittable Hittable;

        public Action<Target> OnDestroyed;

        public void SetActive(bool active)
        {
            Active = active;
        }

        public virtual void SetTag(string tag)
        {
            gameObject.tag = tag;
        }

        public void SetDestroyed()
        {
            if (!Active) return;
            OnDestroyed?.Invoke(this);
        }
    }
}
