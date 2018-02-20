﻿using ggj2018.Core.Camera;
using ggj2018.Core.VFX;
using ggj2018.ggj2018.Players;
using ggj2018.ggj2018.UI;

using UnityEngine;

namespace ggj2018.ggj2018.Camera
{
    public class Viewer : BaseViewer
    {
        [Header("UI")]

        [SerializeField]
        private PlayerUIPage _playerUIPrefab;

        public PlayerUIPage PlayerUI { get; private set; }

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            PlayerUI = Instantiate(_playerUIPrefab, transform);
            PlayerUI.GetComponent<Canvas>().worldCamera = UICamera;
        }

        protected override void OnDestroy()
        {
            Destroy(PlayerUI.gameObject);
            PlayerUI = null;

            base.OnDestroy();
        }
#endregion

        public void Initialize(Player owner)
        {
            SetFov(owner.Bird.Type.ViewFOV);
            SetOrbitRadius(owner.Bird.Type.FollowOrbitRadius);

            SetGlobalPostProcessProfile(owner.Bird.Type.PostProcessProfile.Clone());
        }
    }
}
