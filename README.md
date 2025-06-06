# Unity Projectile System

The `Scripts` folder contains code from a projectile shooting system used in a game.
(For context: the game is a **space shooter** genre, where a spaceship fires various types of projectiles.)

* By combining simple shooting patterns (e.g. **3 projectiles at once**, **spiral pattern**) with various settings via the editor (such as **speed**, **acceleration**, **aliveTime**, etc.), it's possible to create many interesting **shot patterns**.
* Supports object pooling
* The scripts use **Odin Inspector** and **Odin Serializer**. (You can either comment out Odin-related parts or include the Odin package in your project.)
* There are 2 dependencies on the classes `Spaceship` and `ShipStats` – these have been replaced with **empty placeholder classes**.
