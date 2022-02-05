using System;

//Coordinate System
//0,0 0,1
//1,0 1,1

Console.Write("How large would you like your grid? ");
int size = Convert.ToInt32(Console.ReadLine());
StringGrid gameGrid = new StringGrid(size);

Console.Write("Player one, choose your symbol: ");
string playerOneMark = Console.ReadLine();
Console.Write("Player two, choose your symbol: ");
string playerTwoMark = Console.ReadLine();

string[] playerMarks = {playerOneMark, playerTwoMark};
string currentPlayerMark = playerOneMark;

bool endConditionMet = false;
string playerInput;

while(!endConditionMet)
{

    RenderGrid.DrawGrid(gameGrid);

    bool remain = true;
    while(remain)
    {
        Console.Write($"{currentPlayerMark} Enter coordinates: ");
        playerInput = Console.ReadLine();
        if(IsPlayerInputValid(gameGrid, currentPlayerMark,playerInput)) {
            int[] coords = CoordParseUtils.CoordParser(playerInput,"0123456789,");
            int x = coords[0];
            int y = coords[1];
            gameGrid.SetMark(x,y,currentPlayerMark);

            remain = false;
        }
    }

    if(CheckWinCondition(gameGrid, currentPlayerMark))
    {
        RenderGrid.DrawGrid(gameGrid);
        Console.WriteLine($"{currentPlayerMark} has won!");   
        endConditionMet = true;     
    }
    if(CheckDrawCondition(gameGrid,playerMarks))
    {
        RenderGrid.DrawGrid(gameGrid);
        Console.WriteLine("The game ends with a draw.");   
        endConditionMet = true;     
    }

    if(currentPlayerMark == playerOneMark) 
        currentPlayerMark = playerTwoMark;
    else
        currentPlayerMark = playerOneMark;
}

static bool IsPlayerInputValid(StringGrid gameGrid, string playerMark, string playerInput) 
{

    if (!CoordParseUtils.CheckParseValidity(playerInput,"0123456789",","))
    {
        Console.WriteLine("Coordinates not found.");
        return false;
    }

    int[] coords = CoordParseUtils.CoordParser(playerInput,"0123456789,");
    int x = coords[0];
    int y = coords[1];

    if(x < 0 || gameGrid.GetSize() <= x)
    {
        Console.WriteLine("Row out of bounds.");
        return false;
    }

    if(y < 0 || gameGrid.GetSize() <= y)
    {
        Console.WriteLine("Column out of bounds.");
        return false;
    }

    if (gameGrid.GetMark(x,y) == " ") {
        return true;
    }
    else 
    {
        Console.WriteLine($"Space is occupied by {gameGrid.GetMark(x,y)}.");
        return false;
    }
}

static bool CheckWinCondition(StringGrid grid, string playerMark) 
{
    for(int x = 0; x < grid.GetSize(); x++)
    {
        if(IsArrayConsistent(grid.GetRow(x),playerMark))
            return true;
    }

    for(int y = 0; y < grid.GetSize(); y++)
    {
        if(IsArrayConsistent(grid.GetColumn(y),playerMark))
            return true;
    }

    if(IsArrayConsistent(grid.GetNegDiagonal(),playerMark))
        return true;
    
    if(IsArrayConsistent(grid.GetPosDiagonal(),playerMark))
        return true;

    return false;
}

static bool CheckDrawCondition(StringGrid grid, string[] playerMarks) 
{
    int count = 0;
    int possibleWins = grid.GetSize() + grid.GetSize() + 2; //2 represents diagonals. (MaxSide - MinSide) * 2 + 2

    for(int x = 0; x < grid.GetSize(); x++)
    {
        if(IsArrayInclusive(grid.GetRow(x),playerMarks,2))
            count++;
    }

    for(int y = 0; y < grid.GetSize(); y++)
    {
        if(IsArrayInclusive(grid.GetColumn(y),playerMarks,2))
            count++;
    }

    if(IsArrayInclusive(grid.GetNegDiagonal(),playerMarks,2))
        count++;

    if(IsArrayInclusive(grid.GetPosDiagonal(),playerMarks,2))
        count++;

    return (count == possibleWins) ? true: false;
}

//Does this array consist only of this mark
static bool IsArrayConsistent(string[] array, string mark)
{
    for(int i = 0; i < array.Length; i++)
    {
        if(array[i] != mark)
            return false;
    }
    return true;
}

//Does this array contain hits amount of unique marks
static bool IsArrayInclusive(string[] array, string[] marks, int hits)
{
    int count = 0;
    for(int i = 0; i < marks.Length; i++)
    {

        for(int j = 0; j < array.Length; j++)
        {
            if (marks[i] == array[j]) 
            {
                count++;
                break;
            }
        }
    }
    return (count == hits) ? true : false;
}

static class RenderGrid 
{

    public static void DrawGrid(StringGrid grid) 
    {

        for(int x = 0; x < grid.GetSize(); x++)
        {
            for(int y = 0; y < grid.GetSize(); y++)
            {
                Console.Write(grid.GetMark(x,y));
                if(y != grid.GetSize() - 1)
                    Console.Write("|");
                else
                    Console.Write("\n");
            }
            if (x != grid.GetSize() - 1)
                for(int y = 0; y < grid.GetSize(); y++)
                {
                    if(y != grid.GetSize() - 1)
                        Console.Write("-+");
                    else
                        Console.Write("-\n");
                }
        }
    }
} 

public static class CoordParseUtils {
    public static bool CheckParseValidity(string input,string acceptedChars, string deliniator)
    {
        for (int i = 0; i < input.Length; i++)
        {
            if(acceptedChars.Contains(input[i]))
            {
                for (int j = i + 1; j < input.Length; j++)
                {
                    if(deliniator.Contains(input[j]))
                    {
                        for (int k = j + 1; k < input.Length; k++)
                        {
                            if(acceptedChars.Contains(input[k]))
                                return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    public static int[] CoordParser(string input, string acceptedChars) 
    {
        string validChars = "";
        string xCoord = "";
        string yCoord = "";
        int passIndex = 0;

        for(int i = 0; i < input.Length; i++) 
        {
            if(acceptedChars.Contains(input[i]))
                validChars += input[i];
        }

        for(int i = 0; validChars[i] != ','; i++)
        {
            xCoord += validChars[i];
            passIndex++;
        }

        for(int i = passIndex + 1; i < validChars.Length; i++)
        {
            yCoord += validChars[i];
        }

        int[] coords = {Convert.ToInt32(xCoord),Convert.ToInt32(yCoord)};
        return coords;
    }
}
public class StringGrid
{
    private int _size;
    private string[,] _grid;

    public StringGrid(int size) 
    {
        _size = size;
        _grid = new string[size,size];
        FillGrid(" ");
    }

    public void FillGrid(string mark)
    {
        for(int x = 0; x < _grid.GetLength(0); x++)
        {
            for(int y = 0; y < _grid.GetLength(1); y++)
            {
                _grid[x,y] = mark;
            }
        }
    }

    public void FillGridDebug()
    {   
        int count = 0;
        for(int x = 0; x < _grid.GetLength(0); x++)
        {
            for(int y = 0; y < _grid.GetLength(1); y++)
            {
                count++;
                _grid[x,y] = Convert.ToString(count);
            }
        }
    }

    public void DisplayGridConsole() 
    {

        for(int x = 0; x < _grid.GetLength(0); x++)
        {
            for(int y = 0; y < _grid.GetLength(1); y++)
            {
                Console.Write(_grid[x,y]);
            }
            Console.WriteLine();
        }
    }

    public int GetSize() 
    {
        return _size;
    }

    public string GetMark(int x, int y) 
    {
        return _grid[x,y];
    }
    public void SetMark(int x, int y, string mark)
    {
        _grid[x,y] = mark;
    }

    public string[] GetRow(int x)
    {  
        string[] array = new string[_size];
        for(int y = 0; y < _grid.GetLength(1); y++)
        {
            array[y] = _grid[x,y];
        }
        return array;
    }

    public string[] GetColumn(int y)
    {  
        string[] array = new string[_size];
        for(int x = 0; x < _grid.GetLength(1); x++)
        {
            array[x] = _grid[x,y];
        }
        return array;
    }

    public string[] GetNegDiagonal() 
    {
        int x = 0;
        int y = 0;
        string[] array = new string[_size];
        for(int i = 0; i < _grid.GetLength(0); i++)
        {
            array[i] = _grid[x,y];
            x++;
            y++;
        }
        return array;
    }

    public string[] GetPosDiagonal() 
    {
        int x = _size - 1;
        int y = 0;
        string[] array = new string[_size];
        for(int i = 0; i < _grid.GetLength(0); i++)
        {
            array[i] = _grid[x,y];
            x--;
            y++;
        }
        return array;
    }

}

//Insane things to do in the future: Color the marks in the winning line
//Rectangular boards
//Make the coordinate system not insane
//Null-proof player input

