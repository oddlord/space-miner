using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace SpaceMiner
{
    // TODO this class is too big, split into multiple sub-components
    public class LevelController : MonoBehaviour
    {
        [Serializable]
        private struct _InternalSetup
        {
            public AudioSource AudioSource;
        }

        [Header("Level Parameters")]
        [SerializeField] private int _initialObstacleCount = 2;
        [SerializeField] private int _obstacleCountIncreasePerWave = 1;

        [Header("Services")]
        [SerializeField] private GameOverScreen _gameOverScreen;
        [SerializeField] private WaveTextController _waveTextController;
        [SerializeField] private ActorSelector _actorSelector;

        [Header("__Internal Setup__")]
        [SerializeField] private _InternalSetup _internalSetup;

        private int _wave;
        private IActor _playerActor;

        private IActorController _actorController;
        private IActor.Factory _actorFactory;
        private IObstacleManager _obstacleManager;
        private IObstacleSpawner _obstacleSpawner;
        private ObservableInt _score;
        private LivesDisplay _livesDisplay;

        [Inject]
        public void Init(
            IActorController actorController, IActor.Factory actorFactory,
            IObstacleManager obstacleManager, IObstacleSpawner obstacleSpawner,
            ObservableInt score, LivesDisplay livesDisplay
        )
        {
            _actorController = actorController;
            _actorFactory = actorFactory;
            _obstacleManager = obstacleManager;
            _obstacleSpawner = obstacleSpawner;
            _score = score;
            _livesDisplay = livesDisplay;
        }

        void Awake()
        {
            _wave = 0;
            _score.Value = 0;

            _obstacleManager.OnAllObstaclesDestroyed += OnAllObstaclesDestroyed;
            _obstacleManager.OnObstacleDestroyed += OnObstacleDestroyed;
            _gameOverScreen.OnPlayAgain += OnPlayAgain;
            _gameOverScreen.OnBack += OnBack;
        }

        void Start()
        {
            _actorSelector.Show(OnActorSelected);
        }

        private void OnActorSelected(IActor actorPrefab)
        {
            _playerActor = _actorFactory.Create(actorPrefab.GetObject());
            _playerActor.GetObject().transform.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 0, 90));
            _playerActor.OnDeath += OnPlayerDeath;

            _actorController.SetActor(_playerActor);
            _livesDisplay.Init(_playerActor.Lives, _playerActor.MaxLives);

            _internalSetup.AudioSource.Play();
            _actorSelector.Hide();
            StartNextWave();
        }

        private void StartNextWave()
        {
            _wave++;
            int obstaclesCount = _initialObstacleCount + _obstacleCountIncreasePerWave * (_wave - 1);
            _obstacleSpawner.SpawnWave(obstaclesCount);
            _waveTextController.Show(_wave);
        }

        private void OnAllObstaclesDestroyed()
        {
            StartNextWave();
        }

        private void OnObstacleDestroyed(Obstacle obstacle)
        {
            _score.Value += obstacle.PointsWorth;
        }

        private void OnPlayAgain()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void OnBack()
        {
            SceneManager.LoadScene(Scenes.START_SCREEN);
        }

        private void OnPlayerDeath(IActor actor)
        {
            _gameOverScreen.Show();
        }

        void OnDestroy()
        {
            _obstacleManager.OnAllObstaclesDestroyed -= OnAllObstaclesDestroyed;
            _obstacleManager.OnObstacleDestroyed -= OnObstacleDestroyed;
            _gameOverScreen.OnPlayAgain -= OnPlayAgain;
            _gameOverScreen.OnBack -= OnBack;
            if (_playerActor != null) _playerActor.OnDeath -= OnPlayerDeath;
        }
    }
}