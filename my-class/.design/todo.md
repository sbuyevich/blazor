# TO DO List

## UI Changes

### Quiz Page

- add `Show Answer` button
- add answer time and correct columns in students answers grid
- add other a.JPG image in question 

if  Teacher clicks on `Show Answer` button 
- a.jpg replace q.jpg
- answer time and correct columns are populated from table in students grid 

### Quiz Result Page
Create new Quiz Result page only for teacher with 
- students answers grid aggregated all correct and incorrect answers for each students in quiz
- add export button to export csv file from QuizAnswers table with columns
    - StudentDisplayName
    - QuestionText
    - CorrectAnswer
    - Answer
    - Answer Time = EndedAtUtc - StartedAtUtc in seconds
    - IsCorrect 

### School/Class page 
Create new page only for teacher with
- school dropdown value from Schools table. it has `Add` button on right side to insert new school value in popup.
- class dropdown filtered by selected school from Classes table. it has `Add` button on right side to insert new class values in popup.
- add students grid filtered by selected class with search bar by user display name.