--Author:yuanrui#live.cn,https://github.com/yuanrui
--License:MIT
--TODO:create or change your database, you can change the table name adapted to you project

/*==============================================================*/
/* TABLE: CATEGORY                                              */
/* DESCRIPTION: THE LOG CATEGORY								*/
/*==============================================================*/
CREATE TABLE CATEGORY
(
  ID   INTEGER NOT NULL,
  NAME NVARCHAR2(128) NOT NULL
);

ALTER TABLE CATEGORY
  ADD CONSTRAINT PK_CATEGORY_ID PRIMARY KEY (ID);

ALTER TABLE CATEGORY
  ADD CONSTRAINT UQ_CATEGORY_NAME UNIQUE (NAME);

/*==============================================================*/
/* TABLE: LOG	                                                */
/* DESCRIPTION: THE LOG ENTRY DATA								*/
/*==============================================================*/
CREATE TABLE LOG
(
  ID                INTEGER NOT NULL,
  EVENT_ID          INTEGER,
  PRIORITY          INTEGER NOT NULL,
  SEVERITY          NVARCHAR2(32) NOT NULL,
  TITLE             NVARCHAR2(256),
  TIMESTAMP         DATE NOT NULL,
  MACHINE_NAME      NVARCHAR2(32) NOT NULL,
  APPDOMAIN_NAME    NVARCHAR2(512) NOT NULL,
  PROCESS_ID        NVARCHAR2(256) NOT NULL,
  PROCESS_NAME      NVARCHAR2(512) NOT NULL,
  THREAD_NAME       NVARCHAR2(512),
  WIN32_THREAD_ID   NVARCHAR2(128),
  MESSAGE           NVARCHAR2(2000),
  FORMATTED_MESSAGE NCLOB
);

ALTER TABLE LOG
  ADD CONSTRAINT PK_LOG_ID PRIMARY KEY (ID);

/*==============================================================*/
/* TABLE: CategoryLog                                           */
/* DESCRIPTION: THE Category Log map table   					*/
/*==============================================================*/
CREATE TABLE CATEGORY_LOG
(
  LOG_ID      INTEGER NOT NULL,
  CATEGORY_ID INTEGER NOT NULL
);

ALTER TABLE CATEGORY_LOG
  ADD CONSTRAINT PK_CATEGORY_LOG_ID PRIMARY KEY (LOG_ID, CATEGORY_ID);

ALTER TABLE CATEGORY_LOG
  ADD CONSTRAINT FK_CATEGORY_LOG_CATEGORY FOREIGN KEY (CATEGORY_ID)
  REFERENCES CATEGORY (ID) ON DELETE CASCADE;

ALTER TABLE CATEGORY_LOG
  ADD CONSTRAINT FK_CATEGORY_LOG_ID FOREIGN KEY (LOG_ID)
  REFERENCES LOG (ID) ON DELETE CASCADE;

/*==============================================================*/
/* SEQUENCE: SEQ_CATEGORY                                       */
/* DESCRIPTION: THE SEQUENCE TO CATEGORY TABLE					*/
/*==============================================================*/
CREATE SEQUENCE SEQ_CATEGORY
INCREMENT BY 1
START WITH 1
MAXVALUE 9223372036854775807
NOCYCLE;

/*==============================================================*/
/* SEQUENCE: SEQ_LOG	                                        */
/* DESCRIPTION: THE SEQUENCE TO LOG TABLE						*/
/*==============================================================*/
CREATE SEQUENCE SEQ_LOG
INCREMENT BY 1
START WITH 1
MAXVALUE 9223372036854775807
NOCYCLE;

/*==============================================================*/
/* PROCEDURE: ADD_CATEGORY                                      */
/* DESCRIPTION: ADD CATEGORY AND LOGID TO CATEGORY_LOG TABLE    */
/* IF CategoryName NOT EXISTS THEN INSERT VALUE TO CATEGORY     
   TABLE														*/
/*==============================================================*/
CREATE OR REPLACE PROCEDURE ADD_CATEGORY(
	CategoryName NVARCHAR2,
	LogID INTEGER
) is
	v_COUNT NUMBER;
	v_ID NUMBER;
BEGIN
    SELECT COUNT(1) INTO v_COUNT
    FROM CATEGORY
    WHERE NAME = CategoryName;
		
	IF v_COUNT = 0 THEN
		BEGIN
			SELECT SEQ_CATEGORY.NEXTVAL INTO v_ID FROM DUAL;
			INSERT INTO CATEGORY VALUES(v_ID, CategoryName);
			INSERT INTO CATEGORY_LOG VALUES(LogID, v_ID);			 
		END;
		ELSE
			INSERT INTO CATEGORY_LOG 
			SELECT LogID, C.ID FROM CATEGORY C
			WHERE C.NAME=CategoryName;
		END IF;
		COMMIT;
END ADD_CATEGORY;

/*==============================================================*/
/* PROCEDURE: WRITE_LOG										    */
/* DESCRIPTION: ADD LOG DATA TO LOG TABLE, AND RETURN LogId     */
/*==============================================================*/
CREATE OR REPLACE PROCEDURE WRITE_LOG( 
  EventID INTEGER,
  Priority INTEGER, 
  Severity NVARCHAR2, 
  Title NVARCHAR2, 
  Timestamp DATE,
  MachineName NVARCHAR2, 
  AppDomainName NVARCHAR2,
  ProcessID NVARCHAR2,
  ProcessName NVARCHAR2,
  ThreadName NVARCHAR2,
  Win32ThreadId NVARCHAR2,
  Message NVARCHAR2,
  FormattedMessage NVARCHAR2,
  LogId out INTEGER) is
  v_ID INTEGER;
BEGIN
	SELECT SEQ_LOG.NEXTVAL INTO v_ID FROM DUAL;
	INSERT INTO LOG
	VALUES(v_ID, EventID, Priority, Severity, Title, Timestamp, MachineName, AppDomainName, ProcessID, ProcessName, ThreadName, Win32ThreadId, Message, FormattedMessage);
	SELECT v_ID INTO LogId FROM DUAL;
END WRITE_LOG;