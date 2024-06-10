```
$ dotnet run
Sharpl v1

   1 (say "hello")
   2 
hello
_
```

## debugging
`decode` may be used to display the VM operations emitted for an expression.

```
   1 (decode (+ 1 2))
   2 
1    Push 1
2    Push 2
3    CallMethod + 2
_
```