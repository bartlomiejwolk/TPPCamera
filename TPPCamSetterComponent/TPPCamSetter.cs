// Copyright (c) 2015 Bartlomiej Wolk (bartlomiejwolk@gmail.com)
// 
// This file is part of the TPPCamera extension for Unity. Licensed under the
// MIT license. See LICENSE file in the project root folder.

using TPPCamera.TPPCamComponent;
using UnityEngine;

namespace TPPCamera.TPPCamSetterComponent {

    public sealed class TPPCamSetter : MonoBehaviour {
        #region FIELDS

        [SerializeField]
        private TPPCam cameraCo;

        [SerializeField]
        private Properties properties;

        [SerializeField]
        private TPPCamProperties tppCamProperties;

        #endregion FIELDS

        #region PROPERTIES

        public TPPCam CameraCo {
            get { return cameraCo; }
            set { cameraCo = value; }
        }

        /// <summary>
        ///     Keeps info about what properties of the target camera component
        ///     should be updated.
        /// </summary>
        public Properties Properties {
            get { return properties; }
            set { properties = value; }
        }

        /// <summary>
        ///     Properties used to update the target camera component.
        /// </summary>
        public TPPCamProperties TppCamProperties {
            get { return tppCamProperties; }
            set { tppCamProperties = value; }
        }

        #endregion PROPERTIES

        #region METHODS

        public void UpdateTPPCam() {
            HandleUpdateCameraOffset();
        }

        private void HandleUpdateCameraOffset() {
            var isSet = FlagsHelper.IsSet(
                Properties,
                Properties.CameraOffset);

            if (!isSet) return;

            CameraCo.CameraOffset = TppCamProperties.CameraOffset;
        }

        private void HandleUpdateLookAtPointOffset() {
            var isSet = FlagsHelper.IsSet(
                Properties,
                Properties.LookAtPointOffset);

            if (!isSet) return;

            CameraCo.LookAtPointOffset = TppCamProperties.LookAtPointOffset;
        }

        #endregion METHODS
    }

}