using System;

public class InputNumberNegativeException : Exception {
    public InputNumberNegativeException(string message) : base(message) { }
}