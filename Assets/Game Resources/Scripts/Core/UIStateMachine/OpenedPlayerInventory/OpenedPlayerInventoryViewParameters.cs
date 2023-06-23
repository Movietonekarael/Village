using GameCore.GUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "View Data", menuName = "Game UI/View panels data/Player Inventory", order = 2)]
public sealed class OpenedPlayerInventoryViewParameters : ScriptableObject, IUIParameters
{
    public GameObject CanvasPrefab;
    public GameObject ButtonPrefab;
    public GameObject DragAndDropPrefab;
    public uint NumberOfItemsInRow = 8;
    public Vector2Int ItemIndent = new(80, -80);
    public Vector2Int ItemDelta = new(100, -100);
    [HideInInspector] public int ItemsNumber;
}
