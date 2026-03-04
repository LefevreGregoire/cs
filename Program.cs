using System;
using System.Collections.Generic;

// ============================================================
//  ASCII MAZE - C# Console
//  Only modified cells are redrawn via Console.SetCursorPosition()
// ============================================================

// Grid dimensions
const int Width = 50;
const int Height = 20;

// Console offsets
const int OffsetY = 3;
const int OffsetX = 0;

// Cell grid dimensions (for recursive backtracker)
const int CellWidth = Width / 2;     // 25
const int CellHeight = Height / 2;    // 10
const int DirectionCount = 4;
const int CellScale = 2;

// UI Strings
const string Title = "╔══════════════════════════════════════════════════╗\n║          🏃 ASCII MAZE  C#  🏃             ║\n╚══════════════════════════════════════════════════╝";
const string Controls = "  [Z/↑] Up   [S/↓] Down   [Q/←] Left   [D/→] Right   [Esc] Quit";
const string WinMessage = """
  ╔════════════════════════════════╗
  ║   🎉  CONGRATULATIONS !  🎉    ║
  ║   You found the exit!          ║
  ╚════════════════════════════════╝
""";
const string LoseMessage = "\n  Game abandoned. See you soon!";
const string QuitPrompt = "  Press any key to quit...";

// Color constants
const ConsoleColor WallColor = ConsoleColor.DarkGray;
const ConsoleColor PlayerColor = ConsoleColor.Yellow;
const ConsoleColor ExitColor = ConsoleColor.Green;
const ConsoleColor CorridorColor = ConsoleColor.DarkBlue;
const ConsoleColor HeaderColor = ConsoleColor.Cyan;
const ConsoleColor ControlsColor = ConsoleColor.DarkCyan;
const ConsoleColor WinColor = ConsoleColor.Green;
const ConsoleColor LoseColor = ConsoleColor.Red;

// ASCII characters
const char WallChar = '█';
const char PlayerChar = '@';
const char ExitChar = '★';
const char CorridorChar = '·';

var maze = new CellType[Width, Height];

// Direction arrays
var offsetY = OffsetY;
var offsetX = OffsetX;

var dx = new[] { 0, 1, 0, -1 };
var dy = new[] { -1, 0, 1, 0 };

var rng = new Random();

// Initialize all cells as walls
for (int y = 0; y < Height; y++)
    for (int x = 0; x < Width; x++)
        maze[x, y] = CellType.Wall;

var stackX = new int[CellWidth * CellHeight];
var stackY = new int[CellWidth * CellHeight];
var stackTop = 0;

var visited = new bool[CellWidth, CellHeight];

var startCX = 0;
var startCY = 0;
visited[startCX, startCY] = true;
maze[startCX * CellScale, startCY * CellScale] = CellType.Corridor;

stackX[stackTop] = startCX;
stackY[stackTop] = startCY;
stackTop++;

while (stackTop > 0)
{
    var cx = stackX[stackTop - 1];
    var cy = stackY[stackTop - 1];

    var directions = new List<int> { 0, 1, 2, 3 };
    rng.Shuffle(directions);

    var found = false;
    foreach (var dir in directions)
    {
        var nx = cx + dx[dir];
        var ny = cy + dy[dir];
        if (nx >= 0 && nx < CellWidth && ny >= 0 && ny < CellHeight && !visited[nx, ny])
        {
            maze[cx * CellScale + dx[dir], cy * CellScale + dy[dir]] = CellType.Corridor;
            maze[nx * CellScale, ny * CellScale] = CellType.Corridor;
            visited[nx, ny] = true;
            stackX[stackTop] = nx;
            stackY[stackTop] = ny;
            stackTop++;
            found = true;
            break;
        }
    }
    if (!found) stackTop--;
}

// Set player position and exit
var playerX = 0;
var playerY = 0;
var exitX = (CellWidth - 1) * CellScale;
var exitY = (CellHeight - 1) * CellScale;

maze[playerX, playerY] = CellType.Player;
maze[exitX, exitY] = CellType.Exit;

// Display initial maze
Console.Clear();
Console.CursorVisible = false;

Console.SetCursorPosition(0, 0);
Console.ForegroundColor = HeaderColor;
Console.WriteLine(Title);
Console.ResetColor();

for (int y = 0; y < Height; y++)
{
    for (int x = 0; x < Width; x++)
    {
        Console.SetCursorPosition(offsetX + x, offsetY + y);
        DrawCell(x, y);
    }
}

Console.SetCursorPosition(0, offsetY + Height + 1);
Console.ForegroundColor = ControlsColor;
Console.Write(Controls);
Console.ResetColor();

void DrawCell(int x, int y)
{
    Console.SetCursorPosition(offsetX + x, offsetY + y);
    var cell = maze[x, y];
    (Console.ForegroundColor, var ch) = cell switch
    {
        CellType.Wall => (WallColor, WallChar),
        CellType.Player => (PlayerColor, PlayerChar),
        CellType.Exit => (ExitColor, ExitChar),
        _ => (CorridorColor, CorridorChar)
    };
    Console.Write(ch);
    Console.ResetColor();
}

// Game loop
var won = false;

while (!won)
{
    var key = Console.ReadKey(true).Key;

    var nextX = playerX;
    var nextY = playerY;
    var shouldQuit = false;

    switch (key)
    {
        case ConsoleKey.Z:
        case ConsoleKey.UpArrow:
            nextY--;
            break;
        case ConsoleKey.S:
        case ConsoleKey.DownArrow:
            nextY++;
            break;
        case ConsoleKey.Q:
        case ConsoleKey.LeftArrow:
            nextX--;
            break;
        case ConsoleKey.D:
        case ConsoleKey.RightArrow:
            nextX++;
            break;
        case ConsoleKey.Escape:
            shouldQuit = true;
            break;
    }

    if (shouldQuit) break;

    if (nextX >= 0 && nextX < Width && nextY >= 0 && nextY < Height && maze[nextX, nextY] != CellType.Wall)
    {
        if (maze[nextX, nextY] == CellType.Exit) won = true;

        maze[playerX, playerY] = CellType.Corridor;
        DrawCell(playerX, playerY);

        playerX = nextX;
        playerY = nextY;
        maze[playerX, playerY] = CellType.Player;
        DrawCell(playerX, playerY);
    }
}

// Display end screen
Console.SetCursorPosition(0, offsetY + Height + 3);
if (won)
{
    Console.ForegroundColor = WinColor;
    Console.Write(WinMessage);
    Console.ResetColor();
}
else
{
    Console.ForegroundColor = LoseColor;
    Console.WriteLine(LoseMessage);
    Console.ResetColor();
}

Console.SetCursorPosition(0, offsetY + Height + 8);
Console.WriteLine(QuitPrompt);
Console.CursorVisible = true;
Console.ReadKey(true);

enum CellType
{
    Corridor = 0,
    Wall = 1,
    Player = 2,
    Exit = 3
}
