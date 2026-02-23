start for /d /r . %%d in (bin,debug,release,x86,x64) do @if exist "%%d" rd /s/q "%%d"
pause