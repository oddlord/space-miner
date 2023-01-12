using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SpaceMiner
{
    public class SimpleObstacleSpawner : IObstacleSpawner
    {
        public Action<Obstacle> OnObstacleSpawned { get; set; }

        private Obstacle.Factory _obstacleFactory;
        private WaveObstacleCollection _waveObstacleCollection;
        private SpawnPointsContainer _spawnPointsContainer;

        [Inject]
        public void Init(Obstacle.Factory obstacleFactory, WaveObstacleCollection waveObstacleCollection, SpawnPointsContainer spawnPointsContainer)
        {
            _obstacleFactory = obstacleFactory;
            _waveObstacleCollection = waveObstacleCollection;
            _spawnPointsContainer = spawnPointsContainer;
        }

        public Obstacle[] SpawnWave(int amount)
        {
            SpawnPoint[] spawnPoints = Utils.ShuffleArray(_spawnPointsContainer.SpawnPoints);

            Obstacle[] obstacles = new Obstacle[amount];
            for (int i = 0; i < amount; i++)
            {
                Obstacle obstaclePrefab = Utils.GetRandomListElement(_waveObstacleCollection.Prefabs);
                SpawnPoint spawnPoint = spawnPoints[i % spawnPoints.Length];
                Obstacle obstacle = SpawnObstacle(obstaclePrefab, spawnPoint.Position);
                obstacles[i] = obstacle;
            }

            return obstacles;
        }

        public Obstacle SpawnObstacle(Obstacle obstaclePrefab, Vector3 spawnPosition)
        {
            Quaternion spawnRotation = Utils.GetRandom2DRotation();
            Obstacle obstacle = _obstacleFactory.Create(obstaclePrefab.GetObject());
            obstacle.GetObject().transform.SetPositionAndRotation(spawnPosition, spawnRotation);
            OnObstacleSpawned?.Invoke(obstacle);
            return obstacle;
        }
    }
}