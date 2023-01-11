using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace SpaceMiner
{
    public class ActorSelector : SerializedMonoBehaviour
    {
        [Serializable]
        private struct _InternalSetup
        {
            public Transform ActorEntriesContainer;
        }

        [SerializeField] private ActorEntry _actorEntryPrefab;
        [OdinSerialize] private List<IActor> _actors;

        [Header("__Internal Setup__")]
        [SerializeField] private _InternalSetup _internalSetup;

        private Action<IActor> _onSelected;

        public void Show(Action<IActor> onSelected)
        {
            gameObject.SetActive(true);
            _onSelected = onSelected;
            foreach (IActor actor in _actors)
            {
                ActorEntry shipEntry = Instantiate(_actorEntryPrefab, _internalSetup.ActorEntriesContainer);
                shipEntry.Initialize(actor.GetSprite(), () => OnActorSelected(actor));
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnActorSelected(IActor actorPrefab)
        {
            _onSelected(actorPrefab);
        }
    }
}