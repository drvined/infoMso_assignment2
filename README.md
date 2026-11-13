# infoMso_assignment2
lab assignment 2.
- needs the .NET 8 SDK.

# how to run
1. cd ProgrammingLearningApp
2. dotnet run --project ProgrammingLearningApp


in step 1 you pick a example basic, advanced or expert or import a .txt file with a program. 
you can execute it or calculate the metrics in step 2.

# project structure
- ProgrammingLearningApp.Core: all domain logic (commands, parser, executor, metrics, examples). no UI code.
- ProgrammingLearningApp: the console app, only input/output, uses Core.
- ProgrammingLearningApp.Tests: unit tests.

# how to run tests
1. cd ProgrammingLearningApp
2. dotnet test


# program files

Same format as the examples in the assignment 4 spaces for indentation:

Repeat 4 times
    Move 10
    Turn right

The character starts at (0,0) facing east. Going south increases y, going north decreases y.
