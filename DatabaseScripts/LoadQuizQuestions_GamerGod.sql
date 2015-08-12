declare @answers as dbo.AnswerType;

insert into @answers(Content, IsCorrect) values('MMO', 0);
insert into @answers(Content, IsCorrect) values('FPS', 0);
insert into @answers(Content, IsCorrect) values('MOBA', 1);
insert into @answers(Content, IsCorrect) values('Adventure', 0);
exec dbo.InsertQuizQuestion 'To which genre do the games Dota 2 and League of Legends belong?', 4, 2, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Druid', 0);
insert into @answers(Content, IsCorrect) values('Rogue', 0);
insert into @answers(Content, IsCorrect) values('Hunter', 1);
insert into @answers(Content, IsCorrect) values('Warrior', 0);
exec dbo.InsertQuizQuestion 'In World of Warcraft, which class allows the player to tame animals?', 3, 2, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Xbox One', 0);
insert into @answers(Content, IsCorrect) values('Xbox 360', 1);
insert into @answers(Content, IsCorrect) values('Playstation', 0);
insert into @answers(Content, IsCorrect) values('Virtual Boy', 0);
exec dbo.InsertQuizQuestion 'Which console did Microsoft release after the original Xbox?', 2, 2, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Super Nintendo', 0);
insert into @answers(Content, IsCorrect) values('Wii', 1);
insert into @answers(Content, IsCorrect) values('Wii U', 0);
insert into @answers(Content, IsCorrect) values('Virtual Boy', 0);
exec dbo.InsertQuizQuestion 'Which console did Nintendo release after the Gamecube?', 2, 2, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Terrorists and Counter-Terrorists', 1);
insert into @answers(Content, IsCorrect) values('Police and Criminals', 0);
insert into @answers(Content, IsCorrect) values('Rebels and Officers', 0);
insert into @answers(Content, IsCorrect) values('Natives and Explorers', 0);
exec dbo.InsertQuizQuestion 'Counter-Strike has two teams fighting against each other. Who are they?', 2, 2, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Carrot', 0);
insert into @answers(Content, IsCorrect) values('Potato', 1);
insert into @answers(Content, IsCorrect) values('Mushroom', 0);
insert into @answers(Content, IsCorrect) values('Green bean', 0);
exec dbo.InsertQuizQuestion 'What vegetable does GLaDoS become trapped in during Portal 2''s story?', 3, 2, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Washington', 0);
insert into @answers(Content, IsCorrect) values('Gandhi', 1);
insert into @answers(Content, IsCorrect) values('Montezuma', 0);
insert into @answers(Content, IsCorrect) values('Atilla', 0);
exec dbo.InsertQuizQuestion 'Which leader is jokingly referred to as the most aggressive in Civilization V?', 3, 2, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Quarian', 0);
insert into @answers(Content, IsCorrect) values('Krogan', 0);
insert into @answers(Content, IsCorrect) values('Geth', 1);
insert into @answers(Content, IsCorrect) values('Asari', 0);
exec dbo.InsertQuizQuestion 'What is the name of the AI-collective in the Mass Effect universe?', 3, 2, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('The Strip', 1);
insert into @answers(Content, IsCorrect) values('Nipton', 0);
insert into @answers(Content, IsCorrect) values('Little Lamplight', 0);
insert into @answers(Content, IsCorrect) values('Goodsprings', 0);
exec dbo.InsertQuizQuestion 'In Fallout: New Vegas, Mr. House is in charge of which area?', 3, 2, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('TRUE', 1);
insert into @answers(Content, IsCorrect) values('FALSE', 0);
exec dbo.InsertQuizQuestion 'Star Wars: Knights of the Old Republic I and II were created by different developers. True or false?', 2, 2, @answers;