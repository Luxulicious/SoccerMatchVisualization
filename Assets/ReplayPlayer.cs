using Assets.Scripts;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheLuxGames.Visualizer.Domain;
using UnityEditor;
using UnityEngine;

public class ReplayPlayer<TReplay, TBall, TPlayer> : MonoBehaviour
        where TReplay : Replay, new()
        where TBall : Ball, new()
        where TPlayer : Player, new()
{
    [SerializeReference, HideInInspector] private IReader _readerAsReference;
    [SerializeField, HideInInspector] private UnityEngine.Object _readerAsField;

    [ShowInInspector]
    public IReader Reader 
    { 
        get 
        {
            if (_readerAsReference != null) return _readerAsReference;
            else if (_readerAsField != null) return _readerAsReference as IFileReader<TReplay>;
            else return null;
        } 
        set 
        {
            if (value is UnityEngine.Object O)
            {
                _readerAsField = O;
                _readerAsReference = null;
            }
            else
            {
                _readerAsReference = value;
                _readerAsField = null;
            }
        } 
    }

    [SerializeField] private TReplay _replay;
    [SerializeField] private int? currentFrame = null;
    
    [ShowInInspector]
    public int CurrentFrame
    {
        get 
        {
            if (currentFrame == null)
            {
                var frame = _replay.FirstFrameIndex;
                return frame != null ? frame.Value : -1;
            }
            return currentFrame.Value; 
        }
        set
        {
            if (!_replay.Frames.Any(x => x.FrameIndex == value))
            { 
                Debug.LogError($"No frame found at index: {value}");
                return;
            }
            currentFrame = value;
        }
    }

    [Button("Load Replay")]
    protected virtual void Load()
    {
        if (Reader == null)
        {
            Debug.LogError("Reader cannot be left empty!");
            return;
        }
        var reader = Reader as IFileReader<TReplay>;
        if (reader == null)
        {
            Debug.LogError("Invalid reader");
            return;
        }
        var filePath = EditorUtility.OpenFilePanel("Select a file to extract replay data from", "Assets/", "dat");
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("Filepath is empty");
            return;
        }
        if (!File.Exists(filePath))
        {
            Debug.LogError($"Failed to find file at path: '{filePath}'");
            return;
        }

        _replay = reader.ReadFromFile(filePath);
    }
}
