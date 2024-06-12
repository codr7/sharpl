# sharpl

```
$ dotnet run
sharpl v1

   1 (say "hello")
   2 
hello
_
```

## methods
New methods may be defined using `^`.

```
(^foo [x]
  x)

(foo 42)
```
`42`

### lambdas
Leaving out the name creates a lambda.

```
(let [f (^[x] x)]
  (f 42))
```
`42`

## libraries
`lib` may be used to define/extend libraries.

```
(lib foo
  (define bar 42))

foo/bar
```
`42`

When called with one argument, it specifies the current library for the entire file.

```
(lib foo)
(define bar 42)
```

And when called without arguments, it returns the current library.

```
(lib)
```
`(Lib user)`

## testing
`check` fails with an error if the result of evaluating its body isn't equal to the specified value.

Take a look at the [test suite](https://github.com/codr7/sharpl/blob/main/tests.sl) for examples.

```
(check 5
  (+ 2 2))

Sharpl.EvalError: repl@1:2 Check failed: expected 5, actual 4!
```

## debugging
`decode` may be used to display the VM operations emitted for an expression.

```
(decode (+ 1 2))
 
1    Push 1
2    Push 2
3    CallMethod + 2
```