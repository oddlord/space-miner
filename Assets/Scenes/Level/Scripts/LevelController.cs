using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        [Header("States")]
        [SerializeField] private IntState _maxLivesState;
        [SerializeField] protected IntState _livesState;
        [SerializeField] private IntState _scoreState;

        [Header("Services")]
        [SerializeField] private ObstacleManager _obstacleManager;
        [SerializeField] private ObstacleWaveSpawner _obstacleWaveSpawner;
        [SerializeField] private GameOverScreen _gameOverScreen;
        [SerializeField] private WaveTextController _waveTextController;
        [SerializeField] private ActorSelector _actorSelector;
        [SerializeField] private ActorController _actorController;

        [Header("__Internal Setup__")]
        [SerializeField] private _InternalSetup _internalSetup;

        private int _wave;
        private Actor _playerActor;

        void Awake()
        {
            _wave = 0;
            _scoreState.Set(0);

            _obstacleManager.OnAllObstaclesDestroyed += OnAllObstaclesDestroyed;
            _obstacleManager.OnObstacleDestroyed += OnObstacleDestroyed;
            _gameOverScreen.OnPlayAgain += OnPlayAgain;
            _gameOverScreen.OnBack += OnBack;
        }

        void Start()
        {
            _actorSelector.Show(OnActorSelected);
        }

        private void OnActorSelected(Actor actorPrefab)
        {
            _playerActor = Instantiate(actorPrefab, Vector3.zero, Quaternion.Euler(0, 0, 90));
            _playerActor.Initialize(_maxLivesState, _livesState);
            _playerActor.OnDeath += OnPlayerDeath;
            _actorController.SetActor(_playerActor);

            _internalSetup.AudioSource.Play();
            _actorSelector.Hide();
            StartNextWave();
        }

        private void StartNextWave()
        {
            _wave++;
            int obstaclesCount = _initialObstacleCount + _obstacleCountIncreasePerWave * (_wave - 1);
            _obstacleWaveSpawner.Spawn(obstaclesCount);
            _waveTextController.Show(_wave);
        }

        private void OnAllObstaclesDestroyed()
        {
            StartNextWave();
        }

        private void OnObstacleDestroyed(Obstacle obstacle)
        {
            _scoreState.Add(obstacle.PointsWorth);
        }

        private void OnPlayAgain()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void OnBack()
        {
            SceneManager.LoadScene(Scenes.START_SCREEN);
        }

        private void OnPlayerDeath(Actor actor)
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