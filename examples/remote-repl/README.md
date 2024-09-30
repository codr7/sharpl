## intro
The goal of this project was to explore/design a networking API for [Sharpl](https://github.com/codr7/sharpl) by implementing a remote REPL.

## usage
First start the server, the last argument is the password (NOTE: which is sent in cleartext over whatever network).

```
$ dotnet run examples/remote-repl/server 127.0.0.1 8080 xxx
Listening on 127.0.0.1:8080
New client
  (+ 1 2)
3
```

```
$ dotnet run examples/remote-repl/client 127.0.0.1 8080 xxx
Connected to 127.0.0.1:8080
  (+ 1 2)
3
```