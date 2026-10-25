using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SoundSO))]
public class SoundSOEditor : Editor
{
    SerializedProperty _audioClips;
    SerializedProperty _isPlaylist;
    SerializedProperty _randomClipInPlaylist;
    SerializedProperty _outputGroup;
    SerializedProperty _playOnAwake;
    SerializedProperty _loop;
    SerializedProperty _oneshot;

    void OnEnable()
    {
        _audioClips = serializedObject.FindProperty("_audioClips");
        _isPlaylist = serializedObject.FindProperty("_isPlaylist");
        _randomClipInPlaylist = serializedObject.FindProperty("_randomClipInPlaylist");
        _outputGroup = serializedObject.FindProperty("_outputGroup");
        _playOnAwake = serializedObject.FindProperty("_playOnAwake");
        _loop = serializedObject.FindProperty("_loop");
        _oneshot = serializedObject.FindProperty("_oneshot");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Header for clips settings
        EditorGUILayout.LabelField("Clips Settings", EditorStyles.boldLabel);
        // The toggle sits above what it controls
        EditorGUILayout.PropertyField(_isPlaylist);

        if (_isPlaylist.boolValue)
        {
            // Display a list of audio clips
            EditorGUILayout.PropertyField(_audioClips, new GUIContent("Audio Clips"), true);
            // Display the random clip toggle for playlists
            EditorGUILayout.PropertyField(_randomClipInPlaylist);
        }
        else
        {
            // A single sound is the first entry of the same list, so there is always a box to fill
            if (_audioClips.arraySize == 0)
                _audioClips.arraySize = 1;
            EditorGUILayout.PropertyField(_audioClips.GetArrayElementAtIndex(0), new GUIContent("Audio Clip"));
        }

        EditorGUILayout.PropertyField(_outputGroup);

        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Behaviors", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(_playOnAwake);

        // Not in a random playlist, allow loop option since a random playlist will handle repetition
        if(!(_isPlaylist.boolValue && _randomClipInPlaylist.boolValue))
        {
            if(!_oneshot.boolValue)
                EditorGUILayout.PropertyField(_loop);
            else
                _loop.boolValue = false;
        }

        // If looping is enabled, one-shot is not allowed
        if (_loop.boolValue)
            _oneshot.boolValue = false;
        // Not looping, allow one-shot option
        else
            EditorGUILayout.PropertyField(_oneshot);

        // Everything else as Unity would normally draw it
        DrawPropertiesExcluding(serializedObject, "m_Script", "_audioClips", "_isPlaylist", "_randomClipInPlaylist", "_outputGroup", "_playOnAwake", "_loop", "_oneshot");

        serializedObject.ApplyModifiedProperties();
    }
}