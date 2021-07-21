﻿using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TheLuxGames.Visualizer.Util;
using UnityEngine;

namespace TheLuxGames.Visualizer.Behaviours.Soccer
{
    public class DrawPasses : MonoBehaviour
    {
        public struct Threat
        {
            public float distanceFromReceiver;
            public float distanceFromPasser;
            public bool inPassline;
        }

        [SerializeField, Required] private SoccerPlayerComponent _passer;
        [SerializeField, Required] private int _teamIdOfPasser;

        //TODO This should be extracted to a universal constant/so
        [SerializeField, Required] private int[] _validTeamIds;

        [SerializeField, Required] private Transform _passerTransform;

        [SerializeField, Required, FoldoutGroup("Pass Line Rendering")] private Gradient _gradient;
        [SerializeField, Required, FoldoutGroup("Pass Line Rendering")] private List<LineRenderer> _lineRenderers;
        [SerializeField, Required, FoldoutGroup("Pass Line Rendering")] private GameObject _lineRendererPrefab;
        [SerializeField, Required, FoldoutGroup("Pass Line Rendering")] private Transform _lineRendererParentTransform;
        [SerializeField, Required, FoldoutGroup("Pass Line Rendering")] private float _heightOffsetLines = 0.1125f;
        [SerializeField, Required, FoldoutGroup("Pass Line Rendering")] private bool _drawing = false;

        [SerializeField, FoldoutGroup("Safety Formula Settings")] private float _finalMultiplier = 2f;
        [SerializeField, FoldoutGroup("Safety Formula Settings")] private float _inPassLineMultiplier = 1.5f;
        [SerializeField, FoldoutGroup("Safety Formula Settings")] private float passLineWidth = 1f;
        [SerializeField, FoldoutGroup("Safety Formula Settings")] private float threatRadius = 2f;
        [SerializeField, FoldoutGroup("Safety Formula Settings")] private float _threatRadiusPenaltySoftener = 2f;
        [SerializeField, FoldoutGroup("Safety Formula Settings")] private float _baseSafety = 100f;

        [SerializeField, ReadOnly] private List<SoccerPlayerComponent> _allSoccerPlayers = new List<SoccerPlayerComponent>();

        public List<SoccerPlayerComponent> AllSoccerPlayers
        {
            get
            {
                //TODO This should really be done by the instancer/spawner on spawn instead this is not very effecient
                //This is merely a fallback
                if (!_allSoccerPlayers.Any()) _allSoccerPlayers = FindObjectsOfType<SoccerPlayerComponent>().ToList();
                return _allSoccerPlayers;
            }
            set
            {
                _allSoccerPlayers = value;
            }
        }

        public List<SoccerPlayerComponent> AllFellowTeamPlayers
        {
            get
            {
                return AllSoccerPlayers.Where(x =>
                {
                    bool isValidTeamId = IsValidTeamId(x);
                    bool inSameTeam = x.Value.TeamId == TeamId;
                    return isValidTeamId && inSameTeam;
                }).ToList();
            }
        }

        public List<SoccerPlayerComponent> AllEnemies
        {
            get
            {
                return AllSoccerPlayers.Where(x =>
                {
                    bool isValidTeamId = IsValidTeamId(x);
                    bool notInSameTeam = x.Value.TeamId != _teamIdOfPasser;
                    return isValidTeamId && notInSameTeam;
                }).ToList();
            }
        }

        private bool IsValidTeamId(SoccerPlayerComponent soccerPlayer)
        {
            return _validTeamIds.Any(validTeamId => soccerPlayer.Value.TeamId == validTeamId);
        }

        public int TeamId { get => _teamIdOfPasser; set => _teamIdOfPasser = value; }

        public void StartDrawing()
        {
            _drawing = true;
            _lineRenderers.ForEach(x => x.gameObject.SetActive(true));
        }

        public void EndDrawing()
        {
            _drawing = false;
            _lineRenderers.ForEach(x => x.gameObject.SetActive(false));
        }

        public void Update()
        {
            if (_drawing) Draw();
        }

        [Button("Draw")]
        public void Draw()
        {
            while (_lineRenderers.Count < AllFellowTeamPlayers.Count) AddLineRenderer();
            for (int i = 0; i < AllFellowTeamPlayers.Count; i++)
            {
                SoccerPlayerComponent from = _passer;
                SoccerPlayerComponent to = AllFellowTeamPlayers[i];
                var safety = GetPassSafetyPercentage(from, to);
                var safetyEvaluation = safety / 100;
                Color color = _gradient.Evaluate(safetyEvaluation);
                _lineRenderers[i].startColor = color;
                _lineRenderers[i].endColor = color;
                _lineRenderers[i].SetPositions(new Vector3[] { _passerTransform.position + Vector3.up * _heightOffsetLines, to.transform.position + Vector3.up * _heightOffsetLines });
            }
        }

        private void AddLineRenderer()
        {
            var lineRendererGO = Instantiate(_lineRendererPrefab, _lineRendererParentTransform);
            var lineRenderer = lineRendererGO.GetComponent<LineRenderer>();
            _lineRenderers.Add(lineRenderer);
        }

        public float GetPassSafetyPercentage(SoccerPlayerComponent passer, SoccerPlayerComponent receiver)
        {
            Vector3 fromPosition = passer.Value.Position;
            Vector3 receiverPos = receiver.Value.Position;

            var threats = GetPassThreats(fromPosition, receiverPos);
            float safety = CalculateSafety(threats);
            return safety;
        }

        private float CalculateSafety(List<Threat> threats)
        {
            float safety = _baseSafety;
            foreach (Threat threat in threats)
            {
                float threatRadiusPenaltySoftener = _threatRadiusPenaltySoftener > 0 ? _threatRadiusPenaltySoftener : 1;
                float finalMultiplier = _finalMultiplier > 0 ? _finalMultiplier : 1;
                float passerThreatPenalty = CalculatePasserPenalty(threat, threatRadiusPenaltySoftener);
                float receiverThreatPenalty = CalculateReceiverPenalty(threat, threatRadiusPenaltySoftener);
                float safetyPenalty = CalculateFinalPenalty(finalMultiplier, passerThreatPenalty, receiverThreatPenalty);
                safety -= safetyPenalty;
                safety = safety > 0 ? safety : 0;
            }
            return safety;
        }

        private static float CalculateFinalPenalty(float finalMultiplier, float passerThreatPenalty, float receiverThreatPenalty)
        {
            return (passerThreatPenalty + receiverThreatPenalty) * finalMultiplier;
        }

        private float CalculatePasserPenalty(Threat threat, float threatRadiusPenaltySoftener)
        {
            float passerThreatPenalty;
            if (threat.distanceFromPasser > 0) passerThreatPenalty = 2 * _inPassLineMultiplier * (1f / threatRadiusPenaltySoftener / threat.distanceFromPasser);
            else passerThreatPenalty = 0;
            return passerThreatPenalty;
        }

        private float CalculateReceiverPenalty(Threat threat, float threatRadiusPenaltySoftener)
        {
            float receiverThreatPenalty;
            if (threat.distanceFromReceiver > 0) receiverThreatPenalty = 2 * _inPassLineMultiplier * (1f / threatRadiusPenaltySoftener / threat.distanceFromReceiver);
            else receiverThreatPenalty = 0;
            return receiverThreatPenalty;
        }

        private List<Threat> GetPassThreats(Vector3 fromPosition, Vector3 receiverPos)
        {
            var threats = new List<Threat>();
            var passLineCoordinates = GetPassLineAreaCoordinates(fromPosition, receiverPos, passLineWidth);
            AllEnemies.ForEach(enemy =>
            {
                Vector2 enemyPosition = enemy.Value.Position;
                var enemyInPassLine = MathUtil.PointInsidePolygon(passLineCoordinates, enemyPosition);
                var enemyInReceiverThreatRadius = MathUtil.PointInsideCircle(threatRadius, receiverPos, enemyPosition);
                var enemyInThreatRange = enemyInPassLine || enemyInReceiverThreatRadius;

                if (enemyInThreatRange)
                {
                    Threat threat = new Threat();
                    float distanceFromReceiver = Vector2.Distance(enemyPosition, receiverPos);
                    var inThreatRadiusOfReceiver = distanceFromReceiver <= threatRadius;
                    if (inThreatRadiusOfReceiver) threat.distanceFromReceiver = distanceFromReceiver;

                    float distanceFromPasser = Vector2.Distance(enemyPosition, fromPosition);
                    var inThreatRadiusOfPasser = distanceFromPasser <= threatRadius && enemyInPassLine;
                    if (inThreatRadiusOfPasser) threat.distanceFromPasser = distanceFromPasser;

                    threat.inPassline = enemyInPassLine;

                    threats.Add(threat);
                }
            });
            return threats;
        }

        private Vector2[] GetPassLineAreaCoordinates(Vector2 from, Vector2 to, float width)
        {
            var axis = (to - from).normalized;
            return MathUtil.GetAxisAlignedRectangleCoordinates(from, to, width, axis);
        }
    }
}