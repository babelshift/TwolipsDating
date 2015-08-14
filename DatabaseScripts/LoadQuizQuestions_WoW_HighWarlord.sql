declare @answers as dbo.AnswerType;

insert into @answers(Content, IsCorrect) values('Furbolgs', 1);
insert into @answers(Content, IsCorrect) values('Skeletons', 0);
insert into @answers(Content, IsCorrect) values('Whelps', 0);
insert into @answers(Content, IsCorrect) values('Oozes', 0);
exec dbo.InsertQuizQuestion 'Which creatures stand between Felwood and Winterspring?', 5, 8, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Warcraft', 0);
insert into @answers(Content, IsCorrect) values('Warcraft II', 1);
insert into @answers(Content, IsCorrect) values('Warcraft III', 0);
insert into @answers(Content, IsCorrect) values('Hearthstone', 0);
exec dbo.InsertQuizQuestion 'Other than Warlords of Draenor, in which Warcraft-themed game did Khadgar appear as a main character?', 4, 8, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Napoleon Dynamite', 0);
insert into @answers(Content, IsCorrect) values('MC Hammer', 0);
insert into @answers(Content, IsCorrect) values('Vanilla Ice', 0);
insert into @answers(Content, IsCorrect) values('Michael Jackson', 1);
exec dbo.InsertQuizQuestion 'The night elf dance emote is based on which celebrity?', 4, 8, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Ulduar', 0);
insert into @answers(Content, IsCorrect) values('Vault of Archavon', 1);
insert into @answers(Content, IsCorrect) values('Trial of Champions', 0);
insert into @answers(Content, IsCorrect) values('Molten Core', 0);
exec dbo.InsertQuizQuestion 'What was the name of the first raid zone to begin offering PvP gear as loot?', 5, 8, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Zeppelin', 1);
insert into @answers(Content, IsCorrect) values('Boat', 0);
insert into @answers(Content, IsCorrect) values('Subway', 0);
insert into @answers(Content, IsCorrect) values('Gyrocopter', 0);
exec dbo.InsertQuizQuestion 'Which type of vehicle connected Orgrimmar to Undercity?', 4, 8, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Waterfall', 0);
insert into @answers(Content, IsCorrect) values('Mudslides', 0);
insert into @answers(Content, IsCorrect) values('Lava-filled chasm', 1);
insert into @answers(Content, IsCorrect) values('Void holes', 0);
exec dbo.InsertQuizQuestion 'What large environmental change was introduced to The Barrens in the Cataclysm expansion??', 5, 8, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Rockbiter', 0)
insert into @answers(Content, IsCorrect) values('Windfury', 1);
insert into @answers(Content, IsCorrect) values('Grace of air', 0);
insert into @answers(Content, IsCorrect) values('Mana tide', 0);
exec dbo.InsertQuizQuestion 'In the original game, which Shaman totem granted party members with a chance to gain multiple attacks per swing?', 4, 8, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('The raid is teleported out of the zone', 0);
insert into @answers(Content, IsCorrect) values('Multiple mini-bosses are spawned', 1);
insert into @answers(Content, IsCorrect) values('The boss teleports out of the zone', 0);
insert into @answers(Content, IsCorrect) values('The boss continuously sends out shadow bolts until the raid dies', 0);
exec dbo.InsertQuizQuestion 'When too much time has passed in the original Lord Kazzak encounter, what happens?', 5, 8, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Kel''Thuzad', 1);
insert into @answers(Content, IsCorrect) values('Archimonde', 0);
insert into @answers(Content, IsCorrect) values('Lich King', 0);
insert into @answers(Content, IsCorrect) values('Illidan', 0);
exec dbo.InsertQuizQuestion 'Who is the final boss of Naxxramas?', 3, 8, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('The Butcher', 0);
insert into @answers(Content, IsCorrect) values('Anathema', 0);
insert into @answers(Content, IsCorrect) values('Rhok''Delar', 1);
insert into @answers(Content, IsCorrect) values('Cleavermaster', 0);
exec dbo.InsertQuizQuestion 'What was the name of the Hunter-only epic bow as part of the Hunter-only quest in the original game?', 5, 8, @answers;