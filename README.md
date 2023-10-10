# Здравствуйте, меня зовут Александр.
## Это мой домашний проект игры на Unity

![](https://i.ibb.co/3YyRL39/3.jpg)

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
* Unity Network for GameObjects (NGO), Relay;
* Unity Job System

____
Система перемещения основана на машине состояний. [Ссылка](https://bitbucket.org/movietoneofficial/village/src/PublicShow/Assets/Game%20Resources/Scripts/Core/Movement/) на папку в репозитории.
В файле [NPCMovementStateMachine.cs](https://bitbucket.org/movietoneofficial/village/src/PublicShow/Assets/Game%20Resources/Scripts/Core/Movement/NPCMovementStateMachine.cs) описан класс машины состояний.
Состаяния подразделяются на супер-состояния и под-состояния.
К супер-состояниям относятся классы описанные в [NPCMovementGroundedState.cs](https://bitbucket.org/movietoneofficial/village/src/PublicShow/Assets/Game%20Resources/Scripts/Core/Movement/NPCMovementGroundedState.cs) и [NPCMovementJumpState.cs](https://bitbucket.org/movietoneofficial/village/src/PublicShow/Assets/Game%20Resources/Scripts/Core/Movement/NPCMovementJumpState.cs)
К под-состояниям: [NPCMovementIdleState.cs](https://bitbucket.org/movietoneofficial/village/src/PublicShow/Assets/Game%20Resources/Scripts/Core/Movement/NPCMovementIdleState.cs), [NPCMovementWalkState.cs](https://bitbucket.org/movietoneofficial/village/src/PublicShow/Assets/Game%20Resources/Scripts/Core/Movement/NPCMovementWalkState.cs), [NPCMovementRunState.cs](https://bitbucket.org/movietoneofficial/village/src/PublicShow/Assets/Game%20Resources/Scripts/Core/Movement/NPCMovementRunState.cs)
![](https://i.ibb.co/7zvWP47/1.jpg)

____
Для ввода используется Unity Input System. Ввод обрабатывается классом [InputHandler](https://bitbucket.org/movietoneofficial/village/src/PublicShow/Assets/Game%20Resources/Scripts/Core/InputSystem/InputHandler.cs). InputHandler создаётся из ProjectContext (Zenject) и в последующем инжектится во все необходимые объекты.

____
Для взаимодействия с интерактивными объектами используются скрипты в папке [InteractSystem](https://bitbucket.org/movietoneofficial/village/src/PublicShow/Assets/Game%20Resources/Scripts/Core/InteractSystem/).
Классы интерактивных объектов, как [GameItemMono](https://bitbucket.org/movietoneofficial/village/src/PublicShow/Assets/Game%20Resources/Scripts/Core/Inventory/GameItemMono.cs), подписываются под интерфейс [IInteractable](https://bitbucket.org/movietoneofficial/village/src/PublicShow/Assets/Game%20Resources/Scripts/Core/InteractSystem/IInteractable.cs).
Для взаимодействия с ними на GameObject игрока накинут [Interactor](https://bitbucket.org/movietoneofficial/village/src/PublicShow/Assets/Game%20Resources/Scripts/Core/InteractSystem/Interactor.cs).

____

Скрипты инвентаря хранятся в папке [Inventory](https://bitbucket.org/movietoneofficial/village/src/PublicShow/Assets/Game%20Resources/Scripts/Core/Inventory/).
Все объекты, имеющие инвентарь, реализуют интерфейс [IInventory](https://bitbucket.org/movietoneofficial/village/src/PublicShow/Assets/Game%20Resources/Scripts/Core/Inventory/IInventory.cs).
Инвентарь игрока описывает класс [PlayerInventory](https://bitbucket.org/movietoneofficial/village/src/PublicShow/Assets/Game%20Resources/Scripts/Core/Inventory/PlayerInventory/PlayerInventory.cs).

![](https://i.ibb.co/cLGHBdx/2.jpg)

____

В папке [UI](https://bitbucket.org/movietoneofficial/village/src/PublicShow/Assets/Game%20Resources/Scripts/Core/UI/) находятся скрипты связанные с пользовательским интерфеском:
* Машина состояний для интервейса;
* Состояния, реализующие MVC паттерн;
* Окна, для ввода кода подключения и вывода сообщений об ошибках;

Во время игры возможна мгновенная смена управления на геймпад со сменой подсказок в интерфейсе и появлением виктуального курсора для работы с инвентарём.

![](https://i.ibb.co/fCzpJW4/5.jpg)

____

В игре реальзован мультиплеер для 5-ти человек с использованием NGO и Relay. Для отзывчивости управления перемещение в персонажей в мультиплеере реализовано по принципу Player Authoritative.

____

В игре доступна физика костей. В [этой папке](https://bitbucket.org/movietoneofficial/village/src/PublicShow/Assets/Game%20Resources/Scripts/JiggleBones/) находятся скрипты переписанные мной под Unity Job System из репозитория [UnityJigglePhysics](https://github.com/naelstrof/UnityJigglePhysics).

____

В игре используется библиотека Addressables для асинхронной загрузки игровых ассетов. ProjectContext из Zenject переписан под загрузку из ассет бандлов адрессаблсов, а не из папки ресурсов.

![](https://i.ibb.co/VQJGkwv/10.jpg)

____

Для персонажей создан стилизованый шейдер с использованием Shader Graph.

![](https://i.ibb.co/gTjzZ2y/9.jpg)

____

Единственый на данный момент персонаж создан самостоятельно в Blender. Для оптимизации в игре для каждой части персонажа было создано до 5-ти лодов. Текстурирование производилось в Substance Painter.

![](https://i.ibb.co/dQ3RvLC/6.jpg)
![](https://i.ibb.co/wNbXVxZ/7.jpg)

##### Игра доступна для скачивания [здесь](https://drive.google.com/file/d/1DeWqxPi5ow6d7k4frS0GNNk94iFeGkLx/view?usp=drive_link).