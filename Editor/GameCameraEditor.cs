using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameCamera))]
class GameCameraEditor : Editor {

    #region FIELDS
    private GameCamera Script { get; set; }
    #endregion

    #region SERIALIZED PROPERTIES

    private SerializedProperty cameraLimits;
    private SerializedProperty cameraOcclusionLayerMask;
    private SerializedProperty cameraOffset;
    private SerializedProperty cameraRotationSpeed;
    private SerializedProperty followSpeed;
    private SerializedProperty lookAtPointOffset;
    private SerializedProperty lookAtPointWhenNotVisible;
    private SerializedProperty mode;
    private SerializedProperty movementVelocityOffset;
    private SerializedProperty offsetWhenNotVisible;
    private SerializedProperty perspectiveChangeSpeed;
    private SerializedProperty targetTransform;
    #endregion

    private void OnEnable() {
        cameraLimits = serializedObject.FindProperty("cameraLimits");
        cameraOcclusionLayerMask =
            serializedObject.FindProperty("cameraOcclusionLayerMask");
        cameraOffset = serializedObject.FindProperty("cameraOffset");
        cameraRotationSpeed =
            serializedObject.FindProperty("cameraRotationSpeed");
        followSpeed = serializedObject.FindProperty("followSpeed");
        lookAtPointOffset = serializedObject.FindProperty("lookAtPointOffset");
        lookAtPointWhenNotVisible =
            serializedObject.FindProperty("lookAtPointWhenNotVisible");
        mode = serializedObject.FindProperty("mode");
        movementVelocityOffset =
            serializedObject.FindProperty("movementVelocityOffset");
        offsetWhenNotVisible =
            serializedObject.FindProperty("offsetWhenNotVisible");
        perspectiveChangeSpeed =
            serializedObject.FindProperty("perspectiveChangeSpeed");
        targetTransform = serializedObject.FindProperty("targetTransform");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.PropertyField(mode);
        EditorGUILayout.PropertyField(
            targetTransform,
            new GUIContent("Target",
                "Transform to follow."));
        EditorGUILayout.PropertyField(
            cameraOcclusionLayerMask,
            new GUIContent(
                "Layer Mask",
                "If target gets behind an object specified by the layer mask " +
                "it will be considered as not visible."));
        EditorGUILayout.PropertyField(
            cameraOffset,
            new GUIContent(
                "Camera Offset",
                "Default camera position relative to the target."));
        EditorGUILayout.PropertyField(lookAtPointOffset);
        EditorGUILayout.PropertyField(followSpeed);
        EditorGUILayout.PropertyField(
            cameraRotationSpeed,
            new GUIContent(
                "Cam. Rot. Speed",
                ""));
        EditorGUILayout.PropertyField(perspectiveChangeSpeed);
        EditorGUILayout.PropertyField(movementVelocityOffset);
        EditorGUILayout.PropertyField(cameraLimits);
        EditorGUILayout.PropertyField(lookAtPointWhenNotVisible);
        EditorGUILayout.PropertyField(
            offsetWhenNotVisible,
            new GUIContent(
                "Offset When Not Visible",
                "Offset applied to 'Camera Offset' when target is not " +
                "visible to the camera."));

        serializedObject.ApplyModifiedProperties();
    }
}
