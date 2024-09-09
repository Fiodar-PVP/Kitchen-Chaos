using System;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{
    [SerializeField] private PlatesCounter platesCounter;
    [SerializeField] private Transform counterTopPoint;
    [SerializeField] private Transform plateVisual;
    [SerializeField] private float plateVisualOffSetY = 0.1f;

    private List<GameObject> platesVisualGameObjects;

    private void Awake()
    {
        platesVisualGameObjects = new List<GameObject>();
    }

    private void Start()
    {
        platesCounter.OnPlatesSpawned += PlatesCounter_OnPlatesSpawned;
        platesCounter.OnPlatesRemoved += PlatesCounter_OnPlatesRemoved;
    }

    private void PlatesCounter_OnPlatesRemoved(object sender, EventArgs e)
    {
        GameObject lastSpawnedPlate = platesVisualGameObjects[platesVisualGameObjects.Count - 1];
        platesVisualGameObjects.Remove(lastSpawnedPlate);
        Destroy(lastSpawnedPlate);
    }

    private void PlatesCounter_OnPlatesSpawned(object sender, EventArgs e)
    {
        Transform plateVisualTransform = Instantiate(plateVisual, counterTopPoint);

        plateVisualTransform.localPosition = new Vector3(0, plateVisualOffSetY * platesVisualGameObjects.Count, 0);

        platesVisualGameObjects.Add(plateVisualTransform.gameObject);
    }
}
