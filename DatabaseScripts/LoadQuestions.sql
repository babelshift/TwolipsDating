declare @answers as dbo.AnswerType;

--insert into @answers(Content, IsCorrect) values('4', 1);
--insert into @answers(Content, IsCorrect) values('5', 0);
--insert into @answers(Content, IsCorrect) values('-1', 0);
--insert into @answers(Content, IsCorrect) values('10', 0);
--exec dbo.InsertQuestion '2 + 2 = x. Find x.', @answers;

--delete from @answers;
--insert into @answers(Content, IsCorrect) values('Bricks', 0);
--insert into @answers(Content, IsCorrect) values('Feathers', 0);
--insert into @answers(Content, IsCorrect) values('They weigh the same.', 1);
--exec dbo.InsertQuestion 'What is heavier? Two pounds of bricks or two pounds of feathers?', @answers;

--delete from @answers;
--insert into @answers(Content, IsCorrect) values('Baltimore', 0);
--insert into @answers(Content, IsCorrect) values('Tallahassee', 1);
--insert into @answers(Content, IsCorrect) values('Miami', 0);
--insert into @answers(Content, IsCorrect) values('Tampa', 0);
--exec dbo.InsertQuestion 'What is the capitol city of Florida?', @answers;

--delete from @answers;
--insert into @answers(Content, IsCorrect) values('0', 0);
--insert into @answers(Content, IsCorrect) values('10', 0);
--insert into @answers(Content, IsCorrect) values('24', 1);
--insert into @answers(Content, IsCorrect) values('50', 0);
--exec dbo.InsertQuestion 'How many inches are in 2 feet?', @answers;

--delete from @answers;
--insert into @answers(Content, IsCorrect) values('1', 0);
--insert into @answers(Content, IsCorrect) values('2', 0);
--insert into @answers(Content, IsCorrect) values('3', 0);
--insert into @answers(Content, IsCorrect) values('4', 1);
--exec dbo.InsertQuestion 'Four doors are open. Close two doors. Open one door. How many doors were originally open?', @answers;

delete from @answers;
insert into @answers(Content, IsCorrect) values('1', 0);
insert into @answers(Content, IsCorrect) values('2', 0);
insert into @answers(Content, IsCorrect) values('3', 0);
insert into @answers(Content, IsCorrect) values('4', 1);
exec dbo.InsertQuestion 'Four doors are open. Close two doors. Open one door. How many doors were originally open?', @answers;

--delete from dbo.TagQuestions;
--delete from dbo.AnsweredQuestions;
--update dbo.Questions
--set CorrectAnswerId = null;
--delete from dbo.Answers;
--delete from dbo.Questions;