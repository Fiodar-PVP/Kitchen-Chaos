# Kitchen Chaos

# Project purpose

Welcome to the GitHub repository for Kitchen Chaos! 

The main idea behind this project is to show/polish/acquire skills necessary for building a functional single player game with the following transformation into a multiplayer game. This is not intended for any form of monetization nor will it contain any such features now or in future. 

## Singleplayer Game

The Singleplayer game was built from scratch. The logic is being separated from visuals by using events.

### Main components of the code base: 
- `KitchenObject`: Represents an object like a Tomato or CheeseSlices. It's attached to the object that exists in the world.
- `KitchenObjectSO`: Definition of each type, holds all the data for each KitchenObject like Prefab, Name and Sprite.
- `BaseCounter`: Base class that all Counters inherit. Defines an Interact(); function which the player calls to interact with any counter.
- `IKitchenObjectParent`: Each KitchenObject has a Parent which holds it, and each Parent can only hold a single KitchenObject.
- `DeliveryManager`: Manages all the Recipe spawning and delivery.
- `RecipeSO`: Definition of a Recipe.
- `KitchenGameManager`: General script that handles the main game state like Countdown, GamePlaying and GameOver.


### Description
The player starts the game with a simple main menu. After clicking "Play", at first, the loading scene is loaded. It is followed by the transition to the final Game Scene.\
There the player is prompted with a tutorial screen which shows basic controls. Since the game implements key rebinding in GameInput class the control keys are dynamically populated. Once the player "Interact" with tutorial the countdown starts after which the game begins.\
On the left side the player can see a dynamically generated list of recipes. All icons for recipes are also dynamically generated. The player is able to move around and "Interact" with different counters. \
For example:
- Interacting with the Container Counter will spawn a KitchenObject and give it to the player. 
- That KitchenObject will follow the player.
- Interacting with ClearCounter allows the player to place/pick up KitchenObject on/from it.
- The player can use CuttingCounter to cut certain KitchenObjects utilizing "AlternateInteract".
- After several cuts the old KitchenObject will be destroyed and a new one spawned (Tomato -> TomatoSlices). 
- By interacting with StoveCounter the certain KitchenObjects can be cooked.\
The player can use Plate to assemble different KitchenObjects according to the recipes on the left side and deliver them to the customers. 


## Multiplayer Game

Once the game starts the player can choose whether they would like to play the multiplayer or singleplayer version of the game.

- Implemented Network for GameObject using Rpc/Network Variables to synchronize data across multiple clients.
- Implemented Lobby Unity feature to Create/Join public/private Lobbies.
- Implemented Relay Unity solution to be able to connect players together with Relay.
- Made a Character Selection Scene.

### Lobby Management
After choosing the multiplayer option, the player is loaded to the Lobby Scene. There they can see all available Lobbies created by other players. The list of available lobbies is dynamically updated and filtered to have at least one available slot to join. The player has an option to create a new Lobby (configure it to be public/private) and set its name. Also there are options for Quick join/Join via Code or simply clicking any Lobby from the available Lobby list.  Creation/Connection to the Lobby is done via Unity Relay to be able to connect players via Relay. 

### Character Management (in Multiplayer lobbies)
- After joining/creating a lobby, the player is moved to Character Select Scene, where they can see the lobby name/code.
- They can configure their character color or flag their readiness to play the game. 
   - The color selection is being immediately synchronized by the server. 
   - In addition to that, the server validates that two players do not end up with the same color. 
- Once everyone in the Character Select Scene flag themselves as ready, the server loads the game scene where each player will be prompted with a tutorial screen. 

The server waits until everybody reads the rules and controls. After each player interacts with the tutorial, the server will synchronize the game state and start the countdown to begin the game.

There is also an option to pause the game by any player, and the game will remain paused until each player who clicked pause unpauses themselves. 


### Data Synchronization
To synchronize data across multiple clients the Network for GameObject was used. The client authoritative approach has been implemented. The data synchronization is done via RPCs and NetworkVariable along with the decision which data should be server-side and what can be client-side.


# Acknowledgement
All assets in the game has been provided by [Code Monkey](https://www.youtube.com/@CodeMonkeyUnity) (visual prefabs/animations/materials/meshes/textures/sound) in his courses, while all code has been fully written by me (including scene set up).

# Contact information
- Fiodar-PVP - f.perakhozhau@gmail.com
