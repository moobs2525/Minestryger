using Avalonia.Controls;
using System.Diagnostics;

namespace Minestryger;

public partial class MainWindow : Window
{
    private MineFieldElement[,] mineFields = new MineFieldElement[9, 9];
    private Stopwatch stopwatch = new Stopwatch();

    public MainWindow()
    {
        InitializeComponent();
        CreateMineField();
        PlaceMines();
        stopwatch.Start();
    }

    private void CreateMineField()
    {
        int rows = 9;
        int columns = 9;

        // Create the rows
        for (int row = 0; row < rows; row++)
        {
            mineGrid.RowDefinitions.Add(new RowDefinition());
        }

        // Create the columns
        for (int column = 0; column < columns; column++)
        {
            mineGrid.ColumnDefinitions.Add(new ColumnDefinition());
        }

        // Create the buttons
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                // Create a MineFieldElement for this position
                MineFieldElement mineField = new MineFieldElement();

                mineField.Row = row;
                mineField.Column = column;

                // Store it in the 2D array
                mineFields[row, column] = mineField;


                // Create the button
                Button button = new Button();
                

                button.Width = 50;
                button.Height = 50;

                // Connect the button to the MineFieldElement
                button.Tag = mineField;
                
                button.Click += ButtonClick;

                // Place the button in the Grid
                Grid.SetRow(button, row);
                Grid.SetColumn(button, column);

                mineGrid.Children.Add(button);
            }
        }
    }
    private void PlaceMines()
    {
        mineFields[0, 1].HasMine = true;
        mineFields[1, 4].HasMine = true;
        mineFields[2, 7].HasMine = true;
        mineFields[3, 3].HasMine = true;
        mineFields[4, 8].HasMine = true;
    }
    private void ButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Button button = (Button)sender!;

        MineFieldElement mineField = (MineFieldElement)button.Tag!;

        if (mineField.HasMine)
        {
            button.Content = "MINE";
            //txtTime.Text = "Game Over!";
            
            DisableAllButtons();
            stopwatch.Stop();
            txtTime.Text = $"Game Over! Tid: {stopwatch.Elapsed.TotalSeconds} sekunder";
        }
        else
        {
            int mines = CountNeighborMines(mineField.Row, mineField.Column);


            button.Content = mines.ToString();

            mineField.IsRevealed = true;

            if (CheckWin())
            {
                stopwatch.Stop();
                txtTime.Text = $"Du har vundet! Tid: {stopwatch.Elapsed.TotalSeconds} sekunder";
                DisableAllButtons();
            }
        }
    }
    private int CountNeighborMines(int row, int column)
    {
        int mineCount = 0;

        for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
        {
            for (int columnOffset = -1; columnOffset <= 1; columnOffset++)
            {
                int neighborRow = row + rowOffset;
                int neighborColumn = column + columnOffset;

                // Skip the field itself
                if (rowOffset == 0 && columnOffset == 0)
                {
                    continue;
                }

                // Check that we are still inside the board
                if (neighborRow >= 0 && neighborRow < 9 &&
                    neighborColumn >= 0 && neighborColumn < 9)
                {
                    if (mineFields[neighborRow, neighborColumn].HasMine)
                    {
                        mineCount++;
                    }
                }
            }
        }

        return mineCount;
    }
    private bool CheckWin()
    {
        for (int row = 0; row < 9; row++)
        {
            for (int column = 0; column < 9; column++)
            {
                MineFieldElement mineField = mineFields[row, column];

                // If we find a safe field that has not been revealed,
                // the player has not won yet
                if (!mineField.HasMine && !mineField.IsRevealed)
                {
                    return false;
                }
            }
        }

        // All safe fields have been revealed
        return true;
    }
    private void DisableAllButtons()
    {
        foreach (var control in mineGrid.Children)
        {
            if (control is Button button)
            {
                button.IsEnabled = false;
            }
        }
    }
    private void RestartClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // Stop og nulstil tiden
        stopwatch.Reset();

        // Fjern alle gamle knapper fra spillepladen
        mineGrid.Children.Clear();

        // Fjern gamle rækker og kolonner
        mineGrid.RowDefinitions.Clear();
        mineGrid.ColumnDefinitions.Clear();

        // Lav et helt nyt array med felter
        mineFields = new MineFieldElement[9, 9];

        // Lav spillepladen igen
        CreateMineField();

        // Placér miner igen
        PlaceMines();

        // Fjern gammel tekst fra Game Over / Win
        txtTime.Text = "";

        // Start tiden igen
        stopwatch.Start();
    }
}