using System.Windows;

namespace _2048Game
{
    public partial class GameOver : Window
    {
        public GameOver(int highScore, int Score)
        {
            InitializeComponent();
            HighScoreText.Text = $"High Score: {highScore}";
            ScoreText.Text = $"Score: {Score}";
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

