# DunGen

API for customizable dungeon generation.  The main way this is done is by using _commands_ that specify how rooms should be generated.  This allows control over things like which room to generate off of, which direction to generate the room in, and if all the rooms should be shuffled up.  This is all done with a seeded random value generator so as to allow for perfect reproduction if desired.  Includes a visualizer program for a simple example usage.

On floors of size less than 10,000 (the number of rooms to generate), the process of generation is often instantaneous.  For floors at the magnitude of 100,000 rooms the process can take nearly a minute.  Of course, this is very dependent on the commands being used and how fast the processor being used is.  On that note, the process of generating a floor is not parallelizable.

As of 2/26/2017, this project is still rather early in its life with much work to be done.  This includes more features that can be added and more performance improvements that can be made.


## Ideas

### Command structure:
    - Numbers for numerical options lifted out, placed after command
        + this will require restructuring of parsing/getting commands
        + this would be useful for some of the new comand options below


### Command options:
    - Max distance from center
        + can only generate rooms to a certain distance from the "center"
    - Re-center
        + to make previous option more interesting
    - Reuse count
        + if specified, can only be run that many times before being skipped
    - Specifying order to build directions in
        + currently order is always north, east, south, west


### FloorSprawler changes:
    - Probably a new shuffle method
        + chchel has an interesting one on their branch
