# Здравствуйте, меня зовут Александр.
## Это мой домашний проект игры на Unity

![](https://i.ibb.co/Qkt566M/3.jpg)

Все скрипты проекта, написанные мной, хранятся здесь [Assets/Game Resouces/Scripts/](https://bitbucket.org/movietoneofficial/village/src/master/Assets/Game%20Resources/Scripts/)


В игре на данный момент реализованы следующие системы:
* Система передвижения;
* Система инвентаря;
* Система взаимодействия;
* Машина состояний для UI;
* Всплывающие окна для сообщений и ввода;
* Player authoritative мультиплеер до 5 человек.

Стэк дополнительных технологий, использованных в игре:
* Zenject
* Addressables;
* Unity Network for GameObjects (NGO)
* Unity Job System

____
Система перемещения основана на машине состояний [Movement State Mashine](https://bitbucket.org/movietoneofficial/village/src/master/Assets/Resouces/Scripts/Core/Controls/Movement%20State%20Mashine/)
В файле NPCMovement.cs описан класс машины состояний.
Состаяния подразделяются на супер-состояния и под-состояния.
К супер-состояниям относятся классы описанные в [NPCMovementGroundedState.cs](https://bitbucket.org/movietoneofficial/village/src/master/Assets/Resouces/Scripts/Core/Controls/Movement%20State%20Mashine/NPCMovementGroundedState.cs) и [NPCMovementJumpState.cs](https://bitbucket.org/movietoneofficial/village/src/master/Assets/Resouces/Scripts/Core/Controls/Movement%20State%20Mashine/NPCMovementJumpState.cs)
К под-состояниям: [NPCMovementIdleState.cs](https://bitbucket.org/movietoneofficial/village/src/master/Assets/Resouces/Scripts/Core/Controls/Movement%20State%20Mashine/NPCMovementIdleState.cs), [NPCMovementWalkState.cs](https://bitbucket.org/movietoneofficial/village/src/master/Assets/Resouces/Scripts/Core/Controls/Movement%20State%20Mashine/NPCMovementWalkState.cs), [NPCMovementRunState.cs](https://bitbucket.org/movietoneofficial/village/src/master/Assets/Resouces/Scripts/Core/Controls/Movement%20State%20Mashine/NPCMovementRunState.cs)

![](https://lh3.googleusercontent.com/drive-viewer/AJc5JmRIxSmhc9TrSAUgN-kJt8BfCMBOISXW_MG6nER5C4pLdICyo8qtj4WbjDN-PRlRcuUv90z5ZAg=w1920-h969)
____
Для ввода используется Unity Input System. Весь ввод обрабатывается классом [PlayerController](https://bitbucket.org/movietoneofficial/village/src/master/Assets/Resouces/Scripts/Core/Controls/PlayerController.cs). В классе реализован паттерн singleton. Все объекты, которым нужен ввод подписываются к событиям экземпляра этого класса.
____

Для взаимодействия с интерактивными объектами используются скрипты в папке [InteractSystem](https://bitbucket.org/movietoneofficial/village/src/master/Assets/Resouces/Scripts/Core/InteractSystem/).
Классы интерактивных объектов, как [Food](https://bitbucket.org/movietoneofficial/village/src/master/Assets/Resouces/Scripts/Core/InteractSystem/Food.cs), наследуют интерфейс [IInteractable](https://bitbucket.org/movietoneofficial/village/src/master/Assets/Resouces/Scripts/Core/InteractSystem/IInteractable.cs).
Для взаимодействия с ними на GameObject игрока накинут [Interactor](https://bitbucket.org/movietoneofficial/village/src/master/Assets/Resouces/Scripts/Core/InteractSystem/Interactor.cs).

____

Скрипты инвентаря хранятся в [Inventory](https://bitbucket.org/movietoneofficial/village/src/master/Assets/Resouces/Scripts/Core/Inventory/).
Все объекты, имеющие инвентарь, реализуют интерфейс [IInventory](https://bitbucket.org/movietoneofficial/village/src/master/Assets/Resouces/Scripts/Core/Inventory/IInventory.cs).
Инвентарь игрока описывает класс [PlayerInventory](https://bitbucket.org/movietoneofficial/village/src/master/Assets/Resouces/Scripts/Core/Inventory/PlayerInventory.cs). Игровой интерфейс открытого инвентаря управляется классом [PlayerInventoryPanelUI](https://bitbucket.org/movietoneofficial/village/src/master/Assets/Resouces/Scripts/Core/Inventory/PlayerInventoryPanelUI.cs). Панель снизу, отображающая быстродоступные элементы инвентаря, управляется классом [FastInventoryPanelUI](https://bitbucket.org/movietoneofficial/village/src/master/Assets/Resouces/Scripts/Core/Inventory/FastInventoryPanelUI.cs).

![](https://lh3.googleusercontent.com/drive-viewer/AJc5JmQ-sq4yTgf6Wb_hmqZRrPIhdfqXmUAfUZ8Uj6i8BlP9pseaZYYFnu_aSFlC82c2QCu1EHJ_m6c=w1920-h969)

![](https://lh3.googleusercontent.com/drive-viewer/AJc5JmSjF2cKlrVN6qjMTFqfqK2XUwXrB6N9djD0CC6LGByXALuD33dslVfbqkkHQM4ECn9K8eqbbkg=w1920-h969)

____

##### Игра доступна для скачивания [здесь](https://drive.google.com/file/d/1V3xDTf3tZiV95Gqm-K6exu4DKfgINBat/view?usp=sharing).
Управление WASD.
Переключения по панели быстрого доступа 1-8.
Взаимодействие E.
Открытие инвентаря TAB.
Прыжок Space.