
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_BattleStat_Battle]') AND parent_object_id = OBJECT_ID('dbo.BattleStat'))
alter table dbo.BattleStat  drop constraint FK_BattleStat_Battle


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_BattleStat_Team]') AND parent_object_id = OBJECT_ID('dbo.BattleStat'))
alter table dbo.BattleStat  drop constraint FK_BattleStat_Team


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_UserOpenId_User]') AND parent_object_id = OBJECT_ID('dbo.UserOpenId'))
alter table dbo.UserOpenId  drop constraint FK_UserOpenId_User


    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.Battle') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.Battle

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.BattleStat') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.BattleStat

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.Team') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.Team

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.[User]') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.[User]

    if exists (select * from dbo.sysobjects where id = object_id(N'dbo.UserOpenId') and OBJECTPROPERTY(id, N'IsUserTable') = 1) drop table dbo.UserOpenId

    create table dbo.Battle (
        Id INT IDENTITY NOT NULL,
       Name NVARCHAR(255) not null unique,
       StartDateUTC DATETIME not null,
       EndDateUTC DATETIME not null,
       primary key (Id)
    )

    create table dbo.BattleStat (
        Id INT IDENTITY NOT NULL,
       BattleId INT not null,
       TeamId INT not null,
       Stat_RawInput NVARCHAR(255) null,
       Stat_ScrubbedInput NVARCHAR(255) null,
       Stat_Level INT null,
       Stat_Name NVARCHAR(255) null,
       Stat_Defense NVARCHAR(255) null,
       Stat_DefenseValue DECIMAL(19,5) null,
       Stat_AdditionalInfo NVARCHAR(255) null,
       primary key (Id)
    )

    create table dbo.Team (
        Id INT IDENTITY NOT NULL,
       Name NVARCHAR(255) not null unique,
       primary key (Id)
    )

    create table dbo.[User] (
        Id INT IDENTITY NOT NULL,
       Email NVARCHAR(255) not null,
       DisplayName NVARCHAR(255) not null,
       JoinDateUTC DATETIME not null,
       primary key (Id)
    )

    create table dbo.UserOpenId (
        Id INT IDENTITY NOT NULL,
       UserId INT not null,
       OpenIdentifier NVARCHAR(255) not null unique,
       primary key (Id)
    )

    alter table dbo.BattleStat 
        add constraint FK_BattleStat_Battle 
        foreign key (BattleId) 
        references dbo.Battle

    alter table dbo.BattleStat 
        add constraint FK_BattleStat_Team 
        foreign key (TeamId) 
        references dbo.Team

    alter table dbo.UserOpenId 
        add constraint FK_UserOpenId_User 
        foreign key (UserId) 
        references dbo.[User]
