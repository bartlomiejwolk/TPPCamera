using UnityEditor;

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

        EditorGUILayout.PropertyField(cameraLimits);
        EditorGUILayout.PropertyField(cameraOcclusionLayerMask);
        EditorGUILayout.PropertyField(cameraOffset);
        EditorGUILayout.PropertyField(cameraRotationSpeed);
        EditorGUILayout.PropertyField(followSpeed);
        EditorGUILayout.PropertyField(lookAtPointOffset);
        EditorGUILayout.PropertyField(lookAtPointWhenNotVisible);
        EditorGUILayout.PropertyField(mode);
        EditorGUILayout.PropertyField(movementVelocityOffset);
        EditorGUILayout.PropertyField(offsetWhenNotVisible);
        EditorGUILayout.PropertyField(perspectiveChangeSpeed);
        EditorGUILayout.PropertyField(targetTransform);

        serializedObject.ApplyModifiedProperties();
    }
}
