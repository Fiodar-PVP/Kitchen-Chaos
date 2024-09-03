using UnityEngine;

/// <summary>
/// A ScriptableObject that holds data for a kitchen object, including its prefab, sprite, and name.
/// </summary>
[CreateAssetMenu()]
public class KitchenObjectSO : ScriptableObject
{
    public Transform prefab;
    public Sprite sprite;
    public string objectName;
}
