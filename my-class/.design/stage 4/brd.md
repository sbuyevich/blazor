# Quiz Reading

- Quiz class is loaded in memory when the teacher loads the quiz page.
- Quiz should read from the quiz folder defined in RootFolder, populating Quiz class values.
- TimeLimitSeconds value in QuizQuestion class is overridden from Quiz class.
- correctAnswer remains a string and is compared with the student's Answer string.
- Student Answer can be empty when the student does not have time to answer.

## Assumptions
Question subfolders have the same named files:
- q.jpg presents the question when the quiz starts.
- q.json presents data for answer validation.

# Quiz Answer Saving

- no need session since this table will present only answers for the one quiz session.
- make QuizAnswers page denormalized with student name, last name and question
- table is truncated when teacher click on Start quiz
- for each question is created record for each active user with current question and start time when teacher clicks on Next button
- end time is update as well as answer and IaCorrect

    

