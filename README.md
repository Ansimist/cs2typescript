# CS2 TypeScript Converter
This program is designed to convert files from the `vts` format to `vts_c`. This file is a TypeScript script that can be read by the game CS2. You can create logic for your maps using this program.

[Download](https://github.com/Ansimist/cs2typescript/releases/download/1.0/cs2typescript.exe)

## How to use the Program
- Launch the program and insert the path to your addon in the `content` folder.
- You need to create a file with the vts format in the `scripts` folder of your addon.
- By modifying the code in this file, the program will automatically create and update files that will be located in the `game` folder.

## How to Include a Script in a Map
- Add an entity of class `point_script`
- Add the `script` key to the entity and write the path to the file there
- Add the `targetname` key to the entity and write the name of the entity there

![Image alt](https://i.imgur.com/IJeOwwO.png)

## Example Addon
You can download and explore an example addon that uses scripts.
[Download](https://github.com/Ansimist/cs2typescript/releases/download/1.0/example_addon.zip)

![Image alt](https://imgur.com/DXDgf9Z.png)