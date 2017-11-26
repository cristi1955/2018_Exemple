CREATE PROCEDURE [dbo].[creareTabela]
AS
	CREATE TABLE T1 (
k1 int PRIMARY KEY,
k2 varchar(50) SPARSE NULL,
k3 smalldatetime
);
RETURN 0
