# Coding Challenges

> Subject: [Interview-Preparation](../README.md)

## Coding Challenges

### Challenge 1: Reverse a String

```csharp
// Multiple approaches

// 1. Using Array.Reverse
public string ReverseString1(string input)
{
    char[] chars = input.ToCharArray();
    Array.Reverse(chars);
    return new string(chars);
}

// 2. Using LINQ
public string ReverseString2(string input)
{
    return new string(input.Reverse().ToArray());
}

// 3. Manual (best for interviews - shows logic)
public string ReverseString3(string input)
{
    char[] chars = input.ToCharArray();
    int left = 0, right = chars.Length - 1;

    while (left < right)
    {
        // Swap
        (chars[left], chars[right]) = (chars[right], chars[left]);
        left++;
        right--;
    }

    return new string(chars);
}

// Time: O(n), Space: O(n)
```

---

### Challenge 2: Find Duplicates in Array

```csharp
// Return all duplicate values

public List<int> FindDuplicates(int[] nums)
{
    var seen = new HashSet<int>();
    var duplicates = new HashSet<int>();

    foreach (var num in nums)
    {
        if (!seen.Add(num))  // ✅ Add returns false if already exists
        {
            duplicates.Add(num);
        }
    }

    return duplicates.ToList();
}

// Time: O(n), Space: O(n)

// Alternative: Using LINQ
public List<int> FindDuplicatesLinq(int[] nums)
{
    return nums.GroupBy(x => x)
               .Where(g => g.Count() > 1)
               .Select(g => g.Key)
               .ToList();
}
```

---

### Challenge 3: FizzBuzz

```csharp
public List<string> FizzBuzz(int n)
{
    var result = new List<string>();

    for (int i = 1; i <= n; i++)
    {
        if (i % 15 == 0)           // ✅ Divisible by both 3 and 5
            result.Add("FizzBuzz");
        else if (i % 3 == 0)
            result.Add("Fizz");
        else if (i % 5 == 0)
            result.Add("Buzz");
        else
            result.Add(i.ToString());
    }

    return result;
}

// Time: O(n), Space: O(n)
```

---

### Challenge 4: Two Sum

```csharp
// Find indices of two numbers that add up to target

public int[] TwoSum(int[] nums, int target)
{
    var map = new Dictionary<int, int>();  // Value -> Index

    for (int i = 0; i < nums.Length; i++)
    {
        int complement = target - nums[i];

        if (map.ContainsKey(complement))
        {
            return new[] { map[complement], i };
        }

        map[nums[i]] = i;
    }

    return Array.Empty<int>();
}

// Time: O(n), Space: O(n)

// Example:
// Input: nums = [2, 7, 11, 15], target = 9
// Output: [0, 1]  (nums[0] + nums[1] = 2 + 7 = 9)
```

---

### Challenge 5: Validate Balanced Parentheses

```csharp
public bool IsValid(string s)
{
    var stack = new Stack<char>();
    var pairs = new Dictionary<char, char>
    {
        { ')', '(' },
        { '}', '{' },
        { ']', '[' }
    };

    foreach (char c in s)
    {
        if (pairs.ContainsValue(c))  // Opening bracket
        {
            stack.Push(c);
        }
        else if (pairs.ContainsKey(c))  // Closing bracket
        {
            if (stack.Count == 0 || stack.Pop() != pairs[c])
                return false;
        }
    }

    return stack.Count == 0;
}

// Time: O(n), Space: O(n)

// Examples:
// "()" → true
// "()[]{}" → true
// "(]" → false
// "([)]" → false
```

---


