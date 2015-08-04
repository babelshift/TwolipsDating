drop procedure dbo.InsertQuizQuestion
go
create procedure dbo.InsertQuizQuestion
@content varchar(255),
@points int,
@quizId int,
@answers dbo.AnswerType readonly
as
	declare @latestQuestionId int
	declare @correctAnswerContent varchar(255)
	declare @correctAnswerId int
	declare @questionTypeId int

	select @questionTypeId = 3;

	-- check if question already exists
	if exists
	(
		select 1 from dbo.Questions
		where content = @content
		and points = @points
		and questionTypeId = @questionTypeId
	)
	begin
		-- question already exists in database, assign it to the quiz
		select @latestQuestionId = Id from dbo.Questions
		where content = @content
		and points = @points
		and questionTypeId = @questionTypeId

		-- don't associate this question with the quiz if it already is
		if not exists
		(
			select 1 from dbo.QuizQuestions
			where Question_Id = @latestQuestionId
		)
		insert into dbo.QuizQuestions(Quiz_id, Question_id)
		values(@quizId, @latestQuestionId);
	end
	else
	begin
		-- insert the question
		insert into dbo.Questions(Content, Points, QuestionTypeId)
		values(@content, @points, @questionTypeId);

		-- get the question's id
		select @latestQuestionId = SCOPE_IDENTITY();

		-- insert the answers
		insert into dbo.Answers(Content, QuestionId)
		select Content, @latestQuestionId
		from @answers;

		-- get the correct answer content (this will fail if there is more than one correct answer
		select @correctAnswerContent = Content
		from @answers
		where IsCorrect = 1;

		-- get the correct answer id based on this question id and the correct answer content
		select @correctAnswerId = Id
		from dbo.Answers
		where Content = @correctAnswerContent
		and QuestionId = @latestQuestionId;

		update dbo.Questions
		set CorrectAnswerId = @correctAnswerId
		where Id = @latestQuestionId;
		
		insert into dbo.QuizQuestions(Quiz_id, Question_id)
		values(@quizId, @latestQuestionId);
	end
go

