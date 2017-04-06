#SysCfgTools


A set of tools for complimenting the standard Foxboro System Configurator installation.

##BackupDeversioner

Command line utility for converting a postgres 8.x backup files to 7.3 (for making unofficial windows vista/7/8
system configurator installation backup files compatible with standard installation).

##SuperBackupUtility

A drop in replacement for the standard backup utility for unoficiall windows vista/7/8 installations 
of system configurator.  Supports backup to network, and auto deversion to old backup format for 
compatibility with standard system configurator installations.

##IED Copier

Copies an IED, optionally renaming point names by find/replacing common tokens.

##Port Switcher

Moves an Slave/Master to a new protocol group, or moves a Slave/Master protocol group to a new serial port.
Currently tested on DNP only, but should also work for modbus.  Not sure about 61850.

##64bit Tools

In order to support 64bit systems, several 16bit components need to be rewritten.  So far the following tools
have been rewritten in C# and provided here:

###crc16.exe
This generates the 16bit crcs used in the configurations

###idfc.exe
The IDF Calc compiler.  This has not been extensively tested for compatability 
so use at own risk.
