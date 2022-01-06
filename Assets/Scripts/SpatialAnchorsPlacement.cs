using System.Collections.Generic;
using UnityEngine;

public class SpatialAnchorsPlacement : MonoBehaviour
{
    [SerializeField]
    private Transform anchorTransform;

    private List<ulong> anchorsCreated = new List<ulong>();

    void Update()
    {
        if(OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            Logger.Instance.LogInfo("Button.SecondaryIndexTrigger: creating anchor");
            ulong anchorHandle = SpatialAnchorsManager.Instance.CreateSpatialAnchor(anchorTransform);

            // create a new anchor on the current session
            GameObject newAnchor = Instantiate(SpatialAnchorsManager.Instance.anchorPrefab);
            SpatialAnchorsManager.Instance.resolvedAnchors.Add(anchorHandle, newAnchor);

            // add it to a list so we can make them persistent
            anchorsCreated.Add(anchorHandle);
        }
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            Logger.Instance.LogInfo("Button.One: saving anchors");
            foreach (var handle in anchorsCreated)
            {
                SpatialAnchorsManager.Instance.SaveAnchor(handle, SpatialAnchorsManager.StorageLocation.LOCAL);
            }
        }
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            Logger.Instance.LogInfo("Button.Two: resolving anchors");
            SpatialAnchorsManager.Instance.QueryAllLocalAnchors();
        }
    }
}
