using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace BlockGame
{
    class Program
    {
        // 게임 설정
        const int BOARD_WIDTH = 10;
        const int BOARD_HEIGHT = 20;
        const int RENDER_OFFSET_X = 5;
        const int RENDER_OFFSET_Y = 2;

        static int[,] board = new int[BOARD_WIDTH, BOARD_HEIGHT];
        static int score = 0;
        static int level = 1;
        static int linesCleared = 0;
        static bool gameOver = false;

        // 테트리스 블록 정의 (I, J, L, O, S, T, Z)
        static int[][][] shapes = new int[][][]
        {
            new int[][] { new int[] { 0, 1 }, new int[] { 1, 1 }, new int[] { 2, 1 }, new int[] { 3, 1 } }, // I
            new int[][] { new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 1, 1 }, new int[] { 2, 1 } }, // J
            new int[][] { new int[] { 2, 0 }, new int[] { 0, 1 }, new int[] { 1, 1 }, new int[] { 2, 1 } }, // L
            new int[][] { new int[] { 1, 0 }, new int[] { 2, 0 }, new int[] { 1, 1 }, new int[] { 2, 1 } }, // O
            new int[][] { new int[] { 1, 0 }, new int[] { 2, 0 }, new int[] { 0, 1 }, new int[] { 1, 1 } }, // S
            new int[][] { new int[] { 1, 0 }, new int[] { 0, 1 }, new int[] { 1, 1 }, new int[] { 2, 1 } }, // T
            new int[][] { new int[] { 0, 0 }, new int[] { 1, 0 }, new int[] { 1, 1 }, new int[] { 2, 1 } }  // Z
        };

        static ConsoleColor[] blockColors = new ConsoleColor[]
        {
            ConsoleColor.Cyan,    // I
            ConsoleColor.Blue,    // J
            ConsoleColor.DarkYellow, // L (Orange 대용)
            ConsoleColor.Yellow,  // O
            ConsoleColor.Green,   // S
            ConsoleColor.Magenta, // T
            ConsoleColor.Red      // Z
        };

        static int currentShapeIndex;
        static int[][] currentBlock;
        static int currentX, currentY;
        static ConsoleColor currentColor;

        static int nextShapeIndex;
        static int[][] nextBlock;
        static ConsoleColor nextColor;

        static Random random = new Random();

        static void Main(string[] args)
        {
            Console.Title = "C# Console Tetris - Antigravity";
            Console.CursorVisible = false;
            Console.Clear();

            InitializeGame();

            Stopwatch gameTimer = new Stopwatch();
            gameTimer.Start();
            long lastMoveTime = 0;

            while (!gameOver)
            {
                // 입력 처리
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    HandleInput(key);
                }

                // 자동 낙하 로직
                long currentTime = gameTimer.ElapsedMilliseconds;
                int dropInterval = Math.Max(100, 800 - (level - 1) * 100);

                if (currentTime - lastMoveTime > dropInterval)
                {
                    if (!MoveBlock(0, 1))
                    {
                        PlaceBlock();
                        ClearLines();
                        SpawnBlock();
                    }
                    lastMoveTime = currentTime;
                }

                Render();
                Thread.Sleep(20); // CPU 점유율 조절
            }

            DrawGameOver();
        }

        static void InitializeGame()
        {
            for (int x = 0; x < BOARD_WIDTH; x++)
                for (int y = 0; y < BOARD_HEIGHT; y++)
                    board[x, y] = 0;

            score = 0;
            level = 1;
            linesCleared = 0;
            gameOver = false;

            nextShapeIndex = random.Next(shapes.Length);
            nextBlock = shapes[nextShapeIndex];
            nextColor = blockColors[nextShapeIndex];

            SpawnBlock();
        }

        static void SpawnBlock()
        {
            currentShapeIndex = nextShapeIndex;
            currentBlock = nextBlock;
            currentColor = nextColor;

            currentX = BOARD_WIDTH / 2 - 2;
            currentY = 0;

            // 다음 블록 미리 생성
            nextShapeIndex = random.Next(shapes.Length);
            nextBlock = shapes[nextShapeIndex];
            nextColor = blockColors[nextShapeIndex];

            if (!IsValidPosition(currentX, currentY, currentBlock))
            {
                gameOver = true;
            }
        }

        static void HandleInput(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    MoveBlock(-1, 0);
                    break;
                case ConsoleKey.RightArrow:
                    MoveBlock(1, 0);
                    break;
                case ConsoleKey.DownArrow:
                    MoveBlock(0, 1);
                    break;
                case ConsoleKey.UpArrow:
                    RotateBlock();
                    break;
                case ConsoleKey.Spacebar:
                    HardDrop();
                    break;
            }
        }

        static bool MoveBlock(int dx, int dy)
        {
            if (IsValidPosition(currentX + dx, currentY + dy, currentBlock))
            {
                currentX += dx;
                currentY += dy;
                return true;
            }
            return false;
        }

        static void RotateBlock()
        {
            int[][] rotated = new int[currentBlock.Length][];
            for (int i = 0; i < currentBlock.Length; i++)
            {
                // (x, y) -> (-y, x) 중심축 보정 없이 단순 회전
                int rx = -currentBlock[i][1];
                int ry = currentBlock[i][0];
                rotated[i] = new int[] { rx, ry };
            }

            // 회전 후 위치 보정 (중심 맞추기)
            int minX = rotated.Min(p => p[0]);
            int minY = rotated.Min(p => p[1]);
            for (int i = 0; i < rotated.Length; i++)
            {
                rotated[i][0] -= minX;
                rotated[i][1] -= minY;
            }

            if (IsValidPosition(currentX, currentY, rotated))
            {
                currentBlock = rotated;
            }
        }

        static void HardDrop()
        {
            while (MoveBlock(0, 1)) { }
            PlaceBlock();
            ClearLines();
            SpawnBlock();
        }

        static bool IsValidPosition(int nx, int ny, int[][] block)
        {
            foreach (var p in block)
            {
                int x = nx + p[0];
                int y = ny + p[1];

                if (x < 0 || x >= BOARD_WIDTH || y >= BOARD_HEIGHT)
                    return false;

                if (y >= 0 && board[x, y] != 0)
                    return false;
            }
            return true;
        }

        static void PlaceBlock()
        {
            foreach (var p in currentBlock)
            {
                int x = currentX + p[0];
                int y = currentY + p[1];
                if (y >= 0)
                    board[x, y] = (int)currentColor + 1; // 0은 빈칸이므로 +1
            }
        }

        static void ClearLines()
        {
            int linesThisTurn = 0;
            for (int y = BOARD_HEIGHT - 1; y >= 0; y--)
            {
                bool full = true;
                for (int x = 0; x < BOARD_WIDTH; x++)
                {
                    if (board[x, y] == 0)
                    {
                        full = false;
                        break;
                    }
                }

                if (full)
                {
                    linesThisTurn++;
                    for (int moveY = y; moveY > 0; moveY--)
                    {
                        for (int x = 0; x < BOARD_WIDTH; x++)
                        {
                            board[x, moveY] = board[x, moveY - 1];
                        }
                    }
                    for (int x = 0; x < BOARD_WIDTH; x++)
                    {
                        board[x, 0] = 0;
                    }
                    y++; // 같은 줄 다시 체크
                }
            }

            if (linesThisTurn > 0)
            {
                linesCleared += linesThisTurn;
                score += (int)Math.Pow(2, linesThisTurn) * 100;
                level = (linesCleared / 10) + 1;
            }
        }

        static void Render()
        {
            // 테두리 및 배경
            DrawBorder();

            // 보드 상태 출력
            for (int y = 0; y < BOARD_HEIGHT; y++)
            {
                Console.SetCursorPosition(RENDER_OFFSET_X, RENDER_OFFSET_Y + y);
                for (int x = 0; x < BOARD_WIDTH; x++)
                {
                    if (board[x, y] != 0)
                    {
                        Console.ForegroundColor = (ConsoleColor)(board[x, y] - 1);
                        Console.Write("■■");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("· "); // 빈 공간 가이드
                    }
                }
            }

            // 현재 움직이는 블록 출력
            Console.ForegroundColor = currentColor;
            foreach (var p in currentBlock)
            {
                int x = currentX + p[0];
                int y = currentY + p[1];
                if (y >= 0 && y < BOARD_HEIGHT && x >= 0 && x < BOARD_WIDTH)
                {
                    Console.SetCursorPosition(RENDER_OFFSET_X + x * 2, RENDER_OFFSET_Y + y);
                    Console.Write("■■");
                }
            }

            // 정보창
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(RENDER_OFFSET_X + BOARD_WIDTH * 2 + 5, RENDER_OFFSET_Y);
            Console.Write($"SCORE: {score}");
            Console.SetCursorPosition(RENDER_OFFSET_X + BOARD_WIDTH * 2 + 5, RENDER_OFFSET_Y + 1);
            Console.Write($"LEVEL: {level}");
            Console.SetCursorPosition(RENDER_OFFSET_X + BOARD_WIDTH * 2 + 5, RENDER_OFFSET_Y + 2);
            Console.Write($"LINES: {linesCleared}");

            // 다음 블록 미리보기
            Console.SetCursorPosition(RENDER_OFFSET_X + BOARD_WIDTH * 2 + 5, RENDER_OFFSET_Y + 5);
            Console.Write("NEXT:");
            for (int i = 0; i < 4; i++)
            {
                Console.SetCursorPosition(RENDER_OFFSET_X + BOARD_WIDTH * 2 + 5, RENDER_OFFSET_Y + 6 + i);
                Console.Write("        "); // 지우기
            }
            Console.ForegroundColor = nextColor;
            foreach (var p in nextBlock)
            {
                Console.SetCursorPosition(RENDER_OFFSET_X + BOARD_WIDTH * 2 + 5 + p[0] * 2, RENDER_OFFSET_Y + 7 + p[1]);
                Console.Write("■■");
            }

            Console.ResetColor();
        }

        static void DrawBorder()
        {
            Console.ForegroundColor = ConsoleColor.White;
            // 상단
            Console.SetCursorPosition(RENDER_OFFSET_X - 1, RENDER_OFFSET_Y - 1);
            Console.Write("+" + new string('-', BOARD_WIDTH * 2) + "+");
            // 하단
            Console.SetCursorPosition(RENDER_OFFSET_X - 1, RENDER_OFFSET_Y + BOARD_HEIGHT);
            Console.Write("+" + new string('-', BOARD_WIDTH * 2) + "+");
            // 좌우
            for (int i = 0; i < BOARD_HEIGHT; i++)
            {
                Console.SetCursorPosition(RENDER_OFFSET_X - 1, RENDER_OFFSET_Y + i);
                Console.Write("|");
                Console.SetCursorPosition(RENDER_OFFSET_X + BOARD_WIDTH * 2, RENDER_OFFSET_Y + i);
                Console.Write("|");
            }
        }

        static void DrawGameOver()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(10, 10);
            Console.WriteLine("========================");
            Console.SetCursorPosition(10, 11);
            Console.WriteLine("       GAME OVER        ");
            Console.SetCursorPosition(10, 12);
            Console.WriteLine($"     FINAL SCORE: {score}");
            Console.SetCursorPosition(10, 13);
            Console.WriteLine("========================");
            Console.ResetColor();
            Console.SetCursorPosition(10, 15);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
