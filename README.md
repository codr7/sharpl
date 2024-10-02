# sharpl

```
$ dotnet run
sharpl v28

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

- [Pairs](https://github.com/codr7/sharpl/tree/main#pairs) (`a:b`) are used everywhere, all the time.
- Range support (`min..max:stride`).
- [Maps](https://github.com/codr7/sharpl/tree/main#maps) (`{k1:v1...kN:vN}`) are ordered.
- [Methods](https://github.com/codr7/sharpl/tree/main#methods) may be [composed](https://github.com/codr7/sharpl/tree/main#composition) using `&`.
- [Varargs](https://github.com/codr7/sharpl/tree/main#varargs) (`foo*`), similar to Python.
- [Declarative destructuring](https://github.com/codr7/sharpl/tree/main#destructuring) (`(let [_:y 1:2] y)`) of bindings.
- Splatting (`[1 2 3]*`), simlar to Python.
- Deferred actions, similar to Go.
- Unified, deeply integrated [iterator](https://github.com/codr7/sharpl/tree/main#iterators) protocol.
- Default decimal type is [fixpoint](https://github.com/codr7/sharpl/tree/main#fixpoints).
- Nil is written `_`.
- Both `T` and `F` are defined.
- Zeros and empty strings, arrays, lists and maps are considered false.
- `=` is generic and compares values deeply, `is` may be used to compare identity.
- [Lambdas](https://github.com/codr7/sharpl/tree/main#lambdas) look like anonymous methods.
- Compile time eval (called `emit`), similar to Zig's comptime.
- Explicit [tail calls](https://github.com/codr7/sharpl/tree/main#tail-calls) using `return`.
- Parens are used for calls only.
- Many things are callable, simlar to Clojure.
- Methods have their own symbol (^).
- Line numbers are a thing.

## examples
The following examples may give you an idea what Sharpl is currently capable of:

- [Advent of Code 2023](https://github.com/codr7/sharpl/tree/main/examples/aoc23)
- [Fire](https://github.com/codr7/sharpl/tree/main/examples/fire)
- [Remote REPL](https://github.com/codr7/sharpl/tree/main/examples/remote-repl)

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

### destructuring
Bindings support declarative destructuring of pairs.

```
(let [_:r:rr 1:2:3] r:rr)
```
`2:3`

### updates
`set` may be used to update a binding.

```
(let [foo 1 bar 2]
  (set foo 3 bar 4)
  foo:bar)
```
`3:4`

## branching
`if` may be used to conditionally evaluate a block of code.

```
(if T 'is-true)
```
`'is-true`

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
`for` may be used to iterate any number of iterables.

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
  (gensym 'foo)

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

Characters support ranges.

```
[\a..\z:2*]
```
`[\a \c \e \g \i \k \m \o \q \s \u \w \y]`

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


Integers support ranges.

```
[1..10:2*]
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

Fixpoints support ranges.

```
[1.1..1.4:.1*]
```
`[1.1 1.2 1.3]`

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
`map` may be used to map a method over one or more iterables, it returns an iterator.

```
[(map Pair '[foo bar baz] [1 2 3 4])*]
```
`['foo:1 'bar:2 'baz:3]`

### filter
`filter` returns an iterator for items matching the specified predicate.

```
[(filter (^[x] (> x 2)) [1 2 3 4 5])*]
```
`[3 4 5]`

### reduce
`reduce` may be used to transform any iterable into a single value. 

```
(reduce - [2 3] 0)
```
`1`

### sum
`sum` is provided as a shortcut.

```
(sum 1 2 3)
```
`10`

### zip
`zip` may be used to braid any number of iterables.

```
[(zip '[foo bar] '[1 2 3] [T F])*]
```
`['foo:1:T 'bar:2:F]`

### enumerate
`enumerate` may be used to zip any iterable with indexes.

```
[(enumerate 42 '[foo bar])*]
```
`[42:'foo 43:'bar]`

### find
`find-first` may be used to find the first value in an iterable matching the specified predicate, along with its index; or `_` if not found.

```
(find-first (^[x] (> x 3)) [1 3 5 7 9])
```
`5:2`

## time
The time library uses established naming conventions when referring to fields: `Y` for years, `M` for months, `D` for days, `h` for hours, `m` for minutes, `s` for seconds, `ms` for milliseconds and `us` for microseconds.

`time/today` and `time/now` may be used to get the current date/time.

```
(is (time/trunc (time/now)) (time/today))
```
`T`

The `Timestamp` constructor may be used to create new timestamps manually, pass `_` for default.

```
  (Timestamp 2024 _ _ 21 22)
```
`2024-01-01 21:22:00`

### durations
Subtracting timestamps results in a duration.

```
  (- (time/now) (time/today))
```
`16:36:27.2404435`

Suffixes may be used as constructors.

```
(is (time/m 120) (time/h))
```
`T`

When applied to timestamps or durations they return the value for the specified field.

```
(time/D (time/W 2))
```
`14`

Durations may be added/subtracted to/from timestamps.

```
(+ (time/today) (time/m 90))
```
`2024-09-15 01:30:00`

## ranges
Timestamps support ranges.<br/>
The following example generates timestamps between `2024-1-1` and the next day, separated by six hours.

```
(let [t (Timestamp 2024 1 1)]
  [t..(+ t (time/D 1)):(time/h 6)*])
```
`[2024-01-01 00:00:00 2024-01-01 06:00:00 2024-01-01 12:00:00 2024-01-01 18:00:00]`

### months
`MONTHS` maps indexes to month names.

```
time/MONTHS
```
`[_ 'jan 'feb 'mar 'apr 'may 'jun 'jul 'aug 'sep 'oct 'nov 'dec]`

### weekdays
`WEEKDAYS` maps indexes to week day names.

```
time/WEEKDAYS
```
`['su 'mo 'tu 'we 'th 'fr 'sa]`

### time zones
By default all timestamps are local, `time/to-utc` and `time/from-utc` may be used to convert back and forth.

```
(let [t (time/now)]
  (is (time/to-local (time/to-utc t)) t))
```
`T`

## resources
Acquired resources may be released on scope exit using `defer`.

```
(do
  (say 'before)
  (defer (^[] (say 'defer)))
  (say 'after))
```
`
before
after
defer
`

## communication
### pipes
Pipes are unbounded, thread safe communication channels. Pipes may be called without arguments to read and with to write.

### ports
Ports are bidirectional communication channels. Like pipes, ports may be called without arguments to read and with to write.

### polling
`poll` returns the first argument that's ready for reading.

```
(let [p1 (Pipe) p2 (Pipe)]
  (p2 42)
  ((poll [p1 p2])))
```
`42`

## threads
`spawn` may be used to start new threads, each thread runs in a separate VM. A port is created for communication, one side passed as argument and the other returned.

```
(let [p (spawn [p] (p 42))]
  (p))
```

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
`dmit` may be used to display the VM operations emitted for an expression.

```
(dmit '(+ 1 2))
```
```
9    Push 1
10   Push 2
11   CallMethod (Method + []) 2 False
```

## contributing
Contributions are very welcome, feel free to submit pull requests.<br/>
Or even better, register a GitHub issue describing the change and allow us to make sure we agree it's a good idea first.

- Fork the project.
- Create a feature branch (`git checkout -b issue-x`).
- Commit your changes (`git commit -m '...'`).
- Push to the branch (`git push origin issue-x`).
- Open a pull request.