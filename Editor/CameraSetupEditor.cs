using TPPCamera.CameraComponent;
using UnityEditor;

namespace TPPCamera.CameraSetupComponent {

    [CustomEditor(typeof(CameraSetup))]
    class CameraSetupEditor : Editor {

        private Camera Script { get; set; }

        private SerializedProperty cameraCo;

        private void OnEnable() {
            cameraCo = serializedObject.FindProperty("cameraCo");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.PropertyField(cameraCo);

            serializedObject.ApplyModifiedProperties();
        }

    }

}
