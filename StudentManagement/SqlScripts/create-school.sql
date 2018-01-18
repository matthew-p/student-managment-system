USE master;  
GO  
CREATE DATABASE school  
ON   
( NAME = school_dat,  
    FILENAME = '/opt/mssql/data/school.mdf',  
    SIZE = 10,  
    MAXSIZE = 50,  
    FILEGROWTH = 5 )  
LOG ON  
( NAME = school_log,  
    FILENAME = '/opt/mssql/logs/school.ldf',  
    SIZE = 5MB,  
    MAXSIZE = 25MB,  
    FILEGROWTH = 5MB ) ;  
GO 