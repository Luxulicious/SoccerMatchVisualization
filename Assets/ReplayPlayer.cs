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
using UnityEngine.Events;

[Serializable]
public class FrameFrameUnityEvent : UnityEvent<Frame, Frame> { }

public class ReplayPlayer<TReplay, TBall, TPlayer> : MonoBehaviour
        where TReplay : Replay, new()
        where TBall : Ball, new()
        where TPlayer : Player, new()
{
 
    [SerializeReference, HideInInspector] private IFileReader _readerAsReference;
    [SerializeField, HideInInspector] private UnityEngine.Object _readerAsField;

    [ShowInInspector]
    public IFileReader Reader 
    { 
        get 
        {
            if (_readerAsReference != null) return _readerAsReference;
            else if (_readerAsField != null) return _readerAsReference as IReplayReader<TReplay>;
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
    [SerializeField] private int? _currentFrame = null;

    [SerializeField] private FrameFrameUnityEvent _onFrameAdvanced = new FrameFrameUnityEvent();

    [ShowInInspector]
    public int CurrentFrame
    {
        get 
        {
            if (_currentFrame == null)
            {
                var frame = _replay.FirstFrameIndex;
                return frame != null ? frame.Value : -1;
            }
            return _currentFrame.Value; 
        }
        set
        {
            if (!_replay.Frames.ContainsKey(value))
            { 
                Debug.LogError($"No frame found at index: {value}");
                return;
            }
            if (_currentFrame != value)
            {
                var prevFrame = _currentFrame;
                _currentFrame = value;
                if (_currentFrame - prevFrame == 1)
                    _onFrameAdvanced.Invoke((_currentFrame.HasValue ? _replay.Frames[_currentFrame.Value] : null), (_currentFrame.HasValue ? _replay.Frames[prevFrame.Value] : null));
            }
        }
    }

    [Button("Load Replay")]
    public virtual void Load()
    {
        if (Reader == null)
        {
            Debug.LogError("Reader cannot be left empty!");
            return;
        }
        var reader = Reader as IReplayReader<TReplay>;
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

    [Button("Advance Frame")]
    public virtual void AdvanceFrame()
    {
        CurrentFrame += 1;
    }
}
