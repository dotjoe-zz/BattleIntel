
    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Battle_CreatedBy]') AND parent_object_id = OBJECT_ID('dbo.Battle'))
alter table dbo.Battle  drop constraint FK_Battle_CreatedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Battle_LastUpdatedBy]') AND parent_object_id = OBJECT_ID('dbo.Battle'))
alter table dbo.Battle  drop constraint FK_Battle_LastUpdatedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_BattleStat_Battle]') AND parent_object_id = OBJECT_ID('dbo.BattleStat'))
alter table dbo.BattleStat  drop constraint FK_BattleStat_Battle


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_BattleStat_Team]') AND parent_object_id = OBJECT_ID('dbo.BattleStat'))
alter table dbo.BattleStat  drop constraint FK_BattleStat_Team


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_BattleStat_CreatedBy]') AND parent_object_id = OBJECT_ID('dbo.BattleStat'))
alter table dbo.BattleStat  drop constraint FK_BattleStat_CreatedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_BattleStat_LastUpdatedBy]') AND parent_object_id = OBJECT_ID('dbo.BattleStat'))
alter table dbo.BattleStat  drop constraint FK_BattleStat_LastUpdatedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Team_CreatedBy]') AND parent_object_id = OBJECT_ID('dbo.Team'))
alter table dbo.Team  drop constraint FK_Team_CreatedBy


    if exists (select 1 from sys.objects where object_id = OBJECT_ID(N'dbo.[FK_Team_LastUpdatedBy]') AND parent_object_id = OBJECT_ID('dbo.Team'))
alter table dbo.Team  drop constraint FK_Team_LastUpdatedBy


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
       CreatedUTC DATETIME not null,
       CreatedById INT not null,
       LastUpdatedUTC DATETIME null,
       LastUpdatedById INT null,
       primary key (Id)
    )

    create table dbo.BattleStat (
        Id INT IDENTITY NOT NULL,
       BattleId INT not null,
       TeamId INT not null,
       Stat_RawInput NVARCHAR(255) null,
       Stat_Level INT null,
       Stat_Name NVARCHAR(255) null,
       Stat_Defense NVARCHAR(255) null,
       CreatedUTC DATETIME not null,
       CreatedById INT not null,
       LastUpdatedUTC DATETIME null,
       LastUpdatedById INT null,
       primary key (Id)
    )

    create table dbo.Team (
        Id INT IDENTITY NOT NULL,
       Name NVARCHAR(255) not null unique,
       CreatedUTC DATETIME not null,
       CreatedById INT not null,
       LastUpdatedUTC DATETIME null,
       LastUpdatedById INT null,
       primary key (Id)
    )

    create table dbo.[User] (
        Id INT IDENTITY NOT NULL,
       Name NVARCHAR(255) not null,
       Email NVARCHAR(255) null,
       JoinDateUTC DATETIME not null,
       primary key (Id)
    )

    create table dbo.UserOpenId (
        Id INT IDENTITY NOT NULL,
       UserId INT not null,
       OpenIdentifier NVARCHAR(255) not null unique,
       primary key (Id)
    )

    alter table dbo.Battle 
        add constraint FK_Battle_CreatedBy 
        foreign key (CreatedById) 
        references dbo.[User]

    alter table dbo.Battle 
        add constraint FK_Battle_LastUpdatedBy 
        foreign key (LastUpdatedById) 
        references dbo.[User]

    alter table dbo.BattleStat 
        add constraint FK_BattleStat_Battle 
        foreign key (BattleId) 
        references dbo.Battle

    alter table dbo.BattleStat 
        add constraint FK_BattleStat_Team 
        foreign key (TeamId) 
        references dbo.Team

    alter table dbo.BattleStat 
        add constraint FK_BattleStat_CreatedBy 
        foreign key (CreatedById) 
        references dbo.[User]

    alter table dbo.BattleStat 
        add constraint FK_BattleStat_LastUpdatedBy 
        foreign key (LastUpdatedById) 
        references dbo.[User]

    alter table dbo.Team 
        add constraint FK_Team_CreatedBy 
        foreign key (CreatedById) 
        references dbo.[User]

    alter table dbo.Team 
        add constraint FK_Team_LastUpdatedBy 
        foreign key (LastUpdatedById) 
        references dbo.[User]

    alter table dbo.UserOpenId 
        add constraint FK_UserOpenId_User 
        foreign key (UserId) 
        references dbo.[User]
