# sharpl

```
$ dotnet run
sharpl v7

   1 (say "hello")
   2 
hello
```

## introduction
Sharpl is a custom Lisp interpreter implemented in C#.<br/>
It comes with a basic REPL for standalone use, but is designed to be trivial to embed in C# projects.

## bindings
Bindings come in two flavors, with lexical or dynamic scope.

### lexical scope
New lexically scoped bindings may be created using `let`.

```
(let [x 1 
      y (+ x 2)]
  y)
```
`3`

### dynamic scope
New dynamically scoped bindings may be created using `define`.

```
(define foo 35)
  
(^bar []
  foo)

(let [foo (+ foo 7)]
  (bar))
```
`42`

## branching
`if` may be used to conditionally evaluate a block of code.

```
  (if 42 (say "42 is true"))
```
```
42 is true
```

`if-else` may be used to specify an else-clause.

```
  (if-else 42 1 2)
```
`2`

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

### tail calls
`return` may be used to convert any call into a tail call.

```
(^foo []
  2)

(^bar []
  1
  (return (foo))
  3)

(bar)
```
`2`

## composite types

### arrays
Arrays may be created by enclosing a sequence of values in brackets.

```
[1 2 3]
```
`[1 2 3]`

Or by calling the type constructor.

```
(Array 1:2:3*)
```
`[1 2 3]`

### pairs
Pairs may be formed by putting a colon between two values.

```
1:2:3
```
`1:2:3`

Or by calling the type constructor.

```
(Pair [1 2 3]*)
```
`1:2:3`

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

## evaluation
`eval` may be used to evaluate a block of code at emit time, the results are pushed on evaluation.

```
(eval (say "emitting") 42)
```
```
emitting
```
`42`

## tests
`check` fails with an error if the result of evaluating its body isn't equal to the specified value.

Take a look at the [test suite](https://github.com/codr7/sharpl/blob/main/tests/all-tests.sl) for examples.

```
(check 5
  (+ 2 2))

Sharpl.EvalError: repl@1:2 Check failed: expected 5, actual 4!
```

## benchmarks
`benchmark`may be used to measure the number of milliseconds it takes to repeat a block of code a number of times with warmup.

```
dotnet run -c=Release benchmarks.sl
510
266
```

## debugging
`decode` may be used to display the VM operations emitted for an expression.

```
(decode (+ 1 2))
 
1    Push 1
2    Push 2
3    CallMethod + 2
```