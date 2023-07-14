namespace GameCore
{
    namespace Interactions
    {
        public interface IInteractable
        {
            public string InteractionMessage { get; }

            public bool Interact(Interactor interactor);
        }
    }
}