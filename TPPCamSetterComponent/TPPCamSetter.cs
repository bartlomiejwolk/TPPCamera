using TPPCamera.TPPCamComponent;
using UnityEngine;

namespace TPPCamera.TPPCamSetterComponent {

    // todo extract to file
    public enum Properties {

        CameraOffset = 0x0001,
        LookAtPointOffset = 0x0002

    }

    public class TPPCamSetter : MonoBehaviour {

        #region FIELDS

        [SerializeField]
        private TPPCam cameraCo;

        [SerializeField]
        private Properties properties;

        #endregion FIELDS

        #region TPPCam FIELDS
        [SerializeField]
        private Vector3 cameraOffset;

        [SerializeField]
        private Vector2 lookAtPointOffset;
        #endregion

        public Properties Properties {
            get { return properties; }
            set { properties = value; }
        }

    }

}
