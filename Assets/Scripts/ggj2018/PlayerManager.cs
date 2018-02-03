﻿using System;
using System.Collections.Generic;
using System.Linq;

using ggj2018.Core.Camera;
using ggj2018.Core.Input;
using ggj2018.Core.Util;

using ggj2018.ggj2018.Data;
using ggj2018.ggj2018.GameTypes;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.Serialization;

namespace ggj2018.ggj2018
{
    public sealed class PlayerManager : SingletonBehavior<PlayerManager>
    {
// TOOD: move this to its own file
        [Serializable]
        public sealed class CharacterSelectState
        {
            public enum JoinState
            {
                None,
                Joined,
                Ready
            }

#region Join/Ready State
            [SerializeField]
            private JoinState _joinState = JoinState.None;

            public JoinState PlayerJoinState { get { return _joinState; } set { _joinState = value; } }

            public bool IsJoined => PlayerJoinState == JoinState.Joined;

            public bool IsJoinedOrReady => PlayerJoinState == JoinState.Joined || PlayerJoinState == JoinState.Ready;

            public bool IsReady => PlayerJoinState == JoinState.Ready;
#endregion

#region Selected Bird State
            [SerializeField]
            private int _selectedBird;

            public int SelectedBird { get { return _selectedBird; } set { _selectedBird = value; } }

            public BirdData.BirdDataEntry PlayerBirdData => GameManager.Instance.BirdData.Birds.ElementAt(SelectedBird);

            public string PlayerBirdId => PlayerBirdData.Id;
#endregion

            public Viewer Viewer { get; }

            public int ControllerIndex { get; }

            public Player Player { get; set; }

            public CharacterSelectState(int controllerIndex, Viewer viewer)
            {
                ControllerIndex = controllerIndex;
                Viewer = viewer;
            }

            public void NextBird()
            {
                SelectedBird++;
                WrapBird();
            }

            public void PrevBird()
            {
                SelectedBird--;
                WrapBird();
            }

            private void WrapBird()
            {
                if(SelectedBird < 0) {
                    SelectedBird = GameManager.Instance.BirdData.Birds.Count - 1;
                } else if(SelectedBird >= GameManager.Instance.BirdData.Birds.Count) {
                    SelectedBird = 0;
                }
            }
        }

#region Models
        [SerializeField]
        private Predator _predatorModelPrefab;

        public Predator PredatorModelPrefab => _predatorModelPrefab;

        [SerializeField]
        private Prey _preyModelPrefab;

        public Prey PreyModelPrefab => _preyModelPrefab;
#endregion

        [SerializeField]
        [FormerlySerializedAs("_localPlayerPrefab")]
        private Player _playerPrefab;

        private GameObject _playerContainer;

        [SerializeField]
        private PlayerData _playerData;

        public PlayerData PlayerData => _playerData;

        public int MaxLocalPlayers => InputManager.Instance.MaxControllers;

        private readonly List<Player> _players = new List<Player>();

        public IReadOnlyCollection<Player> Players => _players;

        [SerializeField]
        [ReadOnly]
        private readonly List<CharacterSelectState> _characterSelectStates = new List<CharacterSelectState>();

        public IReadOnlyCollection<CharacterSelectState> CharacterSelectStates => _characterSelectStates;

#region Game Related State
        public int PlayerCount => _players.Count;

        [SerializeField]
        [ReadOnly]
        private int _preyCount;

        public int PreyCount => _preyCount;

        public int PredatorCount => PlayerCount - PreyCount;
#endregion

#region Unity Lifecycle
        private void Awake()
        {
            _playerContainer = new GameObject("Players");
        }

        protected override void OnDestroy()
        {
            Destroy(_playerContainer);
            _playerContainer = null;

            base.OnDestroy();
        }
#endregion

        public void Initialize()
        {
            for(int i=0; i<InputManager.Instance.MaxControllers; ++i) {
                Viewer viewer = CameraManager.Instance.AcquireViewer() as Viewer;
                int controllerIndex = InputManager.Instance.AcquireController();

                _characterSelectStates.Add(new CharacterSelectState(controllerIndex, viewer));
            }
            CameraManager.Instance.ResizeViewports();
        }

        [CanBeNull]
        public Player SpawnPlayer(GameType.GameTypes gameType, CharacterSelectState selectState)
        {
            SpawnPoint spawnPoint = SpawnManager.Instance.GetSpawnPoint(gameType, selectState.PlayerBirdData);
            if(null == spawnPoint) {
                Debug.LogError($"No spawn points left for bird type {selectState.PlayerBirdId} in game type {gameType}");
                return null;
            }

            Player player = Instantiate(_playerPrefab, _playerContainer.transform);
            InitializePlayer(player, _players.Count, selectState, spawnPoint);

            Debug.Log($"Spawned {player.State.BirdType.Name} for local player {player.Id} at {spawnPoint.name} ({player.transform.position})");

            AddPlayer(player);
            return player;
        }

        private void InitializePlayer(Player player, int playerId, CharacterSelectState selectState, SpawnPoint spawnPoint)
        {
            Bird birdModel = Instantiate(
                selectState.PlayerBirdData.IsPredator
                    ? (Bird)PredatorModelPrefab
                    : (Bird)PreyModelPrefab,
                player.transform);

            //if(player.isLocal) {
                player.InitializeLocal(playerId, selectState.ControllerIndex, selectState.Viewer, birdModel, selectState.PlayerBirdData);
            /*} else {
                player.InitializeNetwork(playerId, birdModel, selectState.PlayerBirdData);
            }*/

            spawnPoint.Spawn(player);
        }

        public void DespawnPlayer(Player player)
        {
            Debug.Log($"Despawning player {player.Id}");

            RemovePlayer(player);
        }

        // TODO: this no longer works becuase List
        /*public void DespawnAllPlayers()
        {
            Debug.Log("Despawning everybody");
            for(int i=0; i<_players.Count; ++i) {
                RemovePlayer(i);
            }
        }*/

        private void AddPlayer(Player player)
        {
            _players.Add(player);

            if(player.State.BirdType.IsPrey) {
                _preyCount++;
            }
        }

        private void RemovePlayer(Player player)
        {
            if(player.State.BirdType.IsPrey) {
                _preyCount--;
            }

            _players.Remove(player);
            Destroy(player.gameObject);
        }

#if UNITY_EDITOR
        public void DebugStunAll()
        {
            foreach(Player player in _players) {
                player.State.DebugStun();
            }
        }

        public void DebugKillAll()
        {
            foreach(Player player in _players) {
                player.State.DebugKill();
            }
        }
#endif
    }
}
