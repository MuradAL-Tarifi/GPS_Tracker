CREATE TABLE [dbo].[WaslCode] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Code]      NVARCHAR (MAX) NULL,
    [MessageAr] NVARCHAR (MAX) NULL,
    [MessageEn] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_WaslCode] PRIMARY KEY CLUSTERED ([Id] ASC)
);

