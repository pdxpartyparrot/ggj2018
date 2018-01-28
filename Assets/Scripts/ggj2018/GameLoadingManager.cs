﻿using ggj2018.Game.Loading;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public sealed class GameLoadingManager : LoadingManager<GameLoadingManager>
    {
        [SerializeField]
        private GameManager _gameManagerPrefab;

        [SerializeField]
        private PlayerManager _playerManagerPrefab;

        [SerializeField]
        private UIManager _uiManagerPrefab;

        protected override void CreateManagers()
        {
            base.CreateManagers();

            GameManager.CreateFromPrefab(_gameManagerPrefab, ManagersContainer);
            PlayerManager.CreateFromPrefab(_playerManagerPrefab, ManagersContainer);
            SpawnManager.Create(ManagersContainer);
            UIManager.CreateFromPrefab(_uiManagerPrefab, ManagersContainer);
        }

        protected override void InitializeManagers()
        {
            base.InitializeManagers();

            GameManager.Instance.Initialize();
        }
    }
}
