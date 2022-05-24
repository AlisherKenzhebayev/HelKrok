using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGrappleClosenessCheck : MonoBehaviour
{
    private List<GameObjectCloseness> objectDict;

    private void Start()
    {
        objectDict = new List<GameObjectCloseness>();
    }

    public bool HasGameObject(GameObject _gameObject) {
        if (objectDict.Count <= 0) {
            return false;
        }

        return objectDict.Exists(o => o.gameObject == _gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        RaycastHit hitinfo;
        other.Raycast(new Ray(this.transform.position, other.transform.position), out hitinfo, 20f);

        float dist = (this.transform.position - other.ClosestPoint(hitinfo.point)).magnitude;

        objectDict.Add(new GameObjectCloseness(other.gameObject, dist));
    }

    private void OnTriggerExit(Collider other)
    {
        objectDict.RemoveAll(o => o.gameObject.GetInstanceID() == other.gameObject.GetInstanceID());
    }
}

[Serializable]
public class GameObjectCloseness : IComparable {
    public GameObject gameObject;
    public float distance;

    public GameObjectCloseness(GameObject _gameObject, float _distance) {
        gameObject = _gameObject;
        distance = _distance;
    }

    public int CompareTo(object obj)
    {
        if (obj == null) return -1;

        GameObjectCloseness otherItem = obj as GameObjectCloseness;

        if (otherItem != null)
        {
            return this.distance.CompareTo(otherItem.distance);
        }
        else
        {
            throw new ArgumentException("Object is not a GameObjectCloseness");
        }
    }
}