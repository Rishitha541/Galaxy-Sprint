using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 5.5f, -8f);
    [SerializeField] private float followSmoothness = 8f;
    [SerializeField] private float lookAhead = 7f;

    private void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSmoothness * Time.deltaTime);

        Vector3 lookTarget = target.position + Vector3.forward * lookAhead + Vector3.up * 1.5f;
        transform.LookAt(lookTarget);
    }
}
