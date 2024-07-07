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
External bindings are captured at method definition time.

```
(^make-countdown [max]
  (let [n max]
    (^[] (dec n))))

(define foo (make-countdown 42))

(foo)
```
`41`

```
(foo)
```
`40`

### tail calls
`return` may be used to convert any call into a tail call.

This will keep going forever without consuming space:
```
(^foo []
  (return (foo)))

(foo)
```

Without `return` the call quickly runs out of space:
```
(^foo []
  (foo))

(foo)
```
```
System.IndexOutOfRangeException: Index was outside the bounds of the array.
```

## quoting
Expressions may be quoted by prefixing with `'`.

### symbols
The type of a quoted identifier is `Symbol`.

```
(= (Symbol "foo" 42) 'foo42)
```
`T`

## composite types

### arrays
Arrays are fixed size.<br/>
New arrays may be created by enclosing a sequence of values in brackets.

```
[1 2 3]
```
`[1 2 3]`

Or by calling the constructor explicitly.

```
(Array 1:2:3*)
```
`[1 2 3]`

### maps
Maps are ordered lookup tables.<br/>
New maps may be created by enclosing a sequence of pairs in curly braces.

```
{'foo:1 'bar:2 'baz:3}
```
`{'bar:2 'baz:3 'foo:1}`

Or by calling the constructor explicitly.

```
(Map ['foo:1 'bar:2 'baz:3]*)
```
`{'bar:2 'baz:3 'foo:1}`

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

## iterators
`reduce` may be used to transform any iterable into a single value.

```
(reduce + [1 2 3] 0)
```
`6`

Which could also be expressed in a more condensed form thanks to the fact that integers are iterable.

```
(reduce + 4 0)
```

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
`emit` may be used to display the VM operations emitted for an expression.

```
(emit (+ 1 2))
 
1    Push 1
2    Push 2
3    CallMethod + 2
```