﻿using ggj2018.Core.Util;
using ggj2018.Core.Input;
using ggj2018.Core.Camera;
using ggj2018.Core.Network;
using ggj2018.ggj2018.Data;
using ggj2018.Game.Scenes;

using UnityEngine;
using UnityEngine.EventSystems;

namespace ggj2018.ggj2018.Game
{
    public sealed class GameManager : SingletonBehavior<GameManager>
    {
#region Systems
        [Header("Systems")]

        [SerializeField]
        private GameObject _gvrPrefab;

        private GameObject _gvr;

        [SerializeField]
        private EventSystem _eventSystemPrefab;

        private EventSystem _eventSystem;

        public EventSystem EventSystem => _eventSystem;

        [SerializeField]
        private NetworkManager _networkManagerPrefab;

        private NetworkManager _networkManager;

        public NetworkManager NetworkManager => _networkManager;
#endregion

        [Space(10)]

#region Data
        [Header("Data")]

        [SerializeField]
        private ConfigData _configData;

        public ConfigData ConfigData => _configData;

        [SerializeField]
        private BirdData _birdData;

        public BirdData BirdData => _birdData;

        [SerializeField]
        private GameData _gameTypeData;

        public GameData GameTypeData => _gameTypeData;
#endregion

        [Space(10)]

#region Audio
        [Header("Audio")]

        [SerializeField]
        private AudioClip _startupLogoMusicClip;

        public AudioClip StartupLogoMusicClip => _startupLogoMusicClip;

        [SerializeField]
        private AudioClip _characterSelectMusicClip;

        public AudioClip CharacterSelectMusicClip => _characterSelectMusicClip;

        [SerializeField]
        private AudioClip _gameMusic1AudioClip;

        public AudioClip GameMusic1AudioClip => _gameMusic1AudioClip;

        [SerializeField]
        private AudioClip _gameMusic2AudioClip;

        public AudioClip GameMusic2AudioClip => _gameMusic2AudioClip;

        [SerializeField]
        private AudioClip _gameOverMusicAudioClip;

        public AudioClip GameOverMusicAudioClip => _gameOverMusicAudioClip;
#endregion

        [Space(10)]

#region Physics
        [Header("Physics")]

        [SerializeField]
        private PhysicMaterial _frictionlesssMaterial;

        public PhysicMaterial FrictionlessMaterial => _frictionlesssMaterial;
#endregion

        [Space(10)]

        [SerializeField]
        private bool _enableImmunity;

        public bool EnableImmunity => _enableImmunity;

        [SerializeField]
        [ReadOnly]
        private /*readonly*/ GameState _gameState = new GameState();

        public GameState State => _gameState;

#region Unity Lifecycle
        private void Awake()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = ConfigData.TargetFrameRate;
            Debug.Log($"Target frame rate: {Application.targetFrameRate}");

            Debug.Log($"Gravity: {Physics.gravity}");
            Physics.bounceThreshold = Mathf.Infinity;

            _birdData.Initialize();
            _gameTypeData.Initialize();

            if(ConfigData.EnableGVR) {
                Debug.Log("Creating GVR managers...");
                _gvr = Instantiate(_gvrPrefab);
            } else {
                Debug.Log("Creating EventSystem (no VR)...");
                _eventSystem = Instantiate(_eventSystemPrefab);
            }

            Debug.Log("Creating NetworkManager...");
            _networkManager = Instantiate(_networkManagerPrefab);
        }

        protected override void OnDestroy()
        {
            Destroy(_networkManager?.gameObject);
            _networkManager = null;

            Destroy(_eventSystem?.gameObject);
            _eventSystem = null;

            Destroy(_gvr);
            _gvr = null;

            base.OnDestroy();
        }

        private void Update()
        {
            CheckReload();
        }
#endregion

        public void Initialize()
        {
            NetworkManager.Initialize(ConfigData.EnableNetwork);

            CameraManager.Instance.SpawnViewers(ConfigData.MaxLocalPlayers);
        }

        private void CheckReload()
        {
            if(!_gameState.IsPaused) {
                return;
            }

            if(InputManager.Instance.SelectPressed()) {
#if false
                Debug.Log("Restarting game!");
                GameSceneManager.Instance.ReloadMainScene();
#else
                Debug.Log("Quitting game!");
                Application.Quit();
#endif
            }
        }
    }
}

