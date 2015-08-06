declare @answers as dbo.AnswerType;

insert into @answers(Content, IsCorrect) values('Grammar', 0);
insert into @answers(Content, IsCorrect) values('Linguistics', 1);
insert into @answers(Content, IsCorrect) values('Poetry', 0);
insert into @answers(Content, IsCorrect) values('Communication', 0);
exec dbo.InsertQuizQuestion 'What is the scientific study of language?', 0, 5, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('100 to 300', 0);
insert into @answers(Content, IsCorrect) values('1,000 to 3,000', 0);
insert into @answers(Content, IsCorrect) values('5,000 to 7,000', 1);
insert into @answers(Content, IsCorrect) values('10,000 to 20,000', 0);
exec dbo.InsertQuizQuestion 'How many languages are estimated to exist in the world?', 0, 5, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Embossed surface', 1);
insert into @answers(Content, IsCorrect) values('Digital images', 0);
insert into @answers(Content, IsCorrect) values('Sounds and voices', 0);
insert into @answers(Content, IsCorrect) values('Interpretive dance', 0);
exec dbo.InsertQuizQuestion 'Braille is a form of encoding language into what?', 0, 5, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Heart', 0);
insert into @answers(Content, IsCorrect) values('Lungs', 0);
insert into @answers(Content, IsCorrect) values('Brain', 1);
insert into @answers(Content, IsCorrect) values('Liver', 0);
exec dbo.InsertQuizQuestion 'Which organ coordinates all linguistic activity?', 0, 5, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Indo-European', 1);
insert into @answers(Content, IsCorrect) values('Afro-Asiatic', 0);
insert into @answers(Content, IsCorrect) values('Dravidian', 0);
insert into @answers(Content, IsCorrect) values('Malayo-Polynesian', 0);
exec dbo.InsertQuizQuestion 'English is part of what language family?', 0, 5, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Early childhood', 1);
insert into @answers(Content, IsCorrect) values('Mid childhood', 0);
insert into @answers(Content, IsCorrect) values('Teenager', 0);
insert into @answers(Content, IsCorrect) values('Adult', 0);
exec dbo.InsertQuizQuestion 'At what stage of human development is language most easily learned?', 0, 5, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('It is less structured', 1);
insert into @answers(Content, IsCorrect) values('It is able to be encoded', 0);
insert into @answers(Content, IsCorrect) values('It is open ended and productive', 1);
insert into @answers(Content, IsCorrect) values('It has the potential to be loud and obnoxious', 0);
exec dbo.InsertQuizQuestion 'What is one way in which human language is unique compared to other animals?', 0, 5, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Chattering', 0);
insert into @answers(Content, IsCorrect) values('Stuttering', 1);
insert into @answers(Content, IsCorrect) values('Dyslexia', 0);
insert into @answers(Content, IsCorrect) values('Muteness', 0);
exec dbo.InsertQuizQuestion 'Which speech disorder is recognized by repetitions of sounds?', 0, 5, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Noam Chomsky', 0);
insert into @answers(Content, IsCorrect) values('Plato', 0);
insert into @answers(Content, IsCorrect) values('Paul Broca', 1);
insert into @answers(Content, IsCorrect) values('Civil rights', 0);
exec dbo.InsertQuizQuestion 'Who discovered the speech center of the human brain?', 0, 5, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('1984', 1);
insert into @answers(Content, IsCorrect) values('Of Mice and Men', 0);
insert into @answers(Content, IsCorrect) values('Contact', 0);
insert into @answers(Content, IsCorrect) values('Blood Meridian', 0);
exec dbo.InsertQuizQuestion 'Which book involves using tight control over language to control how people think?', 0, 5, @answers;