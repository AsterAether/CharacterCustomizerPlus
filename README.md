# CharacterCustomizerPlus 
#### by Aster

Customize your Survivors even more!
This mod adds even more customization options to your survivors, and even new features like resetting your secondary cooldown on a Commando dash! (And many more)

**This mod is a companion mod to [CharacterCustomizer](https://thunderstore.io/package/AsterAether/CharacterCustomizer/), and contains old features from that mod!**

## Features

* Change a wide variety of settings on your Survivor, from damage output to completely new features.
* Doesn't overwrite default values if the config value is set to 0,
  improving forward compatibility.
  
## Configuration

It is highly recommended to use [BepInEx.ConfigurationManager](https://github.com/BepInEx/BepInEx.ConfigurationManager) to edit the configuration values in-game with the F1 key.
Or something like r2modmans config editor.

To generate the config options for a survivor, you need to set the "Enabled" option to true for this character and restart the game.


The configuration file is located in the config folder of BepInEx, called at.aster.charactercustomizerplus.cfg. 
It initializes with all values set to their default values. If a value is left with the default one (0 in cases of numbers), 
the executing code in the plugin will be skipped, and vanilla risk of rain behavior will be used.

A sample config line would look like this:
```
## If changes for this character are enabled. Set to true to generate options on next startup!
# Setting type: Boolean
# Default value: false
Captain Enabled = false
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

## TODO

* Reimplement live-update
* Check multiplayer compatability

## Changelog

**See:**
[Changelog](https://github.com/AsterAether/CharacterCustomizerPlus/blob/master/CHANGELOG.md)