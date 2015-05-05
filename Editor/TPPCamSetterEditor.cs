using TPPCamera.CameraComponent;
using UnityEditor;
using UnityEngine;

namespace TPPCamera.CameraSetupComponent {

    [CustomEditor(typeof(TPPCamSetter))]
    class TPPCamSetterEditor : Editor {

        private TPPCam Script { get; set; }

        #region SERIALIZED PROPERTIES

        private SerializedProperty cameraCo;
        private SerializedProperty cameraOffset;
        private SerializedProperty lookAtPointOffset;

        #endregion
        #region UNITY MESSAGES

        private void OnEnable() {
            cameraCo = serializedObject.FindProperty("cameraCo");

            // TPPCam properties.
            cameraOffset = serializedObject.FindProperty("cameraOffset");
            lookAtPointOffset =
                serializedObject.FindProperty("lookAtPointOffset");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.PropertyField(cameraCo,
                new GUIContent(
                    "TPPCam Comp.",
                    ""));

            EditorGUILayout.Space();

            GUILayout.Label("TPPCam Properties", EditorStyles.boldLabel);

            DrawCameraOffsetField();
            DrawLookAtPointOffsetField();

            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region INSPECTOR

        private void DrawLookAtPointOffsetField() {
            EditorGUILayout.PropertyField(
                lookAtPointOffset,
                new GUIContent(
                    "Look At Point Offset",
                    ""));
        }

        private void DrawCameraOffsetField() {
            EditorGUILayout.PropertyField(
                cameraOffset,
                new GUIContent(
                    "Camera Offset",
                    "Default camera position relative to the target."));
        }

        #endregion INSPECTOR

    }

}
