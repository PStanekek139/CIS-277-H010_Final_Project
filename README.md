# BSP 2D Map Generator

This project is being developed in Unity by Paul Stanek as the final project for CIS-277 (Advanced C++) at Rock Valley College, Spring 2018.

## Features

This application is able to take a user-specified 2D map size (from 10-60 tiles in each axis) and randomly generate a 2D map with rooms and hallways.  The user can then export the map to a .txt file, or adjust the dimensions and generate a new map.

The camera can be adjusted using the WASD or cursor keys, and can be zoomed in and out using the mouse scroll wheel.  "ESC" exits the program.

## What is BSP?

BSP stands for Binary Space Partitioning.  In this application, BSP is used to take a chosen map size (the "root" object) and recursively divide it down into randomly-sized rectangular sections ("leaves") until a minimum size is achieved.

Next, randomly-sized rectangular rooms are created within each of the resulting sections, and hallways are randomly generated between them.  This guarantees that all rooms are somehow connected.

## Why Unity?

I chose to develop this project using Unity to get more familiar with it.  Previously, I'd only dabbled in smaller Unity tutorials - this project has given me the chance to explore sprite sheets, the UI controls, camera controls, and many many other aspects of Unity.

## Resources

The following tutorials and resources were invaluable in creating this project, and I highly recommend reading them if you are interesting in this type of random map generation:

How to Use BSP Trees to Generate Game Maps (by Timothy Hely): 
[https://gamedevelopment.tutsplus.com/tutorials/how-to-use-bsp-trees-to-generate-game-maps--gamedev-12268](https://gamedevelopment.tutsplus.com/tutorials/how-to-use-bsp-trees-to-generate-game-maps--gamedev-12268)

Binary Trees (by Richard Fleming, Jr): 
[https://www.youtube.com/watch?v=S5y3ES4Rvkk](https://www.youtube.com/watch?v=S5y3ES4Rvkk)

Basic BSP Dungeon Generation (RogueBasin):
[http://www.roguebasin.com/index.php?title=Basic_BSP_Dungeon_generation](http://www.roguebasin.com/index.php?title=Basic_BSP_Dungeon_generation)
