using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "View Data", menuName = "Game UI/View panels data/MainScreen", order = 1)]
public sealed class MainScreenViewParameters : ScriptableObject
{
    public GameObject CanvasPrefab;
    public GameObject ItemCellPrefab;
    public uint NumberOfItemsToShow = 8;
    public Vector2Int ItemIndent = new(80, -80);
    public Vector2Int ItemDelta = new(100, -100);
}
