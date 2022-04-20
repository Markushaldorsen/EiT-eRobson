using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class CircuitComponent
    {
        public string Name { get; }

        public GameObject ModelOn { get; set; }
        public GameObject ModelOff { get; set; }

        public Type ComponentType { get; }
        public bool HasPower { get; set; }

        private bool IsOn { get; set; } = false;
        
        

        public CircuitComponent(string name, GameObject modelOn, GameObject modelOff, Type componentType)
        {
            Name = name;
            ModelOn = modelOn;
            ModelOff = modelOff;
            ComponentType = componentType;
        }

        public void UpdatePosition(Vector3 position, Quaternion rotation)
        {
            // ModelOn.transform.SetPositionAndRotation(position, Quaternion.identity);
            // ModelOff.transform.SetPositionAndRotation(position, Quaternion.identity);
            ModelOn.transform.SetPositionAndRotation(position, rotation);
            ModelOff.transform.SetPositionAndRotation(position, rotation);

        }

        public void SetOn(bool on)
        {
            if (on && ComponentType == Type.NeedPower && !HasPower)
            {
                Debug.Log("<debug> SetOn: device needs power but does not have it");
                IsOn = false;
                return;
            }

            Debug.Log("<debug> SetOn success, val: " + on);
            IsOn = on;
            ModelOn.SetActive(on);
            ModelOff.SetActive(!on);
        }

        public void SetActive()
        {
            if (IsOn){
                ModelOn.SetActive(true);
            } else {
                ModelOff.SetActive(true);
            }
        }

        public void SetInactive(){
            ModelOn.SetActive(false);
            ModelOff.SetActive(false);
        }
    }

    public enum Type
    {
        NeedPower,
        GivesPower,
        PassesPower
    }
}