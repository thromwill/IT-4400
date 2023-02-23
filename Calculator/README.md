This C# WPF calculator is designed to replicate the standard Windows 11 basic calculator with a nearly 1-to-1 layout, sizing, and colors. While most functionality is identical, some preferential changes have been made.

It includes the following features:
<pre>
MATHMATICAL OPERATIONS:
Add: take the sum of the current and previous values
Subtract: take the difference of the current and previous values
Multiply: take the product of the current and previous values
Divide: take the quotient of the current and previous values
Square: square the current value
Square Root: take the square root of the current value
Reciprocal: take the inverse of the current value
Percent: divide the current value by 100
Sign Change: change the sign of the current value (positive/negative)

CALCULATOR OPERATIONS:
Numpad: include a numeric value 0-9
Decimal Point: include a decimal point
Clear Entry: clears current value
Clear: clears all values
Delete: clears rightmost character
Equals: displays operation result

COLOR THEME:
The color theme of the calculator can be switched between Windows light (default) and dark theme.

CONTINUOUS OPERATION:
Continuous operation is supported, which means that there is no need to press "equals" between calculations. 
Additional operations will automatically complete previous operations before continuing.

KEYBOARD INPUT:
In addition to clicking the calculator buttons, keyboard input is also supported for non-numpad
keys: 0-9, ., +, -, *, /, %, =, Enter, Backspace, and Delete.

VALIDATION:
The calculator has been designed to validate every input scenario. Examples include:
- The square root button will have no effect on a negative value.
- If the current value is only a decimal point, operations will not be executed.
- If the current value is zero and a number key is pressed, the 0 will be replaced by
  the number rather than added to it. For example, pressing "5" will display "5" instead of "05".
- Only relevant keyboard buttons will have an effect on the calculator input.
</pre>
