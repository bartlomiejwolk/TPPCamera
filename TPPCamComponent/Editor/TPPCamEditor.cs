// Copyright (c) 2015 Bartlomiej Wolk (bartlomiejwolk@gmail.com)
// 
// This file is part of the TPPCamera extension for Unity. Licensed under the
// MIT license. See LICENSE file in the project root folder.

using TPPCamera.TPPCamSetterComponent;
using UnityEditor;
using UnityEngine;

namespace TPPCamera.TPPCamComponent {

    [CustomEditor(typeof (TPPCam))]
    public sealed class TPPCamEditor : Editor {
        #region FIELDS

        private TPPCam Script { get; set; }

        #endregion FIELDS

        #region SERIALIZED PROPERTIES

        private SerializedProperty cameraOcclusionLayerMask;
        private SerializedProperty cameraOffset;
        private SerializedProperty cameraOffsetLerpSpeed;
        private SerializedProperty cameraRotationSpeed;
        private SerializedProperty deadZone;
        private SerializedProperty followSpeed;
        private SerializedProperty lookAtPointOffset;
        private SerializedProperty lookAtPointWhenNotVisible;
        private SerializedProperty mode;
        private SerializedProperty offsetWhenNotVisible;
        private SerializedProperty targetTransform;

        #endregion SERIALIZED PROPERTIES

        #region UNITY MESSAGES

        public override void OnInspectorGUI() {
            serializedObject.Update();

            DrawVersionLabel();
            DrawTargetTransformField();
            DrawFollowSpeedField();
            DrawCameraOffsetLerpSpeedField();
            DrawCameraRotationSpeedField();
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

            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable() {
            Script = (TPPCam) target;

            deadZone = serializedObject.FindProperty("deadZone");
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
            offsetWhenNotVisible =
                serializedObject.FindProperty("offsetWhenNotVisible");
            cameraOffsetLerpSpeed =
                serializedObject.FindProperty("cameraOffsetLerpSpeed");
            targetTransform = serializedObject.FindProperty("targetTransform");
        }

        #endregion UNITY MESSAGES

        #region INSPECTOR
        private void DrawVersionLabel() {
            EditorGUILayout.LabelField(
                string.Format(
                    "{0} ({1})",
                    TPPCam.VERSION,
                    TPPCam.EXTENSION));
        }


        private void DrawCameraLimitsField() {
            EditorGUILayout.PropertyField(
                deadZone,
                new GUIContent(
                    "DeadZone",
                    "Applies in 'DeadZone' mode."));
        }

        private void DrawCameraOffsetField() {
            EditorGUILayout.PropertyField(
                cameraOffset,
                new GUIContent(
                    "Target Visible",
                    "Default camera position relative to the target."));
        }

        private void DrawCameraOffsetLerpSpeedField() {
            EditorGUILayout.PropertyField(
                cameraOffsetLerpSpeed,
                new GUIContent(
                    "Camera Mov. Speed",
                    "Camera movement speed."));
        }

        private void DrawCameraRotationSpeedField() {
            EditorGUILayout.PropertyField(
                cameraRotationSpeed,
                new GUIContent(
                    "Cam. Rot. Speed",
                    "Camera rotation speed."));
        }

        private void DrawFollowSpeedField() {
            EditorGUILayout.PropertyField(followSpeed);
        }

        private void DrawLookAtPointOffsetField() {
            EditorGUILayout.PropertyField(
                lookAtPointOffset,
                new GUIContent(
                    "Target Visible",
                    "Default camera rotation offset."));
        }

        private void DrawLookAtPointWhenNotVisibleField() {
            EditorGUILayout.PropertyField(
                lookAtPointWhenNotVisible,
                new GUIContent(
                    "Target Not Visible",
                    "Camera rotation when target transform is occluded."));
        }

        private void DrawModeDropdown() {
            EditorGUILayout.PropertyField(
                mode,
                new GUIContent(
                    "Mode",
                    "DeadZone mode creates a dead zone around the target where "
                    +
                    "target can move freely without causing the camera to move. "
                    +
                    "Use 'DeadZone' to set the dead zone."));
        }

        private void DrawOcclusionLayerMaskDropdown() {
            EditorGUILayout.PropertyField(
                cameraOcclusionLayerMask,
                new GUIContent(
                    "Layer Mask",
                    "If target gets behind an object specified by the layer mask "
                    + "it will be considered as occluded."));
        }

        private void DrawOffsetWhenNotVisibleField() {
            EditorGUILayout.PropertyField(
                offsetWhenNotVisible,
                new GUIContent(
                    "Target Not Visible",
                    "Camera position offset when target is not " +
                    "visible to the camera."));
        }

        private void DrawTargetTransformField() {
            EditorGUILayout.PropertyField(
                targetTransform,
                new GUIContent(
                    "Target",
                    "Transform to follow."));
        }

        #endregion INSPECTOR

        #region METHODS

        [MenuItem("Component/TPPCamera/TPPCam")]
        private static void AddTPPCamComponent() {
            if (Selection.activeGameObject != null) {
                Selection.activeGameObject.AddComponent(typeof (TPPCam));
            }
        }

        [MenuItem("Component/TPPCamera/TPPCamSetter")]
        private static void AddTPPCamSetterComponent() {
            if (Selection.activeGameObject != null) {
                Selection.activeGameObject.AddComponent(typeof (TPPCamSetter));
            }
        }
        #endregion METHODS
    }

}