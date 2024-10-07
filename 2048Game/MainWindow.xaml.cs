using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WpfGrid = System.Windows.Controls.Grid;

namespace _2048Game
{
    public partial class MainWindow : Window
    {
        private Grid gameGrid;
        private int currentScore = 0;
        private static int highScore = 0;
        private bool gameEnded = false;

        public MainWindow()
        {
            InitializeComponent();
            gameGrid = new Grid();
            gameGrid.GridUpdated += LoadGame;
            DataContext = gameGrid;
            LoadGame();
            this.Focus();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            Debug.WriteLine($"Key Pressed: {e.Key}");

            string direction = null;
            switch (e.Key)
            {
                case Key.Up:
                case Key.W:
                    direction = "W";
                    break;

                case Key.Left:
                case Key.A:
                    direction = "A";
                    break;

                case Key.Down:
                case Key.S:
                    direction = "S";
                    break;

                case Key.Right:
                case Key.D:
                    direction = "D";
                    break;
            }

            if (direction != null)
            {
                gameGrid.DoTurn(direction);
                LoadGame();
            }
        }

        private void LoadGame()
        {
            Debug.WriteLine("Game loaded");
            GameGrid.Children.Clear();

            currentScore = CalculateCurrentScore();
            

            CurrentScoreText.Text = "Current Score: " + currentScore;
            HighScoreText.Text = "High Score: " + highScore;

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    TextBlock textBlock = new TextBlock
                    {
                        Text = gameGrid.grid[y][x] != 0 ? gameGrid.grid[y][x].ToString() : string.Empty,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 92,
                        Width = 140,
                        Height = 140,
                        TextAlignment = TextAlignment.Center,
                        Background = gameGrid.grid[y][x] != 0 ? Brushes.Gray : Brushes.LightGray
                    };

                    WpfGrid.SetRow(textBlock, y);
                    WpfGrid.SetColumn(textBlock, x);
                    GameGrid.Children.Add(textBlock);

                    if (x == gameGrid.RandomX && y == gameGrid.RandomY)
                    {
                        ApplyGrowAnimation(textBlock);
                    }
                }
            }

            if (IsGameOver())
            {
                ShowGameOverScreen();
            }
        }

        private int CalculateCurrentScore()
        {
            int score = 0;
            foreach (var row in gameGrid.grid)
            {
                foreach (var tile in row)
                {
                    score += tile;
                }
            }
            return score;
        }

        private void ApplyGrowAnimation(UIElement element)
        {
            var scaleTransform = new ScaleTransform(1.0, 1.0);
            element.RenderTransformOrigin = new Point(0.5, 0.5);
            element.RenderTransform = scaleTransform;

            var growAnimation = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(300)
            };

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, growAnimation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, growAnimation);
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (mainWindow != null)
            {
                mainWindow.ResetGame();
            }
            this.Close();
        }

        public void ResetGame()
        {
            gameGrid = new Grid();
            gameGrid.GridUpdated += LoadGame;
            LoadGame();
            this.Focus();
        }

        private bool IsGameOver()
        {
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    if (gameGrid.grid[y][x] == 0)
                    {
                        return false;
                    }

                    if (x < 3 && gameGrid.grid[y][x] == gameGrid.grid[y][x + 1]) return false;
                    if (y < 3 && gameGrid.grid[y][x] == gameGrid.grid[y + 1][x]) return false;
                }
            }
            return true;
        }

        private void ShowGameOverScreen()
        {
            int currentScore = CalculateCurrentScore();
            GameOutput.Text = "Game Over!";
            RestartButton.Visibility = Visibility.Visible;

            if (currentScore > highScore)
            {
                highScore = currentScore;
            }

            GameOver gameOverWindow = new GameOver(highScore, currentScore);
            gameGrid = new Grid();
            gameOverWindow.ShowDialog();
        }
    }
}
