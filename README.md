# sharpl

```
$ dotnet run
sharpl v23

   1 (say 'hello)
   2 
hello
```

## intro
Sharpl is a custom Lisp interpreter implemented in C#.<br/>
<br/>
It's trivial to embed and comes with a simple REPL.<br/>
The code base currently hovers around 5 kloc and has no external dependencies.<br/>
<br/>
All features described in this document are part of the [test suite](https://github.com/codr7/sharpl/tree/main/tests) and expected to work as described.

## novelties

- Pairs (`a:b`) are used everywhere, all the time.
- Maps (`{k1:v1...kN:vN}`) are ordered.
- Methods may be composed using `&`.
- Varargs (`foo*`) are similar to Python.
- Splatting (`[1 2 3]*`) is simlar to Python.
- Unified, deeply integrated iterator protocol.
- Default decimal type is fixpoint.
- Nil is written `_`.
- Both `T` and `F` are defined.
- Zeros and empty strings, arrays, lists and maps are considered false.
- `=` is generic and compares values deeply, `is` may be used to compare identity.
- Lambdas look like anonymous methods.
- Compile time eval (called `emit`), similar to Zig's comptime.
- Explicit tail calls using `return`.
- Parens are used for calls only.
- Many things are callable, simlar to Clojure.
- Line numbers are a thing.

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
New dynamically scoped bindings may be created using `var`.

```
(var foo 35)
  
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

`else` may be used to specify an else-clause.

```
(else F 1 2)
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

(var foo (make-countdown 42))

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

## looping
### loop
`loop` does exactly what it says, and nothing more.

```
(^enumerate [n]
  (let [result (List) i 0]
    (loop
      (push result i)
      (if (is (inc i) n) (return result)))))

(enumerate 3)
```
`[0 1 2]`

### for
`for` may be used to iterate any number of sequences.

```
(let [result {}]
  (for [i [1 2 3] 
        j [4 5 6 7]]
    (result i j))
  result)
```
`{1:4 2:5 3:6}`

## quoting
Expressions may be quoted by prefixing with `'`.

### unquoting
`,` may be used to temporarily decrease quoting depth while evaluating the next form.

```
(let [bar 42]
  '[foo ,bar baz])
```

#### splat
Unquoting may be combined with `*` to expand in place.

```
(let [bar 42 
      baz "abc"]
  '[foo ,[bar baz]* qux])
```
`['foo 42 "abc" 'qux]`

## value and identity
`=` may be used to compare values deeply, while `is` compares identity.<br/>
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
The `Nil` type has one value (`_`), which represents the absence of a value.

### bits
The `Bit` type has two values, `T` and `F`.

### symbols
Symbols may be created by quoting identifiers or using the type constructor.

```
(= (Sym 'foo "bar") 'foobar)
```
`T`

#### unique
`gensym` may be used to create unique symbols.

```
  (Sym 'foo)

'7foo
```

### characters
Character literals may be defined by prefixing with `\`.

```
(char/is-digit \7)
```
`T`

Special characters require one more escape.

```
\\n
```
`\\n`

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

`parse-int` may be used to parse integers from strings, it returns the parsed value and the next index in the string.

```
(parse-int "42foo")
```
`42:2`

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

### strings
String literals may be defined using double quotes.

`up` and `down` may be used to change case.

```
(string/up "Foo")
```
`"FOO"`

`split` may be used to split a string on a pattern.

```
(string/split "foo bar" " ")
```
`["foo" "bar"]`

`reverse` may be used to reverse a string.

```
(string/reverse "foo")
```
`"oof"`

`replace` may be used to replace all occurrences of a pattern.

```
(string/replace "foo bar baz" " ba" "")
```
`"foorz"`

### lengths
`#` or `length` may be used to get the length of any composite value.

```
(= #[1 2 3] (length "foo"))
```
`T`

### stacks
Pairs, arrays, lists and strings all implement the stack trait.

#### push
`push` may be used to push a new item to a stack; for pairs it's added to the front, for others to the back.

```
(let [s 2:3]
  (push s 1)
  s)
```
`1:2:3`

#### peek
`peek` may be used to get the top item in a stack.

```
(peek "abc")
```
`\a`

#### pop
`pop` may be used to remove the top item from a stack.

```
(let [s [1 2 3]]
  (pop s):s)
```
`3:[1 2]`

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

### slices
All composite types may be sliced by indexing using a pair.

```
('{a:1 b:2 c:3 d:4} 'b:'c)
```
`{'b:2 'c:3}`

## iterators

### map
`map` may be used to map a method over one or more sequences.

```
(map Pair '[foo bar baz] [1 2 3 4])
```
`(List 'foo:1 'bar:2 'baz:3)`

### reduce
`reduce` may be used to transform any iterable into a single value.

```
(reduce + [1 2 3] 0)
```
`6`

### find
`find-first` may be used to find the first value in a sequence matching the specified predicate, along with its index; or `_` if not found.

```
(find-first (^[x] (> x 3)) [1 3 5 7 9])
```
`5:2`

## libraries
`lib` may be used to define/extend libraries.

```
(lib foo
  (var bar 42))

foo/bar
```
`42`

When called with one argument, it specifies the current library for the entire file.

```
(lib foo)
(var bar 42)
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

### dynamic
`eval` may be used to dynamically evaluate code at runtime.

```
(eval '(+ 1 2))
```
`3`

### static
`emit` may be used to evaluate code once as it is emitted.

```
(emit '(+ 1 2))
```
`3`

## json
`json/encode` and `json/decode` may be used to convert values to/from [JSON](https://www.json.org/json-en.html).

```
(let [dv {'foo:42 'bar:.5 'baz:"abc" 'qux:[T F _]}
      ev (json/encode dv)]
  ev:(= (json/decode ev) dv))
```
`"{\"bar\":0.5,\"baz\":\"abc\",\"foo\":42,\"qux\":[true,false,null]}":T`

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
`bench`may be used to measure the number of milliseconds it takes to repeat a block of code N times with warmup.

```
dotnet run -c=release benchmarks/fib.sl
```

## building
`dotnet publish` may be used to build a standalone executable.

```
$ dotnet publish
MSBuild version 17.9.8+b34f75857 for .NET
  Determining projects to restore...
  Restored ~/sharpl/sharpl.csproj (in 324 ms).
  sharpl -> ~/sharpl/bin/Release/net8.0/linux-x64/sharpl.dll
  Generating native code
  sharpl -> ~/sharpl/bin/Release/net8.0/linux-x64/publish/
```

## debugging
`demit` may be used to display the VM operations emitted for an expression.

```
(demit '(+ 1 2))
```
```
9    Push 1
10   Push 2
11   CallMethod (Method + []) 2 False
```