# two-beats-in-a-labirinth-


🧭 Two Beasts in a Labyrinth

Two Beasts in a Labyrinth is a simple console-based C# simulation demonstrating autonomous movement logic inside a maze.
The program models a creature (“beast”) navigating a labyrinth using the right-hand wall-following algorithm — a classic maze-solving strategy.

🧩 Project Overview

The program consists of three main components:

Bludiste (Maze) – Handles the maze structure, reads it from user input, and displays the current state after each move.

Prisera (Beast) – Represents the moving entity. It decides how to move based on surrounding walls and open paths using the right-hand rule.

Souradnice (Coordinates) – A lightweight struct for storing 2D coordinates (x, y).

The simulation runs in the console, showing the beast’s position and orientation (> ^ < v) as it moves step by step through the maze.

⚙️ How It Works

The user provides the width, height, and map of the maze as input.

The beast’s initial position and direction are read from the map (symbols like >, <, ^, or v).

The simulation runs for a fixed number of moves (default: 20).

After each move, the updated maze is printed to the console.

🧱 Input Example
10
6
XXXXXXXXXX
X....X.<.X
X....X...X
X.X..X.X.X
X.X....X.X
XXXXXXXXXX


Legend:

X → Wall

. → Empty space

> < ^ v → Beast (and its direction)

🧠 Movement Logic

The beast follows the right-hand rule:

If there’s a wall on the right and space ahead → move forward.

If there’s a wall on the right and a wall ahead → turn left.

If there’s no wall on the right → turn right and move forward.

This continues for each simulation step.

🧰 Technical Notes

Language: C# (.NET Console Application)

Uses only standard libraries (no external dependencies).

The maze is printed to the console after each move for visualization.

The method nacti_cislo() should be adjusted to correctly read numbers from input ('0' → ASCII 48 fix).

🚀 How to Run

Compile the program:

csc Program.cs


Run it:

./Program.exe


Provide input (width, height, and maze) as shown in the example above.
