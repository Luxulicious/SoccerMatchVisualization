using System;
using UnityEngine;

namespace TheLuxGames.Visualizer.Models
{
    public class TeamPlayer : Player
    {
        [SerializeField] private int _teamId;
        private Action<int> _onTeamIdChanged;
        [SerializeField] private int _playerNumber;

        public int TeamId
        {
            get => _teamId; set
            {
                if (_teamId != value)
                    OnTeamIdChanged?.Invoke(value);
                _teamId = value;
            }
        }

        public Action<int> OnTeamIdChanged { get => _onTeamIdChanged; set => _onTeamIdChanged = value; }

        public int PlayerNumber { get => _playerNumber; set => _playerNumber = value; }
    }
}