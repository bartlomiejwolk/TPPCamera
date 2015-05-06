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

        #endregion
        #region UNITY MESSAGES

        private void FixedUpdate() {
            FollowTarget();
        }

        private void Start() {
            smoothCamOffset = CameraOffset;
            cameraTarget = TargetTransform.position;
        }

        #endregion UNITY MESSAGES

        #region METHODS

        private bool DetectOccluders() {
            // get distance and direction for raycast
            var cameraOffsetPos = TargetTransform.position + CameraOffset;
            // Ray length decreased by 0.1 to not hit the floor.
            var tDist = (TargetTransform.position - cameraOffsetPos).magnitude
                        - 0.1f;
            var tDir = (TargetTransform.position - cameraOffsetPos).normalized;

            // check if player visible
            RaycastHit hit;
            if (Physics.Raycast(
                cameraOffsetPos + targetVelocity,
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
            targetVelocity = (TargetTransform.position - lastTargetPos)
                             / Time.fixedDeltaTime;
            targetVelocity = Vector3.Scale(
                targetVelocity,
                MovementVelocityOffset);

            // Camera movement speed.
            // Increase camera speed along with target speed.
            var avgVelocity = (Mathf.Abs(targetVelocity.x)
                               + Mathf.Abs(targetVelocity.y)
                               + Mathf.Abs(targetVelocity.z)) / 3;

            var lerpSpeed = (avgVelocity + FollowSpeed) * Time.fixedDeltaTime;

            // save camera rotation
            var tRot = Quaternion.identity;

            // control camera in Limited mode
            if (Mode == Mode.Limited) {
                if (Mathf.Abs(TargetTransform.position.x - cameraTarget.x)
                    > CameraLimits.x
                    ||
                    Mathf.Abs(TargetTransform.position.z - cameraTarget.z)
                    > CameraLimits.y) {
                    cameraTarget +=
                        (TargetTransform.position - cameraTarget).normalized *
                        (TargetTransform.position - cameraTarget).magnitude
                        * Time.fixedDeltaTime * FollowSpeed;
                }
                else {
                    targetVelocity = Vector3.zero;
                }
                cameraTarget = new Vector3(
                    cameraTarget.x,
                    TargetTransform.position.y,
                    cameraTarget.z);
            }
            else {
                cameraTarget = TargetTransform.position;
            }

            // detect if player is visible and set offset accordingly
            var playerVisible = DetectOccluders();
            var occlusionOffset = CameraOffset;
            var occlusionLookAtPointOffset =
                new Vector3(
                    LookAtPointOffset.x,
                    -smoothCamOffset.y,
                    LookAtPointOffset.y);
            if (!playerVisible) {
                occlusionOffset = OffsetWhenNotVisible;
                occlusionLookAtPointOffset = new Vector3(
                    LookAtPointWhenNotVisible.x,
                    -smoothCamOffset.y,
                    LookAtPointWhenNotVisible.z);
            }
            occlusionOffset += targetVelocity;

            // control occlusion offsets
            smoothCamOffset = Vector3.MoveTowards(
                smoothCamOffset,
                occlusionOffset,
                Time.fixedDeltaTime * PerspectiveChangeSpeed);
            tRot.SetLookRotation(
                (cameraTarget
                 + (occlusionLookAtPointOffset
                    + (transform.position - cameraTarget)))
                - transform.position,
                TargetTransform.up);

            // apply transformations
            transform.position = Vector3.Lerp(
                transform.position,
                cameraTarget + smoothCamOffset,
                lerpSpeed);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                tRot,
                CameraRotationSpeed * Time.fixedDeltaTime);

            // save last target position
            lastTargetPos = TargetTransform.position;
        }

        #endregion METHODS
    }

}