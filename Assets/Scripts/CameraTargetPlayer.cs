using UnityEngine;
using Unity.Cinemachine;

public class CameraTargetPlayer : MonoBehaviour
{
    [SerializeField] private PlayerInteraction interaction;
    [SerializeField] private CinemachineCamera camNormal;
    [SerializeField] private CinemachineCamera camSideClimb;

    private CinemachinePanTilt panTilt;
    private bool wasSideClimbing = false;

    private void Start()
    {
        panTilt = camSideClimb.GetComponent<CinemachinePanTilt>();
    }

    private void LateUpdate()
    {
        bool isSideClimbing = interaction.IsClimbing && interaction.ClimbedFromSide;

        if (isSideClimbing && !wasSideClimbing)
        {
            // Set pan direction based on which side the wall is on
            if (panTilt != null)
                panTilt.PanAxis.Value = interaction.ClimbWallNormal.x < 0 ? 90f : -90f;

            camSideClimb.Priority = 20;
            camNormal.Priority = 10;
        }
        else if (!isSideClimbing && wasSideClimbing)
        {
            camNormal.Priority = 20;
            camSideClimb.Priority = 10;
        }

        wasSideClimbing = isSideClimbing;
    }
}