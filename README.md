# Super Metroid Item Randomizer

### Information

This is a beta version used for testing new patches and item logic, have fun but expect that things can be broken at times.
**Huge shoutout and thanks to Dessyreqt for his work on the randomizer, without that this would not be a thing at all.**
You can find his randomizer here: http://dessyreqt.github.io/smrandomizer/

Changes from the vanilla game and other notes:
* Items are randomized in two pools: Major items/Reserves/E-tanks in one, and the ammo packs in another.
* There is on exception: The e-tank at Hi-Jump is no longer a major item location, it has been moved to the right super missile pack location in Wrecked Ship
* The door into the construction zone (blue brinstar ceiling e-tank room) is now a Blue door and will have zebes activated when you enter it so you can check the items there right away.
* Gravity suit will no longer give you protection from heat, so you will have to find Varia suit as well.
* Some cutscenes have been removed or shortened.
* Ceres station is no longer in the game.

### How to use

* Install .NET Core 1.1
* Clone this repo
* Run: dotnet restore
* Run: dotnet run
* Connect to port 8080 on your server
* Have fun!
