using Sirenix.OdinInspector;
using TheLuxGames.Visualizer.Behaviours.Soccer.Passing;
using UnityEngine;

namespace TheLuxGames.Visualizer.Settings.Soccer.Passing
{
    [CreateAssetMenu(fileName = "Safety Calculation Setttings", menuName = "Soccer/Passing/Safety Calculation Settings")]
    public class SafetyCalculationSettingsData : ScriptableObject
    {
        [SerializeField] private SafetyCalculationSettings _settings;

        public SafetyCalculationSettings Settings { get => _settings; set => _settings = value; }
    }
}
