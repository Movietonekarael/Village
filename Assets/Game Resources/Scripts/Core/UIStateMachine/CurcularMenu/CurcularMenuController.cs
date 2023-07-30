using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace GameCore
{
    namespace GUI
    {
        public enum CurcularMenuType : int
        {
            Auto = 0,
            OneCell = 1,
            TwoCells = 2,
            ThreeCells = 3,
            FourCells = 4,
            FiveCells = 5,
            SixCells = 6,
            SevenCells = 7,
            EightCells = 8,
            NineCells = 9,
            TenCells = 10
        };

        public class CurcularMenuController : UIController<CurcularMenuViewParameters, 
                                                           ICurcularMenuController, 
                                                           CurcularMenuView,
                                                           ICurcularMenuView>, 
                                              ICurcularMenuController
        {
            protected override void InitializeParameters(CurcularMenuViewParameters parameters)
            {
                throw new System.NotImplementedException();
            }

            protected override void OnActivate()
            {
                throw new System.NotImplementedException();
            }

            protected override void OnDeactivate()
            {
                throw new System.NotImplementedException();
            }

            protected override void SubscribeForPermanentEvents()
            {
                throw new System.NotImplementedException();
            }

            protected override void UnsubscribeForPermanentEvents()
            {
                throw new System.NotImplementedException();
            }

            protected override void SubscribeForTemporaryEvents()
            {
                throw new System.NotImplementedException();
            }

            protected override void UnsubscribeForTemporaryEvents()
            {
                throw new System.NotImplementedException();
            }
        }

        public class CurcularMenuUIState : BaseUIState<CurcularMenuViewParameters, CurcularMenuController, ICurcularMenuController>
        {
            protected override void StartState(params bool[] args)
            {
                throw new System.NotImplementedException();
            }

            protected override void EndState()
            {
                throw new System.NotImplementedException();
            }
        }

        public interface ICurcularMenuController : ISpecificController
        {

        }

        public interface ICurcularMenuView : ISpecificView
        {

        }

        [CreateAssetMenu(fileName = "View Data", menuName = "Game UI/View panels data/Curcular Menu", order = 5)]
        public class CurcularMenuViewParameters : ScriptableObject, IUIParameters
        { 

        }
    }
}