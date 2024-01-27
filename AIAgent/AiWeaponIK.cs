using UnityEngine;

[System.Serializable]
public class HumanBone
{
    public HumanBodyBones bone;
    public float weight = 1.0f;
}

public class AiWeaponIK : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform aimTransform;
    [SerializeField] private Transform lookAtTransform;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Vector3 targetOffset;


    [SerializeField] private int iterations = 10;
    [Range(0, 1)]
    public float weight = 1.0f;

    [SerializeField] private float angleLimit = 90.0f;
    [SerializeField] private float distanceLimit = 1.5f;
    [SerializeField] private HumanBone[] humanBones;
    [SerializeField] private Transform[] boneTransforms;

    // Start is called before the first frame update
    private void Start()
    {
        Animator animator = GetComponent<Animator>();
        boneTransforms = new Transform[humanBones.Length];
        for (int i = 0; i < boneTransforms.Length; i++)
        {
            boneTransforms[i] = animator.GetBoneTransform(humanBones[i].bone);
        }
    }

    private Vector3 GetTargetPosition()
    {
        Vector3 targetDirection = (targetTransform.position + targetOffset) - aimTransform.position;
        Vector3 aimDirection = aimTransform.forward;
        float blendOut = 0.0f;

        float targetAngle = Vector3.Angle(targetDirection, aimDirection);
        if (targetAngle > angleLimit)
        {
            blendOut += (targetAngle - angleLimit) / 50.0f;
        }

        float targetDistance = targetDirection.magnitude;
        if (targetDistance < distanceLimit)
        {
            blendOut += distanceLimit - targetDistance;
        }

        Vector3 direction = Vector3.Slerp(targetDirection, aimDirection, blendOut);
        return aimTransform.position + direction;
    }
    private void FixedUpdate()
    {
        RotateCharacterTowardsTarget();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
       
        if (aimTransform == null || targetTransform == null)
        {
            targetTransform = lookAtTransform;
        }

        Vector3 targetPosition = GetTargetPosition();
        if (boneTransforms != null)
        {
            for (int i = 0; i < iterations; i++)
            {
                for (int b = 0; b < boneTransforms.Length; b++)
                {
                    Transform bone = boneTransforms[b];
                    if (bone != null)
                    {
                        float boneWeight = humanBones[b].weight * weight;
                        AimAtTarget(bone, targetPosition, weight);
                    }
                }
            }
        }
    }


    private void AimAtTarget(Transform bone, Vector3 targetPosition, float weight)
    {
        Vector3 aimDirection = aimTransform.forward;
        Vector3 targetDirection = targetPosition - aimTransform.position;
        Quaternion aimTowards = Quaternion.FromToRotation(aimDirection, targetDirection);
        Quaternion blendedRotation = Quaternion.Slerp(Quaternion.identity, aimTowards, weight);
        bone.rotation = blendedRotation * bone.rotation;
    }

    public void SetTargetTransform(Transform target)
    {
        targetTransform = target;
    }

    private void RotateCharacterTowardsTarget()
    {
        if (aimTransform == null || targetTransform == null)
        {
            return;
        }

        Vector3 targetDirection = (targetTransform.position + targetOffset) - transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

}