# CharacterCustomizerPlus 
#### by Aster
**DISCLAIMER: This mod is currently heavy WIP, as it is mostly just ported code from the old version. Expect many breakin bugs!**
**If you find any bugs, please write me a PN on Discord!**

Customize your Survivors even more!
This mod adds even more customization options to your survivors, and even new features like resetting your secondary cooldown on a Commando dash! (And many more)

**This mod is a companion mod to [CharacterCustomizerPlus](https://thunderstore.io/package/AsterAether/CharacterCustomizerPlus/), and contains old features from that mod!**

## Features

* Change a wide variety of settings on your Survivor, from damage output to completely new features.
* Doesn't overwrite default values if the config value is set to 0,
  improving forward compatibility.
* Multiplayer compatible, players can even have different configs if they want!

## Installation

* Install [CharacterCustomizerPlus](https://thunderstore.io/package/AsterAether/CharacterCustomizerPlus/) first.
* Copy the included `CharacterCustomizer.dll` into your BepInEx plugins
  folder.
* Start up the game! This will create the config file. Note that this
  can take a little longer, there are a lot of values to be checked and
  created.

## Configuration

It is highly recommended to use [BepInEx.ConfigurationManager](https://github.com/BepInEx/BepInEx.ConfigurationManager) to edit the configuration values in-game with the F1 key.
**Live update is currently NOT supported!**

The configuration file is located in the config folder of BepInEx, called at.aster.charactercustomizerplus.cfg. 
It initializes with all values set to their default values. If a value is left with the default one (0 in cases of numbers), 
the executing code in the plugin will be skipped, and vanilla risk of rain behavior will be used.

A sample config line would look like this:
```
## Maximum charge duration (logic) for grenades, in seconds. Vanilla value: 3
# Setting type: Single
# Default value: 0
GrenadeTotalChargeDuration = 0
```
The first line is a comment explaining the configuration value, and is automatically updated by the game to include the vanilla RoR2 value of the stat, where possible.
The second line is the type of value expected (Single = Decimal).
And the second line is the actual config value, where you can change the stat to your liking.

CharacterCustomizerPlus will try to add the vanilla values as references in
the comments of the config file. If you seem to be missing some values, either the mod has no access to that, or you need to play a run of the game with the character you want the values for.

Please use dots for separating the decimal values (0.1) and not commas (0,1).

## Available Config Values

**See:**
[Config Values](https://github.com/AsterAether/CharacterCustomizerPlus/blob/master/config_values.md)

## Changelog

**See:**
[Changelog](https://github.com/AsterAether/CharacterCustomizerPlus/blob/master/CHANGELOG.md)