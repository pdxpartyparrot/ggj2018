﻿using System;

using ggj2018.Core.Input;

using UnityEngine;

namespace ggj2018.ggj2018.Data
{
    [CreateAssetMenu(fileName="ConfigData", menuName="ggj2018/Data/Config Data")]
    [Serializable]
    public sealed class ConfigData : ScriptableObject
    {
#region VR
        [SerializeField]
        private bool _enableGVR;

        public bool EnableGVR => _enableGVR;

        public bool EnableVR => EnableGVR;
#endregion

        [SerializeField]
        private bool _enableNetwork;

        public bool EnableNetwork => _enableNetwork;

        public int MaxLocalPlayers => (EnableGVR || EnableNetwork) ? 1 : InputManager.Instance.MaxControllers;

        [SerializeField]
        private float _worldScale = 1.0f;

        public float WorldScale => _worldScale;

        [SerializeField]
        private string _godRayAlphaProperty = "_Usealphatexture";

        public string GodRayAlphaProperty => _godRayAlphaProperty;
    }
}
