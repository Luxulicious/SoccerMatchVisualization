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
[Serializable]
public class ReplayUnityEvent : UnityEvent<Replay> { }
[Serializable]
public class FrameUnityEvent : UnityEvent<Frame> { }

public class ReplayPlayer<TReplay, TBall, TPlayer> : MonoBehaviour
        where TReplay : Replay, new()
        where TBall : Ball, new()
        where TPlayer : Player, new()
{
    [SerializeReference, HideInInspector] private IFileReader _readerAsReference;
    [SerializeField, HideInInspector] private UnityEngine.Object _readerAsField;

    [SerializeField, ReadOnly, HideInEditorMode] private bool _paused = true;
    [ShowInInspector, HideInEditorMode]
    public bool Playing => !_paused && Playable;

    private bool Playable => _replay != null && CurrentFrame != -1 && Application.isPlaying;

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

    public IReplayReader<TReplay> ReplayReader
    {
        get
        {
            if (Reader == null) throw new NullReferenceException("Reader cannot be left empty!");
            var r = Reader as IReplayReader<TReplay>;
            if(r == null) throw new Exception("Invalid reader");
            return r;
        }
    }

    /*[SerializeField, HideInEditorMode, HideIf("@HideReplay")]*/ private TReplay _replay;
    [SerializeField] private int? _currentFrame = null;

    [SerializeField, FoldoutGroup("Events", order: 10)] private FrameFrameUnityEvent _onFrameAdvanced = new FrameFrameUnityEvent();
    [SerializeField, FoldoutGroup("Events", order: 10)] private ReplayUnityEvent _onReplayLoaded = new ReplayUnityEvent();
    [SerializeField, FoldoutGroup("Events", order: 10)] private ReplayUnityEvent _onFirstFrameOfReplayLoaded = new ReplayUnityEvent();

    [ShowInInspector]
    public int CurrentFrame
    {
        get
        {
            if (_currentFrame == null)
            {
                if(_replay != null)
                    _currentFrame = _replay.FirstFrameIndex;
                return _currentFrame != null ? _currentFrame.Value : -1;
            }
            return _currentFrame.Value;
        }
        set
        {
            if (!_replay.Frames.ContainsKey(value))
            {
                Debug.LogError($"No frame found at index: {value}");
                Pause();
                return;
            }
            if (_currentFrame != value)
            {
                var prevFrame = _currentFrame;
                _currentFrame = value;
                if (_currentFrame != prevFrame)
                    _onFrameAdvanced.Invoke((_currentFrame.HasValue ? _replay.Frames[_currentFrame.Value] : null), (_currentFrame.HasValue ? _replay.Frames[prevFrame.Value] : null));
            }
        }
    }


    [SerializeField, ReadOnly, HideInEditorMode] private float elapsedTimeSinceLastFrame = 0f;
    [SerializeField, ReadOnly, HideInEditorMode] private float timeStep => 1.000000000f / _replay.FrameRate;
    [SerializeField, ReadOnly, HideInEditorMode] private bool _started = false;


    private void Update()
    {
        if (!Playing)
        {
            elapsedTimeSinceLastFrame = 0f;
            return;
        }
        if (elapsedTimeSinceLastFrame == 0f && !_started)
        {
            AdvanceFrame();
            _started = true;
        }
        else if (elapsedTimeSinceLastFrame >= timeStep)
        {
            int steps = (int)(elapsedTimeSinceLastFrame / timeStep);
            AdvanceFrame(steps);
            elapsedTimeSinceLastFrame %= timeStep;
        }
        elapsedTimeSinceLastFrame += Time.deltaTime;
    }

    private void AdvanceFrame()
    {
        CurrentFrame++;
    }

    private void AdvanceFrame(int steps)
    {
        CurrentFrame += steps;
    }

    [Button("Load Replay Async"), HideInEditorMode]

    public void LoadReplayAsyncButton()
    {
        StartCoroutine(LoadReplayAsync());
    }

    public virtual IEnumerator LoadReplayAsync()
    {
        var reader = ReplayReader;

        var filePath = EditorUtility.OpenFilePanel("Select a file to extract replay data from", "Assets/", "dat");
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("Filepath is empty");
            yield return null;
        }
        if (!File.Exists(filePath))
        {
            Debug.LogError($"Failed to find file at path: '{filePath}'");
            yield return null;
        }

        _replay = new TReplay();
        yield return reader.ReadFramesAsync(filePath, OnFrameLoaded);
        _onReplayLoaded.Invoke(_replay);
        yield return null;
    }

    public void OnFrameLoaded(Frame frame)
    {
        _replay.Frames.Add(frame.FrameIndex, frame);
        if (_replay.Frames.Count == 1)
        {
            _onFirstFrameOfReplayLoaded.Invoke(_replay);
        }
    }

    [Button("Load Replay"), HideInEditorMode]
    public virtual void LoadReplay()
    {      
        var reader = ReplayReader;

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
        _onReplayLoaded.Invoke(_replay);
    }

    [Button("Play"), HideInEditorMode]
    public void Play()
    {
        if (!Playable) Debug.LogError("Cannot play replay");
        _paused = false;
    }

    [Button("Pause"), HideInEditorMode]
    public void Pause()
    {
        _paused = true;
        _started = false;
    }

    [Button("Reset"), HideInEditorMode]
    public void ResetToFirstFrame()
    {
        CurrentFrame = _replay.FirstFrameIndex.Value; 
        Pause();
    }

    [Button("Previous Frame"), HideInEditorMode]
    public void PreviousFrame()
    {
        CurrentFrame -= 1;
    }

    [Button("Next Frame"), HideInEditorMode]
    public void NextFrame()
    {
        CurrentFrame += 1;
    }
}
