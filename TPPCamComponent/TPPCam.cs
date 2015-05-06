using UnityEngine;

namespace TPPCamera.TPPCamComponent {

    public sealed class TPPCam : MonoBehaviour {

        #region FIELDS

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

        // camera follow target
        private Vector3 cameraTarget;

        // velocity check
        private Vector3 lastTargetPos;

        // movement
        private Vector3 smoothCamOffset;

        private Vector3 targetVelocity;
        #endregion FIELDS

        #region UNITY MESSAGES

        private void FixedUpdate() {
            FollowTarget();
        }

        private void Start() {
            smoothCamOffset = cameraOffset;
            cameraTarget = targetTransform.position;
        }

        #endregion UNITY MESSAGES

        #region METHODS

        private bool DetectOccluders() {
            // get distance and direction for raycast
            var cameraOffsetPos = targetTransform.position + cameraOffset;
            // Ray length decreased by 0.1 to not hit the floor.
            var tDist = (targetTransform.position - cameraOffsetPos).magnitude
                        - 0.1f;
            var tDir = (targetTransform.position - cameraOffsetPos).normalized;

            // check if player visible
            RaycastHit hit;
            if (Physics.Raycast(
                cameraOffsetPos + targetVelocity,
                tDir,
                out hit,
                tDist,
                cameraOcclusionLayerMask)) {
                return false;
            }
            return true;
        }

        private void FollowTarget() {
            if (targetTransform == null) return;

            // target speed check
            targetVelocity = (targetTransform.position - lastTargetPos)
                             / Time.fixedDeltaTime;
            targetVelocity = Vector3.Scale(
                targetVelocity,
                movementVelocityOffset);

            // Camera movement speed.
            // Increase camera speed along with target speed.
            var avgVelocity = (Mathf.Abs(targetVelocity.x)
                               + Mathf.Abs(targetVelocity.y)
                               + Mathf.Abs(targetVelocity.z)) / 3;

            var lerpSpeed = (avgVelocity + followSpeed) * Time.fixedDeltaTime;

            // save camera rotation
            var tRot = Quaternion.identity;

            // control camera in Limited mode
            if (mode == Mode.Limited) {
                if (Mathf.Abs(targetTransform.position.x - cameraTarget.x)
                    > cameraLimits.x
                    ||
                    Mathf.Abs(targetTransform.position.z - cameraTarget.z)
                    > cameraLimits.y) {
                    cameraTarget +=
                        (targetTransform.position - cameraTarget).normalized *
                        (targetTransform.position - cameraTarget).magnitude
                        * Time.fixedDeltaTime * followSpeed;
                }
                else {
                    targetVelocity = Vector3.zero;
                }
                cameraTarget = new Vector3(
                    cameraTarget.x,
                    targetTransform.position.y,
                    cameraTarget.z);
            }
            else {
                cameraTarget = targetTransform.position;
            }

            // detect if player is visible and set offset accordingly
            var playerVisible = DetectOccluders();
            var occlusionOffset = cameraOffset;
            var occlusionLookAtPointOffset =
                new Vector3(
                    lookAtPointOffset.x,
                    -smoothCamOffset.y,
                    lookAtPointOffset.y);
            if (!playerVisible) {
                occlusionOffset = offsetWhenNotVisible;
                occlusionLookAtPointOffset = new Vector3(
                    lookAtPointWhenNotVisible.x,
                    -smoothCamOffset.y,
                    lookAtPointWhenNotVisible.z);
            }
            occlusionOffset += targetVelocity;

            // control occlusion offsets
            smoothCamOffset = Vector3.MoveTowards(
                smoothCamOffset,
                occlusionOffset,
                Time.fixedDeltaTime * perspectiveChangeSpeed);
            tRot.SetLookRotation(
                (cameraTarget
                 + (occlusionLookAtPointOffset
                    + (transform.position - cameraTarget)))
                - transform.position,
                targetTransform.up);

            // apply transformations
            transform.position = Vector3.Lerp(
                transform.position,
                cameraTarget + smoothCamOffset,
                lerpSpeed);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                tRot,
                cameraRotationSpeed * Time.fixedDeltaTime);

            // save last target position
            lastTargetPos = targetTransform.position;
        }

        #endregion METHODS
    }

}