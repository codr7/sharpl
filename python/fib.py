from benchmark import benchmark

print(benchmark(100, '''
def fib(n):
  return n if n < 2 else fib(n-1) + fib(n-2)
''',
'fib(20)'))

print(benchmark(10000, '''
def fib(n, lookup):
  if n in lookup: return lookup[n]
  result = n if n < 2 else fib(n-1, lookup) + fib(n-2, lookup)
  lookup[n] = result
  return result
''',
'fib(70, {})'))

print(benchmark(10000, '''
def fib(n, a, b):
  return a if n == 0 else b if n == 1 else fib(n-1, b, a+b)
''',
'fib(70, 0, 1)'))
