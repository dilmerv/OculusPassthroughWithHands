using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpatialAnchorsPlacement : MonoBehaviour
{
    [SerializeField]
    private Transform anchorTransform;

    [SerializeField]
    private Button eraseAllAnchorsFromStorage;

    [SerializeField]
    private Button eraseAllAnchorsFromMemory;

    [SerializeField]
    private Button resolveAllAnchors;

    private List<ulong> anchorsCreated = new List<ulong>();

    private void Awake()
    {
        eraseAllAnchorsFromMemory.interactable = false;
        eraseAllAnchorsFromMemory.onClick.AddListener(() =>
        {
            Logger.Instance.LogInfo("UI eraseAllAnchorsFromMemory executed");
            DeleteAll();
        });

        eraseAllAnchorsFromStorage.interactable = false;
        eraseAllAnchorsFromStorage.onClick.AddListener(() =>
        {
            Logger.Instance.LogInfo("UI eraseAllAnchorsFromStorage executed");
            DeleteAll(true);
        });

        resolveAllAnchors.onClick.AddListener(() =>
        {
            Logger.Instance.LogInfo("UI resolveAllAnchors executed");
            ResolveAllAnchors();
            eraseAllAnchorsFromStorage.interactable = true;
            eraseAllAnchorsFromMemory.interactable = true;
        });
    }

    private static void DeleteAll(bool fromStorage = false)
    {
        var anchors = SpatialAnchorsManager.Instance.resolvedAnchors;
        foreach (var anchor in anchors)
        {
            if(fromStorage)
                SpatialAnchorsManager.Instance.EraseAnchor(anchor.Key);
            else
                SpatialAnchorsManager.Instance.DestroyAnchor(anchor.Key);
        }
    }

    private void ResolveAllAnchors() => SpatialAnchorsManager.Instance.QueryAllLocalAnchors();

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)) CreateAnchor();
        if (OVRInput.GetDown(OVRInput.Button.One))                   SaveAllAnchors();
        if (OVRInput.GetDown(OVRInput.Button.Two))                   ResolveAllAnchors();
    }

    private void CreateAnchor()
    {
        Logger.Instance.LogInfo("Button.SecondaryIndexTrigger: creating anchor");
        ulong anchorHandle = SpatialAnchorsManager.Instance.CreateSpatialAnchor(anchorTransform);

        if (anchorHandle != SpatialAnchorsManager.invalidAnchorHandle)
        {
            // create a new anchor on the current session
            GameObject newAnchor = Instantiate(SpatialAnchorsManager.Instance.anchorPrefab);
            SpatialAnchorsManager.Instance.resolvedAnchors.Add(anchorHandle, newAnchor);

            // add it to a list so we can make them persistent
            anchorsCreated.Add(anchorHandle);
        }
        else
        {
            Logger.Instance.LogError("Unable to create a new anchor");
        }
    }

    private void SaveAllAnchors()
    {
        Logger.Instance.LogInfo("Button.One: saving anchors");
        foreach (var handle in anchorsCreated)
        {
            SpatialAnchorsManager.Instance.SaveAnchor(handle, SpatialAnchorsManager.StorageLocation.LOCAL);
        }
    }
}
