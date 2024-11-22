using System.Collections;
using UnityEngine;

namespace Player.Grapple
{
    public class GrapplingGun : MonoBehaviour
    {
        private Vector2 grapplePoint;

        [SerializeField] private LayerMask grappleLayer;
        [SerializeField] private float maxDistance = 15;
        [SerializeField] private float launchSpeed = 1;

        [SerializeField] private float grappledSpeed = 0.5f;

        [SerializeField] private int resolution, waveCount, wobbleCount;
        [SerializeField] private float waveSize, animSpeed;

        private LineRenderer line;

        public Transform gunTip, player;
        public Camera a_camera;
        public SpringJoint2D joint;
        public Rigidbody2D body;

        private bool isGrappling = false;

        public SoundEffectPlayer soundEffects;

        void Start()
        {
            line = GetComponent<LineRenderer>();
            joint.enabled = false;
        }

        private void Update()
        {
            // If player is left-clicking, start the grapple
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                StartGrapple();
            }
            // If left-click is released, stop the grapple
            else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                StopGrapple();
            }
            // Only pull grapple if it is active
            else if (isGrappling)
            {
                PullGrapple();
            }
        }

        private void StartGrapple()
        {
            Vector2 mousePos = a_camera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(gunTip.position, mousePos - (Vector2)gunTip.position, maxDistance, grappleLayer);

            if (hit.collider != null)
            {
                if (Vector2.Distance(hit.point, gunTip.position) <= maxDistance)
                {
                    grapplePoint = hit.point;
                    StartCoroutine(AnimRope(grapplePoint));
                    Grapple();
                    isGrappling = true;

                    if (soundEffects != null)
                    {
                        soundEffects.grappleSound();
                    }
                }
            }
        }

        private void StopGrapple()
        {
            joint.enabled = false;
            StopRope();
            isGrappling = false;
        }

        private void PullGrapple()
        {
            Vector3 grapplePos = grapplePoint;
            Vector3 playerPos = player.position;
            float delta = launchSpeed * Time.deltaTime * grappledSpeed;

            player.position = Vector3.Lerp(playerPos, grapplePos, delta);
        }

        public void Grapple()
        {
            joint.autoConfigureDistance = false;
            joint.connectedAnchor = grapplePoint;

            Vector3 distanceVector = gunTip.position - player.position;

            joint.distance = distanceVector.magnitude;
            joint.frequency = launchSpeed;
            joint.enabled = true; // Ensure the joint is enabled
        }

        public IEnumerator AnimRope(Vector3 targetPos)
        {
            line.positionCount = resolution;
            float angle = LookAtAngle(targetPos - transform.position);

            float percent = 0;
            while (percent <= 1f)
            {
                percent += Time.deltaTime * animSpeed;
                SetPoints(targetPos, percent, angle);
                yield return null;
            }
            SetPoints(targetPos, 1, angle);
        }

        public void StopRope()
        {
            StopAllCoroutines();
            line.positionCount = 0;
        }

        private void SetPoints(Vector3 targetPos, float percent, float angle)
        {
            Vector3 ropeEnd = Vector3.Lerp(transform.position, targetPos, percent);
            float length = Vector2.Distance(transform.position, ropeEnd);

            for (int i = 0; i < resolution; i++)
            {
                float xPos = (float)i / resolution * length;
                float reversePercent = (1 - percent);

                float amplitude = Mathf.Sin((reversePercent * wobbleCount * Mathf.PI) * waveSize);

                float yPos = Mathf.Sin((float)waveCount * i / resolution * 2 * Mathf.PI * reversePercent) * amplitude;

                Vector2 pos = RotatePoint(new Vector2(xPos + transform.position.x, yPos + transform.position.y), transform.position, angle);
                line.SetPosition(i, pos);
            }
        }

        Vector2 RotatePoint(Vector2 point, Vector2 pivot, float angle)
        {
            Vector2 dir = point - pivot;
            dir = Quaternion.Euler(0, 0, angle) * dir;
            point = dir + pivot;
            return point;
        }

        private float LookAtAngle(Vector2 target)
        {
            return Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        }
    }
}