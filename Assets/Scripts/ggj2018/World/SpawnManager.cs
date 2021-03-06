﻿using System.Collections.Generic;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ggj2018.Data;
using pdxpartyparrot.ggj2018.Game;
using pdxpartyparrot.ggj2018.GameTypes;

using JetBrains.Annotations;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.World
{
    public sealed class SpawnManager : SingletonBehavior<SpawnManager>
    {
        private static readonly System.Random Random = new System.Random();

        private class SpawnPoints
        {
            private readonly GameTypeData _gameTypeData;

// TODO: dictionary would work better
            private readonly List<SpawnPoint> _predatorSpawnPoints = new List<SpawnPoint>();

            private readonly List<SpawnPoint> _usedPredatorSpawnPoints = new List<SpawnPoint>();

            private readonly List<SpawnPoint> _preySpawnPoints = new List<SpawnPoint>();

            private readonly List<SpawnPoint> _usedPreySpawnPoints = new List<SpawnPoint>();

            public SpawnPoints(GameTypeData gameTypeData)
            {
                _gameTypeData = gameTypeData;
            }

            public void Add(SpawnPoint spawnPoint)
            {
                switch(spawnPoint.Type)
                {
                case SpawnPoint.SpawnPointType.Predator:
                    _predatorSpawnPoints.Add(spawnPoint);
                    break;
                case SpawnPoint.SpawnPointType.Prey:
                    _preySpawnPoints.Add(spawnPoint);
                    break;
                default:
                    Debug.LogError($"TODO: support spawn point type {spawnPoint.Type}");
                    break;
                }
            }

            public void Remove(SpawnPoint spawnPoint)
            {
                switch(spawnPoint.Type)
                {
                case SpawnPoint.SpawnPointType.Predator:
                    _predatorSpawnPoints.Remove(spawnPoint);
                    _usedPredatorSpawnPoints.Remove(spawnPoint);
                    break;
                case SpawnPoint.SpawnPointType.Prey:
                    _preySpawnPoints.Remove(spawnPoint);
                    _usedPreySpawnPoints.Remove(spawnPoint);
                    break;
                default:
                    Debug.LogError($"TODO: support spawn point type {spawnPoint.Type}");
                    break;
                }
            }

            [CanBeNull]
            public SpawnPoint GetSpawnPoint(BirdTypeData birdType)
            {
                return birdType.IsPredator
                    ? GetPredatorSpawnPoint(GameManager.Instance.GameType)
                    : GetPreySpawnPoint(GameManager.Instance.GameType);
            }

            [CanBeNull]
            private SpawnPoint GetPredatorSpawnPoint(GameType gameType)
            {
                SpawnPoint spawnPoint = GetSpawnPoint(_predatorSpawnPoints, _usedPredatorSpawnPoints);
                if(null != spawnPoint) {
                    return spawnPoint;
                }
                return gameType.BirdTypesShareSpawnpoints ? GetSpawnPoint(_preySpawnPoints, _usedPreySpawnPoints) : null;
            }

            [CanBeNull]
            private SpawnPoint GetPreySpawnPoint(GameType gameType)
            {
                SpawnPoint spawnPoint = GetSpawnPoint(_preySpawnPoints, _usedPreySpawnPoints);
                if(null != spawnPoint) {
                    return spawnPoint;
                }
                return gameType.BirdTypesShareSpawnpoints ? GetSpawnPoint(_predatorSpawnPoints, _usedPredatorSpawnPoints) : null;
            }

            [CanBeNull]
            private SpawnPoint GetSpawnPoint(IList<SpawnPoint> from, IList<SpawnPoint> to)
            {
                if(from.Count < 1) {
                    return null;
                }

                SpawnPoint spawnPoint = Random.RemoveRandomEntry(from);
                to.Add(spawnPoint);
                return spawnPoint;
            }

            public void Reset()
            {
                Debug.Log($"Resetting used spawnpoints for game type {_gameTypeData.Name}");

                _predatorSpawnPoints.AddRange(_usedPredatorSpawnPoints);
                _usedPredatorSpawnPoints.Clear();

                _preySpawnPoints.AddRange(_usedPreySpawnPoints);
                _usedPreySpawnPoints.Clear();
            }
        }

        // game type => spawnpoints
        private readonly Dictionary<GameTypeData, SpawnPoints> _spawnPoints = new Dictionary<GameTypeData, SpawnPoints>();

#region Registration
        public void RegisterSpawnPoint(SpawnPoint spawnPoint)
        {
            foreach(GameTypeData gameTypeData in spawnPoint.GameTypes) {
                //Debug.Log($"Registering spawnpoint {spawnPoint.name} for game type {gameTypeData.Name}");

                SpawnPoints spawnPoints = _spawnPoints.GetOrDefault(gameTypeData);
                if(null == spawnPoints) {
                    spawnPoints = new SpawnPoints(gameTypeData);
                    _spawnPoints.Add(gameTypeData, spawnPoints);
                }
                spawnPoints.Add(spawnPoint);
            }
        }

        public void UnregisterSpawnPoint(SpawnPoint spawnPoint)
        {
            foreach(GameTypeData gameTypeData in spawnPoint.GameTypes) {
                //Debug.Log($"Unregistering spawnpoint {spawnPoint.name} for game type {gameTypeData.Name}");

                SpawnPoints spawnPoints = _spawnPoints.GetOrDefault(gameTypeData);
                spawnPoints?.Remove(spawnPoint);
            }
        }
#endregion

        [CanBeNull]
        public SpawnPoint GetSpawnPoint(GameTypeData gameTypeData, BirdTypeData birdType)
        {
            SpawnPoints spawnPoints = _spawnPoints.GetOrDefault(gameTypeData);
            return spawnPoints?.GetSpawnPoint(birdType);
        }

        public void ReleaseSpawnPoints()
        {
            foreach(var kvp in _spawnPoints) {
                kvp.Value.Reset();
            }
        }
    }
}
