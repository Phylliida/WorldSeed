# WorldSeed
A open source framework for easy creation of living creatures, clothing and armor, plants, and terrain.

Plants
----------------

Using the [Synthetic Silviculture](https://storage.googleapis.com/pirk.io/projects/synthetic_silviculture/index.html) approach, allowing creation of vegitation suited for various biomes by just specifying temperature and precipitation (more fine grained control will also be available, of course).

Once I finish reproducing that work, I will work on procedural textures for bark from the great procedural book, and procedural leafs using [this technique](https://www.comp.nus.edu.sg/~leowwk/thesis/saurabhgarg-thesis.pdf).

Creatures
----------------

I want to do a few things:

1. Make a spore type model, using creature skeletons. Have various procedural options for skin/fur/other more esoteric things, with fine tunable face geometry. Think something like [MB-LAB](https://mb-lab-community.github.io/MB-Lab.github.io/) or MakeHuman but more general, just a general purpose character/creature creator.

2. Detailed clothing, armor, and weapon systems, with procedural textures, realistic materials, and procedural sound based on material interactions.

3. Realistically animate them by using an optimization process of learning locomotion using real time [muscle models](https://www.youtube.com/watch?v=higGxGmwDbs). Have an alternate auto-generated animation set that may not look as good, but is instant to create.

4. Simulate vocal cords (something like the [pink trombone](https://dood.al/pinktrombone/), though slightly more general, perhaps using some of the new 2D airflow techniques) and simple cognitive models of audio synthesis through muscles for basic bird/animal noises. Still need to do more research on this note, there might be better approaches for procedural sound.

5. Stretch goal: simulate ecosystems to guide content generation

Terrain
---------------

While I'm inspired by the procworld blog, I think that having an emphasis on deformable terrain using realistic erosion-type techniques, and using procedural textures based on soil composition and geology will result in a more controllable and realistic result. I want the creation of terrain and vegitation (and, ideally, creatures) to all work together. Starting with very high level details like temperature and precipitation, then tweaking the results until it is what you wanted.


Magic System
------------------

I have some ambitious goals for a math-like magic system, but it is less important than the other aspects here. Fundamentally, just more in depth particle/animation support for procedural spell type effects would be enough, I think.



What is this for?
-----------------------

The World Seed is an object in Sword Art Online (presumably a framework of some type) that allows people to create VRMMO worlds. Exactly what that means is unclear, but I'm interpreting it as a sort of kit that lets you create characters, landscape, and vegitation with an intuitive, nice inteface. While I'm interested in the "hosting an MMO" networking problem as well, this is more of a content creation tool.

High level goals
----------------------
- Any content created can be exported in formats that are compatable with other major tools (Maya, Blender, Unreal Engine, Godot, Vr Chat, etc.)
- Lots of examples and tutorials to help users get an idea of what is possible. Tight feedback loop for improving tools.
- In terms of programming, everything should be functional whenever possible. Heavy use of Parallel LINQ to do parallelization. Code needs to stay clean or this will get unweildly very quickly.
