# SudokuSolver

SudokuSolver is a simple sudoku solver that implements Donald Knuth's Dancing Links algorithm to find the first solution to a given Sudoku puzzle.  It expects a path to a text file with a Sudoku puzzle formatted so that the rows are new lines and blank spaces are represented as X's, and it prints the result to the Console and to a text file of the same format as the input.

Originally it was written to answer an interview question, and given that there was a time limit, it's a bit quick and dirty.  I'll be coming back from time to time with the ultimate goal of separating out the Dancing Links code into a general solver API so that it can be used to answer other examples of the exact cover problem as well as output multiple solutions.
