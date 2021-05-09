# ToggUltima
UdonSharp toggle script for standard buttons with useful options.

Action Script:
- Quickly setup buttons to toggle on/off game objects (i.e. mirrors, video players, doors, etc.)
- Ability to temporarily disable objects. (Useful for clearing objects off of a wall for a mirror or videoplayer.)
- Optimized time delay features. (Backend with no constantly running Update() function.)
- Variety of activation options. 

List Script: 
- Automatically disable previously used buttons when a new one is used. (Setup mirrors that automatically turn off when another is turned on.)
- Update a single list of mutually disabled objects without having to manage multiple lists of objects to disable on each toggle. (No need to duplicate your workload managing duplicate lists of mirrors to disable on each interactible object, setup/update one list for all of them and just set which each interactible will enable!)

Remote Script:
- Setup a button that may share the same functionality as another. (Like having multiple light switches that can turn on/off the same bulb or other artistic feature.)

Sync Script:
- Synchornize Action script toggles for both current players and players just joining in.
- Each Action script is represented by a bit within an integer. (Reducing network traffic for managing as many toggles as could be in a puzzle dungeon!)
- Configurable cooldown feature to further reduce network spam.
- Custom Deserialization script that doesn't use OnDeserialization, allowing it to only be run when absolutely necessary. By default, only used while a player is joining and waiting for the integer to be synced with other players. (Still avoiding constantly running scripts!) :D
