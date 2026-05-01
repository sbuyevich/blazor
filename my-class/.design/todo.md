# TODO List

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




## Answers count configuration
Make answer is configurable for each question with default value in quiz.json and overridden in each question.
StudentQuizAnswerPanel should show buttons based on it.
Use SignalR for that.
