declare @answers as dbo.AnswerType;

insert into @answers(Content, IsCorrect) values('Bowser', 0);
insert into @answers(Content, IsCorrect) values('Ganon', 1);
insert into @answers(Content, IsCorrect) values('Link', 0);
insert into @answers(Content, IsCorrect) values('Gerudo', 0);
exec dbo.InsertQuizQuestion 'What is the name of the main antagonist in the Legend of Zelda universe?', 2, 10, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Quadforce', 0);
insert into @answers(Content, IsCorrect) values('Triforce', 1);
insert into @answers(Content, IsCorrect) values('Force of Power', 0);
insert into @answers(Content, IsCorrect) values('Lotus Leaf', 0);
exec dbo.InsertQuizQuestion 'What is the name of the magical triple triangle object that grants certain powers?', 2, 10, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Mushroom Kingdom', 0);
insert into @answers(Content, IsCorrect) values('Azeroth', 0);
insert into @answers(Content, IsCorrect) values('Hyrule', 1);
insert into @answers(Content, IsCorrect) values('Nappa Valley', 0);
exec dbo.InsertQuizQuestion 'What is the name of the kingdom in which the events of the games take place?', 3, 10, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Playstation', 0);
insert into @answers(Content, IsCorrect) values('Nintendo 64', 0);
insert into @answers(Content, IsCorrect) values('Super Nintendo', 0);
insert into @answers(Content, IsCorrect) values('Nintendo', 1);
exec dbo.InsertQuizQuestion 'On which game console did the original Legend of Zelda game release??', 2, 10, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Koji Kondo', 1);
insert into @answers(Content, IsCorrect) values('Tetsuya Nomura', 0);
insert into @answers(Content, IsCorrect) values('Shigeru Miyamoto', 0);
insert into @answers(Content, IsCorrect) values('Shinji Hashimoto', 0);
exec dbo.InsertQuizQuestion 'Who composed the music for the series?', 4, 10, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Shigeru Miyamoto''s imagination', 0);
insert into @answers(Content, IsCorrect) values('Weather patterns', 0);
insert into @answers(Content, IsCorrect) values('F. Scott Fitgerald''s wife', 1);
insert into @answers(Content, IsCorrect) values('The name of a popular food', 0);
exec dbo.InsertQuizQuestion 'The name Zelda was inspired by who?', 4, 10, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('The Adventure of Link', 1);
insert into @answers(Content, IsCorrect) values('A Link to the Past', 0);
insert into @answers(Content, IsCorrect) values('Majora''s Mask', 0);
insert into @answers(Content, IsCorrect) values('Ocarina of Time', 0);
exec dbo.InsertQuizQuestion 'What is the name of the second Legend of Zelda game?', 1, 10, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Ganon', 0);
insert into @answers(Content, IsCorrect) values('Ganondorf', 1);
insert into @answers(Content, IsCorrect) values('Sheik', 0);
insert into @answers(Content, IsCorrect) values('Link', 0);
exec dbo.InsertQuizQuestion 'What is Ganon''s name when in human form?', 3, 10, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Top and Bottom', 0);
insert into @answers(Content, IsCorrect) values('Good and Evil', 0);
insert into @answers(Content, IsCorrect) values('Dark and Light', 1);
insert into @answers(Content, IsCorrect) values('Smart and Stupid', 0);
exec dbo.InsertQuizQuestion 'What is the name of the two opposing worlds/dimensions in A Link to the Past?', 3, 10, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Deku seeds', 1);
insert into @answers(Content, IsCorrect) values('Pebbles', 0);
insert into @answers(Content, IsCorrect) values('Tree nuts', 0);
insert into @answers(Content, IsCorrect) values('Apple cores', 0);
exec dbo.InsertQuizQuestion 'What is the name of the item that can be used as slingshot ammo in Ocarina of Time?', 4, 10, @answers;