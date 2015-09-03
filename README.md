# IsSharp

IsSharp is a guard clause library for C# that provides clean and fluent code with human readable exception messages using expression trees.

Install from [Nuget](https://www.nuget.org/packages/IsSharp/) or build using the source.


## Example Usage

```C#
Guard.Is(() => CostPrice != null);
Guard.Is(() => CostPrice >= 25m && CostPrice <= 45m);
Guard.Is<InvalidConstraintException>(() => CostPrice == 30m);
```
 Or
 ```C#
CostPrice.Is("Cost")
		.NotNull()
		.InRange(25m, 45m)
		.Check(x => x > min && x < max)
		.Check<ArgumentOutOfRangeException>(x => x == 30m);
```


## Example Output
```C#
Guard.Is<NullReferenceException(() => CostPrice != null); // Throws NullReferenceException
/* CostPrice(null) should be not equal to null */

CostPrice.Is("Cost")
  .Check(x => x > min && x < max) // Throws ArgumentException
/* Cost(30) should be greater than min(5) and also Cost(30) should be less than max(10) */
```

## Todo
- Add more fluent methods ie GreaterThan, Whitespace etc
- Add support for classes to the expression to text process
- Add support for exception message verbose settings
