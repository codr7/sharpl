# sharpl

```
$ dotnet run
sharpl v9

   1 (say "hello")
   2 
hello
```

## introduction
Sharpl is a custom Lisp interpreter implemented in C#.<br/>
It's trivial to embed and comes with a basic REPL.<br/>
The code base currently hovers around 4kloc.

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
  (if T (say "is true"))
```
```
is true
```

`if-else` may be used to specify an else-clause.

```
  (if-else F 1 2)
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

This example will keep going forever without consuming more space:
```
(^foo []
  (return (foo)))

(foo)
```

Without `return` it quickly runs out of space:
```
(^foo []
  (foo))

(foo)
```
```
System.IndexOutOfRangeException: Index was outside the bounds of the array.
```

### varargs
`methods` may be defined as taking a variable number of arguments by suffixing the last parameter with `*`.
The name is bound to an array containing all trailing arguments when the method is called.

```
(^foo [bar*]
  (say bar*))
  
(foo 1 2 3)
```
```
123
```

### composition
Methods may be composed using `&`.

```
(^foo [x]
  (+ x 1))
  
(^bar [x]
  (* x 2))

(let [f foo & bar]
  (say f)
  (f 20))
```
```
(Method foo & bar [])
```
`42`

## quoting
Expressions may be quoted by prefixing with `'`.

### symbols
Identifiers turn into symbols when quoted.

```
(= (Symbol "foo" 42) 'foo42)
```
`T`

## composite types

### arrays
Arrays are fixed size sequences of values.<br/>
New arrays may be created by enclosing a sequence of values in brackets.

```
[1 2 3]
```
Or by calling the constructor explicitly.
```
(Array 1:2:3*)
```
`[1 2 3]`

### maps
Maps are ordered mappings from keys to values.<br/>
New maps may be created by enclosing a sequence of pairs in curly braces.

```
{'foo:1 'bar:2 'baz:3}
```
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
`benchmark`may be used to measure the number of milliseconds it takes to repeat a block of code N times with warmup.

```
dotnet run -c=Release benchmarks.sl
497
137
829
```

## debugging
`emit` may be used to display the VM operations emitted for an expression.

```
(emit (+ 1 2))
 
1    Push 1
2    Push 2
3    CallMethod (Method + []) 2 False
```