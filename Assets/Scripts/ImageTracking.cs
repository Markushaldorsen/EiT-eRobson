using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageTracking : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> placeablePrefabs;

    private List<CircuitComponent> components = new List<CircuitComponent>();

    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();
    private ARTrackedImageManager trackedImageManager;

    private void Awake()
    {
        SetupComponents();
        trackedImageManager = FindObjectOfType<ARTrackedImageManager>();
    }

    private void SetupComponents()
    {
        components.Add(new CircuitComponent("Battery", placeablePrefabs[0], placeablePrefabs[0], Type.GivesPower));
        components.Add(new CircuitComponent("USB", placeablePrefabs[1], placeablePrefabs[1], Type.PassesPower));
        components.Add(new CircuitComponent("Dimmer", placeablePrefabs[2], placeablePrefabs[2], Type.PassesPower));
        components.Add(new CircuitComponent("Switch", placeablePrefabs[3], placeablePrefabs[4], Type.PassesPower));
        components.Add(new CircuitComponent("LED", placeablePrefabs[5], placeablePrefabs[6], Type.NeedPower));
        components.Add(new CircuitComponent("Motor", placeablePrefabs[7], placeablePrefabs[8], Type.NeedPower));
    }

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += ImageChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= ImageChanged;
    }

    private void ImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach(ARTrackedImage trackedImage in eventArgs.added)
        {
            AddComponent(trackedImage);
        }

        foreach(ARTrackedImage trackedImage1 in eventArgs.updated)
        {
            UpdateComponent(trackedImage1);

            foreach(ARTrackedImage trackedImage2 in eventArgs.updated)
            {
                if (trackedImage1.referenceImage.name != trackedImage2.referenceImage.name)
                {
                    SetHasPower(trackedImage1, trackedImage2);
                }
            }

            SetOnIfHasPower(trackedImage1);
        }


        foreach(ARTrackedImage trackedImage in eventArgs.removed)
        {
            RemoveComponent(trackedImage);
        }
    }

    private void RemoveComponent(ARTrackedImage trackedImage)
    {
        string componentName = trackedImage.referenceImage.name;

        Destroy(components.Find(c => c.Name == componentName).ModelOn);
        Destroy(components.Find(c => c.Name == componentName).ModelOff);
    }

    private void AddComponent(ARTrackedImage trackedImage)
    {
        string componentName = trackedImage.referenceImage.name;
        Vector3 position = trackedImage.transform.position;

        var modelOn = components.Find(c => c.Name == componentName).ModelOn;
        var modelOff = components.Find(c => c.Name == componentName).ModelOff;

        components.Find(c => c.Name == componentName).ModelOn = Instantiate(modelOn, position, Quaternion.identity);
        components.Find(c => c.Name == componentName).ModelOff = Instantiate(modelOff, position, Quaternion.identity);

        components.Find(c => c.Name == componentName).SetOn(false);
    }

    private void UpdateComponent(ARTrackedImage trackedImage)
    {
        string componentName = trackedImage.referenceImage.name;

        if (trackedImage.trackingState == TrackingState.Tracking) 
        {
            Vector3 position = trackedImage.transform.position;
            Quaternion rotation = trackedImage.transform.rotation;

            components.Find(c => c.Name == componentName).SetActive();
            components.Find(c => c.Name == componentName).UpdatePosition(position, rotation);
        } 
        else 
        {
            components.Find(c => c.Name == componentName).SetInactive();
        }
    }

    private void SetHasPower(ARTrackedImage trackedImage1, ARTrackedImage trackedImage2)
    {
        string componentName1 = trackedImage1.referenceImage.name;
        string componentName2 = trackedImage2.referenceImage.name;

        CircuitComponent component1 = components.Find(c => c.Name == componentName1);
        CircuitComponent component2 = components.Find(c => c.Name == componentName2);

        if (trackedImage1.trackingState == TrackingState.Tracking && 
            trackedImage2.trackingState == TrackingState.Tracking)
        {
            Vector3 position1 = trackedImage1.transform.position;
            Vector3 position2 = trackedImage2.transform.position;

            float distance = Vector3.Distance(position1, position2);
            //print("<debug> Distance to other: " + distance);
        
            if (distance <= 0.1)
            { 
                if (PassesPower(component1, component2))
                {
                    //print("<debug> Set HasPower to true");
                    component1.HasPower = true;
                    component2.HasPower = true;
                }
            } 
            else 
            {
                component1.HasPower = false;
                component2.HasPower = false;
            }
        }
    }
    
    private bool PassesPower(CircuitComponent component1, CircuitComponent component2)
    {
        if (component1.ComponentType == Type.GivesPower ||
            component2.ComponentType == Type.GivesPower ||

            (component1.ComponentType == Type.PassesPower &&  
            component1.HasPower) ||
        
            (component2.ComponentType == Type.PassesPower &&  
            component2.HasPower) ||

            (component1.ComponentType == Type.NeedPower &&  
            component1.HasPower) ||

            (component2.ComponentType == Type.NeedPower &&  
            component2.HasPower))
        {
            return true;
        }
        return false;
    }

    private void SetOnIfHasPower(ARTrackedImage trackedImage)
    {
        string componentName = trackedImage.referenceImage.name;
        CircuitComponent component = components.Find(c => c.Name == componentName);

        if (component.HasPower) 
        {
            component.SetOn(true);
        } 
        else 
        {
            component.SetOn(false);
        }
    }
}