﻿// Copyright (c) 2015 Bartlomiej Wolk (bartlomiejwolk@gmail.com)
// 
// This file is part of the TPPCamera extension for Unity. Licensed under the
// MIT license. See LICENSE file in the project root folder.

using UnityEditor;
using UnityEngine;

namespace TPPCamera.TPPCamSetterComponent {

    [CustomEditor(typeof (TPPCamSetter))]
    public sealed class TPPCamSetterEditor : Editor {
        #region SERIALIZED PROPERTIES

        private SerializedProperty cameraCo;

        #endregion SERIALIZED PROPERTIES

        private TPPCamSetter Script { get; set; }

        #region UNITY MESSAGES

        public override void OnInspectorGUI() {
            serializedObject.Update();

            DrawCameraComponentField();
            DrawPropertiesDropdown();

            EditorGUILayout.Space();

            GUILayout.Label("TPPCam Properties", EditorStyles.boldLabel);

            HandleDrawCameraOffsetField();
            HandleDrawLookAtPointOffsetField();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawCameraComponentField() {
            EditorGUILayout.PropertyField(
                cameraCo,
                new GUIContent(
                    "TPPCam Comp.",
                    "TPPCam component reference."));
        }

        private void OnEnable() {
            Script = (TPPCamSetter) target;

            cameraCo = serializedObject.FindProperty("cameraCo");
        }

        #endregion UNITY MESSAGES

        #region INSPECTOR

        private void DrawPropertiesDropdown() {
            Script.Properties = (Properties) EditorGUILayout.EnumMaskField(
                new GUIContent(
                    "Properties",
                    "Properties of the TPPCam component that can be updated."),
                Script.Properties);
        }

        private void HandleDrawCameraOffsetField() {
            var enabled = FlagsHelper.IsSet(
                Script.Properties,
                Properties.CameraOffset);

            if (!enabled) return;

            Script.TppCamProperties.CameraOffset =
                EditorGUILayout.Vector3Field(
                    new GUIContent(
                        "Camera Offset",
                        "Default camera position relative to the target."),
                    Script.TppCamProperties.CameraOffset);
        }

        private void HandleDrawLookAtPointOffsetField() {
            var enabled = FlagsHelper.IsSet(
                Script.Properties,
                Properties.LookAtPointOffset);

            if (!enabled) return;

            Script.TppCamProperties.LookAtPointOffset =
                EditorGUILayout.Vector2Field(
                    new GUIContent(
                        "Look At Point Offset",
                        "Default camera rotation"),
                    Script.TppCamProperties.LookAtPointOffset);
        }

        #endregion INSPECTOR
    }

}