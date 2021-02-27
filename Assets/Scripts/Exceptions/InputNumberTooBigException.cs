using System;

public class InputNumberTooBigException : Exception {
    public InputNumberTooBigException(string message) : base(message) { }
}