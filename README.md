# Zori's Dungeon Crawler
made using [Terminal.GUI](https://github.com/gui-cs/Terminal.Gui)

This is a typical dungeon crawl rogue-like inspired by [Dungeon Crawl Stone Soup](https://crawl.develz.org/). 
For now, the plan is to just put out a playable release.

## To-Do:
- map gen:
  - Remove isolated rooms
  - Add stairways and link them between maps
  - Add some color
  - *later: sprinkle creatures and items around*
- creatures:
  - figure out the `.json` parser, at least for stats
  - work out damage and armor calculation with inventory and equipment 
- items:
  - make some basic items to carry around and equip

## WIP
- Better map generation
- A more detailed character/action system with focus on stamina and limb damage (something like Kenshi)
- Either a better creature parser or some kind of extensive mod support to allow for user-created actions instead of baked-in functions

## Reading the source code
I'll try to leave more comments around the code, but so far most things are pretty simple. 

The point of entry is `Program.cs`, which goes into `MainMenu.cs`, which finally goes into `Game.cs`. 
`Game.Init()` goes through map generation (described in `Map.cs`), then lets `MapView` class (also in `Game.cs`) render the map and listen for inputs. 

