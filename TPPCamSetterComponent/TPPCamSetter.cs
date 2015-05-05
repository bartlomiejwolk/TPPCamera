using TPPCamera.TPPCamComponent;
using UnityEngine;

namespace TPPCamera.TPPCamSetterComponent {

    public class TPPCamSetter : MonoBehaviour {

        #region FIELDS

        [SerializeField]
        private TPPCam cameraCo;

        [SerializeField]
        private Vector3 cameraOffset;

        [SerializeField]
        private Vector2 lookAtPointOffset;

        #endregion FIELDS
    }

}
