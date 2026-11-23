# infoMso_assignment3
lab assignment 3
the programming learning app with a ui, pathfinding exercises and the RepeatUntil command.
the app is called Wakkaman and the character that walks over the grid is Juba.
- needs .NET 8 SDK.


# how to run the app (ui)
1. install the MAUI workload once: dotnet workload install maui
2. cd ProgrammingLearningApp
3. dotnet run --project ProgrammingLearningApp.Maui -f net8.0-windows10.0.19041.0


the ui is in dutch you can load a random basic, advanced or expert example, pick an example or import a .txt file with a program.
the program shows up in the text field where you can edit it. Uitvoeren runs it, shows the trace and draws the path, Metrics shows the metrics.
the program can be exported as .txt or .json it gets saved in your documents folder.

# how to run the console app
1. cd ProgrammingLearningApp
2. dotnet run --project ProgrammingLearningApp

step 1 you pick a example basic, advanced or expert or import a txt file with a program. 
you can execute it or calculate the metrics in step 2.


# how to run tests
1. cd ProgrammingLearningApp
2. dotnet test ProgrammingLearningApp.Tests


# program files

Same format as the examples in the assignment 4 spaces for indentation:

Repeat 4 times
    Move 10
    Turn right

"Repeat 4" without times also works. RepeatUntil works with the conditions WallAhead and GridEdge:

RepeatUntil WallAhead
    Move 1
    Turn right

The character starts at (0,0) facing east. Going south increases y, going north decreases y.

# exercises

click "Oefening laden" in the app and pick a file from the Exercises folder. grid is shown and the program runs on it.
a grid file looks like this: o is an open cell, + is a wall, x is the end and s is the start:

soo
o+o
oox

walking into a wall or outside the grid gives a runtime error. after running you see if Juba reached the x or not.
leave grid empty to run a program without grid. 

# test programs
the programs folder has program files for testing, you can import them with "Importeren":
- recht_vooruit_oplossing & recht_vooruit_repeatuntil: solve recht_vooruit
- om_de_muur_heen_oplossing: solves om_de_muur_heen
- zigzag_oplossing: solves zigzag
- opdracht_voorbeeld_oplossing: solves opdracht_voorbeeld (the grid from the assignment)
- fout_muur_in: runtime error on om_de_muur_heen, walks into a wall
- fout_buiten_grid: runtime error on recht_vooruit, walks outside the grid
- fout_oneindig: runtime error without grid, hits the step limit
- find_exit_1 & find_exit_2: the RepeatUntil examples from the assignment
- rectangle & nested_repeats: programs to run without grid
