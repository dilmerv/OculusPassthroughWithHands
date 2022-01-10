using System.Collections.Generic;
using System.Linq;
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

    private int prevResolvedAnchorCount = 0;

    private void Awake()
    {
        eraseAllAnchorsFromMemory.onClick.AddListener(() => DeleteAll());
        eraseAllAnchorsFromStorage.onClick.AddListener(() => DeleteAll(true));
        resolveAllAnchors.onClick.AddListener(() => ResolveAllAnchors());
        saveAllAnchors.onClick.AddListener(() => SaveAllAnchors());
    }

    private void DeleteAll(bool fromStorage = false)
    {
        var anchors = SpatialAnchorsManager.Instance.resolvedAnchors.Keys.ToList();

        Logger.Instance.LogInfo($"DeleteAll(fromStorage:{fromStorage}) anchors found:{anchors.Count}");

        foreach (var anchor in anchors)
        {
            Logger.Instance.LogInfo($"Attempting to delete anchor:{anchor}");

            if (fromStorage)
                SpatialAnchorsManager.Instance.EraseAnchor(anchor);
            else
                SpatialAnchorsManager.Instance.DestroyAnchor(anchor);           

            Logger.Instance.LogInfo($"Finished deleting anchor: {anchor}");
        }
    }

    private void ResolveAllAnchors() => SpatialAnchorsManager.Instance.QueryAllLocalAnchors();

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            CreateAnchor();
        }

        if (prevResolvedAnchorCount != SpatialAnchorsManager.Instance.resolvedAnchors.Count)
        {
            prevResolvedAnchorCount = SpatialAnchorsManager.Instance.resolvedAnchors.Count;
            AdditionalFeatures(prevResolvedAnchorCount > 0);
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
        }
        else
        {
            Logger.Instance.LogError("Unable to create a new anchor");
        }
    }

    private void AdditionalFeatures(bool state = true)
    {
        saveAllAnchors.interactable = state;
        eraseAllAnchorsFromStorage.interactable = state;
        eraseAllAnchorsFromMemory.interactable = state;
    }

    private void SaveAllAnchors()
    {
        foreach (var handle in anchorsToBeSaved)
        {
            SpatialAnchorsManager.Instance.SaveAnchor(handle.Key, SpatialAnchorsManager.StorageLocation.LOCAL);
        }
    }
}
