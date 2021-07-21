using Assets.Scripts;
using Assets.Scripts.Domain;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrawPasses : MonoBehaviour
{

    public struct Threat 
    {
        public float distanceFromReceiver;
        public float distanceFromPasser;
        public bool inPassline;
    }

    [SerializeField, Required] private int _teamId;
    [SerializeField, Required] private int[] _validTeamIds;
    [SerializeField, Required] private Transform _playerTransform;
    [SerializeField, Required] private Gradient _gradient;
    [SerializeField, Required] private List<LineRenderer> _lineRenderers;
    [SerializeField, Required] private GameObject _lineRendererPrefab;
    [SerializeField, Required] private Transform _lineRendererParentTransform;
    [SerializeField, ReadOnly] private List<SoccerPlayerComponent> _allSoccerPlayers = new List<SoccerPlayerComponent>();
    [SerializeField, Required] private SoccerPlayerComponent _passer;
    [SerializeField] private float _heightOffsetLines = 0.1125f;
    [SerializeField] private bool _drawing = false;

    [SerializeField, FoldoutGroup("Safety Formula Settings")] float _finalMultiplier = 2f;
    [SerializeField, FoldoutGroup("Safety Formula Settings")] float _inPassLineMultiplier = 1.5f;
    [SerializeField, FoldoutGroup("Safety Formula Settings")] float passLineWidth = 1f;
    [SerializeField, FoldoutGroup("Safety Formula Settings")] float threatRadius = 2f;
    [SerializeField, FoldoutGroup("Safety Formula Settings")] float _threatRadiusPenaltySoftener = 2f;

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
                bool notInSameTeam = x.Value.TeamId != _teamId;
                return isValidTeamId && notInSameTeam;
            }).ToList();
        }
    }

    private bool IsValidTeamId(SoccerPlayerComponent soccerPlayer)
    {
        return _validTeamIds.Any(validTeamId => soccerPlayer.Value.TeamId == validTeamId);
    }

    public int TeamId { get => _teamId; set => _teamId = value; }

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
        if(_drawing) Draw();
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
            _lineRenderers[i].SetPositions(new Vector3[] { _playerTransform.position + (Vector3.up * _heightOffsetLines), to.transform.position + (Vector3.up * _heightOffsetLines) });

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
        float baseSafety = 100f;
        float safety = baseSafety;
        //TODO This calculation is very primitives and needs improvement obv 
        foreach (var threat in threats)
        {
            float threatRadiusPenaltySoftener = _threatRadiusPenaltySoftener > 0 ? _threatRadiusPenaltySoftener : 1;
            float finalMultiplier = _finalMultiplier > 0 ? _finalMultiplier : 1;
            float passerThreatPenalty = threat.distanceFromPasser > 0 ? 
                _inPassLineMultiplier * (1f / threatRadiusPenaltySoftener / threat.distanceFromPasser) * _inPassLineMultiplier 
                : 0;
            float receiverThreatPenalty = threat.distanceFromReceiver > 0 ? 
                _inPassLineMultiplier * (1f / threatRadiusPenaltySoftener / threat.distanceFromReceiver) * _inPassLineMultiplier 
                : 0;
            var safetyPenalty = (passerThreatPenalty + receiverThreatPenalty) * finalMultiplier;
            safety -= safetyPenalty;
            safety = safety > 0 ? safety : 0;
        }
        return safety;
    }

    private List<Threat> GetPassThreats(Vector3 fromPosition, Vector3 receiverPos)
    {
        var threats = new List<Threat>();
        var passLineCoordinates = GetPassLineAreaCoordinates(fromPosition, receiverPos, passLineWidth);
        AllEnemies.ForEach(enemy =>
        {
            Vector2 enemyPosition = enemy.Value.Position;
            var enemyInPassLine = PointInsidePolygon(passLineCoordinates, enemyPosition);
            var enemyInReceiverThreatRadius = PointInsideCircle(threatRadius, receiverPos, enemyPosition);
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
        return GetAxisAlignedRectangleCoordinates(from, to, width, axis);
    }

    //TODO Move these math functions below to their own class
    private static Vector2[] GetAxisAlignedRectangleCoordinates(Vector2 start, Vector2 end, float width, Vector2 axis)
    {
        Vector2 perpendicularDir = new Vector2(-axis.y, axis.x);
        float extents = width / 2;
        Vector2 a = start - extents * perpendicularDir;
        Vector2 b = start + extents * perpendicularDir;
        Vector2 c = end + extents * perpendicularDir;
        Vector2 d = end - extents * perpendicularDir;
        return new Vector2[] { a, b, c, d };
    }

    private static bool PointInsidePolygon(Vector2[] polygon, Vector2 point)
    {
        //Source: http://wiki.unity3d.com/index.php?title=PolyContainsPoint&oldid=20475
        var j = polygon.Length - 1;
        var inside = false;
        for (int i = 0; i < polygon.Length; j = i++)
        {
            if (((polygon[i].y <= point.y && point.y < polygon[j].y) || (polygon[j].y <= point.y && point.y < polygon[i].y)) &&
               (point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x))
                inside = !inside;
        }
        return inside;
    }

    private bool PointInsideCircle(float radius, Vector2 origin, Vector2 point) => Vector2.Distance(origin, point) <= radius;

}
