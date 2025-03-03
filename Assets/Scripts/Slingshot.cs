using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [Header("Inscribed")]
    public GameObject projectilePrefab;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;
    public Transform leftAnchor, rightAnchor; // Anchor points for rubber band
    public LineRenderer lineRenderer; // Line Renderer component
    public AudioSource audioSource; // Audio Source for sound effect
    public AudioClip slingshotReleaseSound; // Sound effect

    [Header("Dynamic")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;

    void Awake()
    {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;

        // Initialize Line Renderer
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        lineRenderer.positionCount = 3; // 3 points: left anchor, projectile, right anchor
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.enabled = false; // Hide initially
    }

    void OnMouseEnter() => launchPoint.SetActive(true);
    void OnMouseExit() => launchPoint.SetActive(false);

    void OnMouseDown()
    {
        aimingMode = true;
        projectile = Instantiate(projectilePrefab);
        projectile.transform.position = launchPos;
        projectile.GetComponent<Rigidbody>().isKinematic = true;

        lineRenderer.enabled = true; // Show rubber band when aiming
    }

    void Update()
    {
        if (!aimingMode) return;

        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);
        Vector3 mouseDelta = mousePos3D - launchPos;

        float maxMagnitude = GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        // Update Line Renderer positions
        lineRenderer.SetPosition(0, leftAnchor.position);
        lineRenderer.SetPosition(1, projPos);
        lineRenderer.SetPosition(2, rightAnchor.position);

        if (Input.GetMouseButtonUp(0))
        {
            aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.linearVelocity = -mouseDelta * velocityMult;

            FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);
            FollowCam.POI = projectile;
            Instantiate(projLinePrefab, projectile.transform);
            projectile = null;
            MissionDemolition.SHOT_FIRED();

            // Hide the rubber band after release
            lineRenderer.enabled = false;

            // Play slingshot release sound
            if (audioSource && slingshotReleaseSound)
            {
                audioSource.PlayOneShot(slingshotReleaseSound);
            }
        }
    }
}
