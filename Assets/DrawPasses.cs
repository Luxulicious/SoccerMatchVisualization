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
    [SerializeField, Required] private int _teamId;
    [SerializeField, Required] private int[] _validTeamIds;
    [SerializeField, Required] private Transform _playerTransform;
    [SerializeField, Required] private Gradient _gradient;
    [SerializeField, Required] private List<LineRenderer> _lineRenderers;
    [SerializeField, Required] private GameObject _lineRendererPrefab;
    [SerializeField, Required] private Transform _lineRendererParentTransform;
    [SerializeField, ReadOnly] private List<SoccerPlayerComponent> _allSoccerPlayers = new List<SoccerPlayerComponent>();
    [SerializeField] private float _heightOffsetLines = 0.1125f;
    [SerializeField] private bool _drawing = false;
    
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

    public List<SoccerPlayerComponent> AllOpposingTeamPlayers
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
            _lineRenderers[i].SetPositions(new Vector3[] { _playerTransform.position + (Vector3.up * _heightOffsetLines), AllFellowTeamPlayers[i].transform.position + (Vector3.up * _heightOffsetLines) });
        }
    }

    private void AddLineRenderer()
    {
        var lineRendererGO = Instantiate(_lineRendererPrefab, _lineRendererParentTransform);
        var lineRenderer = lineRendererGO.GetComponent<LineRenderer>();
        _lineRenderers.Add(lineRenderer);
    }
}
