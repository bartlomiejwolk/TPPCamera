using TPPCamera.TPPCamComponent;
using UnityEngine;

namespace TPPCamera.TPPCamSetterComponent {

    // todo extract to file
    public enum Properties {

        CameraOffset = 0x0001,
        LookAtPointOffset = 0x0002

    }

    public sealed class TPPCamSetter : MonoBehaviour {

        #region FIELDS

        [SerializeField]
        private TPPCam cameraCo;

        [SerializeField]
        private TPPCamProperties tppCamProperties;

        [SerializeField]
        private Properties properties;

        #endregion FIELDS

        #region PROPERTIES
        /// <summary>
        /// Keeps info about what properties of the target camera component
        /// should be updated.
        /// </summary>
        public Properties Properties {
            get { return properties; }
            set { properties = value; }
        }

        /// <summary>
        /// Properties used to update the target camera component.
        /// </summary>
        public TPPCamProperties TppCamProperties {
            get { return tppCamProperties; }
            set { tppCamProperties = value; }
        }

        public TPPCam CameraCo {
            get { return cameraCo; }
            set { cameraCo = value; }
        }

        #endregion

        #region METHODS

        public void UpdateTPPCam() {
            HandleUpdateCameraOffset();
        }

        private void HandleUpdateCameraOffset() {
            var enabled = FlagsHelper.IsSet(
                Properties,
                Properties.CameraOffset);

            if (!enabled) return;

            CameraCo.CameraOffset = TppCamProperties.CameraOffset;
        }

        private void HandleUpdateLookAtPointOffset() {
            var enabled = FlagsHelper.IsSet(
                Properties,
                Properties.LookAtPointOffset);

            if (!enabled) return;

            CameraCo.LookAtPointOffset = TppCamProperties.LookAtPointOffset;
        }


        #endregion
    }

}
