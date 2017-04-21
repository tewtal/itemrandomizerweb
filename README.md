# Super Metroid Item Randomizer

### Information

This is a beta version used for testing new patches and item logic, have fun but expect that things can be broken at times.
**Huge shoutout and thanks to Dessyreqt for his work on the randomizer, without that this would not be a thing at all.**
You can find his randomizer here: http://dessyreqt.github.io/smrandomizer/

Changes from the vanilla game and other notes:
* Items are randomized in two pools: Major items/Reserves/E-tanks in one, and the ammo packs in another.
* There is on exception: The e-tank at Hi-Jump is no longer a major item location, it has been moved to the right super missile pack location in Wrecked Ship
* The door into the construction zone (blue brinstar ceiling e-tank room) is now a Blue door and will have zebes activated when you enter it so you can check the items there right away.
* The red tower elevator room's yellow door is blue to prevent softlocks
* Gravity no longer protects you from enviromental damage (heat, lava, acid, spikes). As a side effect, varia protects you from the same sources of damage twice as much as it normally would
* Because of the above change, Varia will never be in Lower Norfair
* Some cutscenes have been removed or shortened.
* Golden 4 Cutscene is eliminated, if you killed all four bosses, the way to Tourian is open immediately upon entering
* Suit animation cutscenes are gone, so it doesn't place you in the middle of the screen, potentially softlocking you in solid blocks
* You will no longer lose blue speed echoes while taking heat damage
* S/T beam is disabled
* GT Code is disabled
* The Intro sequence and Ceres station are no longer in the game, you instead start directly on Zebes from a blank file.

### How to use

* Install .NET Core 1.1
* Clone this repo
* Run: dotnet restore
* Run: dotnet run
* Connect to port 8080 on your server
* Have fun!
