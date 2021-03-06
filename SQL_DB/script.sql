USE [LogViewer]
GO
/****** Object:  StoredProcedure [dbo].[InsertAvailability]    Script Date: 23.04.2014 14:07:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Andrey Keunetsov
-- Create date: 11.04.2014
-- Description:	Inserts Availability record
-- =============================================
CREATE PROCEDURE [dbo].[InsertAvailability]
   @HostName varchar(255), 
   @IP varchar(255),
   @UserName varchar(255),
   @ServerName varchar(255),
   @DomainName varchar(255)	
AS
BEGIN
	
	SET NOCOUNT ON;

    DECLARE @IdUser INT
    DECLARE @IdServer INT
    DECLARE @IdDomain int

    SELECT  @IdUser = dbo.Users.ID_User
        FROM    dbo.Users
        WHERE   NTLogin = @UserName
	
        IF ( @@ROWCOUNT = 0 ) 
            BEGIN	
                INSERT  INTO dbo.Users
                        ( NTLogin )
                VALUES  ( @UserName   )
                
                SELECT  @IdUser = @@IDENTITY	
             END
      
        SELECT @IdServer = ID_Server FROM [Servers] 
        WHERE ServerName=@ServerName      
        
        
        SELECT  @IdDomain = dbo.[Domains].ID_Domain
        FROM    dbo.[Domains]
        WHERE   DomainName = @DomainName
	
        IF ( @@ROWCOUNT = 0 ) 
            BEGIN	
                INSERT  INTO dbo.[Domains]
                        ( DomainName )
                VALUES  ( @DomainName   )
                
                SELECT  @IdDomain = @@IDENTITY	
             END
         
        INSERT INTO [dbo].[Availability]
                ( 
				   [HostName],
				   [IP] ,
				   [CheckDate],
				   [UserID],
				   [ServerID],
				   [DomainID]
                )
        VALUES  ( 
					@HostName,
					@IP,
					GETDATE(),
					@IdUser,
					@IdServer,
					@IdDomain
                )  
        

         
END

GO
/****** Object:  StoredProcedure [dbo].[spAddRecord]    Script Date: 23.04.2014 14:07:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Andrey Kusnetsov
-- Create date: 09/09/2013
-- Description:	Adding record to db
-- =============================================
CREATE PROCEDURE [dbo].[spAddRecord]
    @MachineName VARCHAR(250) ,
    @UserName VARCHAR(250) ,
    @Server VARCHAR(250) ,
    @IP VARCHAR(20),
    @WinMsgID BIGINT,
    @ActionTime DATETIME,
    @Domain VARCHAR(200),
    @IdRecType TINYINT
AS 
    BEGIN
	--SET NOCOUNT ON;
        DECLARE @IdMachine INT
        DECLARE @IdUser INT
        DECLARE @IdServer INT
        DECLARE @IdDomain int
    
       SELECT  @IdMachine = dbo.Machines.ID_Machine
        FROM    dbo.Machines
        WHERE   Name = @MachineName
        
        
        IF ( @@ROWCOUNT = 0 ) 
            BEGIN	
                INSERT  INTO dbo.Machines
                        ( Name )
                VALUES  ( @MachineName  -- Name - varchar(255)
                          )
                SELECT  @IdMachine = @@IDENTITY	
            END
	
	   SELECT  @IdUser = dbo.Users.ID_User
        FROM    dbo.Users
        WHERE   NTLogin = @UserName
	
        IF ( @@ROWCOUNT = 0 ) 
            BEGIN	
                INSERT  INTO dbo.Users
                        ( NTLogin )
                VALUES  ( @UserName   )
                
                SELECT  @IdUser = @@IDENTITY	
             END
         
        IF(RIGHT(@UserName,1)='$')
          BEGIN
			SET @IdRecType = 3
		  END	
        IF(@IdUser=388 OR @IdUser=550)
          BEGIN 
             SET @IdRecType = 3
          END    
        
       -- IF(@IdMachine=149)
       --   BEGIN
--			SET @IdMachine = 80          
--		  END	
                  
        IF(@IdMachine=80)
			BEGIN
			SET @IdRecType=2
            END
	
	    
	
        
        SELECT @IdServer = ID_Server FROM [Servers] 
        WHERE ServerName=@Server      
        
        
        SELECT  @IdDomain = dbo.[Domains].ID_Domain
        FROM    dbo.[Domains]
        WHERE   DomainName = @Domain
	
        IF ( @@ROWCOUNT = 0 ) 
            BEGIN	
                INSERT  INTO dbo.[Domains]
                        ( DomainName )
                VALUES  ( @Domain   )
                
                SELECT  @IdDomain = @@IDENTITY	
             END
         
        
        
        INSERT INTO dbo.Actions
                ( ID_Machine ,
                  ID_User ,
                  ID_Server ,
                  actts ,
                  IP ,
                  ResolvedHostName ,
                  WindowsActionID,
                  ID_Domain,
                  ID_RecordType                 
                )
        VALUES  ( @IdMachine , -- ID_Machine - int
                  @IdUser , -- ID_User - int
                  @IdServer, -- ID_Server - int
                  @ActionTime , -- actts - datetime
                  @IP , -- IP - varchar(20)
                  @MachineName , -- ResolvedHostName - varchar(250)
                  @WinMsgID, -- WindowsActionID - bigint
                  @IdDomain,
                  @IdRecType
                )  
          
          	
    END

GO
/****** Object:  StoredProcedure [dbo].[spRep1]    Script Date: 23.04.2014 14:07:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		SkyLimited LLC (c) 2013
-- Create date: 18.09.2013
-- Description:	Report composition for SLV3
-- =============================================
CREATE PROCEDURE [dbo].[spRep1]
	-- Add the parameters for the stored procedure here
    @IdServer INT ,
    @dateFrom DATE ,
    @dateTo DATE
AS 
    BEGIN
        SET NOCOUNT ON ;
   
        DECLARE @Result TABLE
            (
              UserName VARCHAR(250) ,
              UserID INT ,
              LastAct DATETIME ,
              MachineList VARCHAR(MAX)
            )
   
        DECLARE @UserName VARCHAR(250)
        DECLARE @LastAct DATETIME
        DECLARE @UserID INT
        DECLARE @Machine VARCHAR(300)
   
    -- Insert statements for procedure here
        IF ( @dateFrom IS NULL ) 
            BEGIN
                INSERT  INTO @Result
                        SELECT  dbo.Users.NTLogin ,
                                dbo.Users.ID_User ,
                                MAX(actts) AS LastAct ,
                                '' AS MachineList
                        FROM    dbo.Actions
                                INNER JOIN Users ON dbo.Users.ID_User = dbo.Actions.ID_User
                        WHERE   ID_Server = @IdServer AND 
                                 (CASE WHEN LEN(dbo.Users.NTLogin)>1 THEN RIGHT(dbo.Users.NTLogin,1) ELSE '$' END) <> '$' AND dbo.Users.NTLogin <> 'АНОНИМНЫЙ ВХОД'
                        GROUP BY NTLogin ,
                                Users.ID_User
            END
        ELSE 
            BEGIN
                INSERT  INTO @Result
                        SELECT  dbo.Users.NTLogin ,
                                dbo.Users.ID_User ,
                                MAX(actts) AS LastAct ,
                                '' AS MachineList
                        FROM    dbo.Actions
                                INNER JOIN Users ON dbo.Users.ID_User = dbo.Actions.ID_User
                        WHERE   ID_Server = @IdServer
                                AND actts BETWEEN @dateFrom AND @dateTo
                                AND 
                                 (CASE WHEN LEN(dbo.Users.NTLogin)>1 THEN RIGHT(dbo.Users.NTLogin,1) ELSE '$' END) <> '$' AND dbo.Users.NTLogin <> 'АНОНИМНЫЙ ВХОД'
                        GROUP BY NTLogin ,
                                Users.ID_User
            END
	
        DECLARE Cur CURSOR
        FOR
            SELECT  UserName ,
                    UserID ,
                    LastAct
            FROM    @Result
        OPEN Cur    
        FETCH NEXT FROM Cur INTO @UserName, @UserID, @LastAct
        IF ( @dateFrom IS NULL ) 
            BEGIN
                WHILE @@FETCH_STATUS = 0 
                    BEGIN
                        DECLARE CurLoc CURSOR
                        FOR
                            SELECT DISTINCT
                                    dbo.Machines.Name
                            FROM    dbo.Actions
                                    INNER JOIN dbo.Machines ON dbo.Actions.ID_Machine = dbo.Machines.ID_Machine
                            WHERE   dbo.Machines.ByPass = 0
                                    AND Actions.ID_User = @UserID   
                                    AND dbo.Machines.Name <> '*Unresolved*'   
                        OPEN CurLoc             
                        FETCH NEXT FROM CurLoc INTO @Machine
                        WHILE @@FETCH_STATUS = 0 
                            BEGIN
                                UPDATE  @Result
                                SET     MachineList = MachineList + ', '
                                        + @Machine
                                WHERE   UserID = @UserID                      
                                FETCH NEXT FROM CurLoc INTO @Machine
                            END
                        CLOSE CurLoc
                        DEALLOCATE CurLoc   
                        FETCH NEXT FROM Cur INTO @UserName, @UserID, @LastAct         	      
                    END
                
            END
        ELSE 
            BEGIN
                WHILE @@FETCH_STATUS = 0 
                    BEGIN	    
                        DECLARE CurLoc CURSOR
                        FOR
                            SELECT DISTINCT
                                    dbo.Machines.Name
                            FROM    dbo.Actions
                                    INNER JOIN dbo.Machines ON dbo.Actions.ID_Machine = dbo.Machines.ID_Machine
                            WHERE   dbo.Machines.ByPass = 0
                                    AND Actions.ID_User = @UserID
                                    AND actts BETWEEN @dateFrom AND @dateTo    AND dbo.Machines.Name <> '*Unresolved*' 
                        OPEN CurLoc
                        FETCH NEXT FROM CurLoc INTO @Machine
                        WHILE @@FETCH_STATUS = 0 
                            BEGIN
                                UPDATE  @Result
                                SET     MachineList = MachineList + ', '
                                        + @Machine
                                WHERE   UserID = @UserID
                                FETCH NEXT FROM CurLoc INTO @Machine
                            END	 
                        CLOSE CurLoc
                        DEALLOCATE CurLoc    
                        FETCH NEXT FROM Cur INTO @UserName, @UserID, @LastAct  
                    END           
            END
        
    END
    CLOSE Cur 
    DEALLOCATE CUR
    SELECT  UserName,UserID,LastAct,RIGHT(MachineList,LEN(MachineList)-1) AS MachineList
    FROM    @Result WHERE LEN(MachineList)>1

GO
/****** Object:  StoredProcedure [dbo].[spRep2]    Script Date: 23.04.2014 14:07:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		SkyLimited LLC (c) 2013
-- Create date: 18.09.2013
-- Description:	Report composition for SLV3
-- =============================================
CREATE PROCEDURE [dbo].[spRep2]
	-- Add the parameters for the stored procedure here
    @IdServer INT ,
    @dateFrom DATE ,
    @dateTo DATE
AS 
    BEGIN
        SET NOCOUNT ON ;
   
        DECLARE @Result TABLE
            (
              MachineName VARCHAR(250) ,
              MachineID INT ,
              LastAct DATETIME ,
              UserList VARCHAR(MAX)
            )
   
        DECLARE @MachineName VARCHAR(250)
        DECLARE @LastAct DATETIME
        DECLARE @MachineID INT
        DECLARE @UserName VARCHAR(300)
   
    -- Insert statements for procedure here
        IF ( @dateFrom IS NULL ) 
            BEGIN
                INSERT  INTO @Result
                        SELECT  dbo.Machines.Name ,
                                dbo.Machines.ID_Machine ,
                                MAX(actts) AS LastAct ,
                                '' AS UserList
                        FROM    dbo.Actions
                                INNER JOIN Machines ON dbo.Machines.ID_Machine = dbo.Actions.ID_Machine
                        WHERE   ID_Server = @IdServer
                                AND dbo.Actions.ID_Machine <> 80
                        GROUP BY Name ,
                                Machines.ID_Machine
            END
        ELSE 
            BEGIN
                INSERT  INTO @Result
                        SELECT  dbo.Machines.Name ,
                                dbo.Machines.ID_Machine ,
                                MAX(actts) AS LastAct ,
                                '' AS UserList
                        FROM    dbo.Actions
                                INNER JOIN Machines ON dbo.Machines.ID_Machine = dbo.Actions.ID_Machine
                        WHERE   ID_Server = @IdServer
                                AND dbo.Actions.ID_Machine <> 80
                                AND CAST(actts AS DATE) BETWEEN @datefrom AND @dateTo
                        GROUP BY Name ,
                                Machines.ID_Machine
            END
	
        DECLARE Cur CURSOR
        FOR
            SELECT  MachineName ,
                    MachineID ,
                    LastAct
            FROM    @Result
        OPEN Cur    
        
        FETCH NEXT FROM Cur INTO @MachineName, @MachineID, @LastAct
        IF ( @dateFrom IS NULL ) 
            BEGIN
                WHILE @@FETCH_STATUS = 0 
                    BEGIN
                        DECLARE CurLoc CURSOR
                        FOR
                            SELECT DISTINCT
                                    dbo.Users.NTLogin
                            FROM    dbo.Actions
                                    INNER JOIN dbo.Users ON dbo.Users.ID_USER = dbo.Actions.ID_User
                            WHERE   dbo.Users.ByPass = 0
                                    AND Actions.ID_Machine = @MachineID
                                    AND ( CASE WHEN LEN(dbo.Users.NTLogin) > 1
                                               THEN RIGHT(dbo.Users.NTLogin, 1)
                                               ELSE '$'
                                          END ) <> '$'
                                    AND dbo.Users.NTLogin <> 'АНОНИМНЫЙ ВХОД'
                        OPEN CurLoc             
                        FETCH NEXT FROM CurLoc INTO @UserName
                        WHILE @@FETCH_STATUS = 0 
                            BEGIN
                                UPDATE  @Result
                                SET     UserList = UserList + ', ' + @UserName
                                WHERE   MachineID = @MachineID
                                FETCH NEXT FROM CurLoc INTO @UserName
                            END
                        CLOSE CurLoc
                        DEALLOCATE CurLoc   
                        FETCH NEXT FROM Cur INTO @MachineName, @MachineID,
                            @LastAct       	      
                    END
                
            END
        ELSE 
            BEGIN
                WHILE @@FETCH_STATUS = 0 
                    BEGIN
                        DECLARE CurLoc CURSOR
                        FOR
                            SELECT DISTINCT
                                    dbo.Users.NTLogin
                            FROM    dbo.Actions
                                    INNER JOIN dbo.Users ON dbo.Users.ID_USER = dbo.Actions.ID_User
                            WHERE   dbo.Users.ByPass = 0
                                    AND Actions.ID_Machine = @MachineID
                                    AND ( CASE WHEN LEN(dbo.Users.NTLogin) > 1
                                               THEN RIGHT(dbo.Users.NTLogin, 1)
                                               ELSE '$'
                                          END ) <> '$'
                                    AND dbo.Users.NTLogin <> 'АНОНИМНЫЙ ВХОД'
                                    AND CAST(actts AS DATE) BETWEEN @datefrom AND @dateTo	
                        OPEN CurLoc             
                        FETCH NEXT FROM CurLoc INTO @UserName
                        WHILE @@FETCH_STATUS = 0 
                            BEGIN
                                UPDATE  @Result
                                SET     UserList = UserList + ', ' + @UserName
                                WHERE   MachineID = @MachineID
                                FETCH NEXT FROM CurLoc INTO @UserName
                            END
                        CLOSE CurLoc
                        DEALLOCATE CurLoc   
                        FETCH NEXT FROM Cur INTO @MachineName, @MachineID,
                            @LastAct       	      
                    END
                
            END
        
    END
    CLOSE Cur 
    DEALLOCATE CUR
    SELECT  MachineName ,
            MachineID ,
            LastAct ,
            RIGHT(UserList, LEN(UserList) - 1) AS MachineList
    FROM    @Result
    WHERE   LEN(UserList) > 1

GO
/****** Object:  Table [dbo].[Actions]    Script Date: 23.04.2014 14:07:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Actions](
	[ID_Action] [bigint] IDENTITY(1,1) NOT NULL,
	[ID_Machine] [int] NOT NULL,
	[ID_User] [int] NOT NULL,
	[ID_Server] [int] NOT NULL,
	[actts] [datetime] NOT NULL,
	[IP] [varchar](20) NOT NULL,
	[ResolvedHostName] [varchar](250) NOT NULL,
	[WindowsActionID] [bigint] NOT NULL,
	[ID_Domain] [int] NOT NULL,
	[ID_RecordType] [tinyint] NOT NULL,
 CONSTRAINT [PK_Actions] PRIMARY KEY CLUSTERED 
(
	[ID_Action] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Availability]    Script Date: 23.04.2014 14:07:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Availability](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[HostName] [varchar](250) NULL,
	[IP] [varchar](50) NULL,
	[CheckDate] [datetime] NULL,
	[UserID] [int] NULL,
	[ServerID] [int] NULL,
	[DomainID] [int] NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DomainGroups]    Script Date: 23.04.2014 14:07:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DomainGroups](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DNGroupName] [varchar](500) NOT NULL,
 CONSTRAINT [PK_DomainGroups] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Domains]    Script Date: 23.04.2014 14:07:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Domains](
	[ID_Domain] [int] IDENTITY(1,1) NOT NULL,
	[DomainName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Domains] PRIMARY KEY CLUSTERED 
(
	[ID_Domain] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Machines]    Script Date: 23.04.2014 14:07:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Machines](
	[ID_Machine] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[ByPass] [tinyint] NOT NULL,
 CONSTRAINT [PK_Machines] PRIMARY KEY CLUSTERED 
(
	[ID_Machine] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Machines_ActivationHistory]    Script Date: 23.04.2014 14:07:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Machines_ActivationHistory](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ID_Machine] [int] NOT NULL,
	[Status] [bit] NOT NULL,
	[Actts] [datetime] NOT NULL,
 CONSTRAINT [PK_Machines_ActivationHistory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Machines_DNHistory]    Script Date: 23.04.2014 14:07:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Machines_DNHistory](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ID_Machine] [int] NOT NULL,
	[DN] [varchar](500) NOT NULL,
	[Actts] [datetime] NOT NULL,
 CONSTRAINT [PK_Machines_DNHistory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Machines_GroupHistory]    Script Date: 23.04.2014 14:07:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Machines_GroupHistory](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ID_Machine] [int] NOT NULL,
	[ActTs] [datetime] NOT NULL,
 CONSTRAINT [PK_Machines_GroupHistory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Machines_GroupHistoryDetail]    Script Date: 23.04.2014 14:07:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Machines_GroupHistoryDetail](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ID_MachineGroupHistory] [bigint] NOT NULL,
	[ID_DomainGroup] [int] NOT NULL,
 CONSTRAINT [PK_Machines_GroupHistoryDetail] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Servers]    Script Date: 23.04.2014 14:07:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Servers](
	[ID_Server] [int] IDENTITY(1,1) NOT NULL,
	[ServerName] [varchar](50) NOT NULL,
	[ByPass] [tinyint] NOT NULL,
 CONSTRAINT [PK_Servers] PRIMARY KEY CLUSTERED 
(
	[ID_Server] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Users]    Script Date: 23.04.2014 14:07:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Users](
	[ID_User] [int] IDENTITY(1,1) NOT NULL,
	[NTLogin] [varchar](500) NOT NULL,
	[UserName] [varchar](500) NULL,
	[ByPass] [tinyint] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[ID_User] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Users_ActivationHistory]    Script Date: 23.04.2014 14:07:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users_ActivationHistory](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ID_User] [int] NOT NULL,
	[Status] [bit] NOT NULL,
	[Actts] [datetime] NOT NULL,
 CONSTRAINT [PK_Users_ActivationHistory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Users_DNHistory]    Script Date: 23.04.2014 14:07:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Users_DNHistory](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ID_User] [int] NOT NULL,
	[DN] [varchar](500) NOT NULL,
	[Actts] [datetime] NOT NULL,
 CONSTRAINT [PK_Users_DNHistory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Users_GroupHistory]    Script Date: 23.04.2014 14:07:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users_GroupHistory](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ID_User] [int] NOT NULL,
	[ActTs] [datetime] NOT NULL,
 CONSTRAINT [PK_Users_GroupHistory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Users_GroupHistoryDetail]    Script Date: 23.04.2014 14:07:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users_GroupHistoryDetail](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ID_UserGroupHistory] [bigint] NOT NULL,
	[ID_DomainGroup] [int] NOT NULL,
 CONSTRAINT [PK_Users_GroupHistoryDetail] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  View [dbo].[AllActions]    Script Date: 23.04.2014 14:07:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[AllActions]
AS
SELECT     dbo.Actions.ID_Action AS ID, dbo.Machines.Name AS Хост, dbo.Servers.ServerName AS Сервер, dbo.Users.NTLogin AS Пользователь, 
                      dbo.Domains.DomainName AS Домен, dbo.Actions.actts AS [Дата/Время], dbo.Actions.IP, dbo.Actions.WindowsActionID AS [№ события Windows], 
                      dbo.Actions.ID_RecordType
FROM         dbo.Actions INNER JOIN
                      dbo.Servers ON dbo.Actions.ID_Server = dbo.Servers.ID_Server INNER JOIN
                      dbo.Machines ON dbo.Actions.ID_Machine = dbo.Machines.ID_Machine LEFT OUTER JOIN
                      dbo.Users ON dbo.Actions.ID_User = dbo.Users.ID_User LEFT OUTER JOIN
                      dbo.Domains ON dbo.Actions.ID_Domain = dbo.Domains.ID_Domain

GO
/****** Object:  View [dbo].[GroupedActions]    Script Date: 23.04.2014 14:07:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[GroupedActions]
AS
SELECT     dbo.Actions.ID_Action AS ID, dbo.Machines.Name AS Хост, dbo.Servers.ServerName AS Сервер, dbo.Users.NTLogin AS Пользователь, 
                      dbo.Domains.DomainName AS Домен, dbo.Actions.actts AS [Дата/Время], dbo.Actions.IP, dbo.Actions.WindowsActionID AS [№ события Windows], 
                      dbo.Actions.ID_RecordType
FROM         dbo.Actions INNER JOIN
                          (SELECT     ID_Server, ID_Machine, ID_User, ID_Domain, MAX(actts) AS actts, MAX(WindowsActionID) AS WindowsActionID
                            FROM          dbo.Actions AS Actions_1
                            GROUP BY ID_Server, ID_Machine, ID_User, ID_Domain) AS e ON e.ID_Server = dbo.Actions.ID_Server AND e.ID_Machine = dbo.Actions.ID_Machine AND 
                      dbo.Actions.ID_User = e.ID_User AND dbo.Actions.ID_Domain = e.ID_Domain AND dbo.Actions.actts = e.actts AND 
                      dbo.Actions.WindowsActionID = e.WindowsActionID INNER JOIN
                      dbo.Servers ON dbo.Actions.ID_Server = dbo.Servers.ID_Server INNER JOIN
                      dbo.Machines ON dbo.Actions.ID_Machine = dbo.Machines.ID_Machine LEFT OUTER JOIN
                      dbo.Users ON dbo.Actions.ID_User = dbo.Users.ID_User LEFT OUTER JOIN
                      dbo.Domains ON dbo.Actions.ID_Domain = dbo.Domains.ID_Domain

GO
ALTER TABLE [dbo].[Actions] ADD  CONSTRAINT [DF_Actions_ID_RecordType]  DEFAULT ((0)) FOR [ID_RecordType]
GO
ALTER TABLE [dbo].[Machines] ADD  CONSTRAINT [DF_Machines_ByPass]  DEFAULT ((0)) FOR [ByPass]
GO
ALTER TABLE [dbo].[Servers] ADD  CONSTRAINT [DF_Servers_ByPass]  DEFAULT ((0)) FOR [ByPass]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_ByPass]  DEFAULT ((0)) FOR [ByPass]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0 -компьютер в домене, 1 - компьютер не в домене, 2 - компьютер не определен' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Actions', @level2type=N'COLUMN',@level2name=N'ID_RecordType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[29] 2[12] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Domains"
            Begin Extent = 
               Top = 6
               Left = 258
               Bottom = 160
               Right = 418
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Actions"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 255
               Right = 220
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Machines"
            Begin Extent = 
               Top = 206
               Left = 314
               Bottom = 310
               Right = 474
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Servers"
            Begin Extent = 
               Top = 6
               Left = 654
               Bottom = 110
               Right = 814
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Users"
            Begin Extent = 
               Top = 141
               Left = 509
               Bottom = 296
               Right = 669
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AllActions'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AllActions'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AllActions'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Actions"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 125
               Right = 220
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "e"
            Begin Extent = 
               Top = 6
               Left = 258
               Bottom = 125
               Right = 431
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Servers"
            Begin Extent = 
               Top = 6
               Left = 456
               Bottom = 110
               Right = 616
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Machines"
            Begin Extent = 
               Top = 6
               Left = 654
               Bottom = 110
               Right = 814
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Users"
            Begin Extent = 
               Top = 114
               Left = 456
               Bottom = 218
               Right = 616
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Domains"
            Begin Extent = 
               Top = 114
               Left = 654
               Bottom = 203
               Right = 814
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
  ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'GroupedActions'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'       Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'GroupedActions'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'GroupedActions'
GO
