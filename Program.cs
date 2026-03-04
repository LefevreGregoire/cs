using System;

// ============================================================
//  ASCII MAZE - C# Console
//  Grid : int[50, 20]  (width=50, height=20)
//  0 = corridor   1 = wall   2 = player   3 = exit
//  Movement : Z/Q/S/D or arrow keys
//  ✅ Optimized : only modified cells are redrawn
//                via Console.SetCursorPosition()
// ============================================================

var maze = new int[50, 20];

var width = 50;
var height = 20;

// Vertical offset in console (number of title lines)
var offsetY = 3;
var offsetX = 0;

// ── Maze generation by « recursive backtracker » ──
var cellW = width / 2;   // 25
var cellH = height / 2;   // 10

for (int y = 0; y < height; y++)
    for (int x = 0; x < width; x++)
        maze[x, y] = 1;

var stackX = new int[cellW * cellH];
var stackY = new int[cellW * cellH];
var stackTop = 0;

var visited = new bool[cellW, cellH];

var dx = new[] { 0, 1, 0, -1 };
var dy = new[] { -1, 0, 1, 0 };

var rng = new Random();

var startCX = 0;
var startCY = 0;
visited[startCX, startCY] = true;
maze[startCX * 2, startCY * 2] = 0;

stackX[stackTop] = startCX;
stackY[stackTop] = startCY;
stackTop++;

while (stackTop > 0)
{
    var cx = stackX[stackTop - 1];
    var cy = stackY[stackTop - 1];

    var order = new[] { 0, 1, 2, 3 };
    for (int i = 3; i > 0; i--)
    {
        var j = rng.Next(i + 1);
        var tmp = order[i]; order[i] = order[j]; order[j] = tmp;
    }

    var found = false;
    for (int d = 0; d < 4; d++)
    {
        var nx = cx + dx[order[d]];
        var ny = cy + dy[order[d]];
        if (nx >= 0 && nx < cellW && ny >= 0 && ny < cellH && !visited[nx, ny])
        {
            maze[cx * 2 + dx[order[d]], cy * 2 + dy[order[d]]] = 0;
            maze[nx * 2, ny * 2] = 0;
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

// ── Player position and exit ──
var playerX = 0;
var playerY = 0;
var exitX = (cellW - 1) * 2;
var exitY = (cellH - 1) * 2;

maze[playerX, playerY] = 2;
maze[exitX, exitY] = 3;

// ── Full initial drawing (only once) ──
Console.Clear();
Console.CursorVisible = false;

Console.SetCursorPosition(0, 0);
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("╔══════════════════════════════════════════════════╗");
Console.WriteLine("║          🏃 ASCII MAZE  C#  🏃             ║");
Console.WriteLine("╚══════════════════════════════════════════════════╝");
Console.ResetColor();

for (int y = 0; y < height; y++)
{
    for (int x = 0; x < width; x++)
    {
        Console.SetCursorPosition(offsetX + x, offsetY + y);
        var cell = maze[x, y];
        if (cell == 1)      { Console.ForegroundColor = ConsoleColor.DarkGray;  Console.Write("█"); }
        else if (cell == 2) { Console.ForegroundColor = ConsoleColor.Yellow;    Console.Write("@"); }
        else if (cell == 3) { Console.ForegroundColor = ConsoleColor.Green;     Console.Write("★"); }
        else                { Console.ForegroundColor = ConsoleColor.DarkBlue;  Console.Write("·"); }
    }
}

Console.SetCursorPosition(0, offsetY + height + 1);
Console.ForegroundColor = ConsoleColor.DarkCyan;
Console.Write("  [Z/↑] Up   [S/↓] Down   [Q/←] Left   [D/→] Right   [Esc] Quit");
Console.ResetColor();

// ── Local action : redraw ONE single cell via SetCursorPosition ──
void DrawCell(int x, int y)
{
    Console.SetCursorPosition(offsetX + x, offsetY + y);
    var cell = maze[x, y];
    if (cell == 1)      { Console.ForegroundColor = ConsoleColor.DarkGray;  Console.Write("█"); }
    else if (cell == 2) { Console.ForegroundColor = ConsoleColor.Yellow;    Console.Write("@"); }
    else if (cell == 3) { Console.ForegroundColor = ConsoleColor.Green;     Console.Write("★"); }
    else                { Console.ForegroundColor = ConsoleColor.DarkBlue;  Console.Write("·"); }
    Console.ResetColor();
}

// ── Game loop ──
var won = false;

while (!won)
{
    var key = Console.ReadKey(true).Key;

    var nextX = playerX;
    var nextY = playerY;

    if      (key == ConsoleKey.Z || key == ConsoleKey.UpArrow)    nextY--;
    else if (key == ConsoleKey.S || key == ConsoleKey.DownArrow)  nextY++;
    else if (key == ConsoleKey.Q || key == ConsoleKey.LeftArrow)  nextX--;
    else if (key == ConsoleKey.D || key == ConsoleKey.RightArrow) nextX++;
    else if (key == ConsoleKey.Escape) break;

    if (nextX >= 0 && nextX < width && nextY >= 0 && nextY < height && maze[nextX, nextY] != 1)
    {
        if (maze[nextX, nextY] == 3) won = true;

        // ✅ Erase old position (corridor) → only 1 cell redrawn
        maze[playerX, playerY] = 0;
        DrawCell(playerX, playerY);

        // ✅ Draw new position → only 1 cell redrawn
        playerX = nextX;
        playerY = nextY;
        maze[playerX, playerY] = 2;
        DrawCell(playerX, playerY);
    }
}

// ── Victory screen ──
Console.SetCursorPosition(0, offsetY + height + 3);
if (won)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("  ╔════════════════════════════════╗");
    Console.WriteLine("  ║   🎉  CONGRATULATIONS !  🎉    ║");
    Console.WriteLine("  ║   You found the exit!          ║");
    Console.WriteLine("  ╚════════════════════════════════╝");
    Console.ResetColor();
}
else
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("\n  Game abandoned. See you soon!");
    Console.ResetColor();
}

Console.SetCursorPosition(0, offsetY + height + 8);
Console.WriteLine("  Press any key to quit...");
Console.CursorVisible = true;
Console.ReadKey(true);
