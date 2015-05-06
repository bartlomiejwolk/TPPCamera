using UnityEngine;

namespace TPPCamera.TPPCamComponent {

    public sealed class TPPCam : MonoBehaviour {

        #region FIELDS
        // velocity check
        private Vector3 lastTargetPos;

        // movement
        private Vector3 smoothCamOffset;

        private Vector3 targetVelocity;

        // camera follow target
        private Vector3 cameraTarget;
        #endregion FIELDS

        #region INSPECTOR FIELDS
        // limited camera
        [SerializeField]
        private Vector2 cameraLimits = new Vector2(5f, 5f);

        [SerializeField]
        private LayerMask cameraOcclusionLayerMask = 1 << 9;

        [SerializeField]
        private Vector3 cameraOffset;

        [SerializeField]
        private float cameraRotationSpeed = 5f;

        [SerializeField]
        private float followSpeed = 5f;

        [SerializeField]
        private Vector2 lookAtPointOffset;

        // see through camera variables
        [SerializeField]
        private Vector3 lookAtPointWhenNotVisible;

        [SerializeField]
        private Mode mode;

        [SerializeField]
        private Vector3 movementVelocityOffset;

        [SerializeField]
        private Vector3 offsetWhenNotVisible;

        [SerializeField]
        private float perspectiveChangeSpeed = 10f;

        // main variables
        [SerializeField]
        private Transform targetTransform;
        #endregion

        #region PROPERTIES
        public Vector2 CameraLimits {
            get { return cameraLimits; }
            set { cameraLimits = value; }
        }

        public LayerMask CameraOcclusionLayerMask {
            get { return cameraOcclusionLayerMask; }
            set { cameraOcclusionLayerMask = value; }
        }

        public Vector3 CameraOffset {
            get { return cameraOffset; }
            set { cameraOffset = value; }
        }

        public float CameraRotationSpeed {
            get { return cameraRotationSpeed; }
            set { cameraRotationSpeed = value; }
        }

        public float FollowSpeed {
            get { return followSpeed; }
            set { followSpeed = value; }
        }

        public Vector2 LookAtPointOffset {
            get { return lookAtPointOffset; }
            set { lookAtPointOffset = value; }
        }

        public Vector3 LookAtPointWhenNotVisible {
            get { return lookAtPointWhenNotVisible; }
            set { lookAtPointWhenNotVisible = value; }
        }

        public Mode Mode {
            get { return mode; }
            set { mode = value; }
        }

        public Vector3 MovementVelocityOffset {
            get { return movementVelocityOffset; }
            set { movementVelocityOffset = value; }
        }

        public Vector3 OffsetWhenNotVisible {
            get { return offsetWhenNotVisible; }
            set { offsetWhenNotVisible = value; }
        }

        public float PerspectiveChangeSpeed {
            get { return perspectiveChangeSpeed; }
            set { perspectiveChangeSpeed = value; }
        }

        public Transform TargetTransform {
            get { return targetTransform; }
            set { targetTransform = value; }
        }

        private Vector3 CameraTarget {
            get { return cameraTarget; }
            set { cameraTarget = value; }
        }

        private Vector3 TargetVelocity {
            get { return targetVelocity; }
            set { targetVelocity = value; }
        }

        private Vector3 LastTargetPos {
            get { return lastTargetPos; }
            set { lastTargetPos = value; }
        }

        private Vector3 SmoothCamOffset {
            get { return smoothCamOffset; }
            set { smoothCamOffset = value; }
        }

        #endregion
        #region UNITY MESSAGES

        private void FixedUpdate() {
            FollowTarget();
        }

        private void Start() {
            SmoothCamOffset = CameraOffset;
            CameraTarget = TargetTransform.position;
        }

        #endregion UNITY MESSAGES

        #region METHODS

        private bool DetectOccluders() {
            // get distance and direction for raycast
            var cameraOffsetPos = TargetTransform.position + CameraOffset;
            // Ray length decreased by 0.1 to not hit the floor.
            var tDist =
                (TargetTransform.position - cameraOffsetPos).magnitude
                - 0.1f;
            var tDir =
                (TargetTransform.position - cameraOffsetPos).normalized;

            // check if player visible
            RaycastHit hit;
            if (Physics.Raycast(
                cameraOffsetPos + TargetVelocity,
                tDir,
                out hit,
                tDist,
                CameraOcclusionLayerMask)) {
                return false;
            }
            return true;
        }

        private void FollowTarget() {
            if (TargetTransform == null) return;

            // target speed check
            TargetVelocity = (TargetTransform.position - LastTargetPos)
                             / Time.fixedDeltaTime;

            TargetVelocity = Vector3.Scale(
                TargetVelocity,
                MovementVelocityOffset);

            // Camera movement speed.
            // Increase camera speed along with target speed.
            var avgVelocity = (Mathf.Abs(TargetVelocity.x)
                               + Mathf.Abs(TargetVelocity.y)
                               + Mathf.Abs(TargetVelocity.z)) / 3;

            var lerpSpeed = (avgVelocity + FollowSpeed)
                * Time.fixedDeltaTime;

            // save camera rotation
            var tRot = Quaternion.identity;

            HandleMode();

            // detect if player is visible and set offset accordingly
            var playerVisible = DetectOccluders();
            var occlusionOffset = CameraOffset;
            var occlusionLookAtPointOffset =
                new Vector3(
                    LookAtPointOffset.x,
                    -SmoothCamOffset.y,
                    LookAtPointOffset.y);
            if (!playerVisible) {
                occlusionOffset = OffsetWhenNotVisible;
                occlusionLookAtPointOffset = new Vector3(
                    LookAtPointWhenNotVisible.x,
                    -SmoothCamOffset.y,
                    LookAtPointWhenNotVisible.z);
            }
            occlusionOffset += TargetVelocity;

            // control occlusion offsets
            SmoothCamOffset = Vector3.MoveTowards(
                SmoothCamOffset,
                occlusionOffset,
                Time.fixedDeltaTime * PerspectiveChangeSpeed);
            tRot.SetLookRotation(
                (CameraTarget
                 + (occlusionLookAtPointOffset
                    + (transform.position - CameraTarget)))
                - transform.position,
                TargetTransform.up);

            // apply transformations
            transform.position = Vector3.Lerp(
                transform.position,
                CameraTarget + SmoothCamOffset,
                lerpSpeed);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                tRot,
                CameraRotationSpeed * Time.fixedDeltaTime);

            // save last target position
            LastTargetPos = TargetTransform.position;
        }

        private void HandleMode() {
            // control camera in Limited mode
            if (Mode == Mode.Limited) {
                if (Mathf.Abs(TargetTransform.position.x - CameraTarget.x)
                    > CameraLimits.x
                    ||
                    Mathf.Abs(TargetTransform.position.z - CameraTarget.z)
                    > CameraLimits.y) {

                    CameraTarget +=
                        (TargetTransform.position - CameraTarget).normalized
                        * (TargetTransform.position - CameraTarget).magnitude
                        * Time.fixedDeltaTime * FollowSpeed;
                }
                else {
                    TargetVelocity = Vector3.zero;
                }
                CameraTarget = new Vector3(
                    CameraTarget.x,
                    TargetTransform.position.y,
                    CameraTarget.z);
            }
            else {
                CameraTarget = TargetTransform.position;
            }
        }

        #endregion METHODS
    }

}