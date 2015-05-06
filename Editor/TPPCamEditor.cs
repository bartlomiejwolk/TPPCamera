using UnityEditor;
using UnityEngine;

namespace TPPCamera.TPPCamComponent {

    [CustomEditor(typeof (TPPCam))]
    public sealed class TPPCamEditor : Editor {

        #region FIELDS

        private TPPCam Script { get; set; }

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
        //private SerializedProperty movementVelocityOffset;
        private SerializedProperty offsetWhenNotVisible;
        private SerializedProperty perspectiveChangeSpeed;
        private SerializedProperty targetTransform;

        #endregion

        #region UNITY MESSAGES

        private void OnEnable() {
            cameraLimits = serializedObject.FindProperty("cameraLimits");
            cameraOcclusionLayerMask =
                serializedObject.FindProperty("cameraOcclusionLayerMask");
            cameraOffset = serializedObject.FindProperty("cameraOffset");
            cameraRotationSpeed =
                serializedObject.FindProperty("cameraRotationSpeed");
            followSpeed = serializedObject.FindProperty("followSpeed");
            lookAtPointOffset =
                serializedObject.FindProperty("lookAtPointOffset");
            lookAtPointWhenNotVisible =
                serializedObject.FindProperty("lookAtPointWhenNotVisible");
            mode = serializedObject.FindProperty("mode");
            //movementVelocityOffset =
            //    serializedObject.FindProperty("movementVelocityOffset");
            offsetWhenNotVisible =
                serializedObject.FindProperty("offsetWhenNotVisible");
            perspectiveChangeSpeed =
                serializedObject.FindProperty("perspectiveChangeSpeed");
            targetTransform = serializedObject.FindProperty("targetTransform");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            DrawTargetTransformField();
            DrawFollowSpeedField();
            DrawCameraRotationSpeedField();
            DrawPerspectiveChangeSpeedField();
            DrawModeDropdown();
            DrawOcclusionLayerMaskDropdown();

            GUILayout.Label("Camera Offset", EditorStyles.boldLabel);

            DrawCameraOffsetField();
            DrawOffsetWhenNotVisibleField();

            GUILayout.Label("Look At Point Offset", EditorStyles.boldLabel);

            DrawLookAtPointOffsetField();
            DrawLookAtPointWhenNotVisibleField();

            GUILayout.Label("Other", EditorStyles.boldLabel);

            DrawCameraLimitsField();
            //DrawMovementVelocityOffsetField();

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region INSPECTOR

        //private void DrawMovementVelocityOffsetField() {
        //    EditorGUILayout.PropertyField(
        //        movementVelocityOffset,
        //        new GUIContent(
        //            "Movement Velocity Offset",
        //            "???\nTarget velocity will be scaled by this value. " +
        //            "In result, this affects camera lerp speed and occlusion " +
        //            "offset."));
        //}

        private void DrawCameraLimitsField() {
            EditorGUILayout.PropertyField(
                cameraLimits,
                new GUIContent(
                    "Camera Limits",
                    "Applies in 'Limited' mode."));
        }

        private void DrawLookAtPointWhenNotVisibleField() {
            EditorGUILayout.PropertyField(
                lookAtPointWhenNotVisible,
                new GUIContent(
                    "Target Not Visible",
                    "Use to offset target pivot point when target is occluded."));
        }

        private void DrawOffsetWhenNotVisibleField() {
            EditorGUILayout.PropertyField(
                offsetWhenNotVisible,
                new GUIContent(
                    "Target Not Visible",
                    "Offset applied to 'Camera Offset' when target is not " +
                    "visible to the camera."));
        }

        private void DrawPerspectiveChangeSpeedField() {
            EditorGUILayout.PropertyField(
                perspectiveChangeSpeed,
                new GUIContent(
                    "Perspective Speed",
                    "When target gets occluded and camera wants to change " +
                    "position to see the target again, this is how fast this " +
                    "will happen."));
        }

        private void DrawLookAtPointOffsetField() {
            EditorGUILayout.PropertyField(
                lookAtPointOffset,
                new GUIContent(
                    "Target Visible",
                    "Camera rotation offset."));
        }

        private void DrawCameraOffsetField() {
            EditorGUILayout.PropertyField(
                cameraOffset,
                new GUIContent(
                    "Target Visible",
                    "Default camera position relative to the target."));
        }

        private void DrawOcclusionLayerMaskDropdown() {
            EditorGUILayout.PropertyField(
                cameraOcclusionLayerMask,
                new GUIContent(
                    "Layer Mask",
                    "If target gets behind an object specified by the layer mask "
                    + "it will be considered as occluded."));
        }

        private void DrawModeDropdown() {
            EditorGUILayout.PropertyField(
                mode,
                new GUIContent(
                    "Mode",
                    "Limited mode creates a dead zone around the target where "
                    +
                    "target can move freely without causing the camera to move. "
                    +
                    "Use 'Camera Limits' to set the dead zone."));
        }

        private void DrawCameraRotationSpeedField() {
            EditorGUILayout.PropertyField(
                cameraRotationSpeed,
                new GUIContent(
                    "Cam. Rot. Speed",
                    ""));
        }

        private void DrawFollowSpeedField() {
            EditorGUILayout.PropertyField(followSpeed);
        }

        private void DrawTargetTransformField() {
            EditorGUILayout.PropertyField(
                targetTransform,
                new GUIContent(
                    "Target",
                    "Transform to follow."));
        }

        #endregion INSPECTOR

    }

}
