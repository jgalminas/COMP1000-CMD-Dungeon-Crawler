# COMP1000 - Dungeon Crawler

<img alt="Gameplay in basic mode" src="https://i.imgur.com/jSyh1hX.gif" width="100%" height="100%" />

## How to install
On the right hand side of this GitHub page you will find releases. 
1. Simply click on the latest release and download the `Crawler.<version>.zip`.
2. Extract the `Crawler` folder from the zip.
3. Open the `Crawler` and run the `Crawler.exe` file. 

## How to play
### Objective
The objective of the game is simple &mdash; reach the the exit of the dungeon without dying!

### Starting the game
These are the steps that must be followed in order for the user to start the game.
1. Open the `Crawler.exe` executable file via command line or the GUI.
2. Upon input prompt write `load <map name>` and press <kbd>ENTER</kbd>. There are two maps to choose from:
   - `Simple.map`
   - `Advanced.map`
3. Once the map is loaded the user can choose to play in advanced mode by typing `advanced` and pressing <kbd>ENTER</kbd>.
4. Finally, enter `play` and then press <kbd>ENTER</kbd> to start the game!

*Note: When playing in advanced mode, monsters move, can eat coins to get stronger and even attack you.*

### Controls
#### Basic

<kbd>W</kbd> : Move Up
<kbd>A</kbd> : Move Left
<kbd>S</kbd> : Move Down
<kbd>D</kbd> : Move Right

<kbd>Q</kbd> : Quit
#### Advanced
<kbd>P</kbd> : Pickup Coins <br/>

<kbd>SPACE</kbd> : Attack

#### Elements
`M` : Monsters
`-` : Empty Spaces
`#` : Walls
`C` : Coins
`@` : Player
`X` : Exit

<br>*Note: once the game has started you do not need to press <kbd>ENTER</kbd> to register your action.*
***
## Implementation
The project follows an object oriented approach and most of the entities in the game are represented by classes and objects. I decided to use object orientation because it made the code more maintainable and easier to understand. Additionally, it made implementing some of the advanced features much easier, for example, the player having to press <kbd> P </kbd> to pick up coins when standing over them.

Additionally, I decided to include some abstraction to re-use some of the common properties and functionallity between the player and the monsters, such as, attack, health, position, etc. This may not have a big impact on the project at the moment, but it would be beneficial if the project ever gets bigger.

## Resources Used
This section is dedicated for links to all of the resources and tutorials I used to implement the functionality of the game. All of the sources I used were from [Microsoft Docs](https://docs.microsoft.com/).
 * [Reading a file](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-read-from-a-text-file) - I used this source to learn how to read data from a .txt file.
 * [Abstraction](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/abstract) - I used this page as a reference for implementing abstraction.
 * [Method overriding](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/override) - I used this as a reference for implementing method overriding.
 * [Switch Statement](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/statements/selection-statements) - Referred to this page for information about switch statements.
 * [Console.ReadKey()](https://docs.microsoft.com/en-us/dotnet/api/system.console.readkey?view=net-6.0) - I used this page to get an understanding of the `ReadKey()` function and how I can use it in the project.
***
## Video

[Evaluation video for the project.](https://youtu.be/uvHubjfeLio)
