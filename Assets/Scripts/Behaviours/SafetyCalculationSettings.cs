using System;
using UnityEngine;

namespace TheLuxGames.Visualizer.Behaviours.Soccer.Passing
{
    [Serializable]
    public class SafetyCalculationSettings
    {
        [SerializeField] private float _finalMultiplier = 2f;
        [SerializeField] private float _inPassLineMultiplier = 1.5f;
        [SerializeField] private float passLineWidth = 1f;
        [SerializeField] private float threatRadius = 2f;
        [SerializeField] private float _threatRadiusPenaltySoftener = 2f;
        [SerializeField] private float _baseSafety = 100f;

        public float FinalMultiplier { get => _finalMultiplier; set => _finalMultiplier = value; }
        public float InPassLineMultiplier { get => _inPassLineMultiplier; set => _inPassLineMultiplier = value; }
        public float ThreatRadiusPenaltySoftener { get => _threatRadiusPenaltySoftener; set => _threatRadiusPenaltySoftener = value; }
        public float BaseSafety { get => _baseSafety; set => _baseSafety = value; }
        public float ThreatRadius { get => threatRadius; set => threatRadius = value; }
        public float PassLineWidth { get => passLineWidth; set => passLineWidth = value; }
    }
}