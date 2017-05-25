BEGIN TRANSACTION;
CREATE TABLE [T_Role] (
    [Id] nvarchar NOT NULL,
    [Name] nvarchar ,
    [Enabled] bit NOT NULL,
    [Remark] nvarchar ,
    [CreatedBy] nvarchar ,
    [CreatedAt] datetime NOT NULL,
    [ModifiedBy] nvarchar ,
    [ModifiedAt] datetime NOT NULL,
    PRIMARY KEY (Id)
);
CREATE TABLE [T_Permission] (
    [Id] nvarchar NOT NULL,
    [ParentId] nvarchar ,
    [Name] nvarchar ,
    [Url] nvarchar ,
    [Icon] nvarchar ,
    [Type] int NOT NULL,
    [Enabled] bit NOT NULL,
    [SortIndex] int NOT NULL,
    [Level] int NOT NULL,
    [Path] nvarchar ,
    [LinkTarget] nvarchar ,
    [Callback] nvarchar ,
    [Remark] nvarchar ,
    [CreatedBy] nvarchar ,
    [CreatedAt] datetime NOT NULL,
    [ModifiedBy] nvarchar ,
    [ModifiedAt] datetime NOT NULL,
    PRIMARY KEY (Id)
);
CREATE TABLE [T_Account] (
    [Id] int NOT NULL,
    [Name] nvarchar ,
    [UserName] nvarchar ,
    [Password] nvarchar ,
    [Salt] nvarchar ,
    [Email] nvarchar ,
    [Telephone] nvarchar ,
    [Mobile] nvarchar ,
    [LockIp] nvarchar ,
    [PasswordModifiedAt] datetime NOT NULL,
    [Enabled] bit NOT NULL,
    [CreatedBy] nvarchar ,
    [CreatedAt] datetime NOT NULL,
    [ModifiedBy] nvarchar ,
    [ModifiedAt] datetime NOT NULL,
    PRIMARY KEY (Id)
);
CREATE TABLE [T_Role_Permission] (
    [RoleId] nvarchar NOT NULL,
    [PermissionId] nvarchar NOT NULL,
    PRIMARY KEY (RoleId, PermissionId),
    FOREIGN KEY (RoleId) REFERENCES T_Role (Id),
    FOREIGN KEY (PermissionId) REFERENCES T_Permission (Id)
);
CREATE TABLE [T_Account_Role] (
    [RoleId] int NOT NULL,
    [AccountId] nvarchar NOT NULL,
    PRIMARY KEY (RoleId, AccountId),
    FOREIGN KEY (RoleId) REFERENCES T_Account (Id),
    FOREIGN KEY (AccountId) REFERENCES T_Role (Id)
);

COMMIT;
