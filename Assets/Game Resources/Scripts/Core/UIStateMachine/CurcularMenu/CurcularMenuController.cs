using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GameCore.GUI
{
    public enum CurcularMenuType : int
    {
        Auto = 0,
        OneCell,
        TwoCells,
        ThreeCells,
        FourCells,
        FiveCells,
        SixCells,
        SevenCells,
        EightCells,
        NineCells,
        TenCells
    };

    public class CurcularMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject _cellPrefab;

        [SerializeField] private CurcularMenuType _curcularMenuType;

        [SerializeField] private List<Sprite> _spritesOfCellItems;
        [SerializeField] private List<UnityEvent> _events;
        private CurcularCell[] _cells;

        private void OnEnable()
        {

        }

        private void OnDisable()
        {
            
        }

        private void Start()
        {
            InstantiateCells();
        }

        private void InstantiateCells()
        {
            /*
            switch(_curcularMenuType)
            {

            }
            */
        }
    }
}

