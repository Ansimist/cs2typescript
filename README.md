# CS2 TypeScript Converter

CS2 TypeScript Converter is a tool for converting files from the `.vts` format to `.vts_c`. These files are TypeScript scripts that can be used in the game **Counter-Strike 2 (CS2)** to create custom logic for your maps.

## Features
- Automatically converts `.ts` or `.vts` files to `.vts_c`.
- Real-time updates to game folder whenever changes are made.
- Simplifies map scripting with TypeScript.

[**Download**](https://github.com/Ansimist/cs2typescript/releases/download/1.0/cs2typescript.exe)

---

## How to Use
1. Place the path to your addon's `content` folder inside the `config.json` file.
2. Launch the program.
3. Create a `.ts` or `.vts` file in the `scripts` folder of your addon.
4. Modify the code in this file; the program will automatically generate and update corresponding files in the `game` folder.

---

## How to Include a Script in a Map
To include a script in your CS2 map:

1. Add an entity of class `point_script` to your map.
2. Set the `script` key to the path of your generated file.
3. Set the `targetname` key to the name of the entity.

Below is an example:

![point_script Example](https://i.imgur.com/IJeOwwO.png)

---

## Example Addon
Explore an example addon that demonstrates how to use scripts with this program:

[**Download the Example Addon**](https://github.com/Ansimist/cs2typescript/releases/download/1.0/example_addon.zip)

![Example Addon Preview](https://imgur.com/DXDgf9Z.png)

---

### Notes
- Ensure your `config.json` file is correctly configured before starting the program.
- The program monitors changes in your `.ts` or `.vts` files and updates corresponding `.vts_c` files in real-time.
