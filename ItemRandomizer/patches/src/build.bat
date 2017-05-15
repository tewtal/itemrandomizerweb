@echo off
setlocal enabledelayedexpansion

FOR %%G IN (*.asm) DO (
    SET ASM_FILE=%%G
    SET IPS_FILE=!ASM_FILE:~0,-4!.ips    
    xkas -ips -o ..\!IPS_FILE! !ASM_FILE!
) 