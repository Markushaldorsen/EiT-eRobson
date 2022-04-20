using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ImageTrackingObjectManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Image manager on the AR Session Origin")]
    ARTrackedImageManager m_ImageManager;

    /// <summary>
    /// Get the <c>ARTrackedImageManager</c>
    /// </summary>
    public ARTrackedImageManager ImageManager
    {
        get => m_ImageManager;
        set => m_ImageManager = value;
    }

    [SerializeField]
    [Tooltip("Reference Image Library")]
    XRReferenceImageLibrary m_ImageLibrary;

    /// <summary>
    /// Get the <c>XRReferenceImageLibrary</c>
    /// </summary>
    public XRReferenceImageLibrary ImageLibrary
    {
        get => m_ImageLibrary;
        set => m_ImageLibrary = value;
    }

    [SerializeField]
    [Tooltip("Prefab for tracked 1 image")]
    GameObject[] m_prefabs;

    /// <summary>
    /// Get the one prefab
    /// </summary>
    public GameObject[] prefabs
    {
        get => m_prefabs;
        set => m_prefabs = value;
    }

    List<GameObject> m_spawnedPrefabs;
    
    /// <summary>
    /// get the spawned one prefab
    /// </summary>
    public List<GameObject> SpawnedPrefabs
    {
        get => m_spawnedPrefabs;
        set => m_spawnedPrefabs = value;
    }

    int m_NumberOfTrackedImages;
    
    NumberManager m_OneNumberManager;
    NumberManager m_TwoNumberManager;

    void OnEnable()
    {   
        m_ImageManager.trackedImagesChanged += ImageManagerOnTrackedImagesChanged;
    }

    void OnDisable()
    {
        m_ImageManager.trackedImagesChanged -= ImageManagerOnTrackedImagesChanged;
    }

    void ImageManagerOnTrackedImagesChanged(ARTrackedImagesChangedEventArgs obj)
    {
        // added, spawn prefab
        for(int i = 0; i < obj.added.Count; i++)
        {
            if (obj.added[i].referenceImage.guid == m_ImageLibrary[i].guid)
            {
                SpawnedPrefabs.Add(Instantiate(prefabs[i], obj.added[i].transform.position, obj.added[i].transform.rotation));
            }
        }

        for(int i = 0; i < obj.updated.Count; i++)
        {
            if (obj.updated[i].referenceImage.guid == m_ImageLibrary[i].guid)
            {
                SpawnedPrefabs[i].transform.SetPositionAndRotation(obj.updated[i].transform.position, obj.updated[i].transform.rotation);
            }
        }

        for(int i = 0; i < obj.removed.Count; i++)
        {
            if (obj.removed[i].referenceImage.guid == m_ImageLibrary[i].guid)
            {
                Destroy(SpawnedPrefabs[i]);
            }
        }
    }

    public int NumberOfTrackedImages()
    {
        m_NumberOfTrackedImages = 0;
        foreach (ARTrackedImage image in m_ImageManager.trackables)
        {
            if (image.trackingState == TrackingState.Tracking)
            {
                m_NumberOfTrackedImages++;
            }
        }
        return m_NumberOfTrackedImages;
    }
}