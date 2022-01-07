using UnityEngine;

public class HandGestures : MonoBehaviour
{
    [SerializeField]
    private OVRHand ovrHand;

    [SerializeField]
    private OVRHand.Hand HandType = OVRHand.Hand.None;

    private void OnEnable()
    {
        if(ovrHand == null)
        {
            Logger.Instance.LogError("ovrHand must be set in the inspector...");
        }
        else
        {
            Logger.Instance.LogInfo("ovrHand was set correctly in the inspector...");
        }
    }
    void Update()
    {
        // index finger pinch creates an anchor
        if(ovrHand.GetFingerIsPinching(OVRHand.HandFinger.Index))
        {
            Logger.Instance.LogInfo($"Hand ({HandType}) Pinch Strength ({ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Index)})");
        }
    }
}
