var s:string;
    r:real;
    i,j,n:integer;
	begin
    r:=0;
    readln(s);
    for i:=1 to length(s) do begin
       n:=0;
       for j:=1 to length(s) do begin
          if s[i]=s[j] then inc(n);
       end;
       r:=r+1/n;
    enh;
    writeln('количество различных букв = ', r:1:0);
end.