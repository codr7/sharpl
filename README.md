# sharpl

```
$ dotnet run
sharpl v11

   1 (say 'hello)
   2 
hello
```

## introduction
Sharpl is a custom Lisp interpreter implemented in C#.<br/>
<br/>
It's trivial to embed and comes with a simple REPL.<br/>
The code base currently hovers around 4kloc and has no external dependencies.<br/>
<br/>
All features described in this document are part of the [test suite](https://github.com/codr7/sharpl/tree/main/tests) and expected to work as described.

## novelties

- Pairs, arrays, maps and method compositions have dedicated syntax.
- Varargs use `*`, similar to Python.
- Splatting is supported using `*`, simlar to Python.
- Unified, deeply integrated iterator protocol.
- Default decimal type is fixpoint.
- Nil is called `_`.
- Both true (`T`) and false (`F`) are defined.
- Zeros and empty strings, arrays and maps are considered false.
- `=` is generic and compares values deeply, `is` may be used to compare identity.
- Lambdas actually look like anonymous methods.
- `eval` evaluates its arguments at emit time.
- Explicit tail calls using `return`.
- There is no List, except for nested pairs, which is not as bad as it sounds thanks to the syntax.
- Parens are used for calls only.
- Most things are callable, simlar to Clojure.
- Maps are ordered, not hashed.
- Errors include line numbers, even inside the REPL.

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
  (if T (say 'is-true))
```
```
is-true
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
(foo & bar [values*])
```
`42`

## quoting
Expressions may be quoted by prefixing with `'`.

## value vs. identity
`=` may be used to compare values deeply, while `is` compares identities.<br/>
For some types they return the same result; integers, strings, pairs, methods etc.

For others; like arrays and maps; two values may well be equal despite having different identities.
```
(= [1 2 3] [1 2 3])
```
`T`

```
(is [1 2 3] [1 2 3])
```
`F`


## types

### nil
The `Nil` type has one value, `_`; which really represents the absence of a value.

### bits
The `Bit` type has two values, `T` and `F`.

### symbols
Symbols may be created by quoting identifiers.

```
(= (Symbol 'foo 42) 'foo42)
```
`T`

### strings
Strings use double quotes.

```
(string/up "Foo")
```
`"FOO"`

### integers
Integers support the regular arithmetic operations.

```
(- (+ 1 4 5) 3 2)
```
`5`

Negative integers lack syntax, and must be created by way of subtraction.

```
(- 42)
```
`-42`

`range` may be used to create a new integer range.

```
[(range 1 10 2)*]
```
`[1 3 5 7 9]`

### fixpoints
Decimal expressions are read as fixpoint values with specified number of decimals.<br/>
Like integers, fixpoints support the regular arithmetic operations.

```
1.234
```
`1.234`

Leading zero is optional.

```
(= 0.123 .123)
```
`T`

Also like integers; negative fixpoints lack syntax, and must be created by way of subtraction.

```
(- 1.234)
```
`-1.234`

`range` may be used to create a new fixpoint range.

```
[(range 1.1 1.4 .1)*]
```
`[1.1 1.2 1.3 1.4]`

## composite types

### pairs
Pairs may be formed by putting a colon between two values.

```
1:2:3
```
`1:2:3`

Or by calling the constructor explicitly.

```
(Pair [1 2 3]*)
```
`1:2:3`

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
'{foo:1 bar:2 baz:3}
```
Or by calling the constructor explicitly.
```
(Map '[foo:1 bar:2 baz:3]*)
```
`{'bar:2 'baz:3 'foo:1}`

## iterators
`reduce` may be used to transform any iterable into a single value.

```
(reduce + [1 2 3] 0)
```
`6`

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

## errors
`fail` may be used to signal an error.<br/>
Since there are no facilities for handling errors yet, this means evaluation will stop unconditionally.

```
(fail 'bummer)
```
```
Sharpl.EvalError: repl@1:2 bummer
```

## evaluation
`eval` may be used to evaluate a block of code at emit time.

```
(eval (say 'emitting) 42)
```
```
emitting
```
`42`

## tests
`check` fails with an error if the result of evaluating its body isn't equal to the specified value.
```
(check 5
  (+ 2 2))
```
```
Sharpl.EvalError: repl@1:2 Check failed: expected 5, actual 4!
```

When called with a single argument, it simply checks if it's true.
```
(check 0)
```
```
Sharpl.EvalError: repl@1:2 Check failed: expected T, actual 0!
```

Take a look at the [test suite](https://github.com/codr7/sharpl/blob/main/tests/all-tests.sl) for examples.

## benchmarks
`benchmark`may be used to measure the number of milliseconds it takes to repeat a block of code N times with warmup.

```
dotnet run -c Release benchmarks.sl
```

## debugging
`emit` may be used to display the VM operations emitted for an expression.

```
(emit (+ 1 2))
 
1    Push 1
2    Push 2
3    CallMethod (+ []) 2 False
```