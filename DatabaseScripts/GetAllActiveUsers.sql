use twolipsaqoechisy;
select * from dbo.aspnetusers 
where isactive = 1
order by datelastlogin desc
