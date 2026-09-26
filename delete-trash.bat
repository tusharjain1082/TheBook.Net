start for /d /r . %%d in (bin,obj,debug,release,x64,.vs) do @if exist "%%d" rd /s/q "%%d"
pause