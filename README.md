# Zori's Dungeon Crawler
made using [Terminal.GUI](https://github.com/gui-cs/Terminal.Gui)

This is a typical dungeon crawl rogue-like inspired by [Dungeon Crawl Stone Soup](https://crawl.develz.org/). 
For now, the plan is to just put out a playable release.

## Future plans
- Better, diverse map generation
- Try out a more detailed character/action system with focus on stamina and limb damage (something like Kenshi)
- Either a better creature parser or some kind of extensive mod support to allow for user-created actions instead of baked-in functions

## Reading the source code
I'll try to leave more comments around the code, but so far most things are pretty simple. 

The point of entry is `Program.cs`, which goes into `MainMenu.cs`, which finally goes into `Game.cs`. 
`Game.Init()` goes through map generation (described in `Map.cs` (and `MapGen.cs`, duh)), then lets `MapView` class (also in `Game.cs`) render the map and listen for inputs.

## To-Do:
- map gen:
  - Remove isolated rooms
    - Grab flood fill from [here](https://github.com/azsdaja/FloodSpill-CSharp) or try to figure it out myself (hard af)
  - Add stairways and link them between maps
  - Add some color 
    - Terminal.GUI has tools to change rune colors. Somewhere. I think.
  - *later: sprinkle creatures and items around*
- creatures:
  - work out damage and armor calculation with inventory and equipment 
    - Actually, work out the whole RPG system
  - figure out the `.json` parser, at least for stats
- items:
  - make some basic items to carry around and equip (also implement parser)
  - make equipment actually contribute to calculations
  - figure out how the active abilities from items will work
- UI:
  - do something about the input listener, since it's currently glued to the only window there is
  - add inventory
  - add tile/entity inspection
- AI: 
  - add time progression

