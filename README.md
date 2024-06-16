# sharpl

```
$ dotnet run
sharpl v2

   1 (say "hello")
   2 
hello
_
```

## bindings
Bindings come in two flavors, with lexical or dynamic scope.

### lexical scope
New lexically scoped bindings may be created using `let`.

```
(let [x 1 
      y (+ x 2)]
  y)

3
```

### dynamic scope
New dynamically scoped bindings may be created using `define`.

```
(^foo []
  (define bar (do (say "compiling") 1)
          baz 2)
  (+ bar baz))
```
`compiling`

```
(foo)
```
`3`

```
bar
```
`Sharpl.EmitError: repl@6:1 Unknown id: bar`

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

### closures
Methods keep copies of any external bindings referenced from their body.

```
(let [foo (let [bar 42]
           (^[] bar))]
  (foo))
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