using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TicTacSwap {
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        private string _currentPlayer = "X";
        private int _movesSinceLastSwap = 1;
        private bool GameOver;


        public MainWindow() {
            InitializeComponent();
            Show();
        }

        private void GameButton_Click(object sender, RoutedEventArgs e) {
            if (GameOver) return;

            // Figure out which button was clicked
            var button = (Button)sender;

            // Check if the button is empty
            if (!string.IsNullOrEmpty(button.Content.ToString())) {
                MessageBox.Show("Idiot lol");
                return;
            }


            // Set the button to the current player
            button.Content = _currentPlayer;

            // Switch the current player
            _currentPlayer = _currentPlayer == "X" ? "O" : "X";
            SetCurrentPlayerIndicator();

            _movesSinceLastSwap++;

            // Check if the swap condition is met
            if (_movesSinceLastSwap >= 2) SwapButton.IsEnabled = true;

            if (!BoardIsEmpty()) ResetButton.IsEnabled = true;
            CheckWin();
        }

        private void SetCurrentPlayerIndicator() {
            if (_currentPlayer == "X") {
                Xlabel.Visibility = Visibility.Visible;
                Olabel.Visibility = Visibility.Hidden;
            }
            else {
                Xlabel.Visibility = Visibility.Hidden;
                Olabel.Visibility = Visibility.Visible;
            }
        }

        private void CheckWin() {
            // Define winning patterns: rows, columns, and diagonals
            var patterns = new List<List<string>> {
                new List<string> { "0,0", "0,1", "0,2" }, // Top row
                new List<string> { "1,0", "1,1", "1,2" }, // Middle row
                new List<string> { "2,0", "2,1", "2,2" }, // Bottom row
                new List<string> { "0,0", "1,0", "2,0" }, // Left column
                new List<string> { "0,1", "1,1", "2,1" }, // Middle column
                new List<string> { "0,2", "1,2", "2,2" }, // Right column
                new List<string> { "0,0", "1,1", "2,2" }, // Diagonal from top-left to bottom-right
                new List<string> { "0,2", "1,1", "2,0" } // Diagonal from top-right to bottom-left
            };

            // Iterate through each winning pattern
            foreach (var content1 in from pattern in patterns
                     let content1 = GetButtonContent(pattern[0])
                     let content2 = GetButtonContent(pattern[1])
                     let content3 = GetButtonContent(pattern[2])
                     where !string.IsNullOrEmpty(content1) && content1 == content2 && content2 == content3
                     select content1) {
                // Show a message indicating the winner
                MessageBox.Show($"{content1} wins!");
                GameOver = true;
                return; // Exit the method since the game has ended
            }

            // Check for a tie
            if (IsBoardFull()) MessageBox.Show("It's a tie!");
        }

        // Helper method to get content of a button given its position "row,col"
        private string GetButtonContent(string position) {
            var parts = position.Split(',');
            var row = int.Parse(parts[0]);
            var col = int.Parse(parts[1]);
            var button = (Button)GameGrid.Children
                .Cast<UIElement>()
                .First(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col);
            return button.Content.ToString();
        }

        // Helper method to check if the board is full
        private bool IsBoardFull() {
            foreach (var child in GameGrid.Children) {
                if (child is Button button && string.IsNullOrEmpty(button.Content.ToString())) return false; // If any button is empty, return false
            }

            return true; // If no empty button found, return true
        }

        private bool BoardIsEmpty() {
            foreach (var child in GameGrid.Children) {
                if (child is Button button && !string.IsNullOrEmpty(button.Content.ToString())) return false; // If any button is empty, return false
            }

            return true; // If no empty button found, return true
        }

        private void ClearBoardButton_Click(object sender, RoutedEventArgs e) {
            GameOver = false;

            // Clear the content of all buttons
            foreach (var child in GameGrid.Children) {
                if (child is Button button) button.Content = "";
            }

            // Reset the current player
            _currentPlayer = "X";
            SetCurrentPlayerIndicator();

            // Reset the moves since last swap
            _movesSinceLastSwap = 1;

            // Disable the swap button
            SwapButton.IsEnabled = false;

            // Clear the ClearBoardButton
            ResetButton.IsEnabled = false;
        }

        private void SwapButton_Click(object sender, RoutedEventArgs e) {
            // Check if the swap condition is met
            if (_movesSinceLastSwap < 2) {
                MessageBox.Show("You can't swap yet!");
                return;
            }

            // Swap all taken fields
            foreach (var child in GameGrid.Children) {
                if (child is Button button) {
                    switch ((string)button.Content) {
                        case "X":
                            button.Content = "O";
                            break;
                        case "O":
                            button.Content = "X";
                            break;
                        default:
                            continue;
                    }
                }
            }

            // Swap the current player
            _currentPlayer = _currentPlayer == "X" ? "O" : "X";
            SetCurrentPlayerIndicator();

            // Reset the moves since last swap
            _movesSinceLastSwap = 0;

            // Disable the swap button
            SwapButton.IsEnabled = false;
        }

        private void RagequitButton_Click(object sender, RoutedEventArgs e) {
            Environment.Exit(0);
        }
    }
}