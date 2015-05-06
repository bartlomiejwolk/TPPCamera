using UnityEngine;

namespace TPPCamera.TPPCamSetterComponent {

    [System.Serializable]
    public sealed class TPPCamProperties {

        [SerializeField]
        private Vector3 cameraOffset;

        [SerializeField]
        private Vector2 lookAtPointOffset;

        public Vector3 CameraOffset {
            get { return cameraOffset; }
            set { cameraOffset = value; }
        }

        public Vector2 LookAtPointOffset {
            get { return lookAtPointOffset; }
            set { lookAtPointOffset = value; }
        }

    }

}
