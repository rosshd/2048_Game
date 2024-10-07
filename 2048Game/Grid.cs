using System;
using System.ComponentModel;
using System.Diagnostics;

namespace _2048Game
{
    public class Grid : INotifyPropertyChanged
    {
        private int gameId;
        private static int nextId = 0;
        private Random random = new Random();
        private static int randomX;
        private static int randomY;

        public int[][] grid { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event Action GridUpdated;
        public int RandomX => randomX;
        public int RandomY => randomY;
         
        public Grid()
        {
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            this.grid = new int[4][];
            for (int i = 0; i < 4; i++)
            {
                grid[i] = new int[4];
            }
            AddRandomTile();
        }

        private void AddRandomTile()
        {
            randomX = random.Next(0, 4);
            randomY = random.Next(0, 4);
            while (grid[randomY][randomX] != 0)
            {
                randomX = random.Next(0, 4);
                randomY = random.Next(0, 4);
            }
            grid[randomY][randomX] = 2;
            OnPropertyChanged(nameof(grid));
        }

        public void UpdateGrid(int row, int col, int value)
        {
            grid[row][col] = value;
            OnPropertyChanged(nameof(grid));
        }

        public int getId()
        {
            return gameId;
        }

        public int[] MergeRow(int[] row)
        {
            List<int> mergedRow = new List<int>();

            foreach (int value in row)
            {
                if (value != 0)
                {
                    mergedRow.Add(value);
                }
            }

            for (int i = 0; i < mergedRow.Count - 1; i++)
            {
                if (mergedRow[i] == mergedRow[i + 1])
                {
                    mergedRow[i] *= 2;
                    mergedRow[i + 1] = 0;
                }
            }

            mergedRow.RemoveAll(v => v == 0);

            while (mergedRow.Count < 4)
            {
                mergedRow.Add(0);
            }

            return mergedRow.ToArray();
        }


        private int[] reverse(int[] arr)
        {
            int[] clone = new int[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                clone[i] = arr[(arr.Length - i) - 1];
            }
            return clone;
        }

        public void DoTurn(string direction)
        {
            bool gridChanged = false;

            if (direction == "D")
            {
                for (int i = 0; i < 4; i++)
                {
                    int[] oldRow = (int[])grid[i].Clone();
                    grid[i] = reverse(MergeRow(reverse(grid[i])));
                    if (!oldRow.SequenceEqual(grid[i]))
                    {
                        gridChanged = true;
                    }
                }
            }
            else if (direction == "A")
            {
                for (int i = 0; i < 4; i++)
                {
                    int[] oldRow = (int[])grid[i].Clone();
                    grid[i] = MergeRow(grid[i]);
                    if (!oldRow.SequenceEqual(grid[i]))
                    {
                        gridChanged = true;
                    }
                }
            }
            else if (direction == "W")
            {
                for (int i = 0; i < 4; i++)
                {
                    int[] column = new int[4];
                    for (int j = 0; j < 4; j++)
                    {
                        column[j] = grid[j][i];
                    }

                    int[] oldColumn = (int[])column.Clone();
                    column = MergeRow(column);

                    for (int j = 0; j < 4; j++)
                    {
                        grid[j][i] = column[j];
                    }

                    if (!oldColumn.SequenceEqual(column))
                    {
                        gridChanged = true;
                    }
                }
            }
            else if (direction == "S")
            {
                for (int i = 0; i < 4; i++)
                {
                    int[] column = new int[4];
                    for (int j = 0; j < 4; j++)
                    {
                        column[j] = grid[j][i];
                    }

                    int[] oldColumn = (int[])column.Clone();
                    column = reverse(MergeRow(reverse(column)));

                    for (int j = 0; j < 4; j++)
                    {
                        grid[j][i] = column[j];
                    }

                    if (!oldColumn.SequenceEqual(column))
                    {
                        gridChanged = true;
                    }
                }
            }

            if (gridChanged)
            {
                AddRandomTile();
            }

            GridUpdated?.Invoke();
        }

    }
}

