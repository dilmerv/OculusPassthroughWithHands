using System.Collections.Generic;
using TMPro;
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
    private Button saveAllAnchors;

    [SerializeField]
    private Button resolveAllAnchors;

    private Dictionary<ulong, GameObject> anchorsToBeSaved = new Dictionary<ulong, GameObject>();

    private int resolvedAnchorCount = 0;

    private void Awake()
    {
        eraseAllAnchorsFromMemory.onClick.AddListener(() => DeleteAll());
        eraseAllAnchorsFromStorage.onClick.AddListener(() => DeleteAll(true));
        resolveAllAnchors.onClick.AddListener(() => ResolveAllAnchors());
        saveAllAnchors.onClick.AddListener(() => SaveAllAnchors());
    }

    private void DeleteAll(bool fromStorage = false)
    {
        // deep copy of anchors
        // needed to avoid dealing with decrements from original anchor reference
        var anchors = new Dictionary<ulong, GameObject>(SpatialAnchorsManager.Instance.resolvedAnchors);
        Logger.Instance.LogInfo($"DeleteAll(fromStorage:{fromStorage}) anchors found:{anchors.Keys.Count}");

        foreach (var anchor in anchors)
        {
            Logger.Instance.LogInfo($"Attempting to delete anchor:{anchor.Key}");

            if (fromStorage)
                SpatialAnchorsManager.Instance.EraseAnchor(anchor.Key);
            else
                SpatialAnchorsManager.Instance.DestroyAnchor(anchor.Key);           

            Logger.Instance.LogInfo($"Finished deleting anchor: {anchor.Key}");
        }
    }

    private void ResolveAllAnchors()
    {
        SpatialAnchorsManager.Instance.QueryAllLocalAnchors();
        EnableEraseFeatures();
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            saveAllAnchors.interactable = true;
            CreateAnchor();
        }
    }

    private void CreateAnchor()
    {
        ulong anchorHandle = SpatialAnchorsManager.Instance.CreateSpatialAnchor(anchorTransform);

        if (anchorHandle != SpatialAnchorsManager.invalidAnchorHandle)
        {
            // create a new anchor on the current session
            GameObject newAnchor = Instantiate(SpatialAnchorsManager.Instance.anchorPrefab);
            newAnchor.GetComponentInChildren<TextMeshProUGUI>().text = $"{anchorHandle}";

            SpatialAnchorsManager.Instance.resolvedAnchors.Add(anchorHandle, newAnchor);

            // add it to a list so we can make them persistent
            anchorsToBeSaved.Add(anchorHandle, newAnchor);

            EnableEraseFeatures();
        }
        else
        {
            Logger.Instance.LogError("Unable to create a new anchor");
        }
    }

    private void EnableEraseFeatures()
    {
        eraseAllAnchorsFromStorage.interactable = true;
        eraseAllAnchorsFromMemory.interactable = true;
    }

    private void SaveAllAnchors()
    {
        foreach (var handle in anchorsToBeSaved)
        {
            SpatialAnchorsManager.Instance.SaveAnchor(handle.Key, SpatialAnchorsManager.StorageLocation.LOCAL);
        }
    }
}
