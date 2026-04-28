# TODO List

## Quiz Page

- add `Show Answer` button
- add answer time and correct columns in student answers grid
- add another a.JPG image in question 

if Teacher clicks on `Show Answer` button 
- a.jpg replace q.jpg
- answer time and correct columns are populated from table in student grid 

### Quiz Result Page
Create new Quiz Result page only for teacher with 
- student answers grid aggregating all correct and incorrect answers for each student in quiz
- add export button to export csv file from QuizAnswers table with columns
    - StudentDisplayName
    - QuestionText
    - CorrectAnswer
    - Answer
    - Answer Time = EndedAtUtc - StartedAtUtc in seconds
    - IsCorrect 

## School/Class page 
Create new page only for teacher with
- school dropdown value from Schools table. it has `Add` button on right side to insert new school value in popup.
- class dropdown filtered by selected school from Classes table. it has `Add` button on right side to insert new class values in popup.
- add student grid filtered by selected class with search bar by user display name.


## Answers count configuration
Make answer is configurable for each question with default value in quiz.json and overridden in each question.
StudentQuizAnswerPanel should show buttons based on it.
Use SignalR for that.
