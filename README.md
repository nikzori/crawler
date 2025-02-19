# Zori's Dungeon Crawler
made using [Terminal.GUI](https://github.com/gui-cs/Terminal.Gui)

This is going to be a typical dungeon crawl rogue-like inspired by [Dungeon Crawl Stone Soup](https://crawl.develz.org/). 
For now, the plan is to just put out a playable release.

## Future plans
- Better, diverse map generation
- Try out a more detailed character/action system with focus on stamina and limb damage (something like Kenshi)
- Either a better creature parser or some kind of extensive mod support to allow for user-created actions instead of baked-in functions

## Reading the source code
I'll try to leave more comments around the code, but so far most things are pretty simple. 

The point of entry is `Program.cs`, which goes into `MainMenu.cs`, which finally goes into `Game.cs`. UI is managed in `UI.cs`, controls are also handled there.

## To-Do:
- map gen:
  - Remake dungeon controller to support multi-floor dungeons
    - Move static/const stuff to a different class
    - Figure out a way to link stairs
- creatures:
  - work out damage and armor calculation with inventory and equipment 
    - Actually, work out the whole RPG system
  - figure out the `.json` parser, at least for stats
- items:
  - make some basic items to carry around and equip (also implement parser)
  - make equipment actually contribute to calculations
  - figure out how the active abilities from items will work
- UI:
  - add test items to inventory
  - add tile/entity inspection
- AI: 
  - add time progression
  - hook up basic functions like attacking and moving around
  - add some idle routines: patrolling, sleeping
  - add a basic mob spawner 
- performance:
  - get rid of screen tear when moving on the Y axis (ssh only)
