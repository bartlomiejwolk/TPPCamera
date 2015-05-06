﻿using TPPCamera.TPPCamComponent;
using UnityEditor;
using UnityEngine;
using Vexe.Runtime.Extensions;

namespace TPPCamera.TPPCamSetterComponent {

    [CustomEditor(typeof(TPPCamSetter))]
    class TPPCamSetterEditor : Editor {

        private TPPCamSetter Script { get; set; }

        #region SERIALIZED PROPERTIES

        private SerializedProperty cameraCo;
        //private SerializedProperty cameraOffset;
        //private SerializedProperty lookAtPointOffset;

        #endregion
        #region UNITY MESSAGES

        private void OnEnable() {
            Script = (TPPCamSetter) target;

            cameraCo = serializedObject.FindProperty("cameraCo");

            // TPPCam properties.
            //cameraOffset = serializedObject.FindProperty("cameraOffset");
            //lookAtPointOffset =
            //    serializedObject.FindProperty("lookAtPointOffset");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.PropertyField(cameraCo,
                new GUIContent(
                    "TPPCam Comp.",
                    ""));

            DrawPropertiesDropdown();

            EditorGUILayout.Space();

            GUILayout.Label("TPPCam Properties", EditorStyles.boldLabel);

            HandleDrawCameraOffsetField();
            HandleDrawLookAtPointOffsetField();

            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region INSPECTOR
        private void DrawPropertiesDropdown() {
            Script.Properties = (Properties)EditorGUILayout.EnumMaskField(
                new GUIContent(
                    "Properties",
                    ""),
                Script.Properties);
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
                        ""),
                    Script.TppCamProperties.LookAtPointOffset);
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

        #endregion INSPECTOR

    }

}
