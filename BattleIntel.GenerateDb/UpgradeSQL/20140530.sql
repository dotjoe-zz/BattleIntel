alter table dbo.BattleStat 
    add IsDeleted BIT
alter table dbo.IntelReport 
    add UpdatedText NVARCHAR(MAX)
alter table dbo.IntelReport 
    add HadTruncatedLine BIT

update IntelReport set HadTruncatedLine = 0;
update BattleStat set IsDeleted = 0;

alter table dbo.IntelReport
	alter column HadTruncatedLine BIT not null
alter table dbo.BattleStat 
    alter column IsDeleted BIT not null