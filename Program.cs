using System;
using System.Collections.Generic;

// ============================================================
//  ASCII MAZE - C# Console
//  Only modified cells are redrawn via Console.SetCursorPosition()
// ============================================================

const int Width = 50;
const int Height = 20;

const int OffsetY = 3;
const int OffsetX = 0;

const int CellWidth = Width / 2;    
const int CellHeight = Height / 2;    
const int DirectionCount = 4;
const int CellScale = 2;

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

const ConsoleColor WallColor = ConsoleColor.DarkGray;
const ConsoleColor PlayerColor = ConsoleColor.Yellow;
const ConsoleColor ExitColor = ConsoleColor.Green;
const ConsoleColor CorridorColor = ConsoleColor.DarkBlue;
const ConsoleColor HeaderColor = ConsoleColor.Cyan;
const ConsoleColor ControlsColor = ConsoleColor.DarkCyan;
const ConsoleColor WinColor = ConsoleColor.Green;
const ConsoleColor LoseColor = ConsoleColor.Red;

const char WallChar = '█';
const char PlayerChar = '@';
const char ExitChar = '★';
const char CorridorChar = '·';

var maze = new CellType[Width, Height];
var dx = new[] { 0, 1, 0, -1 };
var dy = new[] { -1, 0, 1, 0 };
var rng = new Random();

var (playerX, playerY) = GenerateMaze(maze);
DrawInitialScreen(playerX, playerY);

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

DrawTextXY(0, OffsetY + Height + 3, WinColor, won ? WinMessage : LoseMessage);
DrawTextXY(0, OffsetY + Height + 8, null, QuitPrompt);
Console.CursorVisible = true;
Console.ReadKey(true);

void DrawTextXY(int x, int y, ConsoleColor? color, string text)
{
    Console.SetCursorPosition(x, y);
    if (color.HasValue)
        Console.ForegroundColor = color.Value;
    Console.WriteLine(text);
    Console.ResetColor();
}

void DrawCell(int x, int y)
{
    Console.SetCursorPosition(OffsetX + x, OffsetY + y);
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

(int, int) GenerateMaze(CellType[,] grid)
{
    for (int y = 0; y < Height; y++)
        for (int x = 0; x < Width; x++)
            grid[x, y] = CellType.Wall;

    CarvePassages(0, 0, grid);

    var playerX = 0;
    var playerY = 0;
    var exitX = (CellWidth - 1) * CellScale;
    var exitY = (CellHeight - 1) * CellScale;

    grid[playerX, playerY] = CellType.Player;
    grid[exitX, exitY] = CellType.Exit;

    return (playerX, playerY);
}

void CarvePassages(int cx, int cy, CellType[,] grid)
{
    grid[cx * CellScale, cy * CellScale] = CellType.Corridor;

    var directions = new List<int> { 0, 1, 2, 3 };
    rng.Shuffle(directions);

    foreach (var dir in directions)
    {
        var nx = cx + dx[dir];
        var ny = cy + dy[dir];

        if (nx >= 0 && nx < CellWidth && ny >= 0 && ny < CellHeight && 
            grid[nx * CellScale, ny * CellScale] == CellType.Wall)
        {
            grid[cx * CellScale + dx[dir], cy * CellScale + dy[dir]] = CellType.Corridor;
            
            CarvePassages(nx, ny, grid);
        }
    }
}

void DrawInitialScreen(int playerX, int playerY)
{
    Console.Clear();
    Console.CursorVisible = false;

    DrawTextXY(0, 0, HeaderColor, Title);

    for (int y = 0; y < Height; y++)
    {
        for (int x = 0; x < Width; x++)
        {
            DrawCell(x, y);
        }
    }

    DrawTextXY(0, OffsetY + Height + 1, ControlsColor, Controls);
}

enum CellType
{
    Corridor = 0,
    Wall = 1,
    Player = 2,
    Exit = 3
}
