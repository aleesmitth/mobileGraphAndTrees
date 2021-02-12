using System;

public class InputWasNotANumberException : Exception {
    public InputWasNotANumberException(string message) : base(message) { }
}