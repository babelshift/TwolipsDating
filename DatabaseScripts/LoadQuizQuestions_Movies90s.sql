declare @answers as dbo.AnswerType;

delete from @answers;
insert into @answers(Content, IsCorrect) values('Marsellus Wallace',1);
insert into @answers(Content, IsCorrect) values('Jules Winnfield',0);
insert into @answers(Content, IsCorrect) values('Mia Wallace',0);
insert into @answers(Content, IsCorrect) values('Butch Coolidge',0);
exec dbo.InsertQuizQuestion 'Who did Vincent Vega work for in Pulp Fiction?',3,17, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Theft',0);
insert into @answers(Content, IsCorrect) values('Murder',1);
insert into @answers(Content, IsCorrect) values('Inside trading',0);
insert into @answers(Content, IsCorrect) values('Arson',0);
exec dbo.InsertQuizQuestion 'What was Andy Dufresne in jail for in The Shawshank Redemption?',1,17, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Hookshot',0);
insert into @answers(Content, IsCorrect) values('Singing Killer',0);
insert into @answers(Content, IsCorrect) values('Blackwater',0);
insert into @answers(Content, IsCorrect) values('Ghostface',1);
exec dbo.InsertQuizQuestion 'What was the nickname of the killer in Scream?',2,17, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Robert Paulson',1);
insert into @answers(Content, IsCorrect) values('Thomas Anderson',0);
insert into @answers(Content, IsCorrect) values('Billy Bob',0);
insert into @answers(Content, IsCorrect) values('Nicolas Cage',0);
exec dbo.InsertQuizQuestion 'Finish this sentence from Fight Club: His name was ___.',2,17, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Forrest Gump',0);
insert into @answers(Content, IsCorrect) values('Titanic',1);
insert into @answers(Content, IsCorrect) values('Star Trek VI',0);
insert into @answers(Content, IsCorrect) values('Star Wars: Episode I',0);
exec dbo.InsertQuizQuestion 'Which movie from the 90s is the highest grossing film of all time?',2,17, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Johnson',0);
insert into @answers(Content, IsCorrect) values('Wilson',0);
insert into @answers(Content, IsCorrect) values('Smith',1);
insert into @answers(Content, IsCorrect) values('Lars',0);
exec dbo.InsertQuizQuestion 'What was the last name of the main Agent in The Matrix?',1,17, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Flowers',0);
insert into @answers(Content, IsCorrect) values('A box of chocolates',1);
insert into @answers(Content, IsCorrect) values('Running shoes',0);
insert into @answers(Content, IsCorrect) values('A race',0);
exec dbo.InsertQuizQuestion 'According to Forrest Gump''s mother, life is like what?',1,17, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Rose',1);
insert into @answers(Content, IsCorrect) values('Tulip',0);
insert into @answers(Content, IsCorrect) values('Dandelion',0);
insert into @answers(Content, IsCorrect) values('Penta',0);
exec dbo.InsertQuizQuestion 'In American Beauty, which flowers makes multiple symbolic appearances?',3,17, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Wes Anderson',0);
insert into @answers(Content, IsCorrect) values('M. Night Shyamalan',1);
insert into @answers(Content, IsCorrect) values('Steven Spielberg',0);
insert into @answers(Content, IsCorrect) values('Robert Zemeckis',0);
exec dbo.InsertQuizQuestion 'Who directed The Sixth Sense?',2,17, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Carl Sagan',1);
insert into @answers(Content, IsCorrect) values('Isaac Asimov',0);
insert into @answers(Content, IsCorrect) values('Stephen King',0);
insert into @answers(Content, IsCorrect) values('Mark Twain',0);
exec dbo.InsertQuizQuestion 'Contact was based on a book by which author?',3,17, @answers;