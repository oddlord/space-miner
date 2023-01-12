using UnityEngine;
using System;
using System.Collections;
using Zenject;
using Oddlord.RequireInterface;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpaceMiner
{
    public class Asteroid : MonoBehaviour, IObstacle
    {
        [Serializable]
        private struct _InternalSetup
        {
            public SpriteRenderer SpriteRenderer;
            public Collider2D Collider;
            public AudioSource AudioSource;
        }

        public int PointsWorth { get; set; }

        [Header("Audio")]
        [SerializeField] private AudioClip _destructionSound;

        [Header("__Internal Setup__")]
        [SerializeField] private _InternalSetup _internalSetup;

        [Header("Asteroid Configuration")]
        [HideInInspector] public bool SplitOnHit;
        [RequireInterface(typeof(IObstacle))]
        [HideInInspector] public UnityEngine.Object FragmentPrefab;
        private IObstacle _iFragmentPrefab => FragmentPrefab as IObstacle;
        [HideInInspector] public int FragmentsToSpawn = 2;

        public Action<IObstacle> OnDestroyed { get; set; }

        private IObstacleSpawner _obstacleSpawner;

        [Inject]
        public void Init(IObstacleSpawner obstacleSpawner)
        {
            _obstacleSpawner = obstacleSpawner;
        }

        public GameObject GetGO() => gameObject;

        private void OnHit()
        {
            if (SplitOnHit)
            {
                for (int i = 0; i < FragmentsToSpawn; i++)
                    _obstacleSpawner.SpawnObstacle(_iFragmentPrefab, transform.position);
            }

            PlayAudio(_destructionSound);
            SetColliderEnabled(false);
            SetSpriteAlpha(0);
            StartCoroutine(DestructionCoroutine(_destructionSound.length));
            OnDestroyed?.Invoke(this);
        }

        private IEnumerator DestructionCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(this.gameObject);
        }

        private void SetColliderEnabled(bool enabled)
        {
            _internalSetup.Collider.enabled = enabled;
        }

        private void SetSpriteAlpha(float alpha)
        {
            Color color = _internalSetup.SpriteRenderer.color;
            color.a = alpha;
            _internalSetup.SpriteRenderer.color = color;
        }

        private void PlayAudio(AudioClip clip)
        {
            _internalSetup.AudioSource.clip = clip;
            _internalSetup.AudioSource.Play();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            bool hitByProjectile = other.CompareTag(Tags.PROJECTILE);
            bool hitByActor = other.CompareTag(Tags.ACTOR);
            if (hitByProjectile || hitByActor) OnHit();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Asteroid))]
    public class Asteroid_Editor : Editor
    {
        private Asteroid _asteroid;

        private SerializedProperty _splitOnHit;
        private SerializedProperty _fragmentPrefab;
        private SerializedProperty _fragmentsToSpawn;

        void OnEnable()
        {
            _asteroid = (Asteroid)target;

            _splitOnHit = this.serializedObject.FindProperty("SplitOnHit");
            _fragmentPrefab = this.serializedObject.FindProperty("FragmentPrefab");
            _fragmentsToSpawn = this.serializedObject.FindProperty("FragmentsToSpawn");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            serializedObject.Update();

            EditorGUILayout.LabelField("Asteroid", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_splitOnHit);
            if (_splitOnHit.boolValue)
            {
                EditorGUILayout.PropertyField(_fragmentPrefab);
                EditorGUILayout.PropertyField(_fragmentsToSpawn);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
