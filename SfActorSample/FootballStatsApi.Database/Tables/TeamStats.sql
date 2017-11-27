CREATE TABLE [dbo].[TeamStats]
(
	[TeamId] NVARCHAR(3) NOT NULL ,
	[Year] SMALLINT NOT NULL,
	[Week] TINYINT NOT NULL,
	[TeamName] NVARCHAR(128) NOT NULL,
	[PointsFor] SMALLINT NOT NULL,
	[PointsAgainst] SMALLINT NOT NULL,
	[Wins] TINYINT NOT NULL,
	[Losses] TINYINT NOT NULL,
	[GamesPlayed] TINYINT NOT NULL,
	[Created] DATETIME NOT NULL DEFAULT getutcdate(),
	[LastUpdated] DATETIME NOT NULL DEFAULT getutcdate(),
    PRIMARY KEY ([TeamId], [Year], [Week])
)
