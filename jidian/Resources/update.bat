@echo off
@ping 127.0.0.1 -n 2 >nul
xcopy newjidian.exe jidian.exe /y
del newjidian.exe
start jidian.exe
del %0