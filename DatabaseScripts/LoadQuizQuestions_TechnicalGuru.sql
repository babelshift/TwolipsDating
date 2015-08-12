declare @answers as dbo.AnswerType;

insert into @answers(Content, IsCorrect) values('Videocassette Recorder', 1);
insert into @answers(Content, IsCorrect) values('Visual Control Room', 0);
insert into @answers(Content, IsCorrect) values('Voices of Civil Rights', 0);
insert into @answers(Content, IsCorrect) values('Voltage Controlled Resistance', 0);
exec dbo.InsertQuizQuestion 'What does VCR most likely stand for?', 2, 1, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Digital Video Disc', 0);
insert into @answers(Content, IsCorrect) values('Digital Versatile Disc', 1);
insert into @answers(Content, IsCorrect) values('Digital Video Drive', 0);
insert into @answers(Content, IsCorrect) values('Digital Virtual Disk', 0);
exec dbo.InsertQuizQuestion 'What does DVD most likely stand for?', 2, 1, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('500 MB', 0);
insert into @answers(Content, IsCorrect) values('1.3 GB', 0);
insert into @answers(Content, IsCorrect) values('2.5 GB', 0);
insert into @answers(Content, IsCorrect) values('4.7 GB', 1);
exec dbo.InsertQuizQuestion 'How much data can a standard single layer DVD hold?', 4, 1, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('300 MB', 0);
insert into @answers(Content, IsCorrect) values('500 MB', 0);
insert into @answers(Content, IsCorrect) values('700 MB', 1);
insert into @answers(Content, IsCorrect) values('900 MB', 0);
exec dbo.InsertQuizQuestion 'How much data can a standard CD hold?', 4, 1, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('15 GB', 0);
insert into @answers(Content, IsCorrect) values('20 GB', 0);
insert into @answers(Content, IsCorrect) values('25 GB', 1);
insert into @answers(Content, IsCorrect) values('30 GB', 0);
exec dbo.InsertQuizQuestion 'How much data can a standard single sided Blu-ray disc hold?', 4, 1, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('CPU', 0);
insert into @answers(Content, IsCorrect) values('RAM', 0);
insert into @answers(Content, IsCorrect) values('Motherboard', 1);
insert into @answers(Content, IsCorrect) values('Power supply', 0);
exec dbo.InsertQuizQuestion 'ATX is a configuration for which computer component?', 5, 1, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('1920x1080', 1);
insert into @answers(Content, IsCorrect) values('3200x1800', 0);
insert into @answers(Content, IsCorrect) values('1280x720', 0);
insert into @answers(Content, IsCorrect) values('2048x1152', 0);
exec dbo.InsertQuizQuestion 'What is the most popular display resolution when using 1080p?', 2, 1, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('Red', 0);
insert into @answers(Content, IsCorrect) values('Blue', 1);
insert into @answers(Content, IsCorrect) values('Black', 0);
insert into @answers(Content, IsCorrect) values('Brown', 0);
exec dbo.InsertQuizQuestion 'Which color of Cherry MX mechanical keyboard switches has an audible click?', 5, 1, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('SLI', 1);
insert into @answers(Content, IsCorrect) values('LIS', 0);
insert into @answers(Content, IsCorrect) values('APC', 0);
insert into @answers(Content, IsCorrect) values('CPU', 0);
exec dbo.InsertQuizQuestion 'What acronym is used to describe the use of multiple NVIDIA GPUs in a single PC.', 4, 1, @answers;

delete from @answers;

insert into @answers(Content, IsCorrect) values('TRUE', 1);
insert into @answers(Content, IsCorrect) values('FALSE', 0);
exec dbo.InsertQuizQuestion 'HDMI is capable of carrying both video and audio. True or false?', 2, 1, @answers;