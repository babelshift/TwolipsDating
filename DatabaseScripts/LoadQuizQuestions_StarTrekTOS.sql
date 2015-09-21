declare @answers as dbo.AnswerType;

delete from @answers;
insert into @answers(Content, IsCorrect) values('Kirk',1);
insert into @answers(Content, IsCorrect) values('Spock',0);
insert into @answers(Content, IsCorrect) values('McCoy',0);
insert into @answers(Content, IsCorrect) values('Scotty',0);
exec dbo.InsertQuizQuestion 'Who was the captain of the USS Enterprise?',1,21, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Kirk',0);
insert into @answers(Content, IsCorrect) values('Spock',1);
insert into @answers(Content, IsCorrect) values('McCoy',0);
insert into @answers(Content, IsCorrect) values('Scotty',0);
exec dbo.InsertQuizQuestion 'Who was the first officer of the USS Enterprise?',1,21, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Kirk',0);
insert into @answers(Content, IsCorrect) values('Spock',0);
insert into @answers(Content, IsCorrect) values('McCoy',1);
insert into @answers(Content, IsCorrect) values('Scotty',0);
exec dbo.InsertQuizQuestion 'Who was the chief medical officer of the USS Enterprise?',1,21, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Kirk',0);
insert into @answers(Content, IsCorrect) values('Spock',0);
insert into @answers(Content, IsCorrect) values('McCoy',0);
insert into @answers(Content, IsCorrect) values('Scotty',1);
exec dbo.InsertQuizQuestion 'Who was the chief engineer of the USS Enterprise?',1,21, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('1',0);
insert into @answers(Content, IsCorrect) values('2',0);
insert into @answers(Content, IsCorrect) values('3',1);
insert into @answers(Content, IsCorrect) values('4',0);
exec dbo.InsertQuizQuestion 'How many seasons did The Original Series have?',3,21, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('To get past their violent history',1);
insert into @answers(Content, IsCorrect) values('They thought it would be trendy and cool',0);
insert into @answers(Content, IsCorrect) values('So they could get better grades on tests',0);
insert into @answers(Content, IsCorrect) values('To resist the various creatures that are powered by emotion throughout the galaxy',0);
exec dbo.InsertQuizQuestion 'Why did vulcans decide to behave in extremely logical manners?',3,21, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('TRUE',0);
insert into @answers(Content, IsCorrect) values('FALSE',1);
insert into @answers(Content, IsCorrect) values('',0);
insert into @answers(Content, IsCorrect) values('',0);
exec dbo.InsertQuizQuestion 'Kirk often made deals with the enemy for control of his starship. True or false?',2,21, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Gorn',1);
insert into @answers(Content, IsCorrect) values('Spyrn',0);
insert into @answers(Content, IsCorrect) values('Sleeslak',0);
insert into @answers(Content, IsCorrect) values('Tribble',0);
exec dbo.InsertQuizQuestion 'What was the name of the creature that Kirk was forced to fight alone?',3,21, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Andorians',0);
insert into @answers(Content, IsCorrect) values('Argelians',0);
insert into @answers(Content, IsCorrect) values('Amoeba',0);
insert into @answers(Content, IsCorrect) values('Klingons',1);
exec dbo.InsertQuizQuestion 'Which of the following races was most continiously hostile to the Federation?',2,21, @answers;
delete from @answers;
insert into @answers(Content, IsCorrect) values('Antimatter',0);
insert into @answers(Content, IsCorrect) values('Transporter',1);
insert into @answers(Content, IsCorrect) values('Telepathy',0);
insert into @answers(Content, IsCorrect) values('Shuttlecraft',0);
exec dbo.InsertQuizQuestion 'How did crewmembers most commonly go from the starship to various planet surfaces?',2,21, @answers;